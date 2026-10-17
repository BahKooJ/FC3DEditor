

using TMPro;
using UnityEngine;

public class NavMeshListItemView : MonoBehaviour {

    // - Unity Refs -
    public CustomDropdownListItem dropdownListItem;
    public NavMeshEditPanel view;
    public ContextMenuHandler contextMenu;
    public TMP_Text label;
    public TMP_InputField inputField;

    private void Start() {

        contextMenu.items = new() {
            ("Rename", StartRenaming),
            ("Clear", Clear),
            ("Delete", Delete)
        };

    }

    public void StartRenaming() {

        Main.ignoreAllInputs = true;

        inputField.gameObject.SetActive(true);

        inputField.text = label.text;

        inputField.Select();

    }

    public void StartRenaming(string defaultName) {

        Main.ignoreAllInputs = true;

        inputField.gameObject.SetActive(true);

        inputField.text = defaultName;

        inputField.Select();

    }

    void Clear() {

        view.controller.ClearNavMesh(dropdownListItem.index);

    }

    void Delete() {

        view.controller.RemoveNavMesh(dropdownListItem.index);

    }

    public void OnFinshRenaming() {

        Main.ignoreAllInputs = false;

        label.text = inputField.text;

        view.controller.RenameNavMesh(label.text, dropdownListItem.index);
        dropdownListItem.parent.items[dropdownListItem.index] = label.text;
        dropdownListItem.parent.UpdateLabel();

        inputField.gameObject.SetActive(false);

    }

}