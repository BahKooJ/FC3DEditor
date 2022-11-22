using FCopParser;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class Main : MonoBehaviour {

    //TODO: Create tile pallet, tile graphics view, and texture view.

    public GameObject meshSection;
    public GameObject heightMapChannelPoint;

    IFFParser iffFile = new IFFParser(File.ReadAllBytes("C:/Users/Zewy/Desktop/Mp MOD"));
    public FCopLevel level;

    public Texture2D levelTexturePallet;

    public PointListView listView;

    public Tile selectedTile = null;
    public TileColumn selectedColumn = null;
    public LevelMesh selectedSection = null;
    public List<GameObject> heightPointObjects = new List<GameObject>();

    public bool debug = false;

    public int selectedListItem = -1;

    // Start is called before the first frame update
    void Start() {

        Application.targetFrameRate = 60;

        level = new FCopLevel(iffFile.parsedData);

        RefreshTextures();

        listView.controller = this;

        RenderFullMap();

    }

    TileColumn copyColumn;

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ListMoveItemUp();
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            ListMoveItemDown();
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            Compile();
        } else if (Input.GetKeyDown(KeyCode.O)) {
            copyColumn = selectedColumn;
        } else if (Input.GetKeyDown(KeyCode.P)) {

            //foreach (var foo in copyColumn.tiles) {
            //    foo.graphicsIndex = 0;
            //}

            selectedColumn.tiles = copyColumn.tiles;
            selectedSection.RefreshMesh();
        } else if (Input.GetKeyDown(KeyCode.Delete)) {
            RemoveSelectedTile();
        }
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

    public void OnTileSelected(Tile tile, TileColumn column, LevelMesh section) {

        selectedTile = tile;
        selectedColumn = column;
        selectedSection = section;

        if (debug) {

            listView.Clear();
            listView.AddItems(tile);

            return;
        }

        foreach (var obj in heightPointObjects) { Destroy(obj); }
        heightPointObjects.Clear();

        AddHeightObjects(0);
        AddHeightObjects(1);
        AddHeightObjects(2);
        AddHeightObjects(3);

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

    public void RemoveSelectedTile() {

        if (selectedTile != null) {
            selectedColumn.tiles.Remove(selectedTile);

            selectedTile = null;
        }

        selectedSection.RefreshMesh();

    }

    public void ListMoveItemUp() {

        if (selectedListItem == -1 || selectedTile == null) {
            return;
        }

        var verticies = MeshType.VerticiesFromID(selectedTile.parsedTile.number5);

        var vertex = verticies[selectedListItem];

        verticies.RemoveAt(selectedListItem);

        if (selectedListItem == 0) {

            verticies.Add(vertex);

        } else {

            verticies.Insert(selectedListItem - 1, vertex);

        }

        selectedSection.RefreshMesh();

        selectedListItem = -1;

        listView.Clear();
        listView.AddItems(selectedTile);

    }

    public void ListMoveItemDown() {

        if (selectedListItem == -1 || selectedTile == null) {
            return;
        }

        var verticies = MeshType.VerticiesFromID(selectedTile.parsedTile.number5);

        var vertex = verticies[selectedListItem];

        verticies.RemoveAt(selectedListItem);

        if (selectedListItem == verticies.Count) {

            verticies.Insert(0, vertex);

        } else {

            verticies.Insert(selectedListItem + 1, vertex);

        }

        selectedSection.RefreshMesh();

        selectedListItem = -1;

        listView.Clear();
        listView.AddItems(selectedTile);

    }

    void AddHeightObjects(int corner) {

        var worldX = selectedSection.x + selectedColumn.x;
        var worldY = -(selectedSection.y + selectedColumn.y);

        switch (corner) {
            case 1:
                worldX += 1;
                break;
            case 2:
                worldY -= 1;
                break;
            case 3:
                worldX += 1;
                worldY -= 1;
                break;
            default:
                break;
        }

        var point = Instantiate(heightMapChannelPoint, new Vector3(worldX, selectedColumn.heights[corner].height1, worldY), Quaternion.identity);
        var script = point.GetComponent<HeightMapChannelPoint>();
        script.heightPoint = selectedColumn.heights[corner];
        script.channel = 1;
        script.section = selectedSection;

        heightPointObjects.Add(point);

        point = Instantiate(heightMapChannelPoint, new Vector3(worldX, selectedColumn.heights[corner].height2, worldY), Quaternion.identity);
        script = point.GetComponent<HeightMapChannelPoint>();
        script.heightPoint = selectedColumn.heights[corner];
        script.channel = 2;
        script.section = selectedSection;

        heightPointObjects.Add(point);

        point = Instantiate(heightMapChannelPoint, new Vector3(worldX, selectedColumn.heights[corner].height3, worldY), Quaternion.identity);
        script = point.GetComponent<HeightMapChannelPoint>();
        script.heightPoint = selectedColumn.heights[corner];
        script.channel = 3;
        script.section = selectedSection;

        heightPointObjects.Add(point);

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
                }

                x += 16;
            }
            x = 0;
            y += 16;

        }

    }

}
