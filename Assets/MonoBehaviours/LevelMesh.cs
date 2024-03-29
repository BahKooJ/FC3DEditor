
using FCopParser;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;


public class LevelMesh : MonoBehaviour {

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

    interface AnimatedTile { 

        public Tile tile { get; set; }
        public int textureIndex { get; set; }

        public bool Update(List<Vector2> textureCoords);



    }

    class VectorAnimatedTile : AnimatedTile {

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
                    } else {
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

            } else {

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
            } else {
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

    class FrameAnimatedTile : AnimatedTile {

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

            var animationData = (TileUVAnimationMetaData)tile.uvAnimationData;

            if (timer >= TileUVAnimationMetaData.secondsPerValue * animationData.frameDuration) {
                ChangeTexture(textureCoords);
                frame++;

                if (frame == animationData.frames) {
                    frame = 0;
                }

                timer -= TileUVAnimationMetaData.secondsPerValue * animationData.frameDuration;
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
            } else {
                textureCoords[textureIndex] = GetTextureCoord(tile, (frame * 4));
                textureCoords[textureIndex + 1] = GetTextureCoord(tile, (frame * 4) + 1);
                textureCoords[textureIndex + 2] = GetTextureCoord(tile, (frame * 4) + 2);
            }

        }

        Vector2 GetTextureCoord(Tile tile, int i) {
            return new Vector2(
                    TextureCoordinate.GetX(tile.animatedUVs[i] + tile.texturePalette * 65536),
                    TextureCoordinate.GetY(tile.animatedUVs[i] + tile.texturePalette * 65536)
                );
        }

    }

    class ShaderAnimatedTile {

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

    // Prefabs
    public GameObject subMeshTransparent;

    Mesh mesh;
    MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public FCopLevelSection section;
    public Texture2D levelTexturePallet;
    public Main controller;

    SubMesh transparentSubMesh;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> textureCords = new List<Vector2>();

    List<Color> vertexColors = new();

    public List<Tile> sortedTilesByTriangle = new List<Tile>();

    List<AnimatedTile> animatedTiles = new();
    List<ShaderAnimatedTile> shaderAnimatedTiles = new();

    public float x = 0;
    public float y = 0;

    // Start is called before the first frame update
    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();

        transparentSubMesh = new();
        var subMesh = Instantiate(subMeshTransparent);
        subMesh.transform.SetParent(transform, false);
        subMesh.GetComponent<MeshFilter>().mesh = transparentSubMesh.mesh;
        transparentSubMesh.meshRenderer = subMesh.GetComponent<MeshRenderer>();
        transparentSubMesh.meshRenderer.material.mainTexture = levelTexturePallet;

        meshRenderer.material.mainTexture = levelTexturePallet;
        RefreshMesh();

    }

    // Update is called once per frame
    void Update() {

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

        transparentSubMesh.Update();

    }

