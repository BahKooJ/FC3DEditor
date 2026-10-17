

using FCopParser;
using System.Linq;
using UnityEngine;

// Yup... This is an exact copy of texture presets view with different types. What of it?
// Update: Well thanks past me, now I have to copy and past all the dragging code to all thre- five preset copies
public class ShaderPresetsView : MonoBehaviour {

    // Prefabs
    public GameObject shaderPresetItem;
    public GameObject shaderPresetsDirectoryItem;
    public GameObject backShaderPresetsItem;

    // View refs
    public RectTransform presetListContent;

    public ShaderEditMode controller;

    void Start() {
        Init();
    }

    public void Init() {

        if (ShaderEditMode.showColorPresets) {
            return;
        }

        AddBackItem();

        foreach (var folder in controller.currentShaderPresets.subFolders) {
            AddDirectoryListItem(folder);
        }

        foreach (var preset in controller.currentShaderPresets.presets) {
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

        foreach (var folder in controller.currentShaderPresets.subFolders) {
            AddDirectoryListItem(folder);
        }

        foreach (var preset in controller.currentShaderPresets.presets) {
            AddListItem(preset);
        }

    }

    public void RefreshSelection() {

        foreach (Transform obj in presetListContent) {

            var preset = obj.gameObject.GetComponent<ShaderPresetViewItem>();

            if (preset != null) {
                preset.DeSelect();
            }

        }

    }

    void AddBackItem() {

        if (controller.currentShaderPresets.parent != null) {

            var item = Instantiate(backShaderPresetsItem);

            var script = item.GetComponent<BackShaderPresetsViewItem>();

            script.controller = controller;
            script.view = this;

            item.transform.SetParent(presetListContent, false);

        }

    }

    void AddListItem(ShaderPreset preset, bool forceNameChange = false) {

        var item = Instantiate(shaderPresetItem);

        var script = item.GetComponent<ShaderPresetViewItem>();

        script.controller = controller;
        script.view = this;
        script.preset = preset;
        script.forceNameChange = forceNameChange;

        item.transform.SetParent(presetListContent, false);

    }

    void AddDirectoryListItem(ShaderPresets presets, bool forceNameChange = false) {

        var item = Instantiate(shaderPresetsDirectoryItem);

        var script = item.GetComponent<ShaderPresetsDirectoryViewItem>();

        script.controller = controller;
        script.presets = presets;
        script.view = this;
        script.forceNameChange = forceNameChange;

        item.transform.SetParent(presetListContent, false);

    }


    // Unity callbacks

    public void OnClickAddPresetButton() {

        if (ShaderEditMode.showColorPresets) return;

        if (controller.AddPreset()) {

            AddListItem(controller.currentShaderPresets.presets.Last(), true);

        }

    }

    public void OnClickAddDirectoryButton() {

        if (ShaderEditMode.showColorPresets) return;

        controller.AddPresetsDirectory();

        AddDirectoryListItem(controller.currentShaderPresets.subFolders.Last(), true);

    }

    public void OnClickSavePresetsButton() {

        if (ShaderEditMode.showColorPresets) return;

        OpenFileWindowUtil.SaveFile("Presets", "Presets", path => {

            var fileName = Utils.RemovePathingFromFilePath(path);

            Presets.uvPresets.directoryName = Utils.RemoveExtensionFromFileName(fileName);
            Presets.shaderPresets.directoryName = Utils.RemoveExtensionFromFileName(fileName);
            Presets.colorPresets.directoryName = Utils.RemoveExtensionFromFileName(fileName);
            Presets.actorSchematics.directoryName = Utils.RemoveExtensionFromFileName(fileName);

            Presets.SaveToFile(Utils.RemoveExtensionFromFileName(fileName));

        });

    }

    public void OnClickOpenPresetsButton() {

        if (ShaderEditMode.showColorPresets) return;

        OpenFileWindowUtil.OpenFile("Presets", "", path => {

            Presets.ReadFile(path);

            controller.currentShaderPresets = Presets.shaderPresets;

            Refresh();

        });

    }


}