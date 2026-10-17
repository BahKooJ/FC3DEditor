
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using FCopParser;
using UnityEngine.EventSystems;

public class TextureUVMapper : MonoBehaviour {

    [HideInInspector]
    public bool editTransparency = false;
    [HideInInspector]
    public bool refusedUVDrag = true;

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

    [HideInInspector]
    public List<FrameItemView> frameItems;
    [HideInInspector]
    public int frameSelected = -1;

    [HideInInspector]
    public int bmpID;

    bool preventDropdownCallback = false;

    void ScaleToScreen() {

        var screenWidth = (Screen.width / Main.uiScaleFactor) * 0.40f;
        var screenHeight = (Screen.height / Main.uiScaleFactor) * 0.80f;

        var multiplierWidth = screenWidth / ((RectTransform)transform).rect.width;

        var multiplierHeight = screenHeight / ((RectTransform)transform).rect.height;

        var multiplier = new float[] { multiplierWidth, multiplierHeight }.Min();

        transform.localScale = new Vector3(multiplier, multiplier, multiplier);

    }

    void Start() {

        refusedUVDrag = true;

        // For whatever reason, despite being set to -1 on declaration, it's 0 and I don't understand why
        frameSelected = -1;

        InitView();

        ScaleToScreen();

        exportcontextMenu.items = new() {
            ("Export Bitmap", ExportTexture),
            ("Export Color Palette", ExportColorPalette),
            ("Export BMP and Palette", ExportCbmp)
        };

        importcontextMenu.items = new() {
            ("Import Bitmap", ImportTexture),
            ("Import Color Palette", ImportColorPalette),
            ("Import BMP and Palette", ImportCbmp)
        };

    }

    float startUVDragX = -1f;
    float startUVDragY = -1f;
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

        if (Main.ignoreAllInputs) {
            return;
        }

