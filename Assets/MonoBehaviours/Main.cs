using FCopParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static TextureTransparentMapper;

public class Main : MonoBehaviour {

    public const int uiLayerMask = 480;

    public static bool ignoreAllInputs = false;
    public static bool debug = false;

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

    public GameObject playerCamera;
    public GameObject playModePlayer;

    public GameObject line3d;
    public GameObject axisControl;

    // Temp
    public GameObject boundsPrefab;

    // --View Refs--
    public GameObject canvas;
    public GameObject mainCamera;

    IFFParser iffFile;
    public FCopLevel level;

    public List<LevelMesh> sectionMeshes = new();

    public Texture2D levelTexturePallet;

    public List<Sprite> bmpTextures = new();

    public static EditMode editMode;

    void Start() {

        DialogWindowUtil.canvas = canvas;
        ContextMenuUtil.canvas = canvas;
        OpenFileWindowUtil.canvas = canvas;

        Physics.queriesHitBackfaces = true;

        iffFile = FileManagerMain.iffFile;
        level = FileManagerMain.level;

        Application.targetFrameRate = 60;

        RefreshTextures();

        RenderFullMap();

        //RenderSection(3);

    }

    void Update() {

        //TestSphereRayOnLevelMesh();

        editMode.Update();

        if (ignoreAllInputs) { return; }

        if (Controls.OnDown("Save")) {
            Compile();
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            SettingsManager.showShaders = !SettingsManager.showShaders;
            RefreshLevel();
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            SettingsManager.showTransparency = !SettingsManager.showTransparency;
            SettingsManager.clipBlack = !SettingsManager.clipBlack;
            RefreshLevel();
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            SettingsManager.showAnimations = !SettingsManager.showAnimations;
            RefreshLevel();
        }
        if (Controls.OnDown("Undo")) {
            Undo();
        }
        if (!(Controls.IsDown("Select") && Input.GetMouseButton(0))) {
            counterActionAddedOnCurrentSelectHold = false;
        }

    }

    public void Undo() {

        // This is here because if mouse 0 is held down it most likely means something is being modified.
        // If so it wants to wait for the action to be done before allowing undoing.
        if (Input.GetMouseButton(0)) {
            return;
        }

        if (counterActions.Count > 0) {
            counterActions.Last().Action();

            // Some counter actions account for them double adding counter actions.
            // Sinse the method also tests to make sure undo isn't pressed before adding a counter actions-
            // problems can arise.
            if (counterActions.Count > 0) {
                counterActions.RemoveAt(counterActions.Count - 1);

            }

        }

    }

    public void DisableMainCamera() {
        mainCamera.SetActive(false);
    }

    public void EnableMainCamera() {
        mainCamera.SetActive(true);
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

        try {
            level.Compile();
        } catch (MeshIDException) {
            DialogWindowUtil.Dialog("Compile Error: Invalid Level Geometry", "One or more tiles geomtry is invalid." +
                " This error can be cause by manually changing the height channel of a vertex. The selected tile overlay" +
                " will be red if the geometry is invalid.");
            return;
        } catch (TextureArrayMaxExceeded) {
            DialogWindowUtil.Dialog("Compile Error: Unique Tile Texture Mapping Exceeded", "The max of 1024 UVs have been exceeded in one or more sections. " +
                "Please report this error");
            return;
        }
        catch (GraphicsArrayMaxExceeded) {
            DialogWindowUtil.Dialog("Compile Error: Unique Tile Graphics Exceeded", "The max of 1024 Tile Graphics have been exceeded in one or more sections. " +
                "This should be very rare, please report this error");
            return;
        }

        var index = iffFile.parsedData.files.FindIndex(file => {

            return file.dataFourCC == "Ctil";

        });

        iffFile.parsedData.files.RemoveAll(file => {

            return file.dataFourCC == "Ctil";

        });

        foreach (var section in level.sections) {
            iffFile.parsedData.files.Insert(index, section.parser.rawFile);
            index++;
        }

        iffFile.Compile();

        FreeMove.StopLooking();

        OpenFileWindowUtil.SaveFile("Output", "Mission File", path => {

            File.WriteAllBytes(path, iffFile.bytes);

        });

    }

    public void ChangeEditMode(EditMode mode) {

        if (editMode != null) {
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

    void RenderSection(int id) {

        var section = Instantiate(meshSection, new Vector3(0, 0, 0), Quaternion.identity);
        var script = section.GetComponent<LevelMesh>();
        script.section = level.sections[id - 1];
        script.levelTexturePallet = levelTexturePallet;
        script.controller = this;

    }

    void RenderFullMap() {

        var x = 0;
        var y = 0;

        var itx = 0;
        var ity = 0;

        foreach (var row in level.layout) {

            foreach (var column in row) {

                if (column != 0) {
                    var section = Instantiate(meshSection, new Vector3(x, 0, -y), Quaternion.identity);
                    var script = section.GetComponent<LevelMesh>();
                    script.section = level.sections[column - 1];
                    script.levelTexturePallet = levelTexturePallet;
                    script.controller = this;
                    script.x = x;
                    script.y = y;
                    script.arrayX = itx; 
                    script.arrayY = ity;
                    sectionMeshes.Add(script);
                }
                itx++;
                x += 16;
            }
            itx = 0;
            x = 0;
            y += 16;
            ity++;

        }

    }

}
