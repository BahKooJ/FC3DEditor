using FCopParser;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FileManagerMain : MonoBehaviour {

    public RectTransform canvas;

    public GameObject HomeScreenView;
    public GameObject MapOpenerView;
    public GameObject DialogWindow;
    public GameObject SettingsView;
    public GameObject ContextMenu;
    public GameObject ContextMenuItem;
    public GameObject OpenFileWindow;
    public GameObject HeadsUpText;
    public GameObject LoadingScreen;


    public static IFFParser iffFile;
    public static FCopLevel level;

    void Start() {

        if (SettingsManager.keyBinds.Count == 0) {
            SettingsManager.ParseSettings();
        }

        DialogWindowUtil.prefab = DialogWindow;
        DialogWindowUtil.canvas = canvas.gameObject;

        ContextMenuUtil.container = ContextMenu;
        ContextMenuUtil.contextMenuItem = ContextMenuItem;
        ContextMenuUtil.canvas = canvas.gameObject;

        OpenFileWindowUtil.prefab = OpenFileWindow;
        OpenFileWindowUtil.canvas = canvas.gameObject;

        HeadsUpTextUtil.prefab = HeadsUpText;

        LoadingScreenUtil.prefab = LoadingScreen;
        LoadingScreenUtil.canvas = canvas.gameObject;

        try {
            FCopLevel.globalData = new FCopGlobalData(File.ReadAllBytes("GlblData"));
        }
        catch {
            DialogWindowUtil.Dialog("No Global Data Found", "The file \"GlblData\" was not found. Please drag the GlblData file into the same directory as the editor.");
        }

    }

    public FCopLevel GetFile(string path) {

        var fileContent = File.ReadAllBytes(path);

        if (Path.GetExtension(path) == ".ncfc") {

            try {

                return new FCopLevel(fileContent);

            }
            catch {

                DialogWindowUtil.Dialog("Invalid or Corrupted File", "Unable to parse Non-Compressed Future Cop file");
                return null;

            }

        }
        else {

            try { 

                iffFile = new IFFParser(fileContent);
                return new FCopLevel(iffFile.parsedData);

            }
            catch (InvalidFileException) {

                DialogWindowUtil.Dialog("Select Future Cop mission File", "This file is not a mission file");
                return null;

            }


        }

    }

    public void OpenFile(string path) {

        var fileContent = File.ReadAllBytes(path);

        if (Path.GetExtension(path) == ".ncfc") {

            //try {
                LoadingScreenUtil.Show();
                level = new FCopLevel(fileContent);
                SceneManager.LoadScene("Scenes/LevelEditorScene", LoadSceneMode.Single);

            //}
            //catch {
            //    LoadingScreenUtil.End();
            //    DialogWindowUtil.Dialog("Invalid or Corrupted File", "Unable to parse Non-Compressed Future Cop file");
            //}

        }
        else {

            try {
                LoadingScreenUtil.Show();
                iffFile = new IFFParser(fileContent);
                level = new FCopLevel(iffFile.parsedData);
                SceneManager.LoadScene("Scenes/LevelEditorScene", LoadSceneMode.Single);

            }
            catch (InvalidFileException) {
                LoadingScreenUtil.End();
                DialogWindowUtil.Dialog("Select Future Cop mission File", "This file is not a mission file");
            }

        }

    }

    public void OpenMapOpenerView() {

        var view = Instantiate(MapOpenerView);
        view.GetComponent<MapOpenerView>().main = this;
        view.transform.SetParent(canvas, false);

    }

    public void OpenSettings() {

        foreach (Transform child in canvas) {
            Destroy(child.gameObject);
        }

        var view = Instantiate(SettingsView);
        view.GetComponent<SettingsView>().main = this;
        view.transform.SetParent(canvas, false);

    }

    public void OpenHome() {

        foreach (Transform child in canvas) {
            Destroy(child.gameObject);
        }

        var view = Instantiate(HomeScreenView);
        view.GetComponent<HomeScreenView>().main = this;
        view.transform.SetParent(canvas, false);

    }


}
