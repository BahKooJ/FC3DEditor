

using FCopParser;

public class TilePreset {

    public Tile tile;
    public Tile transformedTile = null;

    public TilePreset(int meshID, int culling) {
        tile = new Tile(null, meshID, culling);

    }

    public int MeshID() {

        if (transformedTile == null) {
            var id = MeshType.IDFromVerticies(tile.verticies);

            return (int)id;
        }
        else {
            var id = MeshType.IDFromVerticies(transformedTile.verticies);

            return (int)id;
        }

    }

    public Tile Create(TileColumn column) {

        if (transformedTile == null) {
            return new Tile(tile, column, null);
        }
        else {
            return new Tile(transformedTile, column, null);
        }


    }

    public void RotateClockwise() {

        transformedTile ??= tile.Clone();

        var result = transformedTile.RotateVerticesClockwise();

    }

    public void RotateCounterClockwise() {

        transformedTile ??= tile.Clone();

        var result = transformedTile.RotateVerticesCounterClockwise();

    }

    public void MoveHeightChannelsToNextChannel() {

        transformedTile ??= tile.Clone();

        var result = transformedTile.MoveHeightChannelsToNextChannel();

        if (result == Tile.TransformResult.Invalid) {
            transformedTile = null;
        }

    }
}