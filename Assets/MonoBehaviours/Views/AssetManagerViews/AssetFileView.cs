
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FCopParser;

public class AssetFileView : MonoBehaviour {

    // - Unity Assets -
    public Sprite wavIcon;
    public Sprite modelIcon;
    public Sprite textureIcon;
    public Sprite streamIcon;
    public Sprite musicIcon;

    // - Unity Refs -
    public TMP_Text title;
    public Image icon;
    public Image background;
    public ContextMenuHandler contextMenu;
    public TMP_InputField nameInput;

    // - Parameters -
    public AssetFile file;
    public AssetManagerView view;

    void Start () {

        switch (file.assetType) {
            case AssetType.WavSound:
                icon.sprite = wavIcon;
                break;
            case AssetType.Texture:
                icon.sprite = textureIcon;
                break;
            case AssetType.Object:
                icon.sprite = modelIcon;
                break;
            case AssetType.Music:
                icon.sprite = musicIcon;
                break;
            case AssetType.Stream:
                icon.sprite = streamIcon;
                break;
        }

        title.text = file.asset.name;

        if (file.asset.isGlobalData) {
            contextMenu.items = new() {
                ("Export", Export)
            };
        }
        else if (file.assetType == AssetType.Stream) {
            contextMenu.items = new() {
                ("Rename", StartRename),
                ("Delete", StreamDelete),
            };
        }
        else if (file.directory.canAddFiles) {
            contextMenu.items = new() {
                ("Rename", StartRename),
                ("Delete", Delete),
                ("Export", Export),
                ("Import", Import),
                ("Import Parsed", ImportParsed),
            };
        }
        else {

            contextMenu.items = new() {
                ("Rename", StartRename),
                ("Export", Export),
                ("Import", Import),
                ("Import Parsed", ImportParsed),
            };

        }

        if (file.asset.isGlobalData) {
            background.color = Main.globalColor;
        }

    }

    void StartRename() {
        nameInput.gameObject.SetActive(true);
        title.gameObject.SetActive(false);
        nameInput.text = file.asset.name;
        nameInput.Select();

        Main.ignoreAllInputs = true;
    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete File", "Are you sure you would like to delete this file? Asset files are normally used by actors" +
            " and the game itself. Deleting an asset file may cause the level to no longer work.", () => {

            view.level.DeleteAsset(file.assetType, file.asset.DataID);
            view.DeleteFile(file);

            view.Refresh();

            return true;

        });

    }

    void StreamDelete() {

        DialogWindowUtil.Dialog("Delete Stream", "It is strongly recommended to not remove streams as they are index based (Forced by scripting). " +
            "Only remove streams if it is the last item, or if there is no scripting.", () => {

            view.level.DeleteAsset(file.assetType, view.level.audio.soundStreams.IndexOf((FCopStream)file.asset));
            view.DeleteFile(file);

            view.Refresh();

            return true;

        });

    }

    void Export() {

        OpenFileWindowUtil.SaveFile("FCEAssets", file.asset.rawFile.dataFourCC + file.asset.DataID, path => {
            File.WriteAllBytes(path, file.asset.rawFile.data.ToArray());
        });

    }

    void Import() {

        OpenFileWindowUtil.OpenFile("FCEAssets", "", path => {
            view.level.ImportAsset(file.assetType, file.asset.DataID, File.ReadAllBytes(path));
            view.Refresh();
        });

    }

    void ImportParsed() {

        OpenFileWindowUtil.OpenFile("FCEAssets", "", path => {
            var data = view.ParsedDataToRaw(file.assetType, path);

            if (data != null) {

                if (file.assetType == AssetType.Object) {
                    view.level.ImportAsset(file.assetType, file.asset.DataID, ((FCopObject)data).Compile().data.ToArray());
                }
                else {
                    view.level.ImportAsset(file.assetType, file.asset.DataID, (byte[])data);
                }

                view.Refresh();

            }
            else {

                if (file.assetType == AssetType.Texture) {
                    ((FCopTexture)file.asset).ImportBMP(File.ReadAllBytes(path));

                    view.main.RefreshTextures();

                    foreach (var section in view.main.sectionMeshes) {
                        section.RefreshTexture();
                        section.RefreshMesh();
                    }
                }

            }

        });

    }

    public void ClearHighlight() {
        background.color = file.asset.isGlobalData ? Main.globalColor : Main.mainColor;
    }

    public void OnClick() {

        view.OnSelectAsset(file);
        view.ClearHighlight();
        background.color = Main.selectedColor;

    }

    public void StopRenaming() {

        title.gameObject.SetActive(true);
        file.asset.name = nameInput.text;
        title.text = file.asset.name;
        nameInput.gameObject.SetActive(false);

        Main.ignoreAllInputs = false;

    }

}