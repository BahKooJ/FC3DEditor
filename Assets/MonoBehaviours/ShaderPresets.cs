

using FCopParser;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ShaderPresets {

    public static string tag = "SHADTAG";

    public string directoryName;

    public ShaderPresets parent = null;
    public List<ShaderPresets> subFolders = new List<ShaderPresets>();

    public List<ShaderPreset> presets = new List<ShaderPreset>();

    public ShaderPresets(string name, ShaderPresets parent) {
        directoryName = name;
        this.parent = parent;
    }

    public ShaderPresets() {
        directoryName = "";
    }

    public string Compile() {

        if (parent != null) { return ""; }

        string SavePreset(List<ShaderPreset> presets) {

            var total = "";

            foreach (var preset in presets) {

                // All Objects:
                // (NAME(string), TYPE(int), MESHTYPE(int), ...

                total += "(\"" + preset.name + "\",";

                total += ((int)preset.shader.type).ToString() + ",";

                total += preset.meshID.ToString() + ",";

                switch (preset.shader.type) {
                    case VertexColorType.MonoChrome:
                        // VALUE(int))

                        var solidMono = (MonoChromeShader)preset.shader;
                        total += solidMono.value.ToString() + "),";

                        break;
                    case VertexColorType.DynamicMonoChrome:
                        // [VALUES(int)])

                        var mono = (DynamicMonoChromeShader)preset.shader;

                        total += "[";

                        foreach (var v in mono.values) {
                            total += v.ToString() + ",";
                        }

                        total = total.Remove(total.Length - 1);

                        total += "]),";

                        break;
                    case VertexColorType.Color:
                        // [VALUES(uShort)])

                        var color = (ColorShader)preset.shader;

                        total += "[";

                        foreach (var v in color.values) {
                            total += v.ToUShort().ToString() + ",";
                        }

                        total = total.Remove(total.Length - 1);

                        total += "]),";

                        break;
                    case VertexColorType.ColorAnimated: 
                        // This should never be the case

                        break;
                }

            }

            // Removes the access comma
            if (total != "") {
                total = total.Remove(total.Length - 1);
            }

            return total;

        }

        string SavePresets(ShaderPresets presets) {

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

        return total;

    }

}

public class ShaderPreset {

    public TileShaders shader;
    public string name;
    public int meshID;

    public ShaderPreset(TileShaders shader, string name, int meshID) {
        this.shader = shader.Clone();
        this.name = name;
        this.meshID = meshID;
    }

    public ShaderPreset() {

    }

}

public class ColorPresets {

    public static string tag = "COLOTAG";

    public List<XRGB555> presets = new();

    public string Compile() {

        var total = "[";

        foreach (var preset in presets) {

            total += preset.ToUShort().ToString() + ",";

        }

        // Removes the access comma
        if (total != "[") {
            total = total.Remove(total.Length - 1);
        }

        return total + "]";

    }

}