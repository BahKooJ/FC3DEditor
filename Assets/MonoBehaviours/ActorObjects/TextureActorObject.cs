

using System.Collections.Generic;
using UnityEngine;
using FCopParser;
using System.Linq;

public class TextureActorObject : ActorObject {

    // - Unity Refs -
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public MeshFilter meshFilter;
    public Material opaqueMateral;
    public Material transparentMateral;
    public Material additiveMateral;

    Mesh mesh;
    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector2> uvs = new();
    List<Color> colors = new();

    public override void Create() {

        SetToCurrentPosition();
        RefreshRotation();

        mesh = new Mesh();
        meshFilter.mesh = mesh;

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

    }

    public override void Refresh() {

        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        colors.Clear();

        Create();

    }

    public override void RefreshRotation() {

        var objRot = placeholderObject.transform.localRotation.eulerAngles;

        objRot.x = -((RangeActorProperty)actor.behavior.propertiesByName["Rotation X"]).value;
        objRot.y = ((RangeActorProperty)actor.behavior.propertiesByName["Rotation Y"]).value;
        objRot.z = -((RangeActorProperty)actor.behavior.propertiesByName["Rotation Z"]).value;

        placeholderObject.transform.localRotation = Quaternion.Euler(objRot);

    }


    public void Generate() {

        TextureSnippet textureSnippet;

        try {
            placeholderObject.SetActive(true);

            if (missingObjectGameobj != null) {
                missingObjectGameobj.SetActive(false);
            }

            textureSnippet = controller.main.level.textureSnippets.First(t => t.id == actor.behavior.propertiesByName["Texture Snippet"].GetCompiledValue());
        }
        catch { 
            placeholderObject.SetActive(false);

            if (missingObjectGameobj == null) {
                var obj = Instantiate(missingObjectPrefab);
                obj.transform.SetParent(transform, false);
                missingObjectGameobj = obj;
            }

            missingObjectGameobj.SetActive(true);

            return;
        }

        if (((ToggleActorProperty)actor.behavior.propertiesByName["Additive"]).value) {
            meshRenderer.material = additiveMateral;
        }
        else if (((ToggleActorProperty)actor.behavior.propertiesByName["Transparent"]).value) {
            meshRenderer.material = transparentMateral;
        }
        else {
            meshRenderer.material = opaqueMateral;
        }

        meshRenderer.material.mainTexture = controller.main.levelTexturePallet;

        // I don't know why I need to switch these and I don't care to find out.
        var widthFromCenter = ((NormalizedValueProperty)actor.behavior.propertiesByName["Height"]).value / 2;
        var heightFromCenter = ((NormalizedValueProperty)actor.behavior.propertiesByName["Width"]).value / 2;

        vertices.Add(new Vector3(-widthFromCenter, 0, -heightFromCenter));
        vertices.Add(new Vector3(widthFromCenter, 0, -heightFromCenter));
        vertices.Add(new Vector3(widthFromCenter, 0, heightFromCenter));
        vertices.Add(new Vector3(-widthFromCenter, 0, heightFromCenter));

        float y = textureSnippet.y + (textureSnippet.texturePaletteID * 256);
        float x = textureSnippet.x;

        uvs.Add(new Vector2((x + textureSnippet.width) / 256f, y / 2580f));
        uvs.Add(new Vector2(x / 256f, y / 2580f));
        uvs.Add(new Vector2(x / 256f, (y + textureSnippet.height) / 2580f));
        uvs.Add(new Vector2((x + textureSnippet.width) / 256f, (y + textureSnippet.height) / 2580f));

        var color = new Color(
            ((RangeActorProperty)actor.behavior.propertiesByName["Red"]).value,
            ((RangeActorProperty)actor.behavior.propertiesByName["Green"]).value,
            ((RangeActorProperty)actor.behavior.propertiesByName["Blue"]).value
            );

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(0);

        triangles.Add(3);
        triangles.Add(2);
        triangles.Add(0);

    }

}