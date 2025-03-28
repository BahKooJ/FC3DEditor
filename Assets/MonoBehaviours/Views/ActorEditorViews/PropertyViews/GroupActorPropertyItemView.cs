

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroupActorPropertyItemView : MonoBehaviour {

    //View refs
    public TMP_Text nameText;
    public Image dropdownIcon;

    // - Parameters -
    [HideInInspector]
    public string commonName;
    public List<ActorProperty> properties;
    public FCopActor actor;
    public ActorEditMode controller;
    [HideInInspector]
    public ActorPropertiesView view;

    [HideInInspector]
    public List<ActorPropertyItemView> initedProperties = new();

    private void Start() {
        
        nameText.text = commonName;

        if (view.openedGroups.Contains(commonName)) {
            OnClick();
        }

    }

    public void OnClick() {

        if (initedProperties.Count > 0) {

            foreach (var prop in initedProperties) {
                Destroy(prop.gameObject);
            }

            initedProperties.Clear();

            dropdownIcon.transform.localEulerAngles = new Vector3(0f, 0f, -90f);

            view.openedGroups.Remove(commonName);

        }
        else {

            var i = 1;
            foreach (var prop in properties) {

                var obj = view.InitProperty(prop);

                if (obj != null) {
                    initedProperties.Add(obj);
                    obj.transform.SetSiblingIndex(transform.GetSiblingIndex() + i);
                    i++;
                }

            }

            dropdownIcon.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

            view.openedGroups.Add(commonName);

        }

    }

}