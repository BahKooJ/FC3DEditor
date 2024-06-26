

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class Schematic {

    public string name;

    public int width;
    public int height;

    public List<HeightPoints> heightMap = new List<HeightPoints>();
    public List<TileColumn> tileColumns = new List<TileColumn>();

    public Schematic transformedSchematic = null;

    public HeightPoints GetHeightPoint(int x, int y) {
        return heightMap[(y * (width + 1)) + x];
    }

    public HeightPoints GetHeightPoint(int x, int y, int width) {
        return heightMap[(y * (width + 1)) + x];
    }

    public TileColumn GetTileColumn(int x, int y) {
        return tileColumns[(y * width) + x];
    }

    public int LowestHeight() {

        var lowestValue = 128;
        
        foreach (var heights in heightMap) {

            foreach (var i in Enumerable.Range(1,3)) {

                if (heights.GetTruePoint(i) < lowestValue && heights.GetTruePoint(i) != -128) {
                    lowestValue = heights.GetTruePoint(i);
                }

            }

            
        }

        return lowestValue;

    }

    public Schematic(List<TileSelection> selectedItems, string name) {

        var sortedSelectedItems = new List<TileSelection>(selectedItems);

        sortedSelectedItems = sortedSelectedItems.OrderBy(item => item.columnWorldX).ThenBy(item => item.columnWorldY).ToList();

        width = (sortedSelectedItems.Max(item => item.columnWorldX) - sortedSelectedItems.Min(item => item.columnWorldX)) + 1;
        height = (sortedSelectedItems.Max(item => item.columnWorldY) - sortedSelectedItems.Min(item => item.columnWorldY)) + 1;

        foreach (var y in Enumerable.Range(0, height + 1)) {

            foreach (var x in Enumerable.Range(0, width + 1)) {

                heightMap.Add(new HeightPoints(-128, -128, -128));

            }

        }

        foreach (var y in Enumerable.Range(0, height)) {

            foreach (var x in Enumerable.Range(0, width)) {

                var heights = new List<HeightPoints>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                tileColumns.Add(new TileColumn(x, y, new(), heights));

            }

        }

        var startingX = sortedSelectedItems.Min(item => item.columnWorldX);
        var startingY = sortedSelectedItems.Min(item => item.columnWorldY);

        foreach (var item in sortedSelectedItems) {

            var emptyColumn = GetTileColumn(item.columnWorldX - startingX, item.columnWorldY - startingY);

            foreach (var vert in item.tile.verticies) {

                emptyColumn.heights[(int)vert.vertexPosition - 1]
                    .SetPoint(item.column.heights[(int)vert.vertexPosition - 1].GetTruePoint(vert.heightChannel), vert.heightChannel);

            }

            emptyColumn.tiles.Add(new Tile(item.tile, emptyColumn, null));

        }

        this.name = name;
    }

    public Schematic(string name, int width, int height, List<HeightPoints> heightMap, List<TileColumn> tileColumns) {
        this.name = name;
        this.width = width;
        this.height = height;
        this.heightMap = heightMap;
        this.tileColumns = tileColumns;
    }

    public Schematic Clone() {

        List<HeightPoints> heightMap = new List<HeightPoints>();
        List<TileColumn> tileColumns = new List<TileColumn>();

        foreach (var newHeight in this.heightMap) {
            heightMap.Add(new HeightPoints(newHeight.height1, newHeight.height2, newHeight.height3));
        }

        var x = 0;
        var y = 0;
        foreach (var newColumn in this.tileColumns) {

            var newTiles = new List<Tile>();

            var heights = new List<HeightPoints>();

            heights.Add(GetHeightPoint(x, y));
            heights.Add(GetHeightPoint(x + 1, y));
            heights.Add(GetHeightPoint(x, y + 1));
            heights.Add(GetHeightPoint(x + 1, y + 1));

            var column = new TileColumn(x, y, newTiles, heights);

            foreach (var newTile in newColumn.tiles) {
                newTiles.Add(new Tile(newTile, column, null));
            }

            tileColumns.Add(column);

            x++;
            if (x == width) {
                y++;
                x = 0;
            }

        }

        return new Schematic(name + " (Clone)", width, height, heightMap, tileColumns);

    }

    public void MirrorDiagonally() {

        MirrorVertically();
        MirrorHorizontally();

    }

    public void MirrorHorizontally() {

        var newHeightOrder = new List<HeightPoints>();

        foreach (var hy in Enumerable.Range(0, height + 1)) {

            foreach (var hx in Enumerable.Range(0, width + 1)) {
                newHeightOrder.Add(GetHeightPoint(width - hx, hy));
            }

        }

        heightMap = newHeightOrder;

        var newTileColum = new List<TileColumn>();

        foreach (var ty in Enumerable.Range(0, height)) {

            foreach (var tx in Enumerable.Range(0, width)) {
                var column = tileColumns[(ty * width) + ((width - 1) - tx)];

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

                var result = tile.MirrorVerticesHorizontally();

                if (result == Tile.TransformResult.Success) {

                    tile.MirrorUVsHorizontally();
                    tile.MirrorShadersHorizontally();

                    validTiles.Add(tile);

                }
                else if (result == Tile.TransformResult.MoveColumnPosX) {

                    if (column.x < width - 1) {

                        var nextColumn = tileColumns[(column.y * width) + (column.x + 1)];

                        tile.column = nextColumn;

                        nextColumn.tiles.Add(tile);
                        movedTiles.Add(tile);

                    }

                }

            }

            column.tiles = validTiles;
        }


    }

    public void MirrorVertically() {

        var newHeightOrder = new List<HeightPoints>();

        foreach (var hy in Enumerable.Range(0, height + 1)) {

            foreach (var hx in Enumerable.Range(0, width + 1)) {
                newHeightOrder.Add(GetHeightPoint(hx, height - hy));
            }

        }

        heightMap = newHeightOrder;

        var newTileColum = new List<TileColumn>();

        foreach (var ty in Enumerable.Range(0, height)) {

            foreach (var tx in Enumerable.Range(0, width)) {
                var column = tileColumns[(((height - 1) - ty) * width) + tx];

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

                var result = tile.MirrorVerticesVertically();

                if (result == Tile.TransformResult.Success) {

                    tile.MirrorUVsVertically();
                    tile.MirrorShadersVertically();

                    validTiles.Add(tile);

                }
                else if (result == Tile.TransformResult.MoveColumnPosY) {

                    if (column.y < (height - 1)) {

                        var nextColumn = tileColumns[((column.y + 1) * width) + (column.x)];

                        tile.column = nextColumn;

                        nextColumn.tiles.Add(tile);
                        movedTiles.Add(tile);

                    }

                }

            }

            column.tiles = validTiles;
        }


    }

    public void RotateClockwise() {

        var newHeightOrder = new List<HeightPoints>();

        var newWidth = height;
        var newHeight = width;

        foreach (var hy in Enumerable.Range(0, newHeight + 1)) {

            foreach (var hx in Enumerable.Range(0, newWidth + 1)) {

                newHeightOrder.Add(GetHeightPoint(hy, newWidth - hx));

            }

        }

        heightMap = newHeightOrder;

        var newTileColum = new List<TileColumn>();

        foreach (var ty in Enumerable.Range(0, newHeight)) {

            foreach (var tx in Enumerable.Range(0, newWidth)) {
                var column = tileColumns[(((newWidth - 1) - tx) * newHeight) + ty];

                var heights = new List<HeightPoints>();

                heights.Add(GetHeightPoint(tx, ty, newWidth));
                heights.Add(GetHeightPoint(tx + 1, ty, newWidth));
                heights.Add(GetHeightPoint(tx, ty + 1, newWidth));
                heights.Add(GetHeightPoint(tx + 1, ty + 1, newWidth));

                column.x = tx;
                column.y = ty;
                column.heights = heights;

                newTileColum.Add(column);
            }

        }

        tileColumns = newTileColum;

        width = newWidth;
        height = newHeight;

        var movedTiles = new List<Tile>();

        foreach (var column in tileColumns) {

            var validTiles = new List<Tile>();

            foreach (var tile in column.tiles) {

                if (movedTiles.Contains(tile)) {
                    validTiles.Add(tile);
                    continue;
                }

                var result = tile.RotateVerticesClockwise();

                if (result == Tile.TransformResult.Success) {

                    tile.RotateUVsClockwise();
                    tile.RotateShadersClockwise();

                    validTiles.Add(tile);

                }
                else if (result == Tile.TransformResult.MoveColumnPosX) {

                    if (column.x < width - 1) {

                        var nextColumn = tileColumns[(column.y * width) + (column.x + 1)];

                        tile.column = nextColumn;

                        nextColumn.tiles.Add(tile);
                        movedTiles.Add(tile);

                    }

                }

            }

            column.tiles = validTiles;

        }

    }

    public void RotateCounterClockwise() {

        var newHeightOrder = new List<HeightPoints>();

        var newWidth = height;
        var newHeight = width;

        foreach (var hy in Enumerable.Range(0, newHeight + 1)) {

            foreach (var hx in Enumerable.Range(0, newWidth + 1)) {

                newHeightOrder.Add(GetHeightPoint(newHeight - hy, hx));

            }

        }

        heightMap = newHeightOrder;

        var newTileColum = new List<TileColumn>();

        foreach (var ty in Enumerable.Range(0, newHeight)) {

            foreach (var tx in Enumerable.Range(0, newWidth)) {
                var column = tileColumns[(tx * newHeight) + ((newHeight - 1) - ty)];

                var heights = new List<HeightPoints>();

                heights.Add(GetHeightPoint(tx, ty, newWidth));
                heights.Add(GetHeightPoint(tx + 1, ty, newWidth));
                heights.Add(GetHeightPoint(tx, ty + 1, newWidth));
                heights.Add(GetHeightPoint(tx + 1, ty + 1, newWidth));

                column.x = tx;
                column.y = ty;
                column.heights = heights;

                newTileColum.Add(column);
            }

        }

        tileColumns = newTileColum;

        width = newWidth;
        height = newHeight;

        var movedTiles = new List<Tile>();

        foreach (var column in tileColumns) {

            var validTiles = new List<Tile>();

            foreach (var tile in column.tiles) {

                if (movedTiles.Contains(tile)) {
                    validTiles.Add(tile);
                    continue;
                }

                var result = tile.RotateVerticesCounterClockwise();

                if (result == Tile.TransformResult.Success) {

                    tile.RotateUVsCounterClockwise();
                    tile.RotateShadersCounterClockwise();

                    validTiles.Add(tile);

                }
                else if (result == Tile.TransformResult.MoveColumnPosY) {

                    if (column.y < height - 1) {

                        var nextColumn = tileColumns[((column.y + 1) * width) + (column.x)];

                        tile.column = nextColumn;

                        nextColumn.tiles.Add(tile);
                        movedTiles.Add(tile);

                    }

                }

            }

            column.tiles = validTiles;

        }

    }

    public void CleanUpUnusedColumnPadding() {

        var tilesLocatedTopRow = false;
        var tilesLocatedBottomRow = false;
        var tilesLocatedLeftColumn = false;
        var tilesLocatedRightColumn = false;

        foreach (var column in tileColumns.GetRange(0, width)) {

            if (column.tiles.Count != 0) {
                tilesLocatedTopRow = true;
                //break;
            }

        }

        foreach (var column in tileColumns.GetRange((height - 1) * width, width)) {

            if (column.tiles.Count != 0) {
                tilesLocatedBottomRow = true;
                //break;
            }

        }

        foreach (var i in Enumerable.Range(0, height)) {
            
            var column = tileColumns[i * width];

            if (column.tiles.Count != 0) {
                tilesLocatedLeftColumn = true;
                //break;
            }

        }

        foreach (var i in Enumerable.Range(0, height)) {

            var column = tileColumns[(i * width) + (width - 1)];

            if (column.tiles.Count != 0) {
                tilesLocatedLeftColumn = true;
                //break;
            }

        }

    }

}