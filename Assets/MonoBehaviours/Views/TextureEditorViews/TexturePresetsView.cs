
using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TexturePresetsView : MonoBehaviour {

    // Prefabs
    public GameObject uvPresetItem;
    public GameObject uvPresetsDirectoryItem;
    public GameObject backUVPresetsItem;

    // View refs
    public RectTransform presetListContent;

    public TextureEditMode controller;

    void Start() {

        AddBackItem();

        foreach (var folder in controller.currentUVPresets.subFolders) {
            AddDirectoryListItem(folder);
        }

        foreach (var preset in controller.currentUVPresets.presets) {
            AddListItem(preset);
        }

    }

    public void Refresh() {

        foreach (Transform obj in presetListContent) {
            Destroy(obj.gameObject);
        }

        AddBackItem();

        foreach (var folder in controller.currentUVPresets.subFolders) {
            AddDirectoryListItem(folder);
        }

        foreach (var preset in controller.currentUVPresets.presets) {
            AddListItem(preset);
        }

    }

    void AddBackItem() {

        if (controller.currentUVPresets.parent != null) {

            var item = Instantiate(backUVPresetsItem);

            var script = item.GetComponent<BackUVPresetsViewItem>();

            script.controller = controller;
            script.view = this;

            item.transform.SetParent(presetListContent, false);

        }

    }

    void AddListItem(UVPreset preset, bool forceNameChange = false) {

        var item = Instantiate(uvPresetItem);

        var script = item.GetComponent<UVPresentViewItem>();

        script.controller = controller;
        script.view = this;
        script.preset = preset;
        script.forceNameChange = forceNameChange;

        item.transform.SetParent(presetListContent, false);

    }

    void AddDirectoryListItem(UVPresets presets, bool forceNameChange = false) {

        var item = Instantiate(uvPresetsDirectoryItem);

        var script = item.GetComponent<UVPresetsDirectoryViewItem>();

        script.controller = controller;
        script.presets = presets;
        script.view = this;
        script.forceNameChange = forceNameChange;

        item.transform.SetParent(presetListContent, false);

    }

    
    // Unity callbacks

    public void OnClickAddPresetButton() {

        if (controller.AddPreset()) {

            AddListItem(controller.currentUVPresets.presets.Last(), true);

        }

    }

    public void OnClickAddDirectoryButton() {

        controller.AddPresetsDirectory();

        AddDirectoryListItem(controller.currentUVPresets.subFolders.Last(), true);

    }

    public void OnClickSavePresetsButton() {

        OpenFileWindowUtil.SaveFile("TexturePresets", TextureEditMode.rootUVPresets.directoryName, path => {

            var fileName = Utils.RemovePathingFromFilePath(path);

            TextureEditMode.rootUVPresets.directoryName = Utils.RemoveExtensionFromFileName(fileName);

            TextureEditMode.rootUVPresets.SaveToFile();

        });

    }

    public void OnClickOpenPresetsButton() {

        OpenFileWindowUtil.OpenFile("TexturePresets", "", path => {

            TextureEditMode.rootUVPresets = UVPresets.ReadFile(path);

            controller.currentUVPresets = TextureEditMode.rootUVPresets;

            Refresh();

        });

    }


}