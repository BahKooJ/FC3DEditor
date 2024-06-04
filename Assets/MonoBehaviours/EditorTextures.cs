

using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class EditorTextures {

    static Dictionary<TileEffectType, UniversalTileTexture> CreateTileEffectTextures() {

        var tileEffectTextures = new Dictionary<TileEffectType, UniversalTileTexture>();

        var presets = Presets.ReadUVPresets("TileEffectTextureData.txt");

        foreach (var subFolder in presets.subFolders) {

            var didParse = Enum.TryParse(subFolder.directoryName, out TileEffectType type);

            if (didParse) {

                tileEffectTextures[type] = new UniversalTileTexture(subFolder);

            }

        }

        return tileEffectTextures;

    }

    public static Dictionary<TileEffectType, UniversalTileTexture> tileEffectTextures = CreateTileEffectTextures();

}

public class UniversalTileTexture {

    public Dictionary<MeshShape, UVPreset> presetsByShape = new();

    public UniversalTileTexture(UVPresets root) {

        foreach (var preset in root.presets) {

            var didParse = Enum.TryParse(preset.name, out MeshShape meshShape);

            if (didParse) {

                presetsByShape[meshShape] = preset;


            }

        }

    }

    public List<int> UVsFromtVertices(List<TileVertex> vertices) {

        if (presetsByShape.ContainsKey(MeshShape.All)) {
            return presetsByShape[MeshShape.All].uvs;
        }

        var shape = MeshUtils.GetTileMeshShape(vertices);

        return presetsByShape[shape].uvs;

    }

}