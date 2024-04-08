
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using FCopParser;

public class TextureUVMapper : MonoBehaviour {

    public bool editTransparency = false;

    public TextureEditMode controller;

    public GameObject textureCoordinatePoint;

    // --View References--
    public TextureTransparentMapper transparentMapper;

    public GameObject texturePaletteDropdown;
    public GameObject texturePalette;
    public GameObject texturePaletteImage;
    public TextureCoordinatesLines textureLines;
    public GameObject uvMapperTools;
    public GameObject transparentMapperTools;
    public GameObject paletteConfirmationButtons;
    public GameObject editPaletteButton;
    public GameObject generageColorPaletteButton;
    public TMP_Dropdown textureTypeDropDown;

    public ContextMenuHandler exportcontextMenu;
    public ContextMenuHandler importcontextMenu;

    // Vector Animation View Refs
    public GameObject vectorAnimationProperties;
    public Slider xVectorSlider;
    public TMP_Text xVectorText;
    public Slider yVectorSlider;
    public TMP_Text yVectorText;

    // Frame Animation View Refs
    public GameObject frameAnimationPanel;
    public Transform framesContent;
    public Slider frameAnimationSpeedSlider;
    public TMP_Text frameAnimationSpeedText;

    // Frame Animation Prefabs
    public GameObject frameListItem;

    public List<FrameItemView> frameItems;
    public int frameSelected = -1;

    public int bmpID;

    bool preventDropdownCallback = false;

    void ScaleToScreen() {

        var screenWidth = Screen.width * 0.40f;
        var screenHeight = Screen.height * 0.80f;

        var multiplierWidth = screenWidth / ((RectTransform)transform).rect.width;

        var multiplierHeight = screenHeight / ((RectTransform)transform).rect.height;

        var multiplier = new float[] { multiplierWidth, multiplierHeight }.Min();

        transform.localScale = new Vector3(multiplier, multiplier, multiplier);

    }

    void Start() {

        // For whatever reason, despite being set to -1 on declaration, it's 0 and I don't understand why
        frameSelected = -1;

        InitView();

        ScaleToScreen();

        exportcontextMenu.items = new() {
            ("Export Bitmap", ExportTexture),
            ("Export Color Palette", ExportColorPalette),
            ("Export Cbmp (BMP and Palette)", ExportCbmp)
        };

        importcontextMenu.items = new() {
            ("Import Bitmap", ImportTexture),
            ("Import Color Palette", ImportColorPalette),
            ("Import Cbmp (BMP and Palette)", ImportCbmp)
        };

    }

    void Update() {

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0) {

            if (IsCursorInTexturePallete()) {

                var scale = texturePaletteImage.transform.localScale;

                scale.x += axis * 4;
                scale.y += axis * 4;

                texturePaletteImage.transform.localScale = scale;

                var position = texturePaletteImage.transform.localPosition;

                position.x -= 265 * (axis * 2);
                position.y -= 265 * (axis * 2);

                texturePaletteImage.transform.localPosition = position;

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

    public bool IsCursorInTexturePallete() {

        Vector2 pointOnPallete = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)texturePalette.transform, Input.mousePosition, Camera.main, out pointOnPallete);

        var palleteRect = (RectTransform)texturePalette.transform;

        return pointOnPallete.x < palleteRect.rect.width && -pointOnPallete.y < palleteRect.rect.height && pointOnPallete.x > -1 && pointOnPallete.y < 1;

    }

    void InitTexturePallette() {

        if (editTransparency) {
            transparentMapper.bmpID = texturePaletteDropdown.GetComponent<TMP_Dropdown>().value;
            transparentMapper.CreateTransparentMap();
            return;
        }

        if (controller.selectedTiles.Count == 0) {

            var bmpID = texturePaletteDropdown.GetComponent<TMP_Dropdown>().value;

            texturePaletteImage.GetComponent<Image>().sprite = controller.main.bmpTextures[bmpID];

            return;
        }

        var sameTexture = true;

        // If multiple tiles are select this checks to see if they're all the same texture
        bmpID = controller.selectedTiles[0].texturePalette;
        foreach (var tile in controller.selectedTiles) {

            if (bmpID != tile.texturePalette) {
                sameTexture = false;
                bmpID = -1;
                break;
            }

        }

        if (sameTexture) {

            var bmpID = controller.selectedTiles[0].texturePalette;

            texturePaletteDropdown.GetComponent<TMP_Dropdown>().value = bmpID;

            texturePaletteImage.GetComponent<Image>().sprite = controller.main.bmpTextures[bmpID];

        }

    }

