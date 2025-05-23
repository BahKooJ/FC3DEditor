﻿

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
    public GameObject HeadsUpText;
    public GameObject ObjectTriangleSelectionOverlayPrefab;

    // - Unity View Refs -
    public GameObject canvas;
    public ObjectEditorView view;

    // - Parameters -
    public static FCopObject fCopObject;
    public static Texture2D levelTexturePallet;
    public FCopLevel level;

    [HideInInspector]
    public ObjectMesh objectMesh;
    [HideInInspector]
    public List<ObjectVertex> gameObjectVertices = new();

    public Action<ObjectVertex> requestedVertexActionCallback = v => { };

    public List<FCopObject.Primitive> selectedPrimitives = new();
    List<ObjectTriangleSelectionOverlay> activeTriangleOverlays = new();

    private void Start() {

        HeadsUpTextUtil.prefab = HeadsUpText;
        HeadsUpTextUtil.canvas = canvas;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Scenes/ObjectEditorScene"));

        level = FileManagerMain.level;

        if (level == null) {

            var iffFile = new IFFParser(File.ReadAllBytes("MissionFiles/Mp"));
            level = new FCopLevel(iffFile.parsedData);

        }

        if (levelTexturePallet == null) {

            RefreshTexture();

        }

        //fCopObject = level.objects[19];

        InitObject();
        InitObjectVertices();

        view.Init();

    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            HeadsUpTextUtil.End();
            ClearVertexCallback();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            Clear();
            view.ClearPrimitivePropertyView();
        }
        

        if (Input.GetMouseButtonDown(0)) {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 8)) {

                var result = TestVertexRaycast(hit);

                if (result == false) {
                    TestObjectMeshRaycast(hit);
                }

            }

        }

    }

    public bool TestVertexRaycast(RaycastHit hit) {

        foreach (var vert in gameObjectVertices) {

            if (hit.colliderInstanceID == vert.boxCollider.GetInstanceID()) {
                requestedVertexActionCallback(vert);
                return true;
            }

        }

        return false;

    }

    public bool TestObjectMeshRaycast(RaycastHit hit) {

        if (objectMesh.meshCollider.GetInstanceID() == hit.colliderInstanceID) {

            SelectFace(objectMesh.sortedPrimitives[hit.triangleIndex]);

            return true;

        }

        return false;

    }

    void SelectFace(FCopObject.Primitive selectedPrimitive) {

        if (Input.GetKey(KeyCode.LeftShift)) {

            if (!selectedPrimitives.Contains(selectedPrimitive)) {

                selectedPrimitives.Add(selectedPrimitive);
                CreateOverlay(selectedPrimitive);

            }
            else {

                selectedPrimitives.Remove(selectedPrimitive);
                RemoveOverlay(selectedPrimitive);

                if (selectedPrimitives.Count == 0) {
                    view.ClearPrimitivePropertyView();
                    return;
                }

            }

        }
        else {

            Clear();
            selectedPrimitives.Add(selectedPrimitive);
            CreateOverlay(selectedPrimitive);

        }
        
        view.RefreshPrimitivePropertyView(selectedPrimitive);

    }

    void CreateOverlay(FCopObject.Primitive selectedPrimitive) {

        var gobj = Instantiate(ObjectTriangleSelectionOverlayPrefab);
        gobj.transform.SetParent(objectMesh.transform, false);
        var overlay = gobj.GetComponent<ObjectTriangleSelectionOverlay>();
        overlay.primitive = selectedPrimitive;
        overlay.fCopTriangles = objectMesh.trianglesByPrimitive[selectedPrimitive];
        overlay.textureOffset = objectMesh.textureOffset;
        overlay.levelTexturePallet = levelTexturePallet;
        overlay.Create();
        activeTriangleOverlays.Add(overlay);

    }

    void RemoveOverlay(FCopObject.Primitive selectedPrimitive) {

        var overlay = activeTriangleOverlays.FirstOrDefault(o => o.primitive == selectedPrimitive);

        activeTriangleOverlays.Remove(overlay);

        if (overlay != null) {

            Destroy(overlay.gameObject);

        }

    }

    void Clear() {

        selectedPrimitives.Clear();

        foreach (var overlay in activeTriangleOverlays) {

            Destroy(overlay.gameObject);

        }

        activeTriangleOverlays.Clear();

    }

    public void ClearVertexCallback() {
        requestedVertexActionCallback = v => { };
    }

    public void BackToLevelEditor() {

        fCopObject = null;
        levelTexturePallet = null;

        SceneManager.UnloadSceneAsync("Scenes/ObjectEditorScene");

        foreach (var obj in SceneManager.GetSceneByName("Scenes/LevelEditorScene").GetRootGameObjects()) {

            if (obj.TryGetComponent(out Main main)) {
                HeadsUpTextUtil.canvas = main.canvas;
            }

            obj.SetActive(true);
        }

    }

    void InitObject() {

        var gobj = Instantiate(ObjectMeshPrefab);
        objectMesh = gobj.GetComponent<ObjectMesh>();
        objectMesh.fCopObject = fCopObject;
        objectMesh.levelTexturePallet = levelTexturePallet;

    }

    void InitObjectVertices() {

        var scale = ObjectMesh.scale;

        var i = 0;
        foreach (var vert in fCopObject.firstElementGroup.vertices) {

            var gobj = Instantiate(ObjectVertexPrefab);
            gobj.transform.position = new Vector3(-(vert.x / scale), vert.y / scale, -(vert.z / scale));
            var objVertex = gobj.GetComponent<ObjectVertex>();
            gameObjectVertices.Add(objVertex);
            objVertex.index = i;
            i++;

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

    public void ChangePrimitiveMaterial(int id) {

        foreach (var prim in selectedPrimitives) {

            prim.materialID = id;

        }

        fCopObject.RefreshTriangles();
        objectMesh.Refresh();

    }

}