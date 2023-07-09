
using System.Collections.Generic;
using System.IO;

public class UVPresets {

    public string directoryName;
    
    public UVPreset parent = null;
    public List<UVPresets> subFolders = new List<UVPresets>();

    public List<UVPreset> presets = new List<UVPreset>();

    public UVPresets(string name) {
        directoryName = name;
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

            total += directoryName + "\",[";

            SavePreset(presets.presets);

            total += ",[";

            foreach (var subPresets in presets.subFolders) {

                total += SavePresets(subPresets);

            }

            total += "]";

            return total;

        }

        var total = SavePresets(this);

        File.WriteAllText("TexturePresets/" + directoryName, total);

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