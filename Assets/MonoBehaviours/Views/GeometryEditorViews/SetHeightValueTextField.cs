
using System;
using System.Linq;
using TMPro;
using UnityEngine;

class SetHeightValueTextField: MonoBehaviour {

    public HeightMapEditMode controller;
    public HeightMapChannelPoint selelctedHeightObject;
    public TMP_InputField field;

    public bool preSelect = false;

    void Start() {

        if (preSelect) {
            field.Select();
        }

        field.text = selelctedHeightObject.heightPoints.GetTruePoint(selelctedHeightObject.channel).ToString();

        ((RectTransform)transform).anchoredPosition = Camera.main.WorldToScreenPoint(selelctedHeightObject.transform.position);

    }

    public void Update() {

        if (controller.main.editMode != controller) {
            Destroy(this.gameObject);
        }

    }

    public void OnFinished() {

        if (!preSelect) {
            return;
        }

        if (field == null) {
            return;
        }

        if (field.text.Count() != 0) {

            foreach (var point in controller.heightPointObjects) {

                if (point.isSelected) {

                    try {
                        point.heightPoints.SetPoint(Int32.Parse(field.text), point.channel);
                    } catch (FormatException) {

                        controller.UnselectHeights();
                        HeightMapEditMode.selectedSection.RefreshMesh();

                        Destroy(this.gameObject);

                        return;
                    }

                    point.RefreshHeight();

                    if (HeightMapEditMode.keepHeightsOnTop) {
                        point.KeepHigherChannelsOnTop();
                    }

                }

            }

        }

        //controller.UnselectAndRefreshHeightPoints();
        HeightMapEditMode.selectedSection.RefreshMesh();

        Destroy(this.gameObject);


    }

}