    void InitView() {

        preventDropdownCallback = true;

        InitTexturePallette();

        uvMapperTools.SetActive(controller.selectedTiles.Count != 0);

        if (controller.selectedTiles.Count != 0) {
            var tile = controller.selectedTiles[0];

            if (tile.isVectorAnimated) {
                textureTypeDropDown.value = (int)TextureType.VectorAnimated;
                StartEditingVectorAnimation();
            }
            else if (tile.animatedUVs.Count != 0) {
                textureTypeDropDown.value = (int)TextureType.FrameAnimated;
                StartEditingFrameAnimation();
            }
            else {
                textureTypeDropDown.value = 0;
            }

        }

        preventDropdownCallback = false;

    }

    public void RefreshView() {

        preventDropdownCallback = true;

        if (editTransparency) {
            return;
        }

        InitTexturePallette();

        uvMapperTools.SetActive(controller.selectedTiles.Count != 0);

        textureLines.gameObject.SetActive(controller.selectedTiles.Count != 0);

        EndEditingVectorAnimation();
        EndEditingFrameAnimation();

        if (controller.selectedTiles.Count != 0) {
            var tile = controller.selectedTiles[0];

            if (tile.isVectorAnimated) {
                textureTypeDropDown.value = (int)TextureType.VectorAnimated;
                StartEditingVectorAnimation();
            }
            else if (tile.animatedUVs.Count != 0) {
                textureTypeDropDown.value = (int)TextureType.FrameAnimated;
                StartEditingFrameAnimation();
            }
            else {
                textureTypeDropDown.value = 0;
            }

        }

        textureLines.ReInit();

        preventDropdownCallback = false;

    }

    void EditTransparency() {

        if (editTransparency) {

            editTransparency = false;

            paletteConfirmationButtons.SetActive(false);
            transparentMapperTools.SetActive(false);
            editPaletteButton.SetActive(true);

            transparentMapper.drawingCursor.gameObject.SetActive(false);

            InitView();

            return;
        }

        EndEditingVectorAnimation();
        EndEditingFrameAnimation();

        editTransparency = true;

        transparentMapper.drawingCursor.gameObject.SetActive(true);

        uvMapperTools.SetActive(false);

        textureLines.ReInit();

        textureLines.gameObject.SetActive(false);

        uvMapperTools.SetActive(false);
        transparentMapperTools.SetActive(true);

        paletteConfirmationButtons.SetActive(true);
        editPaletteButton.SetActive(false);

        InitTexturePallette();

    }

    void StartEditingVectorAnimation() {

        vectorAnimationProperties.SetActive(true);

        xVectorSlider.value = controller.selectedSection.section.animationVector.x;
        yVectorSlider.value = controller.selectedSection.section.animationVector.y;
        xVectorText.text = controller.selectedSection.section.animationVector.x.ToString();
        yVectorText.text = controller.selectedSection.section.animationVector.y.ToString();

        if (controller.selectedSection.section.animationVector.x == 0) {
            xVectorText.text = "Disabled";
        }
        if (controller.selectedSection.section.animationVector.y == 0) {
            yVectorText.text = "Disabled";
        }

        textureLines.ReInit();

    }

    void EndEditingVectorAnimation() {

        vectorAnimationProperties.SetActive(false);

        textureLines.ReInit();

    }

    void StartEditingFrameAnimation(int startingFrame = 0) {

        if (controller.selectedTiles.Count == 0) {
            return;
        }

        frameSelected = startingFrame;

        var tile = controller.selectedTiles[0];

        generageColorPaletteButton.SetActive(false);
        editPaletteButton.SetActive(false);

        frameAnimationPanel.SetActive(true);

        // This method is called twice because of a Unity event handeler
        // so it just clears the existing items to not double add.
        foreach (var item in frameItems) {
            Destroy(item.gameObject);
        }

        frameItems.Clear();

        foreach (var i in Enumerable.Range(0, tile.GetFrameCount())) {
            var obj = Instantiate(frameListItem);
            obj.transform.SetParent(framesContent.transform, false);
            obj.SetActive(true);
            var frameItem = obj.GetComponent<FrameItemView>();
            frameItem.view = this;
            frameItem.controller = controller;
            frameItem.tile = tile;
            frameItem.frame = i;

            frameItems.Add(frameItem);

        }

        frameAnimationSpeedText.text = tile.animationSpeed.ToString();
        frameAnimationSpeedSlider.value = tile.animationSpeed;

    }

    void EndEditingFrameAnimation() {

        generageColorPaletteButton.SetActive(true);
        editPaletteButton.SetActive(true);

        frameAnimationPanel.SetActive(false);

        foreach (var item in frameItems) {
            Destroy(item.gameObject);
        }

        frameItems.Clear();

        frameSelected = -1;

        textureLines.ReInit();

    }

