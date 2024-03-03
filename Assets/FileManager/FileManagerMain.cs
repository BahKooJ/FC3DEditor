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

    }

    public void OpenFile(string path) {

        var fileContent = File.ReadAllBytes(path);

        try {
            iffFile = new IFFParser(fileContent);
        } catch (InvalidFileException) {
            DialogWindowUtil.Dialog("Select Future Cop mission File", "This file is not a mission file");
        }

        FileManagerMain.level = new FCopLevel(FileManagerMain.iffFile.parsedData);
        SceneManager.LoadScene("Scenes/LevelEditorScene", LoadSceneMode.Single);

    }

    public void CreateMission(string path) {

        var fileContent = File.ReadAllBytes(path);

        try {
            iffFile = new IFFParser(fileContent);
        }
        catch (InvalidFileException) {
            DialogWindowUtil.Dialog("Select Future Cop mission File", "This file is not a mission file");
        }

        foreach (Transform child in canvas) {
            Destroy(child.gameObject);
        }

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
