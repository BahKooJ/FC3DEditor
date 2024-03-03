
using UnityEngine;

public class HomeScreenView : MonoBehaviour {

    public FileManagerMain main;

    private void Start() {

    }

    public void OnClickOpenFile() {

        OpenFileWindowUtil.OpenFile("MissionFiles", "", file => main.OpenFile(file));

    }

    public void OnClickCreateFile() {

        OpenFileWindowUtil.OpenFile("MissionFiles", "", file => main.CreateMission(file));

    }

    public void OnClickSettings() {

        main.OpenSettings();

    }

    public void OnClickQuit() {



    }

}