        if (Controls.OnDown("RotateClockwiseUV")) {
            OnClickRotateClockwise();
        }
        if (Controls.OnDown("RotateCounterClockwiseUV")) {
            OnClickRotateCounterClockwise();
        }
        if (Controls.OnDown("FlipTextureCoordsVerticallyUV")) {
            OnFlipTextureCoordsVertically();
        }
        if (Controls.OnDown("FlipTextureCoordsHorizontallyUV")) {
            OnFlipTextureCoordsHorizontally();
        }

    }

    // UV Drag
    private void LateUpdate() {

        if (Input.GetMouseButtonUp(0) && refusedUVDrag && !editTransparency) {

            refusedUVDrag = false;

        }

        if (Input.GetMouseButton(0) && IsCursorInTexturePallete() && !refusedUVDrag && !editTransparency) {

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0) {

                // The UV node is infront of the palette. We still want to drag even if the node is over the cursor.
                // That is, as long as it's not the first thing clicked on. But that's what refusedUVDrag is for.
                if (results[0].gameObject.GetComponent<TextureCoordinatePoint>() != null) {

                    if (results[1].gameObject.name != texturePaletteImage.name) return;

                }
                else {
                    if (results[0].gameObject.name != texturePaletteImage.name) return;
                }
                
            }
            else {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)texturePaletteImage.transform, Input.mousePosition, Camera.main, out Vector2 pointOnPallete);

            var x = Mathf.Floor(pointOnPallete.x);
            var y = Mathf.Floor(pointOnPallete.y);

            if (startUVDragX == -1) {
                startUVDragX = x;
                startUVDragY = y;
            }

            TextureEditMode.AddTileStateCounterAction();
            textureLines.SetUVs((int)x, (int)y, (int)startUVDragX, (int)startUVDragY);
            textureLines.Refresh();

        }
        else {

            startUVDragX = -1;
            startUVDragY = -1;

        }

    }

    void OnDestroy() {

        transparentMapper.GarbageCollectDrawCounterActions();

    }

    public void ApplyCurrentDataToTile(Tile tile) {

        var newUvs = new List<int>();
        foreach (var point in textureLines.points) {

            newUvs.Add(TextureCoordinate.SetPixel((int)point.transform.localPosition.x, (int)point.transform.localPosition.y));

        }

        if (tile.uvs.Count == newUvs.Count) {
            tile.uvs = newUvs;
        }
        else if (tile.uvs.Count < newUvs.Count) {
            tile.uvs = newUvs.GetRange(0, tile.uvs.Count);
        }
        else {
            return;
        }

        tile.texturePalette = bmpID;

    }

    public void RecievePresetData(UVPreset preset) {

        textureLines.ReInit(new (preset.uvs));

        texturePaletteDropdown.GetComponent<TMP_Dropdown>().value = preset.texturePalette;

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

        if (!controller.HasSelection) {

            bmpID = texturePaletteDropdown.GetComponent<TMP_Dropdown>().value;

            texturePaletteImage.GetComponent<Image>().sprite = controller.main.bmpTextures[bmpID];

            return;
        }
        else {

            bmpID = controller.FirstTile.texturePalette;

            texturePaletteDropdown.GetComponent<TMP_Dropdown>().value = bmpID;

            texturePaletteImage.GetComponent<Image>().sprite = controller.main.bmpTextures[bmpID];

        }

    }

    void InitView() {

        preventDropdownCallback = true;

        InitTexturePallette();

        uvMapperTools.SetActive(controller.HasSelection);

        if (controller.HasSelection) {
            var tile = controller.FirstTile;

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

        frameSelected = -1;

        InitTexturePallette();

        uvMapperTools.SetActive(controller.HasSelection);

        textureLines.gameObject.SetActive(controller.HasSelection);

        EndEditingVectorAnimation();
        EndEditingFrameAnimation();

        if (controller.HasSelection) {
            var tile = controller.FirstTile;

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

            transparentMapper.GarbageCollectDrawCounterActions();

            editTransparency = false;

            paletteConfirmationButtons.SetActive(false);
            transparentMapperTools.SetActive(false);
            editPaletteButton.SetActive(true);

            transparentMapper.drawingCursor.gameObject.SetActive(false);

            textureLines.gameObject.SetActive(true);
            textureLines.ReInit();

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

        var firstSection = controller.FirstItem.section;

        xVectorSlider.value = firstSection.section.animationVector.x;
        yVectorSlider.value = firstSection.section.animationVector.y;
        xVectorText.text = firstSection.section.animationVector.x.ToString();
        yVectorText.text = firstSection.section.animationVector.y.ToString();

        if (firstSection.section.animationVector.x == 0) {
            xVectorText.text = "Disabled";
        }
        if (firstSection.section.animationVector.y == 0) {
            yVectorText.text = "Disabled";
        }

        textureLines.ReInit();

    }

    void EndEditingVectorAnimation() {

        vectorAnimationProperties.SetActive(false);

        textureLines.ReInit();

    }

    void StartEditingFrameAnimation(int startingFrame = 0) {

        if (!controller.HasSelection) {
            return;
        }

        frameSelected = startingFrame;

        var tile = controller.FirstTile;

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

        var tile = controller.FirstTile;

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

    #region Unity Event Handlers

    public void CloseWindow() {
        controller.RefreshMeshes();
        Destroy(gameObject);
    }

    public void OnChangeTexturePalleteValue() {

        if (editTransparency || !controller.HasSelection) {
            InitTexturePallette();
            return;
        }

        TextureEditMode.AddTileStateCounterAction();

        controller.ChangeTexturePallette(texturePaletteDropdown.GetComponent<TMP_Dropdown>().value);

        texturePaletteDropdown.GetComponent<TMP_Dropdown>().value = controller.FirstTile.texturePalette;

        InitTexturePallette();

        controller.RefreshTileOverlayTexture();
        if (controller.FirstTile.GetFrameCount() > 0) {
            RefreshUVFrameItems();
        }

    }

    public void OnClickRotateClockwise() {

        TextureEditMode.AddTileStateCounterAction();

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        int index = 1;
        foreach (var point in textureLines.points) {

            var script = point;

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

        TextureEditMode.AddTileStateCounterAction();

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

            var script = point;

            script.ChangePosition((int)(minX + vFlippedX), (int)point.transform.localPosition.y);

        }

        textureLines.Refresh();

        if (frameSelected != -1) {
            RefreshUVFrameItems();
        }

    }

    // Same with this
    public void OnFlipTextureCoordsHorizontally() {

        TextureEditMode.AddTileStateCounterAction();

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

            var script = point;

            script.ChangePosition((int)point.transform.localPosition.x, (int)(minY + vFlippedY));

        }

        textureLines.Refresh();

        if (frameSelected != -1) {
            RefreshUVFrameItems();
        }

    }

    public void OnClickFlipUVOrderVertically() {

        TextureEditMode.AddTileStateCounterAction();

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        if (textureLines.points.Count == 4) {
            textureLines.points[0]
                .ChangePosition((int)oldPoints[3].x, (int)oldPoints[3].y);
            textureLines.points[1]
                .ChangePosition((int)oldPoints[2].x, (int)oldPoints[2].y);
            textureLines.points[2]
                .ChangePosition((int)oldPoints[1].x, (int)oldPoints[1].y);
            textureLines.points[3]
                .ChangePosition((int)oldPoints[0].x, (int)oldPoints[0].y);
        } else {
            textureLines.points[0]
                .ChangePosition((int)oldPoints[2].x, (int)oldPoints[2].y);
            textureLines.points[2]
                .ChangePosition((int)oldPoints[0].x, (int)oldPoints[0].y);
        }

        if (frameSelected != -1) {
            RefreshUVFrameItems();
        }

    }

    public void OnClickFlipUVOrderHorizontally() {

        TextureEditMode.AddTileStateCounterAction();

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        if (textureLines.points.Count == 4) {
            textureLines.points[0]
                .ChangePosition((int)oldPoints[1].x, (int)oldPoints[1].y);
            textureLines.points[1]
                .ChangePosition((int)oldPoints[0].x, (int)oldPoints[0].y);
            textureLines.points[2]
                .ChangePosition((int)oldPoints[3].x, (int)oldPoints[3].y);
            textureLines.points[3]
                .ChangePosition((int)oldPoints[2].x, (int)oldPoints[2].y);
        }
        else {
            textureLines.points[0]
                .ChangePosition((int)oldPoints[1].x, (int)oldPoints[1].y);
            textureLines.points[1]
                .ChangePosition((int)oldPoints[0].x, (int)oldPoints[0].y);
        }

        if (frameSelected != -1) {
            RefreshUVFrameItems();
        }

    }

    public void FlipLastTwoUVs() {

        if (!controller.HasSelection) {
            return;
        }

        if (controller.FirstTile.uvs.Count != 3) {
            return;
        }

        TextureEditMode.AddTileStateCounterAction();

        var secondPoint = textureLines.points[1].transform.localPosition;
        var thirdPoint = textureLines.points[2].transform.localPosition;

        textureLines.points[1].ChangePosition((int)thirdPoint.x, (int)thirdPoint.y);
        textureLines.points[2].ChangePosition((int)secondPoint.x, (int)secondPoint.y);

    }

    public void OnClickRotateCounterClockwise() {

        TextureEditMode.AddTileStateCounterAction();

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        int index = oldPoints.Count - 1;
        foreach (var point in textureLines.points) {

            var script = point;

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
            "Are you sure you would like to generate a color palette? This will overwrite the existing color palette. " +
            "For existing Future Cop textures, it is recommended to use already existing Future Cop color palettes. ", () => {

                var bmpID = texturePaletteDropdown.GetComponent<TMP_Dropdown>().value;

                var bitmap = controller.main.level.textures[bmpID];

                var counts = bitmap.CreateColorPalette();

                bitmap.ClearLookUpData(counts.Item1, counts.Item2);

                QuickLogHandler.Log("Color palette created for bitmap " + (bmpID + 1).ToString(), LogSeverity.Success);

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

        if (!controller.HasSelection) { return; }

        OpenFileWindowUtil.SaveFile("FCEAssets/Textures", "Bitmap Texture", path => {

            var bitmap = controller.main.level.textures[controller.FirstTile.texturePalette].BitmapWithHeader();

            File.WriteAllBytes(Utils.RemoveExtensionFromFileName(path) + ".bmp", bitmap);

        });

    }

    public void ExportColorPalette() {

        if (!controller.HasSelection) { return; }

        OpenFileWindowUtil.SaveFile("FCEAssets/Textures/Color Palettes", "Color Palette", path => {

            var data = controller.main.level.textures[controller.FirstTile.texturePalette].CbmpColorPaletteData();

            File.WriteAllBytes(Utils.RemoveExtensionFromFileName(path) + ".plut", data);

        });

    }

    public void ExportCbmp() {

        if (!controller.HasSelection) { return; }

        OpenFileWindowUtil.SaveFile("FCEAssets/Textures/Cbmp", "Cbmp", path => {

            var data = controller.main.level.textures[controller.FirstTile.texturePalette].rawFile.data;

            File.WriteAllBytes(Utils.RemoveExtensionFromFileName(path) + ".Cbmp", data.ToArray());

        });

    }

    public void ImportTexture() {

        if (!controller.HasSelection) { return; }

        OpenFileWindowUtil.OpenFile("FCEAssets/Textures", "", path => {

            try {

                var texture = controller.main.level.textures[controller.FirstTile.texturePalette];

                texture.ImportBMP(File.ReadAllBytes(path));

                controller.main.RefreshTextures();

                controller.RefreshUVMapper();

                controller.ReInitTileOverlayTexture();

                foreach (var section in controller.main.sectionMeshes) {
                    section.RefreshTexture();
                    section.RefreshMesh();
                }

            }
            catch {

                DialogWindowUtil.Dialog("Invalid File", "Please select a valid bitmap file. The bitmap file must be in XRGB555 format.");

            }

        });

    }

    public void ImportColorPalette() {

        if (!controller.HasSelection) { return; }

        OpenFileWindowUtil.OpenFile("FCEAssets/Textures/Color Palettes", "", path => {

            var texture = controller.main.level.textures[controller.FirstTile.texturePalette];
            try {
                texture.ImportColorPaletteData(File.ReadAllBytes(path));
            }
            catch {
                DialogWindowUtil.Dialog("Invalid File", "Please select a valid color palette file.");

            }

        });

    }

    public void ImportCbmp() {

        if (!controller.HasSelection) { return; }

        OpenFileWindowUtil.OpenFile("FCEAssets/Textures/Cbmp", "", path => {

            try {

                var texture = controller.main.level.textures[controller.FirstTile.texturePalette];

                texture.ImportCbmp(File.ReadAllBytes(path));

                controller.main.RefreshTextures();

                controller.RefreshUVMapper();

                controller.ReInitTileOverlayTexture();

                foreach (var section in controller.main.sectionMeshes) {
                    section.RefreshTexture();
                    section.RefreshMesh();
                }

            }
            catch {

                DialogWindowUtil.Dialog("Invalid File", "Please select a valid Cbmp file.");

            }

        });

    }

    public void OnChangeTextureType() {

        if (!controller.HasSelection) {
            return;
        }

        if (preventDropdownCallback) {
            return;
        }

        EndEditingVectorAnimation();
        EndEditingFrameAnimation();

        var refreshRequired = false;

        TextureEditMode.AddTileStateCounterAction();

        switch ((TextureType)textureTypeDropDown.value) {

            case TextureType.Static:

                foreach (var selection in controller.selectedItems) {

                    if (selection.tile.GetFrameCount() > 0 || selection.tile.isVectorAnimated) {
                        refreshRequired = true;
                    }

                    selection.tile.isVectorAnimated = false;
                    selection.tile.animatedUVs.Clear();

                }

                if (refreshRequired) {
                    controller.RefreshMeshes();
                    controller.RefreshTileOverlayTexture();
                }

                break;
            case TextureType.VectorAnimated:

                foreach (var selection in controller.selectedItems) {

                    if (selection.tile.GetFrameCount() > 0 || !selection.tile.isVectorAnimated) {
                        refreshRequired = true;
                    }

                    selection.tile.isVectorAnimated = true;
                    selection.tile.animatedUVs.Clear();

                }

                if (refreshRequired) {
                    controller.RefreshMeshes();
                    controller.RefreshTileOverlayTexture();
                }

                StartEditingVectorAnimation();
                break;
            case TextureType.FrameAnimated:

                QuickLogHandler.Log("Only one animated tile can be selected (Use presets)", LogSeverity.Info);

                controller.DeselectAllButLastTile();

                var firstTile = controller.FirstTile;

                if (firstTile.GetFrameCount() == 0) {

                    firstTile.animationSpeed = 50;

                    firstTile.animatedUVs.AddRange(firstTile.uvs);
                    firstTile.animatedUVs.AddRange(firstTile.uvs);

                }

                refreshRequired = firstTile.isVectorAnimated;

                firstTile.isVectorAnimated = false;

                if (refreshRequired) {
                    controller.RefreshMeshes();
                    controller.RefreshTileOverlayTexture();
                }

                StartEditingFrameAnimation();
                break;

        }

    }

    public void OnChangeVectorXSlider() {
        
        controller.AddVectorAnimationCounterAction();

        var value = (int)xVectorSlider.value;

        controller.FirstItem.section.section.animationVector.x = value;
        xVectorText.text = value.ToString();

        if (value == 0) {
            xVectorText.text = "Disabled";
        }

        textureLines.RefreshGhostPoints();

    }

    public void OnChangeVectorYSlider() {
        
        controller.AddVectorAnimationCounterAction();

        var value = (int)yVectorSlider.value;

        controller.FirstItem.section.section.animationVector.y = value;
        yVectorText.text = value.ToString();

        if (value == 0) {
            yVectorText.text = "Disabled";
        }

        textureLines.RefreshGhostPoints();

    }

    public void OnClickAddFrame() {

        TextureEditMode.AddTileStateCounterAction();

        var tile = controller.FirstTile;
        var currentUVs = tile.animatedUVs.GetRange(frameSelected * 4, 4);
        tile.animatedUVs.AddRange(currentUVs);

        RefreshUVFrameItems();

    }

    public void OnClickRemoveFrame() {

        TextureEditMode.AddTileStateCounterAction();

        var tile = controller.FirstTile;

        if (tile.GetFrameCount() == 2) {
            return;
        }

        tile.animatedUVs.RemoveRange(frameSelected * 4, 4);

        RefreshUVFrameItems();

    }

    public void OnChangeAnimationSpeedSlide() {

        TextureEditMode.AddTileStateCounterAction();

        var tile = controller.FirstTile;

        tile.animationSpeed = (int)frameAnimationSpeedSlider.value;

        frameAnimationSpeedText.text = tile.animationSpeed.ToString();

    }

    #endregion

    enum TextureType {
        Static = 0,
        VectorAnimated = 1,
        FrameAnimated = 2
    }

}