
using UnityEngine;

public class SettingsView : MonoBehaviour {

    public FileManagerMain main;

    public void OnClickDone() {

        SettingsManager.SaveToFile();

        main.OpenHome();

    }

}