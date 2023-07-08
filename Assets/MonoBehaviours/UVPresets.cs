
using System.Collections.Generic;

public class UVPresets {

    public List<UVPreset> presets = new List<UVPreset>();

}

public class UVPreset {

    public List<int> uvs;
    public int texturePalette;

    public UVPreset(List<int> uvs, int texturePalette) {
        this.uvs = new List<int>(uvs);
        this.texturePalette = texturePalette;
    }
}