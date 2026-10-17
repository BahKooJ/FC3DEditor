

using FCopParser;
using TMPro;
using UnityEngine;

public class GroupDataListItemView : MonoBehaviour {

    // - Unity Refs -
    public ContextMenuHandler contextMenu;
    public TMP_Text nameText;
    public TMP_Text typeText;
    public TMP_InputField nameField;

    // - Parameters -
    [HideInInspector]
    public string groupName;
    [HideInInspector]
    public int id;
    [HideInInspector]
    public GroupDataView view;
    public FCopLevel level;

    void Start() {

        contextMenu.items = new() {
            ("Rename", Rename),
            ("Delete", Delete)
        };

        nameText.text = groupName;

        var types = level.sceneActors.FindAllDerviedTypesFromGroup(id);

        if (types.Count != 0) {

            var baseType = types[0];

            if (FCopActorBehavior.behaviorsByType.TryGetValue(baseType, out var type)) {
                typeText.text = Utils.AddSpacesToString(type.ToString());
            }
            else {
                typeText.text = Utils.AddSpacesToString(baseType.ToString().Remove(0, "FCopParser.FCop".Length));
            }

        }
        else {
            typeText.text = "No Actors";
        }


    }

    public void Rename() {

        nameText.gameObject.SetActive(false);
        nameField.gameObject.SetActive(true);

        Main.ignoreAllInputs = true;

        nameField.text = nameText.text;
        nameField.Select();

    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete Group", "Are you sure you would like to delete this group? " +
            "Scripts that use this group may no longer work properly. Only delete groups if you are sure it is unused. This cannot be undone.", () => {

                if (id == 0) return false;

                level.sceneActors.DeleteGroup(id);

                view.Refresh();

                return true;

            });

    }

    public void OnFinishNameType() {

        groupName = nameField.text;

        if (groupName == "") {
            groupName = "Group";
            nameField.text = "Group";
        }

        level.sceneActors.scriptGroup[id] = groupName;
        nameText.text = groupName;

        nameText.gameObject.SetActive(true);
        nameField.gameObject.SetActive(false);
        Main.ignoreAllInputs = false;

    }

}