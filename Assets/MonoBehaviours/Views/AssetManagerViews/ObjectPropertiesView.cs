﻿

using FCopParser;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering;
using System.Collections.Generic;
using TMPro;

public class ObjectPropertiesView : MonoBehaviour {

    // - Prefabs -
    public GameObject objectPreviewCamera;
    public RenderTexture objectRenderTexture;

    // - Unity Refs -
    public RawImage meshPreview;
    public TMP_Dropdown texturePaletteDropdown;

    // - Parameters -
    [HideInInspector]
    public Main main;
    public FCopObject fCopObject;

    ObjectMesh meshObj;

    bool refuseCallback = true;

    private void Start() {

        InitSchematicMeshOverlay();
        RenderMesh();

        texturePaletteDropdown.ClearOptions();

        foreach (var t in main.level.textures) {
            texturePaletteDropdown.AddOptions(new List<string> { t.name });
        }

        var textureIndex = fCopObject.GetTexturePalette();

        if (textureIndex == -1) {
            texturePaletteDropdown.interactable = false;
        }
        else {
            texturePaletteDropdown.value = textureIndex;
        }

        refuseCallback = false;

    }

    void InitSchematicMeshOverlay() {

        var obj = Instantiate(main.ObjectMesh);
        obj.layer = 8; // UI Mesh
        obj.transform.position = new Vector3(0, 0, 0);
        var objectMesh = obj.GetComponent<ObjectMesh>();
        objectMesh.fCopObject = fCopObject;
        objectMesh.levelTexturePallet = main.levelTexturePallet;
        objectMesh.textureOffset = 0;
        objectMesh.ForceMake();

        foreach (Transform trans in objectMesh.transform) {
            trans.gameObject.layer = 8; // UI Mesh
        }

        meshObj = objectMesh;

    }

    void RenderMesh() {

        var camera = Instantiate(objectPreviewCamera);

        var height = (meshObj.maxY - meshObj.minY) / 2;

        camera.transform.position = new Vector3(0, height, 0);
        camera.transform.eulerAngles = new Vector3(35, 320, 0);
        camera.transform.position -= camera.transform.forward * 3;

        camera.GetComponent<Camera>().Render();

        var texture = new Texture2D(objectRenderTexture.width, objectRenderTexture.height, objectRenderTexture.graphicsFormat, 0, TextureCreationFlags.None);
        RenderTexture.active = objectRenderTexture;
        texture.ReadPixels(new Rect(0, 0, objectRenderTexture.width, objectRenderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        meshPreview.texture = texture;

        DestroyImmediate(meshObj.gameObject);
        DestroyImmediate(camera);

    }

    public void OnChangeTexturePalette() {

        if (refuseCallback) return;

        fCopObject.SetTexturePalette(texturePaletteDropdown.value);

        InitSchematicMeshOverlay();
        RenderMesh();

    }

    public void OnClickEdit() {

        main.OpenObjectEditor(fCopObject);

    }

}