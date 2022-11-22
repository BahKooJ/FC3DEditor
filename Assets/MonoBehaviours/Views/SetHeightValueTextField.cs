

using FCopParser;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

class SetHeightValueTextField: MonoBehaviour {

    public HeightPoint heightPoint;
    public int channel;
    public HeightMapChannelPoint geoPoint;
    public LevelMesh section;

    public TMP_InputField field;

    void Start() {

        field = GetComponent<TMP_InputField>();

        field.Select();
        field.text = heightPoint.GetTruePoint(channel).ToString();

        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = Vector3.one;

        var mousePosition = Input.mousePosition;

        mousePosition.y = -mousePosition.y;
        mousePosition.x /= 100;

        transform.localPosition = mousePosition;

    }

    public void OnFinished() {

        if (field.text.Count() == 0) {

            field.text = heightPoint.GetTruePoint(channel).ToString();

            return;
        
        }


        heightPoint.SetPoint(Int32.Parse(field.text), channel);

        geoPoint.transform.position = new Vector3(geoPoint.transform.position.x, heightPoint.GetPoint(channel), geoPoint.transform.position.z);

        section.RefreshMesh();

        Destroy(this.gameObject);


    }

}