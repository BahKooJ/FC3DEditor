

using FCopParser;
using System;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;

class SetHeightValueTextField: MonoBehaviour {

    public Main controller;

    public TMP_InputField field;

    void Start() {

        field = GetComponent<TMP_InputField>();

        field.Select();

        var height = -200;
        var samePoints = true;

        HeightMapChannelPoint firstSelectedHeight = null;

        foreach (var point in controller.heightPointObjects) {

            if (point.isSelected) {

                if (height == -200) {

                    height = point.heightPoint.GetTruePoint(point.channel);

                    firstSelectedHeight = point;

                } else if (height != point.heightPoint.GetTruePoint(point.channel)) {
                    samePoints = false;
                }

            }

        }

        if (samePoints) {
            field.text = firstSelectedHeight.heightPoint.GetTruePoint(firstSelectedHeight.channel).ToString();
        }

        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = Vector3.one;

        var mousePosition = Input.mousePosition;

        mousePosition.y = -mousePosition.y;
        mousePosition.x /= 100;

        transform.localPosition = mousePosition;

    }

    public void OnFinished() {

        if (field == null) {
            Destroy(this.gameObject);
        }

        if (field.text.Count() != 0) {

            foreach (var point in controller.heightPointObjects) {

                if (point.isSelected) {
                    point.heightPoint.SetPoint(Int32.Parse(field.text), point.channel);
                    point.transform.position = new Vector3(point.transform.position.x, point.heightPoint.GetPoint(point.channel), point.transform.position.z);

                }

            }

        }

        controller.UnselectAndRefreshHeightPoints();
        controller.selectedSection.RefreshMesh();

        Destroy(this.gameObject);


    }

}