
using FCopParser;
using System;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;

class SetHeightValueTextField: MonoBehaviour {

    public GeometryEditMode controller;
    public HeightMapChannelPoint selelctedHeightObject;
    public TMP_InputField field;

    void Start() {

        field = GetComponent<TMP_InputField>();

        field.Select();
        
        field.text = selelctedHeightObject.heightPoints.GetTruePoint(selelctedHeightObject.channel).ToString();

        ((RectTransform)transform).anchoredPosition = Input.mousePosition;

    }

    public void OnFinished() {

        if (field == null) {
            return;
        }

        if (field.text.Count() != 0) {

            foreach (var point in controller.heightPointObjects) {

                if (point.isSelected) {

                    try {
                        point.heightPoints.SetPoint(Int32.Parse(field.text), point.channel);
                    } catch (FormatException) {

                        controller.UnselectAndRefreshHeightPoints();
                        controller.selectedSection.RefreshMesh();

                        controller.RefreshSelectedOverlays();

                        Destroy(this.gameObject);

                        return;
                    }

                    point.transform.position = new Vector3(point.transform.position.x, point.heightPoints.GetPoint(point.channel), point.transform.position.z);

                }

            }

        }

        //controller.UnselectAndRefreshHeightPoints();
        controller.selectedSection.RefreshMesh();

        controller.RefreshSelectedOverlays();

        Destroy(this.gameObject);


    }

}