using FCopParser;
using UnityEngine;

public class HeightMapChannelPoint : MonoBehaviour {

    // Prefabs
    public GameObject setHeightTextField;

    // View refs
    public Material boxMaterial;
    public Material sphereMaterial;
    public MeshRenderer boxRender;
    public MeshRenderer sphereRender;
    public BoxCollider boxCollider;

    public int x;
    public int y;
    public bool isStatic = false;
    public HeightMapEditMode controller;
    public HeightPoints heightPoints;
    public int channel;
    public LevelMesh section;

    public bool isSelected = false;

    public bool preInitSelect = false;

    SetHeightValueTextField dragField = null;

    bool click = false;

    Vector3 previousMousePosition;

    void Start() {

        boxMaterial = boxRender.material;
        sphereMaterial = sphereRender.material;

        if (preInitSelect) {
            boxMaterial.color = Color.white;
            sphereMaterial.color = Color.white;

            boxRender.enabled = true;
            sphereRender.enabled = false;

        } else {
            ResetColors();
        }

    }


    void Update() {

        if (isStatic) {
            return;
        }
        
        if (click) {

            if (Controls.OnUp("Select")) {

                if (dragField != null) {
                    Destroy(dragField.gameObject);
                    dragField = null;
                }

                if (!isSelected) {
                    boxRender.enabled = false;
                    sphereRender.enabled = true;
                }

                click = false;
                section.RefreshMesh();

            }

            var distance = (Input.mousePosition.y - previousMousePosition.y) / 40f;

            if (isSelected) {
                controller.MoveAllHeights(distance);
            } else {
                MoveHeight(distance);
            }

            previousMousePosition = Input.mousePosition;

        }

    }

    public void ResetColors() {
        switch (channel) {
            case 1:
                boxMaterial.color = Color.blue;
                sphereMaterial.color = Color.blue;
                break;
            case 2:
                boxMaterial.color = Color.green;
                sphereMaterial.color = Color.green;
                break;
            case 3:
                boxMaterial.color = Color.red;
                sphereMaterial.color = Color.red;
                break;
        }
    }

    public void RefreshHeight() {
        transform.position = new Vector3(transform.position.x, heightPoints.GetPoint(channel), transform.position.z);
    }

    public void SelectOrDeSelect() {

        if (isSelected) {
            DeSelect();
        } else {
            Select();
        }

    }

    public void Select() {

        isSelected = true;

        if (boxMaterial == null) {
            preInitSelect = true;
            return;
        }
        
        boxMaterial.color = Color.white;
        sphereMaterial.color = Color.white;

        boxRender.enabled = true;
        sphereRender.enabled = false;

    }

    public void DeSelect() {

        isSelected = false;

        boxRender.enabled = false;
        sphereRender.enabled = true;

        ResetColors();

    }

    public void Click() {

        controller.AddHeightMapSaveStateCounterActionFromClick(this);

        var obj = Instantiate(setHeightTextField);

        obj.transform.SetParent(controller.main.canvas.transform, false);

        var script = obj.GetComponent<SetHeightValueTextField>();

        script.controller = controller;
        script.selelctedHeightObject = this;

        dragField = script;

        click = true;
        previousMousePosition = Input.mousePosition;

        boxRender.enabled = true;
        sphereRender.enabled = false;

    }

    public void ChangeExactHeight() {

        Select();

        var mainUI = FindObjectOfType<HeightMapEditPanelView>();

        var obj = Instantiate(setHeightTextField);

        obj.transform.SetParent(mainUI.GetComponentInParent<Canvas>().rootCanvas.transform, false);

        var script = obj.GetComponent<SetHeightValueTextField>();

        script.controller = controller;
        script.preSelect = true;
        script.selelctedHeightObject = this;

    }

    #region Model Mutating

    public void MoveHeight(float distance) {

        heightPoints.AddToPoint(distance, channel);

        transform.position = new Vector3(transform.position.x, heightPoints.GetPoint(channel), transform.position.z);

        if (HeightMapEditMode.keepHeightsOnTop) {

            KeepHigherChannelsOnTop();

        }

        if (dragField != null) {
            dragField.field.text = heightPoints.GetTruePoint(channel).ToString();
            ((RectTransform)dragField.transform).anchoredPosition = Camera.main.WorldToScreenPoint(transform.position);
        }

    }

    public void KeepHigherChannelsOnTop() {

        var padding = 8;
        var gameCoordsPadding = padding / HeightPoints.multiplyer;

        if (channel == 3) {
            return;
        }

        if (channel == 1) {

            if (heightPoints.GetPoint(channel) + gameCoordsPadding > heightPoints.GetPoint(2)) {
                heightPoints.SetPoint(heightPoints.GetTruePoint(channel) + padding, 2);
            }

            if (heightPoints.GetPoint(2) + gameCoordsPadding > heightPoints.GetPoint(3)) {
                heightPoints.SetPoint(heightPoints.GetTruePoint(2) + padding, 3);
            }

        }
        if (channel == 2) {

            if (heightPoints.GetPoint(channel) + gameCoordsPadding > heightPoints.GetPoint(3)) {
                heightPoints.SetPoint(heightPoints.GetTruePoint(channel) + padding, 3);
            }

        }

        foreach (var point in controller.heightPointObjects) {
            point.RefreshHeight();
        }

    }

    #endregion

}
