
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

        ((RectTransform)transform).anchoredPosition = controller.main.WorldToScreenPointScaled(selelctedHeightObject.transform.position);

    }

    public void Update() {

        if (Main.editMode != controller) {
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

            try {
                controller.SetHeightsWithValue(Int32.Parse(field.text));
            } catch (FormatException) {

                controller.UnselectHeights();
                HeightMapEditMode.selectedSection.RefreshMesh();

                Destroy(this.gameObject);

                return;

            }

        }

        //controller.UnselectAndRefreshHeightPoints();
        HeightMapEditMode.selectedSection.RefreshMesh();

        Destroy(this.gameObject);


    }

}