
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshUtils {

    // This code is... very verbose
    static public List<Vector3> FlatScreenVerticies(List<TileVertex> tileVertices, float meshSize) {

        var vertices = new List<Vector3>();

        var isWall = false;

        var lowestHeight = 10;
        var positions = new List<VertexPosition>();
        foreach (var vert in tileVertices) {

            if (lowestHeight > vert.heightChannel) {
                lowestHeight = vert.heightChannel;
            }

            if (positions.Contains(vert.vertexPosition)) {
                isWall = true;
            }

            positions.Add(vert.vertexPosition);

        }

        if (isWall) {

            var isTop = positions.Contains(VertexPosition.TopLeft) && positions.Contains(VertexPosition.TopRight);
            var isLeft = positions.Contains(VertexPosition.TopLeft) && positions.Contains(VertexPosition.BottomLeft);
            var isDiagnal =
                positions.Contains(VertexPosition.TopLeft) && positions.Contains(VertexPosition.BottomRight) ||
                positions.Contains(VertexPosition.BottomLeft) && positions.Contains(VertexPosition.TopRight);


            foreach (var point in tileVertices) {

                switch (point.vertexPosition) {

                    case VertexPosition.TopLeft:

                        if (point.heightChannel == lowestHeight) {

                            vertices.Add(new Vector3(0, -meshSize));

                        }
                        else {

                            vertices.Add(new Vector3(x: 0, y: 0));

                        }

                        break;
                    case VertexPosition.TopRight:

                        if (point.heightChannel == lowestHeight) {

                            vertices.Add(new Vector3(meshSize, -meshSize));

                        }
                        else {

                            vertices.Add(new Vector3(meshSize, 0));

                        }

                        break;
                    case VertexPosition.BottomLeft:

                        if (isDiagnal) {

                            if (point.heightChannel == lowestHeight) {

                                vertices.Add(new Vector3(0, -meshSize));

                            }
                            else {

                                vertices.Add(new Vector3(x: 0, y: 0));

                            }

                        }
                        else {

                            if (point.heightChannel == lowestHeight) {

                                vertices.Add(new Vector3(meshSize, -meshSize));

                            }
                            else {

                                vertices.Add(new Vector3(meshSize, 0));

                            }

                        }

                        break;
                    case VertexPosition.BottomRight:

                        if (point.heightChannel == lowestHeight) {

                            vertices.Add(new Vector3(meshSize, -meshSize));

                        }
                        else {

                            vertices.Add(new Vector3(meshSize, 0));

                        }

                        break;
                }

            }

            return vertices;

        }

        foreach (var point in tileVertices) {

            switch (point.vertexPosition) {

                case VertexPosition.TopLeft:
                    vertices.Add(new Vector3(x: 0, y: 0));

                    break;
                case VertexPosition.TopRight:
                    vertices.Add(new Vector3(meshSize, 0));

                    break;
                case VertexPosition.BottomLeft:
                    vertices.Add(new Vector3(0, -meshSize));

                    break;
                case VertexPosition.BottomRight:
                    vertices.Add(new Vector3(meshSize, -meshSize));

                    break;
            }

        }

        return vertices;

    }

}

class SubMesh {

    public Mesh mesh = new Mesh();
    public MeshRenderer meshRenderer;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> textureCords = new List<Vector2>();
    public List<Color> vertexColors = new();
    public List<AnimatedTile> animatedTiles = new();
    public List<ShaderAnimatedTile> shaderAnimatedTiles = new();

    public int vertexIndex = 0;

    public void Update() {

        var didChange = false;
        foreach (var tile in animatedTiles) {
            var value = tile.Update(textureCords);

            if (value) {
                didChange = true;
            }

        }

        if (didChange) {
            RefreshCurrentUVs();
        }

        //foreach (var tile in shaderAnimatedTiles) {
        //    tile.Update(vertexColors);
        //}

        //RefreshCurrentVertexColors();

    }

    public void ClearMesh() {
        vertexIndex = 0;
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        textureCords.Clear();
        vertexColors.Clear();
        animatedTiles.Clear();
        shaderAnimatedTiles.Clear();
    }

    public void SetMesh() {

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = textureCords.ToArray();
        if (SettingsManager.showShaders) {
            mesh.colors = vertexColors.ToArray();
        }
        else {
            mesh.colors = new Color[0];
        }

        mesh.RecalculateNormals();

    }

    public void RefreshCurrentUVs() {
        mesh.uv = textureCords.ToArray();
    }

    public void RefreshCurrentVertexColors() {
        mesh.colors = vertexColors.ToArray();
    }

}

public interface AnimatedTile {

    public Tile tile { get; set; }
    public int textureIndex { get; set; }

    public bool Update(List<Vector2> textureCoords);



}

public class VectorAnimatedTile : AnimatedTile {

    public Tile tile { get; set; }
    public int textureIndex { get; set; }
    public FCopLevelSection section;

    int displacementX = 0;
    int displacementY = 0;

    public VectorAnimatedTile(Tile tile, int textureIndex, FCopLevelSection section) {
        this.tile = tile;
        this.textureIndex = textureIndex;
        this.section = section;
    }

