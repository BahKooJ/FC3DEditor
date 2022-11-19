

using FCopParser;
using System.Collections.Generic;

public struct TilePreset {

    public static List<TilePreset> defaultPresets = new() { 
        new TilePreset(68,0,0),
        new TilePreset(0,0,0),
        new TilePreset(108,0,0),
        new TilePreset(71,0,0)
    };

    public int meshID;
    public int textureIndex;
    public int graphicsIndex;

    public TilePreset(int meshID, int textureIndex, int graphicsIndex) {
        this.meshID = meshID;
        this.textureIndex = textureIndex;
        this.graphicsIndex = graphicsIndex;
    }

    public Tile Create(bool isStart) {

        return new Tile(new TileBitfield(isStart ? 1 : 0, textureIndex, 2, 0, meshID,graphicsIndex));

    }

}