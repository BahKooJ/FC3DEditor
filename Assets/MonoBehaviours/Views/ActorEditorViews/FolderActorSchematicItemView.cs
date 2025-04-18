
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FolderActorSchematicItemView : MonoBehaviour {

    // - Unity Asset Refs -
    public Sprite backArrowSprite;

    // - Unity Refs -
    public Image icon;
    public ContextMenuHandler contextMenu;
    public InfoBoxHandler infoBoxHandler;
    public TextFieldPopupHandler textFieldPopupHandler;

    // - Parameters -
    public ActorEditMode controller;
    public ActorSchematics actorSchematics;
    [HideInInspector]
    public ActorSchematicView view;
    [HideInInspector]
    public bool isBack = false;

    private void Start() {

        infoBoxHandler.message = actorSchematics.directoryName;

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        textFieldPopupHandler.finishCallback = text => {

            actorSchematics.directoryName = text;

            infoBoxHandler.message = actorSchematics.directoryName;

        };

        if (isBack) {
            icon.sprite = backArrowSprite;
        }

    }

    void Rename() {

        textFieldPopupHandler.OpenPopupTextField(actorSchematics.directoryName);

    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete Actor Schematic Folder", "Are you sure you would like to delete this actor schematic? " +
            "This will delete all schematics inside this folder. This cannot be undone.", () => {

            view.currentDirectory.subFolders.Remove(actorSchematics);

            view.RefreshView();

            return true;
        });

    }

    // - Unity Callbacks -
    public void OnClick() {

        view.SwitchDirectory(actorSchematics);

    }

    public void ReceiveDrag() {

        if (Main.draggingElement.TryGetComponent<ActorSchematicItemView>(out var viewItem)) {

            view.currentDirectory.schematics.Remove(viewItem.actorSchematic);

            actorSchematics.schematics.Add(viewItem.actorSchematic);

            Destroy(viewItem.gameObject);

        }

    }

}