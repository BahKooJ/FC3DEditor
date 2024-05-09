
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    // =WIP=
    public class FCopLevel {

        public FCopRPNS rpns;

        public List<List<int>> layout;

        public List<FCopLevelSection> sections = new();

        public List<FCopTexture> textures = new();

        public List<FCopNavMesh> navMeshes = new();

        public List<FCopObject> objects = new();

        public List<FCopActor> actors = new();

        public IFFFileManager fileManager;

        public FCopLevel(IFFFileManager fileManager) {

            this.fileManager = fileManager;

            layout = FCopLevelLayoutParser.Parse(fileManager.files.First(file => {

                return file.dataFourCC == "Cptc";

            }));

            var rawCtilFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Ctil";

            }).ToList();

            foreach (var rawFile in rawCtilFiles) {
                sections.Add(new FCopLevelSectionParser(rawFile).Parse(this));
            }

            InitData();

        }

        public FCopLevel(int width, int height, IFFFileManager fileManager) {

            this.fileManager = fileManager;

            layout = new List<List<int>>();


            // TODO: This abomination needs to get cleaned up
            foreach (int _ in Enumerable.Range(0, 4)) {

                layout.Add(new List<int>());

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1 });

                foreach (int __ in Enumerable.Range(0, width)) {
                    layout.Last().Add(1);
                }

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1, 0 });

            }

            var id = 2;

            foreach (int _ in Enumerable.Range(0, height)) {

                layout.Add(new List<int>());

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1 });

                foreach (int i in Enumerable.Range(0, width)) {
                    layout.Last().Add(id);
                    id++;
                }

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1, 0 });

            }

            foreach (int _ in Enumerable.Range(0, 4)) {

                layout.Add(new List<int>());

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1 });

                foreach (int __ in Enumerable.Range(0, width)) {
                    layout.Last().Add(1);
                }

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1, 0 });

            }

            layout.Add(new List<int>());

            layout.Last().AddRange(new List<int>() { 0, 0, 0, 0 });

            foreach (int __ in Enumerable.Range(0, width)) {
                layout.Last().Add(0);
            }

            layout.Last().AddRange(new List<int>() { 0, 0, 0, 0, 0 });

            var rawCtilFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Ctil";

            }).ToList();

            var oobSection = new FCopLevelSectionParser(rawCtilFiles[0]).Parse(this);

            oobSection.parser.rawFile = oobSection.parser.rawFile.Clone(1);

            foreach (var h in oobSection.heightMap) {
                h.SetPoint(19, 1);
                h.SetPoint(-128, 2);
                h.SetPoint(-128, 3);
            }

            foreach (var tColumn in oobSection.tileColumns) {

                tColumn.tiles.Clear();

                tColumn.tiles.Add(new Tile(tColumn, MeshType.VerticiesFromID(68), 0, new() { 57200, 57228, 50060, 50032 }, new TileGraphics(116, 6, 0, 0, 1, 0)));

            }

            sections.Add(oobSection);

            foreach (var row in layout) {

                foreach (var column in row) {

                    if (column == 0 || column == 1) {
                        continue;
                    }

                    var newSection = new FCopLevelSectionParser(rawCtilFiles[0]).Parse(this);

                    newSection.parser.rawFile = newSection.parser.rawFile.Clone(column);

                    foreach (var h in newSection.heightMap) {
                        h.SetPoint(-120, 1);
                        h.SetPoint(-100, 2);
                        h.SetPoint(-80, 3);
                    }

                    foreach (var tColumn in newSection.tileColumns) {

                        tColumn.tiles.Clear();

                        tColumn.tiles.Add(new Tile(tColumn, MeshType.VerticiesFromID(68), 0, new() { 57200, 57228, 50060, 50032 }, new TileGraphics(116, 6, 0, 0, 1, 0)));

                    }

                    sections.Add(newSection);

                }


            }

            InitData();

        }

        void InitData() {

            rpns = new FCopRPNS(fileManager.files.First(file => {

                return file.dataFourCC == "RPNS";

            }));

            var rawBitmapFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Cbmp";

            }).ToList();

            var rawNavMeshFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Cnet";

            }).ToList();

            var rawObjectFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Cobj";

            }).ToList();

            var rawActorFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Cact" || file.dataFourCC == "Csac";

            }).ToList();

            foreach (var rawFile in rawBitmapFiles) {
                textures.Add(new FCopTexture(rawFile));
            }

            foreach (var rawFile in rawNavMeshFiles) {
                navMeshes.Add(new FCopNavMesh(rawFile));
            }

            foreach (var rawFile in rawObjectFiles) {
                objects.Add(new FCopObject(rawFile));
            }

            foreach (var rawFile in rawActorFiles) {

                actors.Add(new FCopActor(rawFile));

            }

        }

        public void Compile() {

            foreach (var section in sections) {
                section.Compile();
            }

            foreach (var navMesh in navMeshes) {
                navMesh.Compile();
            }

            foreach (var texture in textures) {
                texture.Compile();
            }

            foreach (var actor in actors) {
                actor.Compile();
            }

            FCopLevelLayoutParser.Compile(layout, fileManager.files.First(file => {

                return file.dataFourCC == "Cptc";

            }));

        }

    }

    public class FCopLevelSection {

        public FCopLevel parent;

        public const int heightMapWdith = 17;

        public const int tileColumnsWidth = 16;

        public List<HeightPoints> heightMap = new List<HeightPoints>();
        public List<TileColumn> tileColumns = new List<TileColumn>();
        List<XRGB555> colors = new List<XRGB555>();
        public AnimationVector animationVector;

        public LevelCulling culling;

        // Until the file can be fully parsed, we need to have the parser
        public FCopLevelSectionParser parser;

        public FCopLevelSection(FCopLevelSectionParser parser, FCopLevel parent) {

            this.parser = parser;
            this.colors = parser.colors;
            this.culling = parser.culling;

            animationVector = new AnimationVector(parser.animationVector);

            foreach (var parsePoint in parser.heightPoints) {
                heightMap.Add(new HeightPoints(parsePoint));
            }

            var count = 0;
            var x = 0;
            var y = 0;
            foreach (var parseColumn in parser.thirdSectionBitfields) {

                // Grabs the tiles for the column in the tiles array. Number 2 is the index of the tiles and number 1 is the count.
                var parsedTiles = parser.tiles.GetRange(parseColumn.number2, parseColumn.number1);

                // Makes the parsed bitfield into a Tile object.
                var tiles = new List<Tile>();

                // Grabs the heights. The heights have already been added so it uses the local height array.
                var heights = new List<HeightPoints>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                var column = new TileColumn(x, y, tiles, heights);

                foreach (var parsedTile in parsedTiles) {
                    tiles.Add(new Tile(parsedTile, column, parser));
                }

                tileColumns.Add(column);

                x++;
                if (x == 16) {
                    y++;
                    x = 0;
                }

                count++;

            }

            this.parent = parent;

        }

        public HeightPoints GetHeightPoint(int x, int y) {
            return heightMap[(y * heightMapWdith) + x];
        }

        public TileColumn GetTileColumn(int x, int y) {
            return tileColumns[(y * tileColumnsWidth) + x];
        }

        class Chunk {

            public int x;
            public int y;

            public List<TileColumn> tileColumns = new List<TileColumn>();

            public Chunk(int x, int y) {
                this.x = x;
                this.y = y;
            }

            public int Count() {

                var total = 0;

                foreach (var column in tileColumns) {

                    total += column.tiles.Count;

                }

                return total;

            }

        }

        // Takes all the higher parsed data and puts them back into their basic data form found in Ctil.
        // This method does all the indexing and compression to allow for FCopLevelParser to convert the data back into binary.
        public void Compile() {

            culling.CalculateCulling(this);

            List<HeightPoint3> heightPoints = new List<HeightPoint3>();
            List<ThirdSectionBitfield> thirdSectionBitfields = new List<ThirdSectionBitfield>();
            List<TileBitfield> tiles = new List<TileBitfield>();

            var textureCoordinates = new List<int>();
            var tileGraphics = new List<TileGraphicsItem>();
            var tileUVAnimationMetaData = new List<TileUVAnimationMetaData>();
            var animatedTextureCoordinates = new List<int>();

            var existingColors = new Dictionary<ushort, (int, XRGB555)>();
            var colorIndex = 0;

            List<Chunk> chunks = new List<Chunk>() { new Chunk(0,0) };

            foreach (var point in heightMap) {
                heightPoints.Add(point.Compile());
            }

            // IMPORTANT: The tile column array inside the Ctil is sorted from left to right, HOWEVER the tile array is not.
            // The tile array stores tiles inside a 4x4 tile chunk. The tiles inside this chunk move from left to right,
            // and chunks move from left to right as well. What needs to be done is take the sorted tile columns and move 
            // them to the 4x4 chunk pattern. This needs to be done for the tile array alone.
            var x = 0;
            var y = 0;
            var chunkX = 0;
            var chunkY = 0;
            foreach (var i in Enumerable.Range(0,256)) {

                var offsetX = ((chunks.Count - 1) % 4) * 4;
                var offsetY = ((chunks.Count - 1) / 4) * 4;

                var index = ((y + offsetY) * 16) + (x + offsetX);

                chunks.Last().tileColumns.Add(tileColumns[index]);

                x++;

                if (x == 4) {
                    y++;
                    x = 0;
                    if (y == 4) {
                        y = 0;
                        chunkX++;

                        if (chunkX == 4) {
                            chunkY++;

                            if (chunkY == 4) {
                                break;
                            }

                            chunkX = 0;
                        }

                        chunks.Add(new Chunk(chunkX, chunkY));

                    }

                }

            }

            foreach (var chunk in chunks) {

                foreach (var column in chunk.tileColumns) {

                    var sortedTiles = new List<TileBitfield>();

                    // Now that the tile columns are sorted to fit the 4x4 chunk pattern in the tile array, we can simple add the tiles after they're sorted.
                    foreach (var tile in column.tiles) {

                        // TODO: Maybe these compressions would be better with dictonaries?

                        // Compresses both the uv mapping and tile graphics
                        int textureIndex = -1;
                        int graphicsIndex = -1;

                        // First the uvs... (Includeding animated UVs)
                        #region CompileTextures

                        if (textureCoordinates.Count != 0) {

                            foreach (var i in Enumerable.Range(0, textureCoordinates.Count - tile.uvs.Count + 1)) {

                                if (textureCoordinates.GetRange(i, tile.uvs.Count).SequenceEqual(tile.uvs)) {

                                    // If no UV animations exist no need to test for them
                                    if (tileUVAnimationMetaData.Count == 0) {

                                        // Index was found, but no uv animation data has been collected so this is a static texture.
                                        // This index is invalid.
                                        if (tile.IsFrameAnimated()) {
                                            break;
                                        }

                                        textureIndex = i;
                                        break;
                                    }

                                    var isTileAnimated = tile.IsFrameAnimated();

                                    var found = false;

                                    // Tests to see if the index is overwritten by animated UVs
                                    foreach (var metaData in tileUVAnimationMetaData) {

                                        if (metaData.textureReplaceOffset / 2 == i) {

                                            // If this tile is not animated but this index is overwritten,
                                            // this index is not valid.
                                            if (!isTileAnimated) {
                                                found = true;
                                                break;
                                            }

                                            if (metaData.frames == tile.GetFrameCount() && metaData.frameDuration == tile.animationSpeed) {

                                                // Tests to see if that animated UVs are the same
                                                if (animatedTextureCoordinates.GetRange(metaData.animationOffset / 2, metaData.frames * 4).SequenceEqual(tile.animatedUVs)) {

                                                    textureIndex = i;
                                                    found = true;
                                                    break;

                                                }

                                            }

                                        }

                                    }

                                    // Index was found, so the loop can stop
                                    if (found && isTileAnimated) { break; }

                                    // If this index isn't overwritten and the tile isn't animated this is a valid index
                                    if (!found && !isTileAnimated) {
                                        textureIndex = i; 
                                        break;
                                    }


                                }

                            }

                        }

                        // No index was found, so new data needs to be added
                        if (textureIndex == -1) {

                            textureIndex = textureCoordinates.Count;

                            if (tile.IsFrameAnimated()) {

                                tileUVAnimationMetaData.Add(tile.CompileFrameAnimation(animatedTextureCoordinates.Count * 2, textureCoordinates.Count * 2));
                                animatedTextureCoordinates.AddRange(tile.animatedUVs);

                            }

                            textureCoordinates.AddRange(tile.uvs);


                            if (textureCoordinates.Count > 1024) {
                                throw new TextureArrayMaxExceeded();
                            }

                        }

                        #endregion

                        // Next the colors... (these are for colored vertices)
                        #region Compile Colors

                        if (tile.shaders.type == VertexColorType.Color) {
                            var shader = (ColorShader)tile.shaders;

                            foreach (var color in shader.values) {

                                if (!existingColors.ContainsKey(color.ToUShort())) {
                                    existingColors.Add(color.ToUShort(), (colorIndex, color));
                                    colorIndex++;
                                }

                            }

                        }

                        if (existingColors.Count > 255) {
                            throw new ColorArrayMaxExceeded();
                        }

                        

                        #endregion

                        // Finally the graphics
                        #region CompileGraphics

                        // TEMPORARY
                        // If tile has animated shaders add SLFX data

                        //if (tile.shaders.type == VertexColorType.ColorAnimated) {

                        //    parser.slfxData = new List<byte> { 16, 128, 50, 20 };

                        //}

                        var compiledGraphics = tile.CompileGraphics(existingColors);

                        if (tileGraphics.Count != 0) {

                            foreach (var i in Enumerable.Range(0, tileGraphics.Count)) {

                                var graphicsItem = tileGraphics[i];

                                if (graphicsItem is TileGraphics) {

                                    var graphics = (TileGraphics)graphicsItem;

                                    if ((TileGraphics)compiledGraphics[0] == graphics) {

                                        if (compiledGraphics.Count == 1) {
                                            graphicsIndex = i;
                                            break;
                                        } else {

                                            var same = true;
                                            var i2 = 0;
                                            foreach (var compiledGraphicsItem in compiledGraphics) {

                                                if (i2 == 0) {
                                                    i2++;
                                                    continue;
                                                }

                                                if (i + i2 > tileGraphics.Count) {
                                                    same = false;
                                                    break;
                                                }

                                                var compiledGraphicsMetaData = (TileGraphicsMetaData)compiledGraphicsItem;

                                                if (!(tileGraphics[i + i2] is TileGraphicsMetaData)) {
                                                    same = false;
                                                    break;
                                                }

                                                var graphicsMetaData = (TileGraphicsMetaData)tileGraphics[i + i2];

                                                if (!compiledGraphicsMetaData.data.SequenceEqual(graphicsMetaData.data)) {
                                                    same = false;
                                                    break;
                                                }

                                                i2++;

                                            }

                                            if (same) {
                                                graphicsIndex = i;
                                                break;
                                            }

                                        }

                                    }

                                }

                            }

                        }

                        if (graphicsIndex == -1) {

                            graphicsIndex = tileGraphics.Count;

                            tileGraphics.AddRange(compiledGraphics);

                            if (tileGraphics.Count > 1023) {
                                throw new GraphicsArrayMaxExceeded();
                            }

                        }

                        #endregion

                        var compiledTile = tile.Compile(textureIndex, graphicsIndex);

                        // Tiles are sorted within a tile column, the order is not completely known but what is know is walls cannot be first
                        if (compiledTile.meshID < 71) {
                            sortedTiles.Insert(0, compiledTile);
                        } else {
                            sortedTiles.Add(compiledTile);
                        }

                    }

                    // Makes sure the last tile value is correct

                    foreach (var i in Enumerable.Range(0, sortedTiles.Count)) {
                        var cTile = sortedTiles[i];
                        cTile.isEndInColumnArray = 0;
                        sortedTiles[i] = cTile;
                    }

                    var lastCTile = sortedTiles.Last();
                    lastCTile.isEndInColumnArray = 1;
                    sortedTiles[sortedTiles.Count - 1] = lastCTile;

                    tiles.AddRange(sortedTiles);

                }

            }

            // Because the tiles are now no longer sorted left to right, it finds the correct index of the tiles for the columns.
            var previousOffsetFromChunk = new Dictionary<int, int>() { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 } };
            var previousChunkY = 0;
            foreach (var column in tileColumns) {

                var tileOffset = 0;

                var offsetChunkX = column.x / 4;
                var offsetChunkY = column.y / 4;

                if (previousChunkY != offsetChunkY) {

                    foreach (var i in Enumerable.Range(0, 4)) {
                        previousOffsetFromChunk[i] = 0;
                    }

                    previousChunkY = offsetChunkY;

                }

                if (!(offsetChunkX == 0 && offsetChunkY == 0)) {
                    var previousChunks = chunks.GetRange(0, (offsetChunkY * 4) + offsetChunkX);

                    foreach (var chunk in previousChunks) {
                        tileOffset += chunk.Count();
                    }

                }

                var offsetTotal = previousOffsetFromChunk[offsetChunkX] + tileOffset;

                var bitField = new ThirdSectionBitfield(column.tiles.Count, offsetTotal);
                thirdSectionBitfields.Add(bitField);

                previousOffsetFromChunk[offsetChunkX] += column.tiles.Count;

            }

            parser.heightPoints = heightPoints;
            parser.culling = culling;
            parser.thirdSectionBitfields = thirdSectionBitfields;
            parser.tiles = tiles;
            parser.textureCoordinates = textureCoordinates;

            colors.Clear();
            foreach (var existingColor in existingColors) {
                colors.Add(existingColor.Value.Item2);
            }
            parser.colors = colors;

            parser.tileGraphics = tileGraphics;
            parser.tileUVAnimationMetaData = tileUVAnimationMetaData;
            parser.animatedTextureCoordinates = animatedTextureCoordinates;

            parser.animationVector = animationVector.Compile();

            parser.Compile();

        }

        public void MirrorDiagonally() {

            MirrorVertically();
            MirrorHorizontally();

        }

        public void MirrorHorizontally() {

            var newHeightOrder = new List<HeightPoints>();

            foreach (var hy in Enumerable.Range(0, 17)) {

                foreach (var hx in Enumerable.Range(0, 17)) {
                    newHeightOrder.Add(GetHeightPoint(16 - hx, hy));
                }

            }

            heightMap = newHeightOrder;

            var newTileColum = new List<TileColumn>();

            foreach (var ty in Enumerable.Range(0, 16)) {

                foreach (var tx in Enumerable.Range(0, 16)) {
                    var column = tileColumns[(ty * 16) + (15 - tx)];

                    var heights = new List<HeightPoints>();

                    heights.Add(GetHeightPoint(tx, ty));
                    heights.Add(GetHeightPoint(tx + 1, ty));
                    heights.Add(GetHeightPoint(tx, ty + 1));
                    heights.Add(GetHeightPoint(tx + 1, ty + 1));

                    column.x = tx;
                    column.y = ty;
                    column.heights = heights;

                    newTileColum.Add(column);
                }

            }

            tileColumns = newTileColum;

            var movedTiles = new List<Tile>();

            foreach (var column in tileColumns) {

                var validTiles = new List<Tile>();

                foreach (var tile in column.tiles) {

                    if (movedTiles.Contains(tile)) {
                        validTiles.Add(tile);
                        continue;
                    }

                    int ogMeshID = (int)MeshType.IDFromVerticies(tile.verticies);

                    var mirorVertices = new List<TileVertex>();

                    foreach (var vertex in tile.verticies) {

                        switch (vertex.vertexPosition) {

                            case VertexPosition.TopLeft:
                                mirorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                                break;
                            case VertexPosition.TopRight:
                                mirorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                                break;
                            case VertexPosition.BottomLeft:
                                mirorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomRight));
                                break;
                            case VertexPosition.BottomRight:
                                mirorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                                break;

                        }

                    }

                    var mirorVID = MeshType.IDFromVerticies(mirorVertices);

                    if (mirorVID != null) {

                        tile.verticies = MeshType.VerticiesFromID((int)mirorVID);

                        FlipUVOrderHorizontal(tile);

                        validTiles.Add(tile);
                    } else {

                        if (new int[] { 71, 72, 73, 74, 75, 76, 77, 78, 107, 109 }.Contains(ogMeshID)) {

                            if (column.x < 15) {

                                var nextColumn = tileColumns[(column.y * 16) + (column.x + 1)];

                                tile.column = nextColumn;

                                nextColumn.tiles.Add(tile);
                                movedTiles.Add(tile);

                            }

                        }

                    }

                }

                column.tiles = validTiles;
            }


        }

        public void MirrorVertically() {

            var newHeightOrder = new List<HeightPoints>();

            foreach (var hy in Enumerable.Range(0, 17)) {

                foreach (var hx in Enumerable.Range(0, 17)) {
                    newHeightOrder.Add(GetHeightPoint(hx, 16 - hy));
                }

            }

            heightMap = newHeightOrder;

            var newTileColum = new List<TileColumn>();

            foreach (var ty in Enumerable.Range(0, 16)) {

                foreach (var tx in Enumerable.Range(0, 16)) {
                    var column = tileColumns[((15 - ty) * 16) + tx];

                    var heights = new List<HeightPoints>();

                    heights.Add(GetHeightPoint(tx, ty));
                    heights.Add(GetHeightPoint(tx + 1, ty));
                    heights.Add(GetHeightPoint(tx, ty + 1));
                    heights.Add(GetHeightPoint(tx + 1, ty + 1));

                    column.x = tx;
                    column.y = ty;
                    column.heights = heights;

                    newTileColum.Add(column);
                }

            }

            tileColumns = newTileColum;

            var movedTiles = new List<Tile>();

            foreach (var column in tileColumns) {

                var validTiles = new List<Tile>();

                foreach (var tile in column.tiles) {

                    if (movedTiles.Contains(tile)) {
                        validTiles.Add(tile);
                        continue;
                    }

                    int ogMeshID = (int)MeshType.IDFromVerticies(tile.verticies);

                    var mirorVertices = new List<TileVertex>();

                    foreach (var vertex in tile.verticies) {

                        switch (vertex.vertexPosition) {

                            case VertexPosition.TopLeft:
                                mirorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                                break;
                            case VertexPosition.TopRight:
                                mirorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomRight));
                                break;
                            case VertexPosition.BottomLeft:
                                mirorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                                break;
                            case VertexPosition.BottomRight:
                                mirorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                                break;

                        }

                    }

                    var mirorVID = MeshType.IDFromVerticies(mirorVertices);

                    if (mirorVID != null) {

                        tile.verticies = MeshType.VerticiesFromID((int)mirorVID);

                        FlipUVOrderVertically(tile);

                        validTiles.Add(tile);
                    } else {

                        if (new int[] { 79, 80, 81, 82, 83, 84, 85, 86, 108, 110 }.Contains(ogMeshID)) {

                            if (column.y < 15) {

                                var nextColumn = tileColumns[((column.y + 1) * 16) + (column.x)];

                                tile.column = nextColumn;

                                nextColumn.tiles.Add(tile);
                                movedTiles.Add(tile);

                            }

                        }

                    }

                }

                column.tiles = validTiles;
            }


        }

        void FlipUVOrderVertically(Tile tile) {

            var uvVectors = new List<int[]>();

            foreach (var uv in tile.uvs) {
                uvVectors.Add(TextureCoordinate.GetVector(uv));
            }

            var newUVs = new List<int[]>();

            if (tile.uvs.Count == 4) {

                newUVs.Add(uvVectors[3]);
                newUVs.Add(uvVectors[2]);
                newUVs.Add(uvVectors[1]);
                newUVs.Add(uvVectors[0]);

            }
            else {

                newUVs.Add(uvVectors[2]);
                newUVs.Add(uvVectors[1]);
                newUVs.Add(uvVectors[0]);

            }

            foreach (var i in Enumerable.Range(0, tile.uvs.Count)) {
                tile.uvs[i] = TextureCoordinate.SetPixel(newUVs[i][0], newUVs[i][1]);
            }

        }

        void FlipUVOrderHorizontal(Tile tile) {

            var positions = new List<VertexPosition>();
            foreach (var vert in tile.verticies) {

                if (positions.Contains(vert.vertexPosition)) {

                    FlipUVPositionHorizontally(tile);
                    return;

                }

                positions.Add(vert.vertexPosition);

            }

            var uvVectors = new List<int[]>();

            foreach (var uv in tile.uvs) {
                uvVectors.Add(TextureCoordinate.GetVector(uv));
            }

            var newUVs = new List<int[]>();

            if (tile.uvs.Count == 4) {

                newUVs.Add(uvVectors[1]);
                newUVs.Add(uvVectors[0]);
                newUVs.Add(uvVectors[3]);
                newUVs.Add(uvVectors[2]);

            }
            else {

                var hasTopLeft = tile.verticies.Exists(v => v.vertexPosition == VertexPosition.TopLeft);
                var hasTopRight = tile.verticies.Exists(v => v.vertexPosition == VertexPosition.TopRight);
                var hasBottomLeft = tile.verticies.Exists(v => v.vertexPosition == VertexPosition.BottomLeft);
                var hasBottomRight = tile.verticies.Exists(v => v.vertexPosition == VertexPosition.BottomRight);

                // Top right triangle or top left triangle
                if ((hasTopRight && !hasBottomLeft) || (hasTopLeft && !hasBottomRight)) {

                    newUVs.Add(uvVectors[1]);
                    newUVs.Add(uvVectors[0]);
                    newUVs.Add(uvVectors[2]);

                } else {

                    newUVs.Add(uvVectors[0]);
                    newUVs.Add(uvVectors[2]);
                    newUVs.Add(uvVectors[1]);

                }



            }

            foreach (var i in Enumerable.Range(0, tile.uvs.Count)) {
                tile.uvs[i] = TextureCoordinate.SetPixel(newUVs[i][0], newUVs[i][1]);
            }

        }

        void FlipUVPositionHorizontally(Tile tile) {

            var uvVectors = new List<int[]>();

            foreach (var uv in tile.uvs) {
                uvVectors.Add(TextureCoordinate.GetVector(uv));
            }

            float minX = uvVectors.Min(v => v[0]);
            float maxX = uvVectors.Max(v => v[0]);

            float width = maxX - minX;
            var center = width / 2f;

            foreach (var uv in uvVectors) {

                var localX = uv[0] - minX;

                var distanceFromCenter = localX - center;

                var vFlippedX = center - distanceFromCenter;

                uv[0] = (int)(minX + vFlippedX);

            }

            foreach (var i in Enumerable.Range(0, tile.uvs.Count)) {
                tile.uvs[i] = TextureCoordinate.SetPixel(uvVectors[i][0], uvVectors[i][1]);
            }

        }

        public void Overwrite(FCopLevelSection section) {

            heightMap.Clear();
            foreach (var newHeight in section.heightMap) {
                heightMap.Add(new HeightPoints(newHeight.height1, newHeight.height2, newHeight.height3));
            }

            tileColumns.Clear();
            var x = 0;
            var y = 0;
            foreach (var newColumn in section.tileColumns) {

                var newTiles = new List<Tile>();

                var heights = new List<HeightPoints>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                var column = new TileColumn(x, y, newTiles, heights);

                foreach (var newTile in newColumn.tiles) {
                    newTiles.Add(new Tile(newTile, column, section));
                }

                tileColumns.Add(column);

                x++;
                if (x == 16) {
                    y++;
                    x = 0;
                }

            }

            colors.Clear();

            animationVector = new AnimationVector(section.animationVector.x, section.animationVector.y);

            foreach (var newColor in section.colors) {

                colors.Add(new XRGB555(newColor.x, newColor.r, newColor.g, newColor.b));

            }

        }

    }

    public class HeightPoints {

        public const float multiplyer = 30f;
        public const float maxValue = SByte.MaxValue / multiplyer;
        public const float minValue = SByte.MinValue / multiplyer;

        public float height1;
        public float height2;
        public float height3;

        public HeightPoints(float height1, float height2, float height3) {
            this.height1 = height1;
            this.height2 = height2;
            this.height3 = height3;
        }

        public HeightPoints(HeightPoint3 parsedHeightPoint3) {
            this.height1 = parsedHeightPoint3.height1 / multiplyer;
            this.height2 = parsedHeightPoint3.height2 / multiplyer;
            this.height3 = parsedHeightPoint3.height3 / multiplyer;
        }

        public float GetPoint(int channel) {

            switch(channel) {
                case 1: return height1;
                case 2: return height2;
                case 3: return height3;
                default: return 0;
            }

        }

        public int GetTruePoint(int index) {

            switch (index) {
                case 1: return (int)Math.Round(height1 * multiplyer);
                case 2: return (int)Math.Round(height2 * multiplyer);
                case 3: return (int)Math.Round(height3 * multiplyer);
                default: return 0;
            }

        }

        public void AddToPoint(float amount, int channel) {

            switch (channel) {
                case 1:
                    height1 += amount;

                    if (height1 > maxValue) {
                        height1 = maxValue;
                    } else if (height1 < minValue) {
                        height1 = minValue;
                    }

                    height1 = (float)Math.Round(height1 * multiplyer) / multiplyer;

                    break;
                case 2:
                    height2 += amount;

                    if (height2 > maxValue) {
                        height2 = maxValue;
                    } else if (height2 < minValue) {
                        height2 = minValue;
                    }

                    height2 = (float)Math.Round(height2 * multiplyer) / multiplyer;

                    break;
                case 3:
                    height3 += amount;

                    if (height3 > maxValue) {
                        height3 = maxValue;
                    } else if (height3 < minValue) {
                        height3 = minValue;
                    }

                    height3 = (float)Math.Round(height3 * multiplyer) / multiplyer;

                    break;
                default: break; 
            }

        }

        public void SetPoint(int value, int channel) {

            switch (channel) {
                case 1:

                    height1 = value / multiplyer;

                    if (height1 > maxValue) {
                        height1 = maxValue;
                    } else if (height1 < minValue) {
                        height1 = minValue;
                    }

                    break;
                case 2:

                    height2 = value / multiplyer;

                    if (height2 > maxValue) {
                        height2 = maxValue;
                    } else if (height2 < minValue) {
                        height2 = minValue;
                    }

                    break;
                case 3:

                    height3 = value / multiplyer;

                    if (height3 > maxValue) {
                        height3 = maxValue;
                    } else if (height3 < minValue) {
                        height3 = minValue;
                    }

                    break;
                default: break;
            }


        }

        public HeightPoint3 Compile() {
            return new HeightPoint3(
                (sbyte)Math.Round(height1 * multiplyer),
                (sbyte)Math.Round(height2 * multiplyer),
                (sbyte)Math.Round(height3 * multiplyer));

        }

    }

    // Columns form form left to right
    public class TileColumn {

        public int x;
        public int y;

        public List<Tile> tiles;

        public List<HeightPoints> heights;

        public TileColumn(int x, int y, List<Tile> tiles, List<HeightPoints> heights) {
            this.x = x;
            this.y = y;
            this.tiles = tiles;
            this.heights = heights;
        }

    }

    // Tiles are sorted into 4x4 chunks
    public class Tile {

        public TileColumn column;

        public bool isEndInColumnArray;
        public List<TileVertex> verticies;

        public List<int> uvs = new();
        public TileShaders shaders;
        public List<int> animatedUVs = new();
        public int animationSpeed = -1;

        public int texturePalette;
        public bool isVectorAnimated;
        public bool isSemiTransparent;
        public int culling;
        public int effectIndex;

        // Original parsed data from file
        TileGraphics graphics;
        List<TileGraphicsMetaData> graphicsMetaData = new();
        TileUVAnimationMetaData? uvAnimationData = null;
        TileBitfield parsedTile;

        public Tile(TileBitfield parsedTile, TileColumn column, FCopLevelSectionParser section) {


            this.column = column;

            isEndInColumnArray = parsedTile.isEndInColumnArray == 1;

            verticies = MeshType.VerticiesFromID(parsedTile.meshID);

            culling = parsedTile.culling;
            effectIndex = parsedTile.number4;

            var textureIndex = parsedTile.textureIndex;
            var graphicsIndex = parsedTile.graphicIndex;

            graphics = (TileGraphics)section.tileGraphics[graphicsIndex];

            if (graphics.graphicsType == 1) {
                graphicsMetaData.Add((TileGraphicsMetaData)section.tileGraphics[graphicsIndex + 1]);
            }
            else if (graphics.graphicsType == 2) {

                if (graphics.isRect == 1) {
                    graphicsMetaData.Add((TileGraphicsMetaData)section.tileGraphics[graphicsIndex + 1]);
                    graphicsMetaData.Add((TileGraphicsMetaData)section.tileGraphics[graphicsIndex + 2]);
                }
                else {
                    graphicsMetaData.Add((TileGraphicsMetaData)section.tileGraphics[graphicsIndex + 1]);
                }

            }

            var shaderData = new List<byte>();

            foreach (var meta in graphicsMetaData) {
                shaderData.AddRange(meta.data);
            }

            shaderData.Add((byte)graphics.lightingInfo);

            switch ((VertexColorType)graphics.graphicsType) {
                case VertexColorType.MonoChrome:
                    shaders = new MonoChromeShader(shaderData[0], graphics.isRect == 1);
                    break;
                case VertexColorType.DynamicMonoChrome:
                    shaders = new DynamicMonoChromeShader(shaderData, graphics.isRect == 1);
                    break;
                case VertexColorType.Color:
                    shaders = new ColorShader(shaderData, section, graphics.isRect == 1);
                    break;
                case VertexColorType.ColorAnimated:
                    shaders = new AnimatedShader(graphics.isRect == 1);
                    break;
            }

            
            foreach (var i in Enumerable.Range(textureIndex, verticies.Count)) {

                uvs.Add(section.textureCoordinates[i]);

            }

            texturePalette = graphics.cbmpID;
            isVectorAnimated = graphics.isAnimated == 1;
            isSemiTransparent = graphics.isSemiTransparent == 1;

            #region ParseAnimationData

            if (section.tileUVAnimationMetaData.Count == 0) {
                return;
            }

            foreach (var metaData in section.tileUVAnimationMetaData) {

                if (metaData.textureReplaceOffset / 2 == textureIndex) {
                    uvAnimationData = metaData;

                    var frameUVs = section.animatedTextureCoordinates.GetRange(metaData.animationOffset / 2, metaData.frames * 4);

                    animatedUVs.AddRange(frameUVs);

                    break;
                }

            }

            if (uvAnimationData != null) {
                animationSpeed = uvAnimationData.Value.frameDuration;
            }

            #endregion

            this.parsedTile = parsedTile;

        }

        public Tile(Tile tile, TileColumn column, FCopLevelSection section) {

            this.column = column;

            isEndInColumnArray = tile.isEndInColumnArray;

            verticies = new List<TileVertex>(tile.verticies);
            uvs = new List<int>(tile.uvs);
            shaders = tile.shaders.Clone();
            animatedUVs = new List<int>(tile.animatedUVs);
            animationSpeed = tile.animationSpeed;

            culling = tile.culling;
            texturePalette = tile.texturePalette;
            isVectorAnimated = tile.isVectorAnimated;
            isSemiTransparent = tile.isSemiTransparent;
            effectIndex = tile.effectIndex;

            graphics = tile.graphics;
            graphicsMetaData = new(tile.graphicsMetaData);
            uvAnimationData = tile.uvAnimationData;
            this.parsedTile = tile.parsedTile;

        }

        // This is only used for flattening a level.
        public Tile(TileColumn column, List<TileVertex> vertices, int culling, List<int> uvs, TileGraphics graphics) {
            this.column = column;
            isEndInColumnArray = true;
            this.verticies = vertices;
            this.culling = culling;
            this.uvs = uvs;
            this.texturePalette = graphics.cbmpID;

            isVectorAnimated = false;
            isSemiTransparent = false;
            effectIndex = 0;

            shaders = new MonoChromeShader(116, vertices.Count == 4);

        }

        public Tile(TileColumn column, int meshID, int culling) {

            this.column = column;
            verticies = MeshType.VerticiesFromID(meshID);
            this.culling = culling;
            this.uvs = new() { 57200, 57228, 50060, 50032 };
            this.texturePalette = 6;

            isVectorAnimated = false;
            isSemiTransparent = false;
            effectIndex = 0;

            shaders = new MonoChromeShader(116, verticies.Count == 4);

        }

        public int GetFrameCount() {
            return animatedUVs.Count / 4;
        }

        public bool IsFrameAnimated() {
            return animatedUVs.Count > 0;
        }

        public void ChangeShader(VertexColorType type) {

            switch (type) {

                case VertexColorType.MonoChrome:
                    shaders = new MonoChromeShader(verticies.Count == 4);
                    break;
                case VertexColorType.DynamicMonoChrome: 
                    shaders = new DynamicMonoChromeShader(shaders);
                    break;
                case VertexColorType.Color: 
                    shaders = new ColorShader(shaders);
                    break;
                case VertexColorType.ColorAnimated:
                    shaders = new AnimatedShader(verticies.Count == 4);
                    break;

            }

        }

        public TileBitfield Compile(int textureIndex, int graphicsIndex) {

            var id = MeshType.IDFromVerticies(verticies);

            if (id == null) {
                throw new MeshIDException();
            }

            parsedTile.isEndInColumnArray = isEndInColumnArray ? 1 : 0;
            parsedTile.meshID = (int)id;
            parsedTile.textureIndex = textureIndex;
            parsedTile.culling = culling;
            parsedTile.number4 = effectIndex;
            parsedTile.graphicIndex = graphicsIndex;

            return parsedTile;

        }

        public List<TileGraphicsItem> CompileGraphics(Dictionary<ushort, (int, XRGB555)> existingColors) {

            var isRect = verticies.Count == 4;

            var graphic = new TileGraphics(graphics.lightingInfo, 
                texturePalette,
                isVectorAnimated ? 1 : 0,
                isSemiTransparent ? 1 : 0, 
                isRect ? 1 : 0, 
                (int)shaders.type);

            var shaderData = new List<byte>();

            if (shaders.type == VertexColorType.Color) {
                shaderData = ((ColorShader)shaders).ColorCompile(existingColors);
            } else {
                shaderData = shaders.Compile();
            }


            var graphicItems = new List<TileGraphicsItem>();

            // If no shader data just uses the existing data
            if (shaderData.Count == 0) {

                graphicItems.Add(graphic);
                foreach (var metaData in graphicsMetaData) {
                    graphicItems.Add(metaData);
                }

            } else {

                graphic.lightingInfo = shaderData.Last();

                graphicItems.Add(graphic);

                if (shaderData.Count > 1) {

                    foreach (var i in Enumerable.Range(0,shaderData.Count / 2)) {
                        graphicItems.Add(new TileGraphicsMetaData(shaderData.GetRange((i * 2), 2)));
                    }

                }

            }



            return graphicItems;

        }

        public TileUVAnimationMetaData CompileFrameAnimation(int animationOffset, int textureReplaceOffset) {
            var metaData = new TileUVAnimationMetaData(GetFrameCount(), 9, animationSpeed, animationOffset, textureReplaceOffset);
            uvAnimationData = metaData;
            return metaData;
        }

    }

    public interface TileShaders {

        public float[][] colors { get; set; }

        public bool isQuad { get; set; }

        public VertexColorType type { get; set; }

        public void Apply();

        public List<byte> Compile();

        public TileShaders Clone();

    }

    public class MonoChromeShader : TileShaders {
        public float[][] colors { get; set; }

        public bool isQuad { get; set; }

        public VertexColorType type { get; set; }

        public byte value;

        public const float white = 116f;

        public MonoChromeShader(byte lightingInfo, bool isQuad) {
            this.value = lightingInfo;
            this.isQuad = isQuad;
            type = VertexColorType.MonoChrome;
            Apply();

        }

        public MonoChromeShader(bool isQuad) {

            this.value = (byte)white;
            this.isQuad = isQuad;
            type = VertexColorType.MonoChrome;
            Apply();

        }

        public MonoChromeShader() {
            type = VertexColorType.MonoChrome;
        }

        public void Apply() {

            var color = value / white;

            var fColors = new float[] { color, color, color };

            if (isQuad) {

                colors = new float[][] {
                    fColors, fColors, fColors, fColors
                };

            } else {

                colors = new float[][] {
                    fColors, fColors, fColors
                };

            }

        }

        public List<byte> Compile() {
            return new List<byte> { value };
        }

        public TileShaders Clone() {

            return new MonoChromeShader(value, isQuad);

        }

    }

    public class DynamicMonoChromeShader : TileShaders {
        public float[][] colors { get; set; }
        public bool isQuad { get; set; }

        public VertexColorType type { get; set; }

        public int[] values;

        public const float white = 29f;

        public DynamicMonoChromeShader(List<byte> data, bool isQuad) {

            this.isQuad = isQuad;
            type = VertexColorType.DynamicMonoChrome;
            var bitField = new BitArray(data.ToArray());

            values = new int[] {

                Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 18, 24)),
                Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 12, 18)),
                Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 6, 12)),
                Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 0, 6))

            };


            Apply();

        }

        public DynamicMonoChromeShader(bool isQuad) {

            this.isQuad = isQuad;
            type = VertexColorType.DynamicMonoChrome;

            values = new int[] {
                (int)white,
                (int)white,
                (int)white,
                (int)white
            };

            Apply();

        }

        public DynamicMonoChromeShader(TileShaders previousShader) {

            if (previousShader.type == VertexColorType.MonoChrome) {

                var monoChrome = (MonoChromeShader)previousShader;

                var whitePercentage = monoChrome.value / MonoChromeShader.white;

                var valueConversion = (int)MathF.Round(white * whitePercentage);

                values = new int[] {
                    valueConversion,
                    valueConversion,
                    valueConversion,
                    valueConversion
                };

            }
            else {

                values = new int[] {
                    (int)white,
                    (int)white,
                    (int)white,
                    (int)white
                };

            }

            this.isQuad = previousShader.isQuad;
            type = VertexColorType.DynamicMonoChrome;

            Apply();

        }

        public DynamicMonoChromeShader() {
            type = VertexColorType.DynamicMonoChrome;
            values = new int[0];
        }

        public void Apply() {

            var total = new List<float[]>();

            if (isQuad) {

                total.Add(new float[] { values[0] / white, values[0] / white, values[0] / white, values[0] / white });
                total.Add(new float[] { values[1] / white, values[1] / white, values[1] / white, values[1] / white });
                total.Add(new float[] { values[3] / white, values[3] / white, values[3] / white, values[3] / white });
                total.Add(new float[] { values[2] / white, values[2] / white, values[2] / white, values[2] / white });


            }
            else {

                total.Add(new float[] { values[0] / white, values[0] / white, values[0] / white, values[0] / white });
                total.Add(new float[] { values[2] / white, values[2] / white, values[2] / white, values[2] / white });
                total.Add(new float[] { values[1] / white, values[1] / white, values[1] / white, values[1] / white });

            }

            colors = total.ToArray();

        }

        public List<byte> Compile() {

            var bitfield = new BitField(24, new() {
                        new BitNumber(6, values[3]),
                        new BitNumber(6, values[2]),
                        new BitNumber(6, values[1]),
                        new BitNumber(6, values[0])
                    });

            return Utils.BitArrayToByteArray(bitfield.Compile()).ToList();

        }

        public TileShaders Clone() {

            return new DynamicMonoChromeShader(this.Compile(), isQuad);

        }

    }

    public class ColorShader : TileShaders {

        public float[][] colors { get; set; }
        public bool isQuad { get; set; }

        public VertexColorType type { get; set; }

        public XRGB555[] values;

        public ColorShader(List<byte> data, FCopLevelSectionParser section, bool isQuad) {

            this.isQuad = isQuad;
            type = VertexColorType.Color;
            var colors = new List<XRGB555>();

            var i = 0;
            foreach (var colorIndex in data) {

                if (isQuad && i == 2) {
                    i++;
                    continue;
                }

                colors.Add(section.colors[colorIndex].Clone());

                i++;
            }

            values = colors.ToArray();

            Apply();

        }

        public ColorShader(XRGB555[] values, bool isQuad) {

            this.isQuad = isQuad;
            type = VertexColorType.Color;

            this.values = values;

            Apply();

        }

        public ColorShader(bool isQuad) {

            this.isQuad = isQuad;
            type = VertexColorType.Color;

            values = new XRGB555[] { 
                new XRGB555(false, 31, 31, 31), 
                new XRGB555(false, 31, 31, 31), 
                new XRGB555(false, 31, 31, 31), 
                new XRGB555(false, 31, 31, 31) 
            };

            Apply();

        }

        public ColorShader(TileShaders previousShader) {

            if (previousShader.type == VertexColorType.MonoChrome) {

                var monoChrome = (MonoChromeShader)previousShader;

                var whitePercentage = monoChrome.value / MonoChromeShader.white;

                var valueConversion = (int)MathF.Round(XRGB555.maxChannelValue * whitePercentage);

                if (valueConversion > XRGB555.maxChannelValue) {
                    valueConversion = (int)XRGB555.maxChannelValue;
                }

                values = new XRGB555[] {
                    new XRGB555(false, valueConversion, valueConversion, valueConversion),
                    new XRGB555(false, valueConversion, valueConversion, valueConversion),
                    new XRGB555(false, valueConversion, valueConversion, valueConversion),
                    new XRGB555(false, valueConversion, valueConversion, valueConversion)
                };

            }
            else if (previousShader.type == VertexColorType.DynamicMonoChrome) {

                // Filler Data
                values = new XRGB555[] {
                    new XRGB555(false, 31, 31, 31),
                    new XRGB555(false, 31, 31, 31),
                    new XRGB555(false, 31, 31, 31),
                    new XRGB555(false, 31, 31, 31)
                };

                var dyanmicMonoChrome = (DynamicMonoChromeShader)previousShader;

                var quadMonoPosToColor = new int[] { 3, 1, 0, 2 };
                var triangleMonoPosToColor = new int[] { 1, 0, 2 };

                var i = 0;
                foreach (var value in dyanmicMonoChrome.values) {

                    var whitePercentage = value / DynamicMonoChromeShader.white;

                    var valueConversion = (int)MathF.Round(XRGB555.maxChannelValue * whitePercentage);

                    if (valueConversion > XRGB555.maxChannelValue) {
                        valueConversion = (int)XRGB555.maxChannelValue;
                    }

                    if (previousShader.isQuad) {
                        values[quadMonoPosToColor[i]] = new XRGB555(false, valueConversion, valueConversion, valueConversion);
                    } else {

                        if (i != 3) {

                            values[triangleMonoPosToColor[i]] = new XRGB555(false, valueConversion, valueConversion, valueConversion);

                        }

                    }

                    i++;

                }

            }
            else {

                values = new XRGB555[] {
                    new XRGB555(false, 31, 31, 31),
                    new XRGB555(false, 31, 31, 31),
                    new XRGB555(false, 31, 31, 31),
                    new XRGB555(false, 31, 31, 31)
                };

            }

            this.isQuad = previousShader.isQuad;
            type = VertexColorType.Color;

            Apply();

        }

        public ColorShader() {

            type = VertexColorType.Color;
            values = new XRGB555[0];

        }

        public void Apply() {

            var total = new List<float[]>();

            if (isQuad) {

                total.Add(values[3].ToColors());
                total.Add(values[1].ToColors());
                total.Add(values[2].ToColors());
                total.Add(values[0].ToColors());


            } else {

                total.Add(values[2].ToColors());
                total.Add(values[0].ToColors());
                total.Add(values[1].ToColors());

            }

            colors = total.ToArray();

        }

        public List<byte> Compile() {
            return new();
        }

        public List<byte> ColorCompile(Dictionary<ushort, (int, XRGB555)> existingColors) {
            var corner1 = (byte)existingColors[values[0].ToUShort()].Item1;
            var corner2 = (byte)existingColors[values[1].ToUShort()].Item1;
            var corner3 = (byte)existingColors[values[2].ToUShort()].Item1;

            if (isQuad) {
                var corner4 = (byte)existingColors[values[3].ToUShort()].Item1;

                return new() { corner1, corner2, 0, corner3, corner4 };

            } else {
                return new() { corner1, corner2, corner3 };
            }


        }

        public TileShaders Clone() {

            var colors = new List<XRGB555>();

            foreach (var color in values) {
                colors.Add(color.Clone());
            }

            return new ColorShader(colors.ToArray(), isQuad);

        }

    }

    // Same with this
    public class AnimatedShader : TileShaders {

        public float[][] colors { get; set; }
        public bool isQuad { get; set; }

        public VertexColorType type { get; set; }

        public AnimatedShader(bool isQuad) {
            this.isQuad = isQuad;
            type = VertexColorType.ColorAnimated;
            Apply();
        }

        public AnimatedShader() {
            type = VertexColorType.ColorAnimated;
        }

        public void Apply() {

            var dummyColors = new float[] { 1f, 1f, 1f };

            if (isQuad) {

                colors = new float[][] {
                    dummyColors, dummyColors, dummyColors, dummyColors
                };

            }
            else {

                colors = new float[][] {
                    dummyColors, dummyColors, dummyColors
                };

            }

        }

        public List<byte> Compile() {
            return new List<byte> { 0 };
        }

        public TileShaders Clone() {
            return new AnimatedShader(this.isQuad);
        }

    }

    public class AnimationVector {

        public const int maxDistance = 27;
        public const float frameTime = 5.95f / 27f;

        public int x;
        public int y;

        public AnimationVector(List<byte> bytes) {
            x = (sbyte)bytes[0];
            y = (sbyte)bytes[1];
        }

        public AnimationVector(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public List<byte> Compile() {
            return new List<byte>() { (byte)x, (byte)y };
        }
    }

    public struct TileVertex {

        public int heightChannel;

        public VertexPosition vertexPosition;

        public TileVertex(int heightChannel, VertexPosition vertexPosition) {
            this.heightChannel = heightChannel;
            this.vertexPosition = vertexPosition;
        }

    }

    public enum VertexColorType {
        MonoChrome = 0,
        DynamicMonoChrome = 1,
        Color = 2,
        ColorAnimated = 3
    }

    public enum VertexPosition {
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 3,
        BottomRight = 4
    }

}