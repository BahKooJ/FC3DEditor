
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    // =WIP=
    public class FCopLevel {

        public List<List<int>> layout;

        public List<FCopLevelSection> sections = new();

        public List<FCopTexture> textures = new();

        public List<FCopNavMesh> navMeshes = new();

        public List<FCopObject> objects = new();

        public List<FCopActor> actors = new();

        public IFFFileManager fileManager;

        public FCopLevel(IFFFileManager fileManager) {

            this.fileManager = fileManager;

            var rawCtilFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Ctil";

            }).ToList();

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

            foreach (var rawFile in rawCtilFiles) {
                sections.Add(new FCopLevelSectionParser(rawFile).Parse(this));
            }

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

            layout = FCopLevelLayoutParser.Parse(fileManager.files.First(file => {

                return file.dataFourCC == "Cptc";

            }));

        }

        public FCopLevel(int width, int height, IFFFileManager fileManager) {

            this.fileManager = fileManager;

            layout = new List<List<int>>();

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

            var rawBitmapFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Cbmp";

            }).ToList();

            var rawNavMeshFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Cnet";

            }).ToList();

            sections.Add(new FCopLevelSectionParser(rawCtilFiles[0]).Parse(this));

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

                    sections.Add(newSection);

                }


            }


            foreach (var rawFile in rawBitmapFiles) {
                textures.Add(new FCopTexture(rawFile));
            }

            foreach (var rawFile in rawNavMeshFiles) {
                navMeshes.Add(new FCopNavMesh(rawFile));
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

        public List<HeightPoint> heightMap = new List<HeightPoint>();

        public List<TileColumn> tileColumns = new List<TileColumn>();

        public List<int> textureCoordinates = new List<int>();

        List<XRGB555> colors = new List<XRGB555>();

        public List<TileGraphics> tileGraphics = new List<TileGraphics>();

        // Until the file can be fully parsed, we need to have the parser on hand
        public FCopLevelSectionParser parser;

        public FCopLevelSection(FCopLevelSectionParser parser, FCopLevel parent) {

            this.parser = parser;

            this.textureCoordinates = parser.textureCoordinates;
            this.colors = parser.colors;
            this.tileGraphics = parser.tileGraphics;

            foreach (var parsePoint in parser.heightPoints) {
                heightMap.Add(new HeightPoint(parsePoint));
            }

            var count = 0;
            var x = 0;
            var y = 0;
            foreach (var parseColumn in parser.thirdSectionBitfields) {

                // Grabs the tiles for the column in the tiles array. Number 2 is the index of the tiles and number 1 is the count.
                var parsedTiles = parser.tiles.GetRange(parseColumn.number2, parseColumn.number1);

                // Makes the parsed bitfield into a Tile object.
                var tiles = new List<Tile>();

                foreach (var parsedTile in parsedTiles) {
                    tiles.Add(new Tile(parsedTile));
                }

                // Grabs the heights. The heights have already been added so it uses the local height array.
                var heights = new List<HeightPoint>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                tileColumns.Add(new TileColumn(x, y, tiles, heights));

                x++;
                if (x == 16) {
                    y++;
                    x = 0;
                }

                count++;

            }

            this.parent = parent;

        }

        public HeightPoint GetHeightPoint(int x, int y) {
            return heightMap[(y * heightMapWdith) + x];
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

        // Compiles the class back into a Ctil Future Cop can read. The changes are applied to the parser object.
        public void Compile() {

            List<HeightPoint3> heightPoints = new List<HeightPoint3>();
            List<ThirdSectionBitfield> thirdSectionBitfields = new List<ThirdSectionBitfield>();
            List<TileBitfield> tiles = new List<TileBitfield>();

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

                    // Makes sure the last tile value is correct
                    foreach (var tile in column.tiles) {
                        tile.isStartInColumnArray = false;
                    }

                    column.tiles.Last().isStartInColumnArray = true;

                    // Now that the tile columns are sorted to fit the 4x4 chunk pattern in the tile array, we can simple add the tiles.
                    foreach (var tile in column.tiles) {
                        tiles.Add(tile.Compile());
                    }



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
            parser.thirdSectionBitfields = thirdSectionBitfields;
            parser.tiles = tiles;
            parser.textureCoordinates = textureCoordinates;
            parser.tileGraphics = tileGraphics;

            parser.Compile();

        }

        public void RotateCounterClockwise() {
            // X becomes Y
            // Y becomes X - length

            var newHeightOrder = new List<HeightPoint>();

            foreach (var hy in Enumerable.Range(0,17)) {

                foreach (var hx in Enumerable.Range(0, 17)) {
                    newHeightOrder.Add(GetHeightPoint(16 - hy, hx));
                }

            }

            heightMap = newHeightOrder;

            var newTileColum = new List<TileColumn>();

            foreach (var ty in Enumerable.Range(0, 16)) {

                foreach (var tx in Enumerable.Range(0, 16)) {
                    var column = tileColumns[(tx * 16) + (15 - ty)];

                    var heights = new List<HeightPoint>();

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

            foreach (var column in tileColumns) {

                var validTiles = new List<Tile>();

                foreach (var tile in column.tiles) {

                    var counterClockVertices = new List<TileVertex>();

                    foreach (var vertex in tile.verticies) {

                        switch(vertex.vertexPosition) {

                            case VertexPosition.TopLeft:
                                counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                                break;
                            case VertexPosition.TopRight:
                                counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                                break;
                            case VertexPosition.BottomLeft:
                                counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomRight));
                                break;
                            case VertexPosition.BottomRight:
                                counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                                break;

                        }

                    }

                    var counterClockID = MeshType.IDFromVerticies(counterClockVertices);

                    if (counterClockID != null) {
                        tile.verticies = MeshType.VerticiesFromID((int)counterClockID);
                        validTiles.Add(tile);
                    } 

                }

                column.tiles = validTiles;
            }

        }

        public void MirorVertically() {

            var newHeightOrder = new List<HeightPoint>();

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

                    var heights = new List<HeightPoint>();

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

            foreach (var column in tileColumns) {

                var validTiles = new List<Tile>();

                foreach (var tile in column.tiles) {

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
                        validTiles.Add(tile);
                    }

                }

                column.tiles = validTiles;
            }


        }

        public void Overwrite(FCopLevelSection section) {

            heightMap.Clear();
            foreach (var newHeight in section.heightMap) {
                heightMap.Add(new HeightPoint(newHeight.height1, newHeight.height2, newHeight.height3));
            }

            tileColumns.Clear();
            var x = 0;
            var y = 0;
            foreach (var newColumn in section.tileColumns) {

                var newTiles = new List<Tile>();

                foreach (var newTile in newColumn.tiles) {
                    newTiles.Add(new Tile(newTile.Compile()));
                }

                var heights = new List<HeightPoint>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                tileColumns.Add(new TileColumn(x, y, newTiles, heights));

                x++;
                if (x == 16) {
                    y++;
                    x = 0;
                }

            }

            textureCoordinates = new List<int>(section.textureCoordinates);

            colors.Clear();

            foreach (var newColor in section.colors) {

                colors.Add(new XRGB555(newColor.x, newColor.r, newColor.g, newColor.b));

            }

            tileGraphics = new List<TileGraphics>(section.tileGraphics);

        }

    }

    public class HeightPoint {

        public const float multiplyer = 30f;
        public const float maxValue = SByte.MaxValue / multiplyer;
        public const float minValue = SByte.MinValue / multiplyer;

        public float height1;
        public float height2;
        public float height3;

        public HeightPoint(float height1, float height2, float height3) {
            this.height1 = height1;
            this.height2 = height2;
            this.height3 = height3;
        }

        public HeightPoint(HeightPoint3 parsedHeightPoint3) {
            this.height1 = parsedHeightPoint3.height1 / multiplyer;
            this.height2 = parsedHeightPoint3.height2 / multiplyer;
            this.height3 = parsedHeightPoint3.height3 / multiplyer;
        }

        public float GetPoint(int index) {

            switch(index) {
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

        public List<HeightPoint> heights;

        public TileColumn(int x, int y, List<Tile> tiles, List<HeightPoint> heights) {
            this.x = x;
            this.y = y;
            this.tiles = tiles;
            this.heights = heights;
        }

    }

    // Tiles are sorted into 4x4 chunks
    public class Tile {

        public bool isStartInColumnArray;
        public List<TileVertex> verticies;
        public int textureIndex;
        public int graphicsIndex;
        public int unknownButVeryImportantNumber;

        public TileBitfield parsedTile;

        public Tile(TileBitfield parsedTile) {

            isStartInColumnArray = parsedTile.number1 == 1;

            verticies = MeshType.VerticiesFromID(parsedTile.number5);
            textureIndex = parsedTile.number2;

            unknownButVeryImportantNumber = parsedTile.number3;

            graphicsIndex = parsedTile.number6;

            this.parsedTile = parsedTile;
        }

        public TileBitfield Compile() {

            //This is still broken

            var id = MeshType.IDFromVerticies(verticies);

            if (id == null) {
                throw new MeshIDException();
            }

            parsedTile.number1 = isStartInColumnArray ? 1 : 0;
            parsedTile.number5 = (int)id;
            parsedTile.number2 = textureIndex;
            parsedTile.number3 = unknownButVeryImportantNumber;
            parsedTile.number6 = graphicsIndex;

            return parsedTile;

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

    public enum VertexPosition {
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 3,
        BottomRight = 4
    }

}