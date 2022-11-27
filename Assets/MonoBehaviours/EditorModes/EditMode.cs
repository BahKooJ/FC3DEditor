
using FCopParser;

public interface EditMode {

    public Main main { get; set; }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section);

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section);

}