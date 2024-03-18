using FCopParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour {

    public static bool ignoreAllInputs = false;
    public static bool debug = false;
    public static bool showShaders = true;

    public GameObject meshSection;
    public GameObject heightMapChannelPoint;
    public GameObject tileHeightMapChannelPoint;
    public GameObject SelectedTileOverlay;
    public GameObject TileTexturePreview;
    public GameObject SectionBoarders;
    public GameObject NavMeshPoint;

    public GameObject BlankActor;
    public GameObject ObjectMesh;


    public GameObject line3d;
    public GameObject axisControl;

    public GameObject canvas;

    IFFParser iffFile;
    public FCopLevel level;

    public List<LevelMesh> sectionMeshes = new();

    public Texture2D levelTexturePallet;

    public List<Sprite> bmpTextures = new();

    public EditMode editMode;

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

        editMode.Update();

        if (ignoreAllInputs) { return; }

        if (Controls.OnDown("Save")) {
            Compile();
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            showShaders = !showShaders;
            RefreshLevel();
        }

    }

    public void RefreshLevel() {

        foreach (var section in sectionMeshes) {
            section.RefreshMesh();
        }

    }

    public void TestRayOnLevelMesh(bool useCursor = false) {

        Ray ray;

        if (useCursor) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        } else {
            ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        }

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1)) {

            foreach (var section in sectionMeshes) {

                if (hit.colliderInstanceID == section.meshCollider.GetInstanceID()) {

                    int clickX = (int)Math.Floor(hit.point.x - section.x);
                    int clickY = (int)Math.Floor(Math.Abs(hit.point.z + section.y));

                    var column = section.section.tileColumns.First(tileColumn => {
                        return tileColumn.x == clickX && tileColumn.y == clickY;
                    });

                    if (Controls.OnDown("Select")) {

                        SelectTile(section.sortedTilesByTriangle[hit.triangleIndex], column, section);

                        section.RefreshMesh();

                    } else {

                        LookTile(section.sortedTilesByTriangle[hit.triangleIndex], column, section);

                    }

                }


            }

        }

    }

    public Vector3? CursorOnLevelMesh() {

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

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {
        editMode.LookTile(tile, column, section);
    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

        editMode.SelectTile(tile, column, section);

    }

    public void RefreshTextures() {

        bmpTextures.Clear();

        foreach (var bmp in level.textures) {

            var bmpTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);

            bmpTexture.filterMode = FilterMode.Point;

            bmpTexture.LoadRawTextureData(bmp.ConvertToARGB32());
            bmpTexture.Apply();

            bmpTextures.Add(Sprite.Create(bmpTexture, new Rect(0, 0, 256, 256), Vector2.zero));

        }

        var texture = new Texture2D(256, 2560, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
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
                    sectionMeshes.Add(script);
                }

                x += 16;
            }
            x = 0;
            y += 16;

        }

    }

}
