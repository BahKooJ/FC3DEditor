using FCopParser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManagerView : MonoBehaviour {

    // - Unity Prefabs -
    public GameObject assetDirectoryPrefab;
    public GameObject assetFilePrefab;

    public GameObject soundPlayer;
    public GameObject soundPropertiesPrefab;

    // - Unity Refs -
    public Transform fileContent;
    public Transform inspectorContent;
    
    // - Parameters -
    public FCopLevel level;

    List<AssetDirectoryView> directories = new();
    List<AssetFileView> files = new();

    AssetDirectory root;

    AssetDirectory currentDirectory;

    private void Start() {

        root = new AssetDirectory(AssetType.Mixed, false, "root", null);

        var soundDir = new AssetDirectory(AssetType.WavSound, true, "Sound Effects", root);

        foreach (var item in level.soundEffects.soundEffects) {

            foreach (var sound in item.Value) {
                soundDir.files.Add(new AssetFile(sound, AssetType.WavSound));
            }

        }

        var textureDir = new AssetDirectory(AssetType.Texture, false, "Textures", root);

        foreach (var texture in level.textures) {

            textureDir.files.Add(new AssetFile(texture, AssetType.Texture));

        }

        var objectDir = new AssetDirectory(AssetType.Object, true, "Objects", root);

        foreach (var obj in level.objects) {

            objectDir.files.Add(new AssetFile(obj, AssetType.Object));

        }

        root.directories.Add(soundDir);
        root.directories.Add(textureDir);
        root.directories.Add(objectDir);

        currentDirectory = root;

        Refresh();

    }

    public void Refresh() {

        foreach (var dir in directories) {
            Destroy(dir.gameObject);
        }

        directories.Clear();

        foreach (var file in files) {
            Destroy(file.gameObject);
        }

        files.Clear();

        if (currentDirectory.parent != null) {

            var obj = Instantiate(assetDirectoryPrefab);
            obj.transform.SetParent(fileContent.transform, false);

            var directoryView = obj.GetComponent<AssetDirectoryView>();
            directoryView.directory = currentDirectory.parent;
            directoryView.view = this;
            directoryView.isParent = true;

            directories.Add(directoryView);

        }

        foreach (var dir in currentDirectory.directories) {

            var obj = Instantiate(assetDirectoryPrefab);
            obj.transform.SetParent(fileContent.transform, false);

            var directoryView = obj.GetComponent<AssetDirectoryView>();
            directoryView.directory = dir;
            directoryView.view = this;

            directories.Add(directoryView);

        }

        foreach (var file in currentDirectory.files) {

            var obj = Instantiate(assetFilePrefab);
            obj.transform.SetParent(fileContent.transform, false);

            var fileView = obj.GetComponent<AssetFileView>();
            fileView.file = file;
            fileView.view = this;

            files.Add(fileView);

        }

    }

    public void ClearInspectorContent() {

        foreach (Transform tran in inspectorContent) {
            Destroy(tran.gameObject);
        }

    }

    public void MoveDirectory(AssetDirectory dir) {
        currentDirectory = dir;
        Refresh();
    }

    public void OnSelectAsset(AssetFile file) {

        ClearInspectorContent();

        if (file.assetType == AssetType.WavSound) {
            InstanciateSoundPlayer((FCopAudio)file.asset);
            InstanciateSoundProperties((FCopAudio)file.asset);

        }

    }

    void InstanciateSoundPlayer(FCopAudio fcopAudio) {

        var obj = Instantiate(soundPlayer);
        obj.transform.SetParent(inspectorContent.transform, false);

        var audioFile = obj.GetComponent<SoundPlayerView>();
        audioFile.audio = fcopAudio;

    }

    void InstanciateSoundProperties(FCopAudio fcopAudio) {

        var obj = Instantiate(soundPropertiesPrefab);
        obj.transform.SetParent(inspectorContent.transform, false);

        var script = obj.GetComponent<SoundEffectPropertiesView>();
        script.audioAsset = fcopAudio;

    }


}
