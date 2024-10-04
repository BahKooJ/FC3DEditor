
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreenView : MonoBehaviour {

    // - Unity Refs -
    public List<Sprite> backgroundImages;
    public Image backgroundImage;

    public FileManagerMain main;

    private void Start() {

        backgroundImage.sprite = backgroundImages[Random.Range(0, backgroundImages.Count)];

    }

    public void OnClickOpenFile() {

        OpenFileWindowUtil.OpenFile("MissionFiles", "", file => main.OpenFile(file));

    }

    public void OnClickCreateFile() {

        main.OpenMapOpenerView();

    }

    public void OnClickSettings() {

        main.OpenSettings();

    }

    public void OnClickQuit() {



    }

    public void OnClickYouTube() {
        Application.OpenURL("https://www.youtube.com/@BahKooJ");
    }

    public void OnClickDiscord() {
        Application.OpenURL("https://discord.gg/dbMxT6Mp2v");
    }

    public void OnClickPatreon() {
        Application.OpenURL("https://www.patreon.com/FCEditor");
    }

}
