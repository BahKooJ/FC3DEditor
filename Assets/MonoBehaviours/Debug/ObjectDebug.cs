

using FCopParser;
using System.Collections.Generic;
using System.IO;
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

        foreach (var bmp in level.textures) {

            var bmpTexture = new Texture2D(256, 256, TextureFormat.RGB565, false);

            bmpTexture.LoadRawTextureData(bmp.ConvertToRGB565());
            bmpTexture.Apply();

            bmpTextures.Add(Sprite.Create(bmpTexture, new Rect(0, 0, 256, 256), Vector2.zero));

        }

        var texture = new Texture2D(256, 2560, TextureFormat.RGB565, false);
        var texturePallet = new List<byte>();

        texturePallet.AddRange(level.textures[0].ConvertToRGB565());
        texturePallet.AddRange(level.textures[1].ConvertToRGB565());
        texturePallet.AddRange(level.textures[2].ConvertToRGB565());
        texturePallet.AddRange(level.textures[3].ConvertToRGB565());
        texturePallet.AddRange(level.textures[4].ConvertToRGB565());
        texturePallet.AddRange(level.textures[5].ConvertToRGB565());
        texturePallet.AddRange(level.textures[6].ConvertToRGB565());
        texturePallet.AddRange(level.textures[7].ConvertToRGB565());
        texturePallet.AddRange(level.textures[8].ConvertToRGB565());
        texturePallet.AddRange(level.textures[9].ConvertToRGB565());


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

        //script.controller = this;

        mesh = script;

    }


}

