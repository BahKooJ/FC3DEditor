
using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundPlayerView : MonoBehaviour {

    // - Unity Refs -
    public Slider progressSlider;
    public TMP_Text bitCountText;
    public TMP_Text sampleRateText;
    public TMP_Text progressTimeText;
    public TMP_Text soundDuractionText;
    public Slider volumeSlider;
    public AudioSource audioPlayer;

    // - Parameter -
    public new FCopAudio audio;

    private void Start() {

        refuseCallback = true;

        bitCountText.text = audio.bitrate.ToString() + "-Bit " + audio.channelCount.ToString() + "-Channel";
        sampleRateText.text = audio.sampleRate.ToString();

        if (audio.bitrate == 8) {
            Load8Bit();
        }
        else {
            Load16Bit();
        }

        soundDuractionText.text = CreateTimeString(audioPlayer.clip.length);
        progressSlider.maxValue = audioPlayer.clip.length;

        float value;
        audioPlayer.outputAudioMixerGroup.audioMixer.GetFloat("Volume", out value);
        volumeSlider.value = value;

        refuseCallback = false;

    }

    bool refuseCallback = false;
    private void Update() {
        
        if (audioPlayer.isPlaying) {
            refuseCallback = true;
            progressTimeText.text = CreateTimeString(audioPlayer.time);
            progressSlider.value = audioPlayer.time;
            refuseCallback = false;
        }

    }

    void Load8Bit() {

        var audioData = audio.GetRawAudio();

        List<float> samples = new();

        foreach (var i in Enumerable.Range(0, audioData.Count())) {
            samples.Add((audioData[i] - 128) / 128.0f);
        }

        // Create AudioClip
        var audioClip = AudioClip.Create("clip", samples.Count() / audio.channelCount, audio.channelCount, audio.sampleRate, false);
        audioClip.SetData(samples.ToArray(), 0);

        audioPlayer.clip = audioClip;

    }

    void Load16Bit() {

        var audioData = audio.GetRawAudio();

        List<float> samples = new();

        foreach (var i in Enumerable.Range(0, audioData.Count() / 2)) {
            var sample = BitConverter.ToInt16(audioData, i * 2);
            samples.Add(sample / 32768.0f);
        }

        // Create AudioClip
        var audioClip = AudioClip.Create("clip", samples.Count(), 1, audio.sampleRate, false);
        audioClip.SetData(samples.ToArray(), 0);

        audioPlayer.clip = audioClip;

    }

    string CreateTimeString(float seconds) {

        var minuteCount = (int)(MathF.Floor(seconds) / 60f);
        var secondCount = (int)MathF.Floor(seconds) - (minuteCount * 60);
        var pointSecondsCount = MathF.Round(seconds % 1f, 1);

        var minString = minuteCount.ToString();
        var secondString = secondCount.ToString();
        var pointString = pointSecondsCount.ToString().Remove(0, 1);

        if (minuteCount < 10) {
            minString = minString.Insert(0, "0");
        }
        if (secondCount < 10) {
            secondString = secondString.Insert(0, "0");
        }

        return minString + ":" + secondString + pointString;

    }

    public void OnClickPlay() {


        if (audioPlayer.isPlaying) {
            audioPlayer.Stop();
        }
        else {
            audioPlayer.Play();
        }

    }

    public void OnSliderChanged() {

        if (refuseCallback) {
            return;
        }

        if (audioPlayer.isPlaying) {
            audioPlayer.time = progressSlider.value;
        }
        else {
            audioPlayer.time = progressSlider.value;
            audioPlayer.Play();
        }

    }

    public void OnAudioSliderChaned() {

        if (refuseCallback) {
            return;
        }

        audioPlayer.outputAudioMixerGroup.audioMixer.SetFloat("Volume", volumeSlider.value);

    }


}