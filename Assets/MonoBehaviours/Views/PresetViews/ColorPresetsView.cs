


using FCopParser;
using System.Linq;
using UnityEngine;

public class ColorPresetsView : MonoBehaviour {

    // Prefabs
    public GameObject colorPresetItem;
    public GameObject colorPresetsDirectoryItem;
    public GameObject backColorPresetsItem;

    // View refs
    public RectTransform presetListContent;

    public ShaderEditMode controller;

    void Start() {

        if (ShaderEditMode.showColorPresets) {
            Init();
        }

    }

    public void Init() {

        if (!ShaderEditMode.showColorPresets) {
            return;
        }
        
        AddBackItem();

        foreach (var folder in controller.currentColorPresets.subFolders) {
            AddDirectoryListItem(folder);
        }

        foreach (var preset in controller.currentColorPresets.presets) {
            AddListItem(preset);
        }

    }

    public void Clear() {

        foreach (Transform obj in presetListContent) {
            Destroy(obj.gameObject);
        }

    }

    public void Refresh() {

        foreach (Transform obj in presetListContent) {
            Destroy(obj.gameObject);
        }

        AddBackItem();

        foreach (var folder in controller.currentColorPresets.subFolders) {
            AddDirectoryListItem(folder);
        }

        foreach (var preset in controller.currentColorPresets.presets) {
            AddListItem(preset);
        }

    }

    void AddBackItem() {

        if (controller.currentColorPresets.parent != null) {

            var item = Instantiate(backColorPresetsItem);

            var script = item.GetComponent<BackColorPresetsViewItem>();

            script.controller = controller;
            script.view = this;

            item.transform.SetParent(presetListContent, false);

        }

    }

    void AddListItem(ColorPreset preset, bool forceNameChange = false) {

        var item = Instantiate(colorPresetItem);

        var script = item.GetComponent<ColorPresetViewItem>();

        script.controller = controller;
        script.view = this;
        script.preset = preset;
        script.forceNameChange = forceNameChange;

        item.transform.SetParent(presetListContent, false);

    }

    void AddDirectoryListItem(ColorPresets presets, bool forceNameChange = false) {

        var item = Instantiate(colorPresetsDirectoryItem);

        var script = item.GetComponent<ColorPresetsDirectoryViewItem>();

        script.controller = controller;
        script.presets = presets;
        script.view = this;
        script.forceNameChange = forceNameChange;

        item.transform.SetParent(presetListContent, false);

    }


    // Unity callbacks

    public void OnClickAddPresetButton() {

        if (!ShaderEditMode.showColorPresets) return;

        if (controller.AddColorPreset()) {

            AddListItem(controller.currentColorPresets.presets.Last(), true);

        }

    }

    public void OnClickAddDirectoryButton() {

        if (!ShaderEditMode.showColorPresets) return;

        controller.AddColorPresetsDirectory();

        AddDirectoryListItem(controller.currentColorPresets.subFolders.Last(), true);

    }

    public void OnClickSavePresetsButton() {

        if (!ShaderEditMode.showColorPresets) return;

        OpenFileWindowUtil.SaveFile("Presets", "Presets", path => {

            var fileName = Utils.RemovePathingFromFilePath(path);

            Presets.uvPresets.directoryName = Utils.RemoveExtensionFromFileName(fileName);
            Presets.shaderPresets.directoryName = Utils.RemoveExtensionFromFileName(fileName);

            Presets.SaveToFile(Utils.RemoveExtensionFromFileName(fileName));

        });

    }

    public void OnClickOpenPresetsButton() {

        if (!ShaderEditMode.showColorPresets) return;

        OpenFileWindowUtil.OpenFile("Presets", "", path => {

            Presets.ReadFile(path);

            controller.currentShaderPresets = Presets.shaderPresets;

            Refresh();

        });

    }


}