

using FCopParser;
using System.Collections.Generic;

public struct TilePreset {

    // CC IS IMPORTANT!!!

    public static List<TilePreset> defaultPresets = new() { 
        new TilePreset(68,0,0,0),
        new TilePreset(0,0,0,0),
        new TilePreset(108,2,0,0),
        new TilePreset(71,0,0,0)
    };

    public int meshID;
    public int unknownButVeryImportantNumber;
    public int textureIndex;
    public int graphicsIndex;

    public TilePreset(int meshID, int unknownButVeryImportantNumber, int textureIndex, int graphicsIndex) {
        this.meshID = meshID;
        this.unknownButVeryImportantNumber = unknownButVeryImportantNumber;
        this.textureIndex = textureIndex;
        this.graphicsIndex = graphicsIndex;
    }

    public Tile Create(bool isStart) {

        return new Tile(new TileBitfield(isStart ? 1 : 0, textureIndex, 2, 0, meshID,graphicsIndex));

    }

}