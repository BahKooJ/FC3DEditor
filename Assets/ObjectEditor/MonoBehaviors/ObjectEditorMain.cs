

using FCopParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectEditorMain : MonoBehaviour {

    // - Prefabs -
    public GameObject ObjectMeshPrefab;
    public GameObject ObjectVertexPrefab;

    // - Unity View Refs -
    public ObjectPropertiesPanelView view;

    // - Parameters -
    public static FCopObject fCopObject;
    public static Texture2D levelTexturePallet;
    public FCopLevel level;

    [HideInInspector]
    public ObjectMesh objectMesh;

    public Action<ObjectVertex> requestedVertexActionCallback = v => { };

    private void Start() {

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Scenes/ObjectEditorScene"));

        level = FileManagerMain.level;

        if (level == null) {

            var iffFile = new IFFParser(File.ReadAllBytes("MissionFiles/Mp"));
            level = new FCopLevel(iffFile.parsedData);

        }

        if (levelTexturePallet == null) {

            RefreshTexture();

        }

        fCopObject = level.objects[19];

        InitObject();
        InitObjectVertices();

        view.Init();

    }

    void InitObject() {

        var gobj = Instantiate(ObjectMeshPrefab);
        objectMesh = gobj.GetComponent<ObjectMesh>();
        objectMesh.fCopObject = fCopObject;
        objectMesh.levelTexturePallet = levelTexturePallet;

    }

    void InitObjectVertices() {

        var scale = ObjectMesh.scale;

        foreach (var vert in fCopObject.firstElementGroup.vertices) {

            var gobj = Instantiate(ObjectVertexPrefab);
            gobj.transform.position = new Vector3(vert.x / scale, vert.y / scale, -(vert.z / scale));

        }
    }

    void RefreshTexture() {

        // Height = Cbmp height * Cbmp Count + Black Padding
        var texture = new Texture2D(256, 2580, TextureFormat.ARGB32, false);
        
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

}