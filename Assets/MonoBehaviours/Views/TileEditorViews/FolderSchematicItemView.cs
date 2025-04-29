
using UnityEngine;
using UnityEngine.UI;

public class FolderSchematicItemView : MonoBehaviour {

    // - Unity Asset Refs -
    public Sprite backArrowSprite;

    // - Unity Refs -
    public Image icon;
    public ContextMenuHandler contextMenu;
    public InfoBoxHandler infoBoxHandler;
    public TextFieldPopupHandler textFieldPopupHandler;

    // - Parameters -
    public TileAddMode controller;
    public Schematics schematics;
    [HideInInspector]
    public SchematicMeshPresetsView view;
    [HideInInspector]
    public bool isBack = false;

    private void Start() {

        infoBoxHandler.message = schematics.directoryName;

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        textFieldPopupHandler.finishCallback = text => {

            schematics.directoryName = text;

            infoBoxHandler.message = schematics.directoryName;

        };

        if (isBack) {
            icon.sprite = backArrowSprite;
        }

    }

    void Rename() {

        textFieldPopupHandler.OpenPopupTextField(schematics.directoryName);

    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete Actor Schematic Folder", "Are you sure you would like to delete this actor schematic? " +
            "This will delete all schematics inside this folder. This cannot be undone.", () => {

                view.currentDirectory.subFolders.Remove(schematics);

                view.RefreshView();

                return true;
            });

    }

    // - Unity Callbacks -
    public void OnClick() {

        view.SwitchDirectory(schematics);

    }

    public void ReceiveDrag() {

        if (Main.draggingElement.TryGetComponent<SchematicMeshItemView>(out var viewItem)) {

            view.currentDirectory.schematics.Remove(viewItem.schematic);

            schematics.schematics.Add(viewItem.schematic);

            Destroy(viewItem.gameObject);

        }

    }

}