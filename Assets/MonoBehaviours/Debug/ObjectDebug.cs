

using FCopParser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ObjectDebug : MonoBehaviour {

    public GameObject cube;

    public GameObject meshPrefab;

    IFFParser iffFile = new IFFParser(File.ReadAllBytes("MissionFiles/Mp"));
    public FCopLevel level;

    List<FCopObject> objects = new();

    List<GameObject> cubes = new();

    public List<Sprite> bmpTextures = new();

    public Texture2D levelTexturePallet;

    ObjectMesh mesh;

    void Start() {

        if (SettingsManager.keyBinds.Count == 0) {
            SettingsManager.ParseSettings();
        }

        level = new FCopLevel(iffFile.parsedData);

        objects = level.objects;

        RefreshTextures();

        ShowObjectVerticies(objects[0]);


    }

    int objIndex = 0;

    void Update() {

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            DestroyCubes();
            objIndex++;
            ShowObjectVerticies(objects[objIndex]);
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            DestroyCubes();
            objIndex--;
            ShowObjectVerticies(objects[objIndex]);

        }

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

    void DestroyCubes() {

        foreach (var gameCube in cubes) {
            Destroy(gameCube.gameObject);
        }

        cubes.Clear();

        Destroy(mesh.gameObject);

    }

    void ShowObjectVerticies(FCopObject obj) {

        var gameObj = Instantiate(meshPrefab);

        var script = gameObj.GetComponent<ObjectMesh>();

        script.fCopObject = obj;
        script.levelTexturePallet = levelTexturePallet;

        mesh = script;

    }


}

