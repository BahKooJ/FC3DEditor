

using FCopParser;
using System.Collections.Generic;

public struct TilePreset {

    public int meshID;
    public int culling;
    public int textureIndex;
    public int graphicsIndex;
    public string previewImagePath;

    public TilePreset(int meshID, int culling, int textureIndex, int graphicsIndex, string previewImagePath) {
        this.meshID = meshID;
        this.culling = culling;
        this.textureIndex = textureIndex;
        this.graphicsIndex = graphicsIndex;
        this.previewImagePath = previewImagePath;
    }

    public Tile Create(bool isStart, TileColumn column) {

        return new Tile(new TileBitfield(isStart ? 1 : 0, textureIndex, culling, 0, meshID,graphicsIndex), column);

    }

}