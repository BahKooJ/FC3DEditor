

using FCopParser;
using TMPro;
using UnityEngine;

public class ValueActorPropertyItemView : MonoBehaviour {

    //View refs
    public TMP_Text nameText;
    public TMP_InputField valueField;

    public ValueActorProperty property;
    public FCopActor actor;
    public ActorEditMode controller;

    void Start() {
        
        nameText.text = property.name;
        valueField.text = property.value.ToString();

    }

}