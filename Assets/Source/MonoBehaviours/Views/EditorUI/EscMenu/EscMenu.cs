

using FCopParser;
using UnityEngine;

public class EscMenu : MonoBehaviour {

    // - Prefabs -
    public GameObject settingsPanelPrefab;

    // - View Refs -
    public EscMenuViewSettingsView viewSettings;
    public EscMenuLevelSettingsView levelSettings;

    // - Parameters -
    public Main main;

    void Start() {

        Main.ignoreAllInputs = true;
        FreeMove.StopLooking();

        viewSettings.main = this.main;
        levelSettings.main = this.main;

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {

            OnClickResume();
        }

    }

    void DeactivateAllViews() {
        viewSettings.gameObject.SetActive(false);
        levelSettings.gameObject.SetActive(false);
    }

    void ClearSettingsView() {

        var views = FindObjectsOfType<SettingsView>();

        foreach (var view in views) {
            Destroy(view.gameObject);
        }

    }

    #region Unity Callbacks

    public void OnClickResume() {
        Main.ignoreAllInputs = false;
        main.isEscMenuOpen = false;

        main.ApplySettings();

        ClearSettingsView();

        Destroy(this.gameObject);
    }

    public void OnClickView() {

        var wasActivated = viewSettings.isActiveAndEnabled;


        DeactivateAllViews();

        viewSettings.gameObject.SetActive(!wasActivated);

    }

    public void OnClickLevel() {

        var wasActivated = levelSettings.isActiveAndEnabled;

        DeactivateAllViews();

        levelSettings.gameObject.SetActive(!wasActivated);
    }

    public void OnClickSettings() {
        DeactivateAllViews();

        var obj = Instantiate(settingsPanelPrefab);
        obj.transform.SetParent(main.canvas.transform, false);

    }

    public void OnClickSave() {
        DeactivateAllViews();
        main.Save();
        Main.ClearCounterActions();

    }

    public void OnClickCompile() {
        DeactivateAllViews();
        main.Compile();
        Main.ClearCounterActions();
    }

    public void OnClickSavePresets() {
        DeactivateAllViews();

        OpenFileWindowUtil.SaveFile("Presets", "Presets", path => {

            var fileName = Utils.RemovePathingFromFilePath(path);

            Presets.uvPresets.directoryName = Utils.RemoveExtensionFromFileName(fileName);
            Presets.shaderPresets.directoryName = Utils.RemoveExtensionFromFileName(fileName);
            Presets.colorPresets.directoryName = Utils.RemoveExtensionFromFileName(fileName);
            Presets.actorSchematics.directoryName = Utils.RemoveExtensionFromFileName(fileName);

            Presets.SaveToFile(Utils.RemoveExtensionFromFileName(fileName));

        });

    }

    public void OnClickOpenPresets() {

        DeactivateAllViews();

        OpenFileWindowUtil.OpenFile("Presets", "", path => {

            Presets.ReadFile(path);

        });

    }

    public void OnClickExit() {
        DeactivateAllViews();
        DialogWindowUtil.Dialog("Exit Confirmation", "Are you sure you would like to exit? \n" +
            "Any unsaved changes or presets will be lost!",() => { main.BackToMainMenu(); return true; });

    }

    #endregion


}