    void Generate(FCopLevelSection section) {

        var vertexIndex = 0;
        var index = 0;
        foreach (var column in section.tileColumns) {

            List<Vector3> AddVerticies(Tile tile) {

                var vertices = new List<Vector3>();

                foreach (var point in tile.verticies) {

                    switch (point.vertexPosition) {

                        case VertexPosition.TopLeft:
                            vertices.Add(new Vector3(x: column.x, y: column.heights[0].GetPoint(point.heightChannel), z: column.y));

                            break;
                        case VertexPosition.TopRight:
                            vertices.Add(new Vector3(x: column.x + 1, y: column.heights[1].GetPoint(point.heightChannel), z: column.y));

                            break;
                        case VertexPosition.BottomLeft:
                            vertices.Add(new Vector3(x: column.x, y: column.heights[2].GetPoint(point.heightChannel), z: column.y + 1));

                            break;
                        case VertexPosition.BottomRight:
                            vertices.Add(new Vector3(x: column.x + 1, y: column.heights[3].GetPoint(point.heightChannel), z: column.y + 1));

                            break;
                    }

                }

                return vertices;

            }

            Vector2 GetTextureCoord(Tile tile, int i) {
                return new Vector2(
                        TextureCoordinate.GetX(tile.uvs[i] + tile.texturePalette * 65536),
                        TextureCoordinate.GetY(tile.uvs[i] + tile.texturePalette * 65536)
                    );
            }

            void MakeEmptyTile(bool isQuad) {

                if (isQuad) {

                    textureCords.Add(new Vector2(5f / 256f, 2565f / 2580f));
                    textureCords.Add(new Vector2(10f / 256f, 2565f / 2580f));
                    textureCords.Add(new Vector2(5f / 256f, 2570f / 2580f));
                    textureCords.Add(new Vector2(10f / 256f, 2570f / 2580f));

                    vertexColors.Add(Color.white);
                    vertexColors.Add(Color.white);
                    vertexColors.Add(Color.white);
                    vertexColors.Add(Color.white);



                }
                else {

                    textureCords.Add(new Vector2(5f / 256f, 2565f / 2580f));
                    textureCords.Add(new Vector2(10f / 256f, 2565f / 2580f));
                    textureCords.Add(new Vector2(5f / 256f, 2570f / 2580f));

                    vertexColors.Add(Color.white);
                    vertexColors.Add(Color.white);
                    vertexColors.Add(Color.white);

                }

            }

            void GenerateTriangle(Tile tile) {

                vertices.AddRange(AddVerticies(tile));

                if (tile.isSemiTransparent && SettingsManager.showTransparency) {

                    if (tile.isVectorAnimated && SettingsManager.showAnimations) {
                        transparentSubMesh.animatedTiles.Add(new VectorAnimatedTile(tile, transparentSubMesh.vertexIndex, section));
                    }

                    if (tile.uvAnimationData != null && SettingsManager.showAnimations) {
                        transparentSubMesh.animatedTiles.Add(new FrameAnimatedTile(tile, transparentSubMesh.vertexIndex));
                    }

                    MakeEmptyTile(false);

                    transparentSubMesh.vertices.AddRange(AddVerticies(tile));

                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 0));
                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 2));
                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 1));

                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));

                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 1);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 2);

                    transparentSubMesh.vertexIndex += 3;

                }
                else {

                    if (tile.isVectorAnimated && SettingsManager.showAnimations) {
                        animatedTiles.Add(new VectorAnimatedTile(tile, vertexIndex, section));
                    }

                    if (tile.uvAnimationData != null && SettingsManager.showAnimations) {
                        animatedTiles.Add(new FrameAnimatedTile(tile, vertexIndex));
                    }

                    textureCords.Add(GetTextureCoord(tile, 0));
                    textureCords.Add(GetTextureCoord(tile, 2));
                    textureCords.Add(GetTextureCoord(tile, 1));

                    vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
                    vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
                    vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));

                }

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                vertexIndex += 3;

            }

            void GenerateSquare(Tile tile) {

                vertices.AddRange(AddVerticies(tile));

                if (tile.isSemiTransparent && SettingsManager.showTransparency) {

                    if (tile.isVectorAnimated && SettingsManager.showAnimations) {
                        transparentSubMesh.animatedTiles.Add(new VectorAnimatedTile(tile, transparentSubMesh.vertexIndex, section));
                    }

                    if (tile.uvAnimationData != null && SettingsManager.showAnimations) {
                        transparentSubMesh.animatedTiles.Add(new FrameAnimatedTile(tile, transparentSubMesh.vertexIndex));
                    }

                    if (tile.shaders.type == VertexColorType.ColorAnimated) {
                        transparentSubMesh.shaderAnimatedTiles.Add(new ShaderAnimatedTile(tile, transparentSubMesh.vertexIndex));
                    }

                    MakeEmptyTile(true);

                    transparentSubMesh.vertices.AddRange(AddVerticies(tile));

                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 0));
                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 1));
                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 3));
                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 2));

                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));
                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[3][0], tile.shaders.colors[3][1], tile.shaders.colors[3][2]));

                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 2);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 1);

                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 1);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 2);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 3);

                    transparentSubMesh.vertexIndex += 4;

                }
                else {

                    if (tile.isVectorAnimated && SettingsManager.showAnimations) {
                        animatedTiles.Add(new VectorAnimatedTile(tile, vertexIndex, section));
                    }

                    if (tile.uvAnimationData != null && SettingsManager.showAnimations) {
                        animatedTiles.Add(new FrameAnimatedTile(tile, vertexIndex));
                    }

                    if (tile.shaders.type == VertexColorType.ColorAnimated) {
                        shaderAnimatedTiles.Add(new ShaderAnimatedTile(tile, vertexIndex));
                    }

                    textureCords.Add(GetTextureCoord(tile, 0));
                    textureCords.Add(GetTextureCoord(tile, 1));
                    textureCords.Add(GetTextureCoord(tile, 3));
                    textureCords.Add(GetTextureCoord(tile, 2));

                    vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
                    vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
                    vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));
                    vertexColors.Add(new Color(tile.shaders.colors[3][0], tile.shaders.colors[3][1], tile.shaders.colors[3][2]));

                }

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);

                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4;

            }


            foreach (var tile in column.tiles) {

                if (tile.verticies.Count == 3) {
                    GenerateTriangle(tile);
                    sortedTilesByTriangle.Add(tile);
                } else if (tile.verticies.Count == 4) {
                    GenerateSquare(tile);
                    sortedTilesByTriangle.Add(tile);
                    sortedTilesByTriangle.Add(tile);
                }

            }

            index++;

        }

    }

    public void RefreshMesh() {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        textureCords.Clear();
        vertexColors.Clear();
        sortedTilesByTriangle.Clear();

        transparentSubMesh.ClearMesh();
        animatedTiles.Clear();
        shaderAnimatedTiles.Clear();

        Generate(section);

        if (transparentSubMesh.vertices.Count != 0) {
            transparentSubMesh.SetMesh();
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = textureCords.ToArray();
        if (SettingsManager.showShaders) {
            mesh.colors = vertexColors.ToArray();
        } else {
            mesh.colors = new Color[0];
        }

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

        if (SettingsManager.clipBlack) {
            meshRenderer.material.SetFloat("_ClipBlack", 0.1f);
        } else {
            meshRenderer.material.SetFloat("_ClipBlack", 0f);
        }


    }

    public void RefreshTexture() {
        levelTexturePallet = controller.levelTexturePallet;
        meshRenderer.material.mainTexture = levelTexturePallet;
        transparentSubMesh.meshRenderer.material.mainTexture = levelTexturePallet;
    }

    public void RefreshCurrentUVs() {
        mesh.uv = textureCords.ToArray();
    }

    public void RefreshCurrentVertexColors() {
        mesh.colors = vertexColors.ToArray();
    }

}