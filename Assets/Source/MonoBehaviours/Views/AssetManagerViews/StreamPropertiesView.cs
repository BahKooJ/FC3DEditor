

using UnityEngine;
using TMPro;
using FCopParser;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class StreamPropertiesView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text indexText;
    public Image animationPreview;

    // - Parameters -
    public FCopStream stream;
    public FCopLevel level;
    public AssetFile assetFile;
    public AssetManagerView view;

    List<Sprite> frames = new();

    Sprite defaultImage;

    private void Start() {

        defaultImage = animationPreview.sprite;

        indexText.text = level.audio.soundStreams.IndexOf(stream).ToString();

        Refresh();

    }

    void Refresh() {

        animationPreview.sprite = defaultImage;

        frames.Clear();

        if (stream.miniAnimation != null) {

            foreach (var frame in stream.miniAnimation.framesBitmapped) {

                var bmpTexture = new Texture2D(FCopMiniAnimation.width, FCopMiniAnimation.height, TextureFormat.ARGB32, false);

                bmpTexture.filterMode = FilterMode.Point;

                bmpTexture.LoadRawTextureData(frame.ToArray());
                bmpTexture.Apply();

                frames.Add(Sprite.Create(bmpTexture, new Rect(0, 0, FCopMiniAnimation.width, FCopMiniAnimation.height), Vector2.zero));

            }

            animationPreview.sprite = frames[0];

        }

    }


    float timer = 0f;
    int frameI = 0;
    private void Update() {

        if (stream.miniAnimation != null) {

            if (timer > 0.07f) {
                if (frameI == frames.Count) {
                    frameI = 0;
                }

                animationPreview.sprite = frames[frameI];
                frameI++;
                timer = 0f;
            }
            else {
                timer += Time.deltaTime;
            }


        }

    }

    public void OnClickExportSound() {

        OpenFileWindowUtil.SaveFile("FCEAssets", stream.name, path => {
            File.WriteAllBytes(path + ".wav", stream.sound.GetFormattedAudio());
        });

    }

    public void OnClickImportSound() {

        OpenFileWindowUtil.OpenFile("FCEAssets", "", path => {

            try {

                var waveParser = new WaveParser(File.ReadAllBytes(path).ToList());

                if (!(waveParser.sampleRate == stream.sound.sampleRate && waveParser.channels == stream.sound.channelCount && waveParser.bitsPerSample == stream.sound.bitrate)) {
                    DialogWindowUtil.Dialog("Incorrect Wave Format", "Wave file is incorrect format, ensure that wave file meets required format:\n" +
                        "Required Sample Rate: " + stream.sound.sampleRate + ", File Sample Rate: " + waveParser.sampleRate + "\n" +
                        "Required Channels: " + stream.sound.channelCount + ", File Channels: " + waveParser.channels + "\n" +
                        "Required Bits Per Sample: " + stream.sound.bitrate + ", File Bits Per Sample: " + waveParser.bitsPerSample);
                    return;
                }

                var data = new List<byte>();

                data.AddRange(BitConverter.GetBytes(waveParser.sampleData.Count));
                data.AddRange(waveParser.sampleData);

                stream.sound.rawFile.data = data;

                view.OnSelectAsset(assetFile);

            }
            catch {

                DialogWindowUtil.Dialog("Invalid File", "Please select a valid file.");

            }

        });

    }

    public void OnClickExportMiniAnimation() {

        if (stream.miniAnimation == null) {
            return;
        }

        OpenFileWindowUtil.SaveFile("FCEAssets", "Mini Animation", path => {
            File.WriteAllBytes(path, stream.miniAnimation.rawFile.data.ToArray());
        });

    }

    public void OnClickImportMiniAnimation() {

        OpenFileWindowUtil.OpenFile("FCEAssets", "", path => {

            try {

                var data = File.ReadAllBytes(path);

                stream.miniAnimation = new FCopMiniAnimation(new IFFDataFile(6, data.ToList(), "canm", 1, -1));

                Refresh();

            }
            catch {

                DialogWindowUtil.Dialog("Invalid File", "Please select a valid file.");

            }

        });

    }

    public void OnClickRemoveMiniAnimation() {

        stream.miniAnimation = null;

        Refresh();

    }

}