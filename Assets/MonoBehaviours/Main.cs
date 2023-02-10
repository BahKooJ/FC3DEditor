using FCopParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class Main : MonoBehaviour {

    public GameObject meshSection;
    public GameObject heightMapChannelPoint;
    public GameObject SelectedTileOverlay;
    public GameObject NavMeshPoint;

    public GameObject BlankActor;
    public GameObject TurretActorObject;
    public GameObject BaseTurretActorObject;


    public GameObject line3d;
    public GameObject axisControl;

    IFFParser iffFile = new(File.ReadAllBytes("C:/Users/Zewy/Desktop/Mp"));
    public FCopLevel level;

    public List<LevelMesh> sectionMeshes = new();

    public Texture2D levelTexturePallet;

    public bool debug = false;

    public EditMode editMode;

    void Start() {

        Application.targetFrameRate = 60;

        level = new FCopLevel(iffFile.parsedData);

        RefreshTextures();

        RenderFullMap();

    }

    void Update() {

        editMode.Update();

        if (Input.GetKeyDown(KeyCode.Equals)) {
            Compile();
        }

    }

    public void TestRayOnLevelMesh() {

        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1)) {

            foreach (var section in sectionMeshes) {

                if (hit.colliderInstanceID == section.meshCollider.GetInstanceID()) {

                    int clickX = (int)Math.Floor(hit.point.x - section.x);
                    int clickY = (int)Math.Floor(Math.Abs(hit.point.z + section.y));

                    var column = section.section.tileColumns.First(tileColumn => {
                        return tileColumn.x == clickX && tileColumn.y == clickY;
                    });

                    if (Input.GetMouseButtonDown(0)) {

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

        level.Compile();

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

        File.WriteAllBytes("Mp MOD", iffFile.bytes);

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

        var texture = new Texture2D(256, 2048, TextureFormat.RGB565, false);
        var texturePallet = new List<byte>();

        texturePallet.AddRange(level.textures[0].ConvertToRGB565());
        texturePallet.AddRange(level.textures[1].ConvertToRGB565());
        texturePallet.AddRange(level.textures[2].ConvertToRGB565());
        texturePallet.AddRange(level.textures[3].ConvertToRGB565());
        texturePallet.AddRange(level.textures[4].ConvertToRGB565());
        texturePallet.AddRange(level.textures[5].ConvertToRGB565());
        texturePallet.AddRange(level.textures[6].ConvertToRGB565());
        texturePallet.AddRange(level.textures[7].ConvertToRGB565());

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
