using FCopParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    public const int interfaceObjectsMask = 512;
    public const int uiLayerMask = 480;
    public static Color mainColor = new Color(0x22 / 255f, 0x22 / 255f, 0x22 / 255f);
    public static Color selectedColor = new Color(0f, 0x33 / 255f, 0f);

    public static bool ignoreAllInputs = false;
    public static bool debug = false;
    public bool isEscMenuOpen = false;
    public static float uiScaleFactor = 1f;

    // Undos
    public static List<CounterAction> counterActions = new();
    public static bool counterActionAddedOnCurrentSelectHold = false;

    public static void AddCounterAction(CounterAction counterAction, bool ignoreSelectionCheck = false) {
        
        if (counterActionAddedOnCurrentSelectHold && !ignoreSelectionCheck) {
            return;
        }

        if (Controls.IsDown("Undo")) {
            return;
        }

        counterActionAddedOnCurrentSelectHold = Input.GetMouseButton(0) || Controls.IsDown("Select");

        counterActions.Add(counterAction);

    }

    public static void ClearCounterActions() {

        QuickLogHandler.Log("Undo history cleared.", LogSeverity.Info);
        counterActions.Clear();

    }

    public static void PopCounterAction() {
        counterActions.RemoveAt(counterActions.Count - 1);
    }

    public static void GarbageCollectCounterActions(Type type) {

        foreach (var counterAction in new List<CounterAction>(counterActions)) {

            if (counterAction.GetType() == type) {
                counterActions.Remove(counterAction);
            }

        }

    }

    public static bool IsMouseOverUI() {

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;

    }

    public GameObject meshSection;
    public GameObject litMeshSection;
    public GameObject heightMapChannelPoint;
    public GameObject tileHeightMapChannelPoint;
    public GameObject vertexColorPoint;
    public GameObject SelectedTileOverlay;
    public GameObject TileTexturePreview;
    public GameObject SectionBoarders;
    public GameObject NavMeshPoint;
    public GameObject schematicMesh;

    public GameObject BlankActor;
    public GameObject ObjectMesh;
    public GameObject actorGroupObjectFab;
    public GameObject ArrowModelFab;

    public GameObject playerCamera;
    public GameObject playModePlayer;

    public GameObject line3d;
    public GameObject axisControl;

    public GameObject escMenuPrefab;
    public GameObject assetManagerPrefab;
    public GameObject miniAssetManagerPrefab;


    // Temp
    public GameObject boundsPrefab;

    // - View Refs -
    public GameObject canvas;
    public CanvasScaler uiScaler;
    public GameObject mainCamera;
    public Light worldLight;

    // - Unity Asset Refs -
    public Texture2D tileEffectTexture;

    public ToolbarView toolbar;

    public FCopLevel level;

    public List<LevelMesh> sectionMeshes = new();

    public Texture2D levelTexturePallet;

    public List<Sprite> bmpTextures = new();

    public static EditMode editMode;

    // Auto save
    float autoSaveTimer;

    void _Debug() {

        var actorEditMode = (ActorEditMode)editMode;

        var actors = level.sceneActors.actors.Where(a => a.behaviorType == ActorBehavior.StaticProp).ToList();

        var actors2 = actors.Where(a => (((FCopBehavior96)a.behavior).propertiesByName["unknown2"].GetCompiledValue()) > 0).ToList();
        var x = 5;
        var y = -5;
        foreach (var a in actors2) {

            actorEditMode.actorObjectsByID[a.DataID].ChangePosition(new Vector3(x, 0, y), AxisControl.Axis.IgnoreY);

            x += 3;

            if (x > 128) {
                x = 5;
                y -= 5;
            }
        }

    }

    void Start() {

        DialogWindowUtil.canvas = canvas;
        ContextMenuUtil.canvas = canvas;
        OpenFileWindowUtil.canvas = canvas;
        HeadsUpTextUtil.canvas = canvas;
        LoadingScreenUtil.canvas = canvas;
        MiniAssetManagerUtil.canvas = canvas;
        MiniAssetManagerUtil.prefab = miniAssetManagerPrefab;

        Physics.queriesHitBackfaces = true;

        level = FileManagerMain.level;

        Application.targetFrameRate = 60;

        RefreshTextures();

        RenderFullMap();

        ApplySettings();

    }

    void Update() {

        autoSaveTimer += Time.deltaTime;

        if (autoSaveTimer >= 600f) {
            AutoSave();
            autoSaveTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isEscMenuOpen) {
            HeadsUpTextUtil.End();
            OpenEscMenu();
        }

        // If the edit mode is null that most likely means unity is transitioning scenes.
        if (editMode == null) {
            return;
        }

        editMode.Update();

        if (Input.GetKeyDown(KeyCode.F10)) {

            _Debug();
        }


        if (ignoreAllInputs) { return; }

        if (Controls.OnDown("Save")) {

            Save();
            counterActions.Clear();
            System.GC.Collect();
        }

        if (Controls.OnDown("Compile")) {
            Compile();
            counterActions.Clear();
            System.GC.Collect();
        }

        if (Controls.OnDown("Undo")) {
            Undo();
        }
        if (!(Controls.IsDown("Select") && Input.GetMouseButton(0))) {
            counterActionAddedOnCurrentSelectHold = false;
        }

        if (Input.GetKeyDown(KeyCode.F12)) {
            canvas.SetActive(!canvas.activeSelf);
        }

        if (Controls.OnDown("JumpToCursor")) {

            var pos = CursorOnLevelMesh();

            if (pos != null) {

                Camera.main.transform.position = (Vector3)pos;

                Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * -4);

            }

        }

        // Temp
        uiScaleFactor = uiScaler.scaleFactor;

    }

    public void ApplySettings() {

        mainCamera.GetComponent<Camera>().fieldOfView = SettingsManager.fov;

        foreach (Transform trans in mainCamera.transform) {
            trans.GetComponent<Camera>().fieldOfView = SettingsManager.fov;
        }

        uiScaler.scaleFactor = SettingsManager.uiScale;
        uiScaleFactor = SettingsManager.uiScale;

    }

    public void Undo() {

        // This is here because if mouse 0 is held down it most likely means something is being modified.
        // If so it wants to wait for the action to be done before allowing undoing.
        if (Input.GetMouseButton(0)) {
            return;
        }

        if (counterActions.Count > 0) {

            QuickLogHandler.Log("Undo " + counterActions.Last().name , LogSeverity.Success);

            counterActions.Last().Action();
            counterActions.RemoveAt(counterActions.Count - 1);

        }

    }

    public void OpenEscMenu() {

        var obj = Instantiate(escMenuPrefab);
        obj.transform.SetParent(canvas.transform, false);
        var script = obj.GetComponent<EscMenu>();
        script.main = this;

        isEscMenuOpen = true;

    }

    public void BackToMainMenu() {

        editMode = null;

        FileManagerMain.iffFile = null;
        FileManagerMain.level = null;

        ClearStaticData();

        SettingsManager.renderDirectionalLight = false;

        Presets.uvPresets = new UVPresets("Texture Presets", null);
        Presets.shaderPresets = new ShaderPresets("Shader Presets", null);
        Presets.colorPresets = new ColorPresets("Color Presets", null);
        Presets.levelSchematics = new();

        ignoreAllInputs = false;
        counterActions.Clear();
        counterActionAddedOnCurrentSelectHold = false;

        SceneManager.LoadScene("Scenes/FileManagerScene", LoadSceneMode.Single);

        System.GC.Collect();

    }

    public void OpenObjectEditor(FCopObject fCopObject) {

        foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
            obj.SetActive(false);
        }
        
        ObjectEditorMain.fCopObject = fCopObject;
        ObjectEditorMain.levelTexturePallet = levelTexturePallet;
        SceneManager.LoadScene("Scenes/ObjectEditorScene", LoadSceneMode.Additive);

    }

    void ClearStaticData() {

        HeightMapEditMode.selectedColumn = null;
        HeightMapEditMode.selectedSection = null;
        HeightMapEditMode.keepHeightsOnTop = false;

        TileAddMode.placementSetting = TileAddMode.SchematicPlacementSetting.Exact;
        TileAddMode.removeAllTilesOnSchematicPlacement = false;

        NavMeshEditMode.copiedNavNodeCoords = null;

        ShaderEditMode.showColorPresets = false;

        TextureEditMode.openUVMapperByDefault = true;

    }

    public void DisableMainCamera() {
        mainCamera.SetActive(false);
    }

    public void EnableMainCamera() {
        mainCamera.SetActive(true);
    }

    public Vector3 WorldToScreenPointScaled(Vector3 worldPoint) {
        return Camera.main.WorldToScreenPoint(worldPoint) / uiScaler.scaleFactor;
    }

    public void RefreshLevel() {

        foreach (var section in sectionMeshes) {
            section.RefreshMesh();
        }

    }

    public LevelMesh GetLevelMesh(int x, int y) {

        return sectionMeshes.First(mesh => { return mesh.arrayX == x && mesh.arrayY == y; });

    }

    public TileSelection GetTileOnLevelMesh(bool useCursor = true) {
        Ray ray;

        if (useCursor) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else {
            ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        }

        RaycastHit hit;

        if (IsMouseOverUI() && useCursor) {
            return null;
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1)) {

            foreach (var section in sectionMeshes) {

                if (hit.colliderInstanceID == section.meshCollider.GetInstanceID()) {

                    int clickX = (int)Math.Floor(hit.point.x - section.x);
                    int clickY = (int)Math.Floor(Math.Abs(hit.point.z + section.y));

                    TileColumn column;

                    // This just sometimes doesn't work and I don't really understand why
                    try {
                        column = section.section.tileColumns.First(tileColumn => {
                            return tileColumn.x == clickX && tileColumn.y == clickY;
                        });
                    }
                    catch {
                        return null;
                    }

                    var selection = new TileSelection(section.sortedTilesByTriangle[hit.triangleIndex], column, section);

                    return selection;

                }


            }

        }

        return null;

    }

    public Vector3? CursorOnLevelMesh() {

        if (IsMouseOverUI()) {
            return null;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1)) {

            foreach (var section in sectionMeshes) {

                if (hit.colliderInstanceID == section.meshCollider.GetInstanceID()) {

                    int clickX = (int)Math.Floor(hit.point.x - section.x);
                    int clickY = (int)Math.Floor(Math.Abs(hit.point.z + section.y));

                    var column = section.section.tileColumns.First(tileColumn => {
                        return tileColumn.x == clickX && tileColumn.y == clickY;
                    });

                    return hit.point;

                }


            }

        }

        return null;

    }

    public void Compile() {

        IFFFileManager compiledFileManager = null;

        try {
            compiledFileManager = level.Compile();
        } 
        catch (MeshIDException) {
            DialogWindowUtil.Dialog("Compile Error: Invalid Level Geometry", "One or more tiles geomtry is invalid." +
                " This error can be cause by manually changing the height channel of a vertex. The selected tile overlay" +
                " will be red if the geometry is invalid.");
            return;
        } 
        catch (TextureArrayMaxExceeded) {
            DialogWindowUtil.Dialog("Compile Error: Unique Tile Texture Mapping Exceeded", "The max of 1024 UVs have been exceeded in one or more sections. " +
                "Please report this error");
            return;
        }
        catch (GraphicsArrayMaxExceeded) {
            DialogWindowUtil.Dialog("Compile Error: Unique Tile Graphics Exceeded", "The max of 1024 Tile Graphics have been exceeded in one or more sections. " +
                "This should be very rare, please report this error");
            return;
        }
        catch (MaxTilesExceeded) {
            DialogWindowUtil.Dialog("Compile Error: Max Tiles in Section Exceeded", "The max of 1024 tiles per section has been exceed in one or more sections." +
                " Be sure to always use a quad tile whenever possible.");
            return;
        }

        if (compiledFileManager == null) {
            return;
        }

        var iffFile = new IFFParser(compiledFileManager);

        iffFile.Compile();

        iffFile.parsedData.CreateFileList("Debug file list.txt");

        FreeMove.StopLooking();

        OpenFileWindowUtil.SaveFile("Output", "Mission File", path => {

            File.WriteAllBytes(path, iffFile.bytes);

        });

    }

    public void Save() {

        List<byte> bytes = new();

        try {

            bytes = level.CompileToNCFCFile();

        }
        catch (MeshIDException) {
            DialogWindowUtil.Dialog("Compile Error: Invalid Level Geometry", "One or more tiles geomtry is invalid." +
                " This error can be cause by manually changing the height channel of a vertex. The selected tile overlay" +
                " will be red if the geometry is invalid.");
            return;

        }

        FreeMove.StopLooking();

        OpenFileWindowUtil.SaveFile("Output", "Level.ncfc", path => {

            if (Path.GetExtension(path) == ".ncfc") {

                File.WriteAllBytes(path, bytes.ToArray());

            }
            else {

                File.WriteAllBytes(path + ".ncfc", bytes.ToArray());

            }


        });

    }

    public void AutoSave() {

        List<byte> bytes = new();

        try {

            bytes = level.CompileToNCFCFile();

        }
        catch {

            QuickLogHandler.Log("Auto save failed", LogSeverity.Warning);

        }

        File.WriteAllBytes("fce_autosave.ncfc", bytes.ToArray());
        QuickLogHandler.Log("Auto save complete", LogSeverity.Success);

    }
    
    public void ChangeEditMode(EditMode mode) {

        ignoreAllInputs = false;

        if (editMode != null) {

            if (editMode is ActorEditMode) {
                ClearCounterActions();
            }

            editMode.OnDestroy();
        }

        editMode = mode;
        editMode.OnCreateMode();

    }

    public void RefreshTextures() {

        bmpTextures.Clear();

        foreach (var bmp in level.textures) {

            var bmpTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);

            if (SettingsManager.renderMode == RenderType.Pixelated) {
                bmpTexture.filterMode = FilterMode.Point;
            }

            bmpTexture.LoadRawTextureData(bmp.ConvertToARGB32());
            bmpTexture.Apply();

            bmpTextures.Add(Sprite.Create(bmpTexture, new Rect(0, 0, 256, 256), Vector2.zero));

        }

        // Height = Cbmp height * Cbmp Count + Black Padding
        var texture = new Texture2D(256, 2580, TextureFormat.ARGB32, false);

        if (SettingsManager.renderMode == RenderType.Pixelated) {
            texture.filterMode = FilterMode.Point;
        }

        var texturePallet = new List<byte>();

        texturePallet.AddRange(level.textures[0].ConvertToARGB32());
        texturePallet.AddRange(level.textures[1].ConvertToARGB32());
        texturePallet.AddRange(level.textures[2].ConvertToARGB32());
        texturePallet.AddRange(level.textures[3].ConvertToARGB32());
        texturePallet.AddRange(level.textures[4].ConvertToARGB32());
        texturePallet.AddRange(level.textures[5].ConvertToARGB32());
        texturePallet.AddRange(level.textures[6].ConvertToARGB32());
        texturePallet.AddRange(level.textures[7].ConvertToARGB32());
        texturePallet.AddRange(level.textures[8].ConvertToARGB32());
        texturePallet.AddRange(level.textures[9].ConvertToARGB32());

        // This is here to add a section of black to the texture.
        // This is used for prevent tiles to display.
        foreach (var _ in Enumerable.Range(0, 20)) {

            foreach (var i in Enumerable.Range(0, 256)) {
                texturePallet.AddRange(new List<byte> { 0, 0, 255, 0 });
            }

        }


        texture.LoadRawTextureData(texturePallet.ToArray());
        texture.Apply();

        levelTexturePallet = texture;

    }

    public void ChangeRenderedLevelMeshes() {

        DialogWindowUtil.Dialog("Warning", "Changing render modes will cause all undo history and selections to clear.", () => {

            ClearStaticData();
            toolbar.SelectHeightMapEditMode();
            Main.ClearCounterActions();

            foreach (var obj in sectionMeshes) {
                Destroy(obj.gameObject);
            }

            sectionMeshes.Clear();

            if (SettingsManager.renderDirectionalLight) {
                RenderFullMapLitMeshes();
            }
            else {
                RenderFullMap();
            }

            return true;

        });

    }

    void RenderSection(int id) {

        var section = Instantiate(meshSection, new Vector3(0, 0, 0), Quaternion.identity);
        var script = section.GetComponent<LevelMesh>();
        script.section = level.sections[id - 1];
        script.levelTexturePallet = levelTexturePallet;
        script.controller = this;

    }

    void RenderFullMap() {

        foreach (var y in Enumerable.Range(0, level.height)) {

            foreach (var x in Enumerable.Range(0, level.width)) {

                var section = Instantiate(meshSection, new Vector3(x * 16, 0, -(y * 16)), Quaternion.identity);
                var script = section.GetComponent<LevelMesh>();
                script.section = level.sections[(y * level.width) + x];
                script.levelTexturePallet = levelTexturePallet;
                script.controller = this;
                script.x = x * 16;
                script.y = y * 16;
                script.arrayX = x;
                script.arrayY = y;
                sectionMeshes.Add(script);
            }

        }

    }

    void RenderFullMapLitMeshes() {

        foreach (var y in Enumerable.Range(0, level.height)) {

            foreach (var x in Enumerable.Range(0, level.width)) {

                var section = Instantiate(litMeshSection, new Vector3(x * 16, 0, -(y * 16)), Quaternion.identity);
                var script = section.GetComponent<LevelMesh>();
                script.section = level.sections[(y * level.width) + x];
                script.levelTexturePallet = levelTexturePallet;
                script.controller = this;
                script.x = x * 16;
                script.y = y * 16;
                script.arrayX = x;
                script.arrayY = y;
                sectionMeshes.Add(script);
            }

        }

    }

}