    float timerX = 0f;
    float timerY = 0f;

    public bool Update(List<Vector2> textureCoords) {

        var didChange = false;

        if (section.animationVector.x != 0) {

            if (timerX >= AnimationVector.frameTime / Mathf.Abs(section.animationVector.x)) {

                if (section.animationVector.x > 0) {
                    displacementX++;
                }
                else {
                    displacementX--;
                }

                timerX -= AnimationVector.frameTime / Mathf.Abs(section.animationVector.x);
                didChange = true;
            }

            timerX += Time.deltaTime;


        }

        if (section.animationVector.y != 0) {

            if (timerY >= AnimationVector.frameTime / Mathf.Abs(section.animationVector.y)) {

                if (section.animationVector.y > 0) {
                    displacementY++;
                }
                else {
                    displacementY--;
                }

                timerY -= AnimationVector.frameTime / Mathf.Abs(section.animationVector.y);
                didChange = true;
            }

            timerY += Time.deltaTime;


        }

        if (section.animationVector.x > 0) {

            if (displacementX > AnimationVector.maxDistance) {
                displacementX = 0;
            }

        }
        else {

            if (displacementX < 0) {
                displacementX = AnimationVector.maxDistance;
            }

        }



        if (section.animationVector.y > 0) {

            if (displacementY > AnimationVector.maxDistance) {
                displacementY = 0;
            }

        }
        else {

            if (displacementY < 0) {
                displacementY = AnimationVector.maxDistance;
            }

        }

        ChangeTexture(textureCoords);

        return didChange;

    }

    public void ChangeTexture(List<Vector2> textureCoords) {

        if (tile.verticies.Count == 4) {
            textureCoords[textureIndex] = GetTextureCoord(tile, 0);
            textureCoords[textureIndex + 1] = GetTextureCoord(tile, 1);
            textureCoords[textureIndex + 2] = GetTextureCoord(tile, 3);
            textureCoords[textureIndex + 3] = GetTextureCoord(tile, 2);
        }
        else {
            textureCoords[textureIndex] = GetTextureCoord(tile, 0);
            textureCoords[textureIndex + 1] = GetTextureCoord(tile, 1);
            textureCoords[textureIndex + 2] = GetTextureCoord(tile, 2);
        }


    }

    Vector2 GetTextureCoord(Tile tile, int i) {
        return new Vector2(
                TextureCoordinate.GetX((tile.uvs[i] + tile.texturePalette * 65536) + displacementX),
                TextureCoordinate.GetY((tile.uvs[i] + tile.texturePalette * 65536) + (256 * displacementY))
            );
    }

}

public class FrameAnimatedTile : AnimatedTile {

    public Tile tile { get; set; }
    public int textureIndex { get; set; }

    int frame = 0;
    float timer = 0f;

    public FrameAnimatedTile(Tile tile, int textureIndex) {
        this.tile = tile;
        this.textureIndex = textureIndex;
    }

    public bool Update(List<Vector2> textureCoords) {

        var didChange = false;

        if (timer >= TileUVAnimationMetaData.secondsPerValue * tile.animationSpeed) {
            ChangeTexture(textureCoords);
            frame++;

            if (frame == tile.GetFrameCount()) {
                frame = 0;
            }

            timer -= TileUVAnimationMetaData.secondsPerValue * tile.animationSpeed;
            didChange = true;
        }

        timer += Time.deltaTime;

        return didChange;

    }

    public void ChangeTexture(List<Vector2> textureCoords) {

        if (tile.verticies.Count == 4) {
            textureCoords[textureIndex] = GetTextureCoord(tile, (frame * 4));
            textureCoords[textureIndex + 1] = GetTextureCoord(tile, (frame * 4) + 1);
            textureCoords[textureIndex + 2] = GetTextureCoord(tile, (frame * 4) + 3);
            textureCoords[textureIndex + 3] = GetTextureCoord(tile, (frame * 4) + 2);
        }
        else {
            textureCoords[textureIndex] = GetTextureCoord(tile, (frame * 4));
            textureCoords[textureIndex + 1] = GetTextureCoord(tile, (frame * 4) + 1);
            textureCoords[textureIndex + 2] = GetTextureCoord(tile, (frame * 4) + 2);
        }

    }

    Vector2 GetTextureCoord(Tile tile, int i) {

        if (tile.animatedUVs.Count <= i) {
            return new Vector2(
                TextureCoordinate.GetX(tile.animatedUVs[0] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(tile.animatedUVs[0] + tile.texturePalette * 65536)
            );
        }

        return new Vector2(
                TextureCoordinate.GetX(tile.animatedUVs[i] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(tile.animatedUVs[i] + tile.texturePalette * 65536)
            );
    }

}

public class ShaderAnimatedTile {

    static System.Random random = new();

    public Tile tile;
    public int colorIndex;

    public ShaderAnimatedTile(Tile tile, int colorIndex) {
        this.tile = tile;
        this.colorIndex = colorIndex;
    }

    public void Update(List<Color> colors) {


    }


}