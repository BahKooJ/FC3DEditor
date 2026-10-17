

using FCopParser;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class UniversalUVMapperView : MonoBehaviour {

    // - Prefabs -
    public GameObject uvPrefab;

    // - View Refs -
    public GameObject texturePalette;
    public GameObject texturePaletteImage;
    public TMP_Dropdown textureDropdown;
    public UniversalUVLines uvLines;

    // - Parameters -
    [HideInInspector]
    public Main main;
    [HideInInspector]
    public int uvCount = 4;
    [HideInInspector]
    public int bmpID = 0;
    [HideInInspector]
    public List<int> uvs = new List<int>();
    [HideInInspector]
    public bool forceRect = false;
    public Action<List<int>, int> onFinishCallback = (p1, p2) => { };

    public FCopLevel level;
    [HideInInspector]
    public List<UniversalUVView> uvObjects = new();

    void Start() {
        


    }

    void Update() {

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0) {

            if (IsCursorInTexturePallete()) {

                var scale = texturePaletteImage.transform.localScale;

                scale.x += axis * 4;
                scale.y += axis * 4;

                if (scale.x > 0f) {

                    texturePaletteImage.transform.localScale = scale;

                    var position = texturePaletteImage.transform.localPosition;

                    position.x -= 265 * (axis * 2);
                    position.y -= 265 * (axis * 2);

                    texturePaletteImage.transform.localPosition = position;

                }

            }

        }

        if (Controls.IsDown("DragPalette")) {

            if (IsCursorInTexturePallete()) {

                var position = texturePaletteImage.transform.localPosition;

                position.x += Input.GetAxis("Mouse X") * 18;
                position.y += Input.GetAxis("Mouse Y") * 18;

                texturePaletteImage.transform.localPosition = position;

            }

        }

    }

    public void Refresh() {

        level = main.level;

        textureDropdown.ClearOptions();
        foreach (var texture in level.textures) {

            textureDropdown.AddOptions(new List<string>() { texture.name });

        }

        textureDropdown.value = bmpID;

        texturePaletteImage.GetComponent<Image>().sprite = main.bmpTextures[bmpID];

        if (uvs.Count == 0) {

            uvs.Clear();

            foreach (var _ in Enumerable.Range(0, uvCount)) {

                uvs.Add(0);

            }

        }
        else {

            uvCount = uvs.Count;

        }

        foreach (var uvObj in uvObjects) {
            Destroy(uvObj.gameObject);
        }

        uvObjects.Clear();

        foreach (var i in Enumerable.Range(0, uvs.Count)) {

            var obj = Instantiate(uvPrefab);
            obj.transform.SetParent(texturePaletteImage.transform, false);

            var uvView = obj.GetComponent<UniversalUVView>();
            uvView.view = this;
            uvView.index = i;
            uvView.ChangePosition(TextureCoordinate.GetXPixel(uvs[i]), TextureCoordinate.GetYPixel(uvs[i]), false);

            uvObjects.Add(uvView);

        }

        uvLines.view = this;
        uvLines.Refresh();

    }

    public void RefreshUVPositions() {

        foreach (var uv in uvObjects) {
            uv.ChangePosition(TextureCoordinate.GetXPixel(uvs[uv.index]), TextureCoordinate.GetYPixel(uvs[uv.index]), false);
        }

    }

    public bool IsCursorInTexturePallete() {

        Vector2 pointOnPallete = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)texturePalette.transform, Input.mousePosition, Camera.main, out pointOnPallete);

        var palleteRect = (RectTransform)texturePalette.transform;

        return pointOnPallete.x < palleteRect.rect.width && -pointOnPallete.y < palleteRect.rect.height && pointOnPallete.x > -1 && pointOnPallete.y < 1;

    }

    public void OnChangeDropdown() {

        bmpID = textureDropdown.value;
        texturePaletteImage.GetComponent<Image>().sprite = main.bmpTextures[bmpID];

    }

    public void OnClickDone() {

        onFinishCallback(uvs, bmpID);

    }

}