

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// I'm at my limit with Unity's built in dropdown that I'm just gonna write my own because I think it'll be fast than trying to work with that nightmare
public class CustomDropdown : MonoBehaviour {

    // - Unity Refs -
    public GameObject listItem;
    public GameObject contentList;
    public Transform contentTransform;
    public TMP_Text label;

    public List<string> items = new();
    public int value = 0;

    private void Start() {

        UpdateLabel();

        foreach (Transform tran in contentTransform.Cast<Transform>().ToList().Skip(1)) {
            Destroy(tran.gameObject);
        }

        var i = 0;
        foreach (var item in items) {

            CreateListItem(item, i);

            i++;
        }

    }

    void CreateListItem(string item, int index) {

        var obj = Instantiate(listItem);
        obj.transform.SetParent(contentTransform, false);

        var script = obj.GetComponent<CustomDropdownListItem>();
        script.text = item;
        script.index = index;

        obj.SetActive(true);

    }

    public void AddItem(string item) {
        items.Add(item);
        CreateListItem(item, items.Count - 1);
    }

    public void OnSelectItem(int index) {
        value = index;
        UpdateLabel();
        contentList.SetActive(false);
    }

    public void UpdateLabel() {
        label.text = items[value];
    }

    public void OnClick() {

        contentList.SetActive(!contentList.activeSelf);

    }

}