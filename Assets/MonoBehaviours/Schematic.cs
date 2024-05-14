

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Schematic {

    public int width;
    public int height;

    public List<HeightPoints> heightMap = new List<HeightPoints>();
    public List<TileColumn> tileColumns = new List<TileColumn>();

    public Schematic(List<TileSelection> selectedItems) {

        var sortedSelectedItems = new List<TileSelection>(selectedItems);

        sortedSelectedItems = sortedSelectedItems.OrderBy(item => item.columnWorldX).ThenBy(item => item.columnWorldY).ToList();

        width = (sortedSelectedItems.Last().columnWorldX - sortedSelectedItems.First().columnWorldX) + 1;
        height = (sortedSelectedItems.Last().columnWorldY - sortedSelectedItems.First().columnWorldY) + 1;

        HeightPoints GetHeightPoint(int x, int y) {
            return heightMap[(y * (width + 1)) + x];
        }

        TileColumn GetTileColumn(int x, int y) {
            return tileColumns[(y * width) + x];
        }

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

        var startingX = sortedSelectedItems.First().columnWorldX;
        var startingY = sortedSelectedItems.First().columnWorldY;

        foreach (var item in sortedSelectedItems) {

            var emptyColumn = GetTileColumn(item.columnWorldX - startingX, item.columnWorldY - startingY);

            foreach (var vert in item.tile.verticies) {

                emptyColumn.heights[(int)vert.vertexPosition - 1]
                    .SetPoint(item.column.heights[(int)vert.vertexPosition - 1].GetTruePoint(vert.heightChannel), vert.heightChannel);

            }

            emptyColumn.tiles.Add(new Tile(item.tile, emptyColumn, null));

        }

    }

}