

using FCopParser;
using System.Collections.Generic;

public class AssetDirectory {

    public AssetDirectory parent;

    public List<AssetFile> files = new();
    public List<AssetDirectory> directories = new();
    public AssetType storedAssets;
    public bool canAddFiles;
    public string name;

    public AssetDirectory(AssetType storedAssets, bool canAddFiles, string name, AssetDirectory parent) {
        this.storedAssets = storedAssets;
        this.canAddFiles = canAddFiles;
        this.name = name;
        this.parent = parent;
    }

}

public class AssetFile {

    public FCopAsset asset;
    public AssetType assetType;

    public AssetFile(FCopAsset asset, AssetType assetType) {
        this.asset = asset;
        this.assetType = assetType;
    }

}

public enum AssetType {
    WavSound,
    Texture,
    Object,
    SndsSound,
    Music,
    MiniAnimation,
    Mixed
}