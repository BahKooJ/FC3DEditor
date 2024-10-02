

using TMPro;
using UnityEngine;

public class CustomDropdownListItem : MonoBehaviour {

    // - Unity Refs -
    public CustomDropdown parent;
    public TMP_Text label;

    public string text;
    public int index;

    private void Start() {
        
        label.text = text;

    }

    public void OnClick() {

        parent.OnSelectItem(index);

    }

}