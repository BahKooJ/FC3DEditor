
using System.Collections.Generic;
using System.IO;

public class UVPresets {

    public string directoryName;
    
    public UVPresets parent = null;
    public List<UVPresets> subFolders = new List<UVPresets>();

    public List<UVPreset> presets = new List<UVPreset>();

    public UVPresets(string name, UVPresets parent) {
        directoryName = name;
        this.parent = parent;
    }

    public void SaveToFile() {

        if (parent != null) { return; }

        string SavePreset(List<UVPreset> presets) {

            var total = "";

            var addCommaPreset = false;
            foreach (var preset in presets) {

                if (addCommaPreset) {
                    total += ",";
                }

                total += "(\"" + preset.name + "\",";

                total += "[";

                var addCommaUV = false;
                foreach (var uv in preset.uvs) {

                    if (addCommaUV) {
                        total += ",";
                    }

                    total += uv.ToString();

                    addCommaUV = true;
                }

                total += "]," + preset.texturePalette.ToString() + ")";

            }

            return total;

        }

        string SavePresets(UVPresets presets) {

            var total = "";

            total += "{\"";

            total += presets.directoryName + "\",[";

            total += SavePreset(presets.presets);

            total += "],[";

            var comma = false;
            foreach (var subPresets in presets.subFolders) {

                if (comma) {
                    total += ",";
                }

                total += SavePresets(subPresets);

                comma = true;

            }

            total += "]}";

            return total;

        }

        var total = SavePresets(this);

        File.WriteAllText("TexturePresets/" + directoryName + ".txt", total);

    }

}

public class UVPreset {

    public List<int> uvs;
    public int texturePalette;
    public string name;

    public UVPreset(List<int> uvs, int texturePalette, string name) {
        this.uvs = new List<int>(uvs);
        this.texturePalette = texturePalette;
        this.name = name;
    }
}