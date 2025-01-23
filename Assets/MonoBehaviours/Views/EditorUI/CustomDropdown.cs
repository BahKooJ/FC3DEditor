

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// I'm at my limit with Unity's built in dropdown that I'm just gonna write my own because I think it'll be fast than trying to work with that nightmare
public class CustomDropdown : MonoBehaviour {

    // - Unity Refs -
    public GameObject listItem;
    public GameObject contentList;
    public GameObject scrollbar;
    public ScrollRect scrollRect;
    public Transform contentTransform;
    public TMP_Text label;
    public UnityEvent onValueChanged;

    public List<string> items = new();
    public List<GameObject> itemObjects = new();
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

        itemObjects.Add(obj);

    }

    public void AddItem(string item) {
        items.Add(item);
        CreateListItem(item, items.Count - 1);
    }

    public void Clear() {

        foreach (var obj in itemObjects) {
            Destroy(obj);
        }

        items.Clear();
        itemObjects.Clear();

    }

    public void ScrollToBottom() {

        scrollRect.normalizedPosition = new Vector2(0f, 0f);

    }

    public void OnSelectItem(int index) {
        value = index;
        UpdateLabel();
        contentList.SetActive(false);

        if (onValueChanged != null) {
            onValueChanged.Invoke();
        }

    }

    public void UpdateLabel() {
        label.text = items[value];
    }

    public void Open() {

        contentList.SetActive(true);

    }

    public void OnClick() {

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results) {

            if (result.gameObject == scrollbar) {
                return;
            }

        }

        contentList.SetActive(!contentList.activeSelf);

    }

}