    public void RefreshUVFrameItems() {

        foreach (var item in frameItems) {
            Destroy(item.gameObject);
        }

        frameItems.Clear();

        var tile = controller.selectedTiles[0];

        foreach (var i in Enumerable.Range(0, tile.animatedUVs.Count / 4)) {
            var obj = Instantiate(frameListItem);
            obj.transform.SetParent(framesContent.transform, false);
            obj.SetActive(true);
            var frameItem = obj.GetComponent<FrameItemView>();
            frameItem.view = this;
            frameItem.controller = controller;
            frameItem.tile = tile;
            frameItem.frame = i;

            frameItems.Add(frameItem);

        }

    }

    public void ClickedFrameItem(int frame) {
        EndEditingFrameAnimation();
        StartEditingFrameAnimation(frame);
        textureLines.ReInit();
    }

    // --Event Handlers--

    public void CloseWindow() {
        controller.selectedSection.RefreshMesh();
        Destroy(gameObject);
    }

    public void OnChangeTexturePalleteValue() {

        if (editTransparency || controller.selectedTiles.Count == 0) {
            InitTexturePallette();
            return;
        }

        controller.ChangeTexturePallette(texturePaletteDropdown.GetComponent<TMP_Dropdown>().value);

        texturePaletteDropdown.GetComponent<TMP_Dropdown>().value = controller.selectedTiles[0].texturePalette;

        InitTexturePallette();

        controller.RefreshTileOverlayTexture();

        if (controller.selectedTiles[0].GetFrameCount() > 0) {
            RefreshUVFrameItems();
        }

    }

    public void OnClickRotateClockwise() {

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        int index = 1;
        foreach (var point in textureLines.points) {

            var script = point.GetComponent<TextureCoordinatePoint>();

            if (index == oldPoints.Count) {

                var vector = oldPoints[0];

                script.ChangePosition((int)vector.x, (int)vector.y);

            } else {

                var vector = oldPoints[index];

                script.ChangePosition((int)vector.x, (int)vector.y);

            }

            index++;

        }

        if (frameSelected != -1) {
            RefreshUVFrameItems();
        }

    }

    // This is misnamed
    public void OnFlipTextureCoordsVertically() {

        var minX = textureLines.points.Min(obj => {
            return obj.transform.localPosition.x;
        });

        var maxX = textureLines.points.Max(obj => {
            return obj.transform.localPosition.x;
        });

        var width = maxX - minX;
        var center = width / 2;

        foreach (var point in textureLines.points) {

            var localX = point.transform.localPosition.x - minX;

            var distanceFromCenter = localX - center;

            var vFlippedX = center - distanceFromCenter;

            var script = point.GetComponent<TextureCoordinatePoint>();

            script.ChangePosition((int)(minX + vFlippedX), (int)point.transform.localPosition.y);

        }

        textureLines.Refresh();

        if (frameSelected != -1) {
            RefreshUVFrameItems();
        }

    }

    // Same with this
    public void OnFlipTextureCoordsHorizontally() {

        var minY = textureLines.points.Min(obj => {
            return obj.transform.localPosition.y;
        });

        var maxY = textureLines.points.Max(obj => {
            return obj.transform.localPosition.y;
        });

        var height = maxY - minY;
        var center = height / 2;

        foreach (var point in textureLines.points) {

            var localY = point.transform.localPosition.y - minY;

            var distanceFromCenter = localY - center;

            var vFlippedY = center - distanceFromCenter;

            var script = point.GetComponent<TextureCoordinatePoint>();

            script.ChangePosition((int)point.transform.localPosition.x, (int)(minY + vFlippedY));

        }

        textureLines.Refresh();

        if (frameSelected != -1) {
            RefreshUVFrameItems();
        }

    }

