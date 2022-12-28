
using FCopParser;
using static System.Collections.Specialized.BitVector32;

public class GeometryAddMode : EditMode {

    public Main main { get; set; }

    public TilePreset? selectedTilePreset = null;

    public GeometryAddMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {

    }

    public void OnDestroy() {

    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {

        if (column != main.selectedColumn && main.selectedColumn != null) {

            main.ClearTileOverlays();

        } else if (column == main.selectedColumn) {
            return;
        }

        main.selectedColumn = column;
        main.selectedSection = section;

        if (selectedTilePreset != null) {

            main.InitTileOverlay(((TilePreset)selectedTilePreset).Create(false));

        }


    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

        if (selectedTilePreset == null) { return; }

        main.selectedColumn = column;
        main.selectedSection = section;

        main.AddTile((TilePreset)selectedTilePreset);

        main.selectedTiles.Clear();

    }

    public void RefreshTilePlacementOverlay() {

        if (main.selectedColumn == null) { return; }

        main.ClearTileOverlays();

        if (selectedTilePreset != null) {

            main.InitTileOverlay(((TilePreset)selectedTilePreset).Create(false));

        }

    }
}