
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class AssetFileView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text title;
    public Image icon;
    public ContextMenuHandler contextMenu;
    public TMP_InputField nameInput;

    // - Parameters -
    public AssetFile file;
    public AssetManagerView view;

    void Start () {

        title.text = file.asset.name;

        if (file.directory.canAddFiles) {
            contextMenu.items = new() {
            ("Rename", StartRename),
            ("Delete", Delete),
            ("Export", Export),
            ("Import", Import),
            ("Add", () => { })
            };
        }
        else {

            contextMenu.items = new() {
            ("Rename", StartRename),
            ("Export", Export),
            ("Import", Import),
            };

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

            view.level.DeleteAsset(file.asset.rawFile.dataFourCC, file.asset.DataID);
            view.DeleteFile(file);

            view.Refresh();

            return true;

        });

    }

    void Export() {

        OpenFileWindowUtil.SaveFile("Output/Exports", file.asset.rawFile.dataFourCC + file.asset.DataID, path => {
            File.WriteAllBytes(path, file.asset.rawFile.data.ToArray());
        });

    }

    void Import() {

        OpenFileWindowUtil.OpenFile("Output/Exports", file.asset.rawFile.dataFourCC + file.asset.DataID, path => {
            view.level.fileManager.ReplaceFile(file.asset.rawFile.dataFourCC, file.asset.DataID, File.ReadAllBytes(path).ToList());
        });

    }

    public void OnClick() {

        view.OnSelectAsset(file);

    }

    public void StopRenaming() {

        title.gameObject.SetActive(true);
        file.asset.name = nameInput.text;
        title.text = file.asset.name;
        nameInput.gameObject.SetActive(false);

        Main.ignoreAllInputs = false;

    }

}