    public void OnClickFlipUVOrderVertically() {

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        if (textureLines.points.Count == 4) {
            textureLines.points[0].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[3].x, (int)oldPoints[3].y);
            textureLines.points[1].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[2].x, (int)oldPoints[2].y);
            textureLines.points[2].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[1].x, (int)oldPoints[1].y);
            textureLines.points[3].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[0].x, (int)oldPoints[0].y);
        } else {
            textureLines.points[0].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[2].x, (int)oldPoints[2].y);
            textureLines.points[2].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[0].x, (int)oldPoints[0].y);
        }

        if (frameSelected != -1) {
            RefreshUVFrameItems();
        }

    }

    public void OnClickFlipUVOrderHorizontally() {

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        if (textureLines.points.Count == 4) {
            textureLines.points[0].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[1].x, (int)oldPoints[1].y);
            textureLines.points[1].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[0].x, (int)oldPoints[0].y);
            textureLines.points[2].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[3].x, (int)oldPoints[3].y);
            textureLines.points[3].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[2].x, (int)oldPoints[2].y);
        }
        else {
            textureLines.points[0].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[1].x, (int)oldPoints[1].y);
            textureLines.points[1].GetComponent<TextureCoordinatePoint>()
                .ChangePosition((int)oldPoints[0].x, (int)oldPoints[0].y);
        }

        if (frameSelected != -1) {
            RefreshUVFrameItems();
        }

    }

    public void FlipLastTwoUVs() {

        if (controller.selectedTiles.Count == 0) {
            return;
        }

        if (controller.selectedTiles[0].uvs.Count != 3) {
            return;
        }

        var secondPoint = textureLines.points[1].transform.localPosition;
        var thirdPoint = textureLines.points[2].transform.localPosition;

        textureLines.points[1].GetComponent<TextureCoordinatePoint>().ChangePosition((int)thirdPoint.x, (int)thirdPoint.y);
        textureLines.points[2].GetComponent<TextureCoordinatePoint>().ChangePosition((int)secondPoint.x, (int)secondPoint.y);

    }

    public void OnClickRotateCounterClockwise() {

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        int index = oldPoints.Count - 1;
        foreach (var point in textureLines.points) {

            var script = point.GetComponent<TextureCoordinatePoint>();

            if (index == oldPoints.Count - 1) {

                var vector = oldPoints.Last();

                script.ChangePosition((int)vector.x, (int)vector.y);

                index = -1;

            } else {

                var vector = oldPoints[index];

                script.ChangePosition((int)vector.x, (int)vector.y);

            }

            index++;

        }

        if (frameSelected != -1) {
            RefreshUVFrameItems();
        }

    }

    public void OnClickGenerateColorPalette() {

        DialogWindowUtil.Dialog("Color Palette Warning", 
            "It is recommended to use already existing Future Cop color palettes. " +
            "Future Cop's color palettes are not fully understood yet and the method for creating them is still work in progress. " +
            "Textures might display low quality or incorrectly.", () => {

            var bitmap = controller.main.level.textures[controller.selectedTiles[0].texturePalette];

            var counts = bitmap.CreateColorPalette();

            bitmap.ClearLookUpData(counts.Item1, counts.Item2);

            return true;

        });


        

    }

    public void OnClickEditTexturePalette() {
        EditTransparency();
    }

    public void OnClickConfirmPaletteChanges() {
        transparentMapper.Apply();

        controller.main.RefreshTextures();
        foreach (var section in controller.main.sectionMeshes) {
            section.RefreshTexture();
            section.RefreshMesh();
        }

        EditTransparency();
    }

    public void ExportTexture() {

        if (controller.selectedTiles.Count == 0) { return; }

        OpenFileWindowUtil.SaveFile("Textures", "Bitmap Texture", path => {

            var bitmap = controller.main.level.textures[controller.selectedTiles[0].texturePalette].BitmapWithHeader();

            File.WriteAllBytes(Utils.RemoveExtensionFromFileName(path) + ".bmp", bitmap);

        });

    }

    public void ExportColorPalette() {

        if (controller.selectedTiles.Count == 0) { return; }

        OpenFileWindowUtil.SaveFile("Textures\\Color Palettes", "Color Palette", path => {

            var data = controller.main.level.textures[controller.selectedTiles[0].texturePalette].CbmpColorPaletteData();

            File.WriteAllBytes(Utils.RemoveExtensionFromFileName(path) + ".plut", data);

        });

    }

    public void ExportCbmp() {

        if (controller.selectedTiles.Count == 0) { return; }

        OpenFileWindowUtil.SaveFile("Textures\\Cbmp", "Cbmp", path => {

            var data = controller.main.level.textures[controller.selectedTiles[0].texturePalette].rawFile.data;

            File.WriteAllBytes(Utils.RemoveExtensionFromFileName(path) + ".Cbmp", data.ToArray());

        });

    }

    public void ImportTexture() {

        if (controller.selectedTiles.Count == 0) { return; }

        OpenFileWindowUtil.OpenFile("Textures", "", path => {

            var texture = controller.main.level.textures[controller.selectedTiles[0].texturePalette];

            texture.ImportBMP(File.ReadAllBytes(path));

            controller.main.RefreshTextures();

            controller.RefreshUVMapper();

            controller.ReInitTileOverlayTexture();

            foreach (var section in controller.main.sectionMeshes) {
                section.RefreshTexture();
                section.RefreshMesh();
            }

        });

    }

    public void ImportColorPalette() {

        if (controller.selectedTiles.Count == 0) { return; }

        OpenFileWindowUtil.OpenFile("Textures\\Color Palettes", "", path => {

            var texture = controller.main.level.textures[controller.selectedTiles[0].texturePalette];

            texture.ImportColorPaletteData(File.ReadAllBytes(path));

        });

    }

    public void ImportCbmp() {

        if (controller.selectedTiles.Count == 0) { return; }

        OpenFileWindowUtil.OpenFile("Textures\\Cbmp", "", path => {

            var texture = controller.main.level.textures[controller.selectedTiles[0].texturePalette];

            texture.ImportCbmp(File.ReadAllBytes(path));

            controller.main.RefreshTextures();

            controller.RefreshUVMapper();

            controller.ReInitTileOverlayTexture();

            foreach (var section in controller.main.sectionMeshes) {
                section.RefreshTexture();
                section.RefreshMesh();
            }

        });

    }

    public void OnChangeTextureType() {

        if (controller.selectedTiles.Count == 0) {
            return;
        }

        if (preventDropdownCallback) {
            return;
        }

        EndEditingVectorAnimation();
        EndEditingFrameAnimation();

        var refreshRequired = false;

        switch ((TextureType)textureTypeDropDown.value) {

            case TextureType.Static:

                foreach (var tile in controller.selectedTiles) {

                    if (tile.GetFrameCount() > 0 || tile.isVectorAnimated) {
                        refreshRequired = true;
                    }

                    tile.isVectorAnimated = false;
                    tile.animatedUVs.Clear();

                }

                if (refreshRequired) {
                    controller.selectedSection.RefreshMesh();
                    controller.RefreshTileOverlayTexture();
                }

                break;
            case TextureType.VectorAnimated:

                foreach (var tile in controller.selectedTiles) {

                    if (tile.GetFrameCount() > 0 || !tile.isVectorAnimated) {
                        refreshRequired = true;
                    }

                    tile.isVectorAnimated = true;
                    tile.animatedUVs.Clear();

                }

                if (refreshRequired) {
                    controller.selectedSection.RefreshMesh();
                    controller.RefreshTileOverlayTexture();
                }

                StartEditingVectorAnimation();
                break;
            case TextureType.FrameAnimated:

                controller.DeselectAllButLastTile();

                var firstTile = controller.selectedTiles[0];

                if (firstTile.GetFrameCount() == 0) {

                    firstTile.animationSpeed = 50;

                    firstTile.animatedUVs.AddRange(firstTile.uvs);
                    firstTile.animatedUVs.AddRange(firstTile.uvs);

                }

                refreshRequired = firstTile.isVectorAnimated;

                firstTile.isVectorAnimated = false;

                if (refreshRequired) {
                    controller.selectedSection.RefreshMesh();
                    controller.RefreshTileOverlayTexture();
                }

                StartEditingFrameAnimation();
                break;

        }

    }

    public void OnChangeVectorXSlider() {

        var value = (int)xVectorSlider.value;

        controller.selectedSection.section.animationVector.x = value;
        xVectorText.text = value.ToString();

        if (value == 0) {
            xVectorText.text = "Disabled";
        }

        textureLines.RefreshGhostPoints();

    }

    public void OnChangeVectorYSlider() {

        var value = (int)yVectorSlider.value;

        controller.selectedSection.section.animationVector.y = value;
        yVectorText.text = value.ToString();

        if (value == 0) {
            yVectorText.text = "Disabled";
        }

        textureLines.RefreshGhostPoints();

    }

    public void OnClickAddFrame() {

        var tile = controller.selectedTiles[0];
        var currentUVs = tile.animatedUVs.GetRange(frameSelected * 4, 4);
        tile.animatedUVs.AddRange(currentUVs);

        RefreshUVFrameItems();

    }

    public void OnClickRemoveFrame() {

        var tile = controller.selectedTiles[0];

        if (tile.GetFrameCount() == 2) {
            return;
        }

        tile.animatedUVs.RemoveRange(frameSelected * 4, 4);

        RefreshUVFrameItems();

    }

    public void OnChangeAnimationSpeedSlide() {

        var tile = controller.selectedTiles[0];

        tile.animationSpeed = (int)frameAnimationSpeedSlider.value;

        frameAnimationSpeedText.text = tile.animationSpeed.ToString();

    }

    enum TextureType {
        Static = 0,
        VectorAnimated = 1,
        FrameAnimated = 2
    }

}