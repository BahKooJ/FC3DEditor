using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class AssetManagerView : MonoBehaviour {

    // - Unity Prefabs -
    public GameObject assetDirectoryPrefab;
    public GameObject assetFilePrefab;

    public GameObject soundPlayer;
    public GameObject soundPropertiesPrefab;
    public GameObject objectPropertiesPrefab;
    public GameObject texturePropertiesPrefab;

    // - Unity Refs -
    public Transform fileContent;
    public Transform inspectorContent;
    public ContextMenuHandler contextMenu;

    // - Parameters -
    public FCopLevel level;
    public Main main;

    List<AssetDirectoryView> directories = new();
    List<AssetFileView> files = new();

    AssetDirectory root;

    AssetDirectory currentDirectory;

    private void Start() {

        root = new AssetDirectory(AssetType.Mixed, false, "root", null);

        var soundDir = new AssetDirectory(AssetType.WavSound, true, "Sound Effects", root);

        foreach (var sound in level.audio.soundEffects) {
            
            soundDir.files.Add(new AssetFile(sound, AssetType.WavSound, soundDir));
            
        }

        var textureDir = new AssetDirectory(AssetType.Texture, false, "Textures", root);

        foreach (var texture in level.textures) {

            textureDir.files.Add(new AssetFile(texture, AssetType.Texture, textureDir));

        }

        var objectDir = new AssetDirectory(AssetType.Object, true, "Objects", root);

        foreach (var obj in level.objects) {

            objectDir.files.Add(new AssetFile(obj, AssetType.Object, objectDir));

        }

        var streamsDir = new AssetDirectory(AssetType.Stream, false, "Streams", root);

        foreach (var stream in level.audio.soundStreams) {
            streamsDir.files.Add(new AssetFile(stream, AssetType.Stream, streamsDir));
        }

        root.directories.Add(soundDir);
        root.directories.Add(textureDir);
        root.directories.Add(objectDir);
        root.directories.Add(streamsDir);

        root.files.Add(new AssetFile(level.audio.music, AssetType.Music, root));

        currentDirectory = root;

        Refresh();

    }

    public void DeleteFile(AssetFile fileItem) {
        currentDirectory.files.Remove(fileItem);
    }

    public void Refresh() {

        if (currentDirectory.canAddFiles) {

            contextMenu.items = new() {
                ("Add Raw", AddRaw),
                ("Add Parsed", AddParsed)
            };

        }
        else {
            contextMenu.items = new() { };
        }



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

        ClearInspectorContent();

    }

    public void ClearHighlight() {

        foreach (var file in files) {
            file.ClearHighlight();
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

        switch (file.assetType) {
            case AssetType.WavSound:
                InstanciateSoundPlayer((FCopAudio)file.asset);
                InstanciateSoundProperties((FCopAudio)file.asset);
                break;
            case AssetType.Texture:
                InstanciateTextureProperties((FCopTexture)file.asset);
                break;
            case AssetType.Object:
                InstanciateObjectProperties((FCopObject)file.asset);
                break;
            case AssetType.Stream:
                InstanciateSoundPlayer(((FCopStream)file.asset).sound);
                break;
            case AssetType.Music:
                InstanciateSoundPlayer((FCopAudio)file.asset);
                break;
            case AssetType.MiniAnimation:
                break;
            case AssetType.Mixed:
                break;
        }


    }

    // Screw it, object return type.
    public object ParsedDataToRaw(AssetType assetType, string path) {

        try {

            switch (assetType) {
                case AssetType.WavSound:

                    var waveParser = new WaveParser(File.ReadAllBytes(path).ToList());

                    if (!(waveParser.sampleRate == 22050 && waveParser.channels == 1 && waveParser.bitsPerSample == 16)) {
                        DialogWindowUtil.Dialog("Incorrect Wave Format", "Wave file is incorrect format, ensure that wave file meets required format:\n" +
                            "Required Sample Rate: 22050, File Sample Rate: " + waveParser.sampleRate + "\n" +
                            "Required Channels: 1, File Channels: " + waveParser.channels + "\n" +
                            "Required Bits Per Sample: 16, File Bits Per Sample: " + waveParser.bitsPerSample);
                        return null;
                    }

                    return waveParser.data.ToArray();
                case AssetType.Object:

                    var wavefrontParser = new WavefrontParser(File.ReadAllText(path).ToList());

                    var emptyRawFile = level.CreateEmptyAssetFile(AssetType.Object);

                    try {
                        var obj = new FCopObject(wavefrontParser, emptyRawFile);

                        return obj;

                    }
                    catch (FCopObject.VertexLimitExceededException) {

                        DialogWindowUtil.Dialog("Cobj Vertex Limit Exceeded", "Future Cop models has a maximum of 256 vertices, this limit was exceeded. " +
                            "Keep the vertex count in mind when making models, this limit can be easily exceeded if not careful.");

                    }
                    catch (FCopObject.InvalidPrimitiveException) {

                        DialogWindowUtil.Dialog("Invalid Face", "Future Cop models only support triangle and quad faces. " +
                            "Any face with more than 4 vertices is invalid.");

                    }
                    return null;
                default:
                    return null;
            }

        }
        catch (InvalidFileException) {

            DialogWindowUtil.Dialog("Invalid File", "Please select a valid wave file.");

            return null;
        }

    }

    void AddRaw() {

        OpenFileWindowUtil.OpenFile("FCEAssets", "", path => {
            var newFile = level.AddAsset(currentDirectory.storedAssets, File.ReadAllBytes(path));
            currentDirectory.files.Add(new AssetFile(newFile, currentDirectory.storedAssets, currentDirectory));
            Refresh();
        });

    }

    void AddParsed() {

        switch (currentDirectory.storedAssets) {

            case AssetType.WavSound:

                OpenFileWindowUtil.OpenFile("FCEAssets", "", path => {

                    var waveData = ParsedDataToRaw(AssetType.WavSound, path);

                    if (waveData == null) return;

                    var newFile = level.AddAsset(AssetType.WavSound, (byte[])waveData);

                    currentDirectory.files.Add(new AssetFile(newFile, currentDirectory.storedAssets, currentDirectory));

                    Refresh();

                });

                break;

            case AssetType.Object:

                OpenFileWindowUtil.OpenFile("FCEAssets", "", path => {

                    var obj = (FCopObject)ParsedDataToRaw(AssetType.Object, path);

                    level.AddAsset(AssetType.Object, obj);

                    currentDirectory.files.Add(new AssetFile(obj, AssetType.Object, currentDirectory));
                    Refresh();

                });

                break;

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

    void InstanciateObjectProperties(FCopObject fCopObject) {

        var obj = Instantiate(objectPropertiesPrefab);
        obj.transform.SetParent(inspectorContent.transform, false);

        var script = obj.GetComponent<ObjectPropertiesView>();
        script.main = main;
        script.fCopObject = fCopObject;

    }

    void InstanciateTextureProperties(FCopTexture fCopTexture) {

        var obj = Instantiate(texturePropertiesPrefab, inspectorContent.transform, false);

        var script = obj.GetComponent<TexturePropertiesView>();
        script.main = main;
        script.texture = fCopTexture;

    }

    public void OnClickDone() {

        Destroy(gameObject);

    }

}
