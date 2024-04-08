

using FCopParser;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class TextureTransparentMapper : MonoBehaviour {

    public TextureEditMode controller;
    public TextureUVMapper uvMapper;

    public Sprite drawOpaqueSprite;
    public Sprite drawTransparentSprite;

    // View Refs
    public RectTransform drawingCursor;
    public Image drawColorButton;

    public Sprite transparentMap;
    public int bmpID;
    bool onlyDrawAlpha = false;

    int penSize = 1;
    bool drawingTransparency = true;

    void Start() {

        drawingCursor.gameObject.SetActive(false);

    }

    void Update() {

        if (!uvMapper.editTransparency) {
            return;
        }

        Draw();

        if (Input.GetKeyDown(KeyCode.LeftBracket)) {
            if (penSize > 1) {
                penSize--;
            }
            drawingCursor.sizeDelta = new Vector2(penSize, penSize);
        }
        if (Input.GetKeyDown(KeyCode.RightBracket)) {
            
            penSize++;

            drawingCursor.sizeDelta = new Vector2(penSize, penSize);
        }

    }

    void Draw() {

        if (!uvMapper.IsCursorInTexturePallete()) {
            return;
        }

        Vector2 pointOnPallete = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)uvMapper.texturePaletteImage.transform, Input.mousePosition, Camera.main, out pointOnPallete);

        if (pointOnPallete.x > 256 || pointOnPallete.y > 256) {
            return;
        }

        if (pointOnPallete.x < 0 || pointOnPallete.y < 0) {
            return;
        }

        if ((penSize - 1) % 2 != 0 && penSize != 1) {
            var offsetPoint = pointOnPallete;
            offsetPoint.x += 0.5f;
            offsetPoint.y += 0.5f;
            drawingCursor.anchoredPosition = offsetPoint;
        } else {
            drawingCursor.anchoredPosition = pointOnPallete;
        }

        if (Input.GetMouseButton(0)) {

            var color = drawingTransparency ? new Color(1f, 1f, 1f, 0.5f) : Color.white;

            if (penSize == 1) {

                var existingColor = transparentMap.texture.GetPixel((int)pointOnPallete.x, (int)pointOnPallete.y);

                if (existingColor != Color.black) {
                    existingColor.a = color.a;
                }

                transparentMap.texture.SetPixel((int)pointOnPallete.x, (int)pointOnPallete.y, existingColor);

            } else {

                var x = (int)pointOnPallete.x;
                var y = (int)pointOnPallete.y;

                var centerX = x - ((penSize - 1) / 2);
                var centerY = y - ((penSize - 1) / 2);
                var width = penSize;
                var height = penSize;

                if (centerX < 0) {
                    width += centerX;
                    centerX = 0;
                }
                if (centerY < 0) {
                    height += centerY;
                    centerY = 0;
                }
                if (centerX + width > 256) {
                    width -= (centerX + width) - 256;
                }
                if (centerY + height > 256) {
                    height -= (centerY + height) - 256;
                }

                var colors = transparentMap.texture.GetPixels(centerX, centerY, width, height);

                foreach (var i in Enumerable.Range(0, colors.Count())) {

                    if (colors[i] != Color.black) {
                        colors[i].a = color.a;
                    }

                }

                transparentMap.texture.SetPixels(centerX, centerY, width, height, colors.ToArray());

            }

            transparentMap.texture.Apply();

        }
    
    }

    public void CreateTransparentMap() {

        controller = uvMapper.controller;

        var bmpTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        
        bmpTexture.filterMode = FilterMode.Point;

        if (onlyDrawAlpha) {
            bmpTexture.LoadRawTextureData(controller.main.level.textures[bmpID].ConvertToAlphaMap());
        } else {
            bmpTexture.LoadRawTextureData(controller.main.level.textures[bmpID].ConvertToARGB32());
        }

        bmpTexture.Apply();

        transparentMap = Sprite.Create(bmpTexture, new Rect(0, 0, 256, 256), Vector2.zero);

        uvMapper.texturePaletteImage.GetComponent<Image>().sprite = transparentMap;

        if (drawingTransparency) {
            drawColorButton.sprite = drawTransparentSprite;
        }
        else {
            drawColorButton.sprite = drawOpaqueSprite;
        }

    }

    public void Apply() {

        var argb32Data = transparentMap.texture.GetRawTextureData();

        var texture = controller.main.level.textures[bmpID];

        foreach (var i in Enumerable.Range(0, argb32Data.Count() / 4)) {

            var xrgb = new XRGB555(new BitArray(texture.bitmap.GetRange(i * 2, 2).ToArray()));

            var alphaValue = argb32Data[i * 4];

            if (alphaValue < 255 && alphaValue > 0) {
                xrgb.x = true;
            } 
            else if (alphaValue == 255) {
                xrgb.x = false;
            }

            texture.bitmap.RemoveRange(i * 2, 2);

            texture.bitmap.InsertRange(i * 2, xrgb.Compile());

        }

    }

    public void OnClickDrawColorButton() {

        drawingTransparency = !drawingTransparency;

        if (drawingTransparency) {
            drawColorButton.sprite = drawTransparentSprite;
        } else {
            drawColorButton.sprite = drawOpaqueSprite;
        }

    }

    public void OnClickShowRGBButton() {



    }

}