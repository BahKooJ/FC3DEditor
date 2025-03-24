

using FCopParser;
using TMPro;
using UnityEngine;

public class TeamDataListItemView : MonoBehaviour {

    // - Unity Refs -
    public ContextMenuHandler contextMenu;
    public TMP_Text nameText;
    public TMP_InputField nameField;

    // - Parameters -
    [HideInInspector]
    public string teamName;
    [HideInInspector]
    public int id;
    [HideInInspector]
    public TeamDataView view;
    public FCopLevel level;

    void Start() {

        contextMenu.items = new() {
            ("Rename", Rename),
            ("Delete", Delete)
        };

        nameText.text = teamName;

    }

    public void Rename() {

        nameText.gameObject.SetActive(false);
        nameField.gameObject.SetActive(true);

        Main.ignoreAllInputs = true;

        nameField.text = nameText.text;
        nameField.Select();

    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete Team", "Are you sure you would like to delete this team?" +
            "Actors that use this team may no longer work properly. Only delete teams if you are sure it is unused. This cannot be undone.", () => {

                if (id == 0) return false;

                level.sceneActors.DeleteTeam(id);

                view.Refresh();

                return true;

            });

    }

    public void OnFinishNameType() {

        teamName = nameField.text;

        level.sceneActors.teams[id] = teamName;
        nameText.text = teamName;

        nameText.gameObject.SetActive(true);
        nameField.gameObject.SetActive(false);
        Main.ignoreAllInputs = false;

    }

}