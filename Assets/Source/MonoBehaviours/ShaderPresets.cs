

using FCopParser;
using System.Collections.Generic;
using System.Text;

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

            var total = new StringBuilder();

            foreach (var preset in presets) {

                total.Append(preset.Compile());
                total.Append(",");

            }

            // Removes the access comma
            if (total.Length != 0) {
                total.Remove(total.Length - 1, 1);
            }

            return total.ToString();

        }

        string SavePresets(ShaderPresets presets) {

            var total = new StringBuilder();

            total.Append("{\"");

            total.Append(presets.directoryName + "\",[");

            total.Append(SavePreset(presets.presets));

            total.Append("],[");

            var comma = false;
            foreach (var subPresets in presets.subFolders) {

                if (comma) {
                    total.Append(",");
                }

                total.Append(SavePresets(subPresets));

                comma = true;

            }

            total.Append("]}");

            return total.ToString();

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
        this.shader = shader.Clone(shader.isQuad);
        this.name = name;
        this.meshID = meshID;
    }

    public ShaderPreset() {

    }

    public ShaderPreset(Tile tile) {
        this.shader = tile.shaders.Clone(tile.isQuad);
        this.name = "Preset";
        this.meshID = (int)MeshType.IDFromVerticies(tile.verticies);
    }

    public string Compile() {

        var total = new StringBuilder();

        // All Objects:
        // (NAME(string), TYPE(int), MESHTYPE(int), ...

        total.Append("(\"" + name + "\",");

        total.Append(((int)shader.type).ToString() + ",");

        total.Append(meshID.ToString() + ",");

        switch (shader.type) {
            case VertexColorType.MonoChrome:
                // VALUE(int))

                var solidMono = (MonoChromeShader)shader;
                total.Append(solidMono.value.ToString() + ")");

                break;
            case VertexColorType.DynamicMonoChrome:
                // [VALUES(int)])

                var mono = (DynamicMonoChromeShader)shader;

                total.Append("[");

                foreach (var v in mono.values) {
                    total.Append(v.ToString() + ",");
                }

                total.Remove(total.Length - 1, 1);

                total.Append("])");

                break;
            case VertexColorType.Color:
                // [VALUES(uShort)])

                var color = (ColorShader)shader;

                total.Append("[");

                foreach (var v in color.values) {
                    total.Append(v.ToUShort().ToString() + ",");
                }

                total.Remove(total.Length - 1, 1);

                total.Append("])");

                break;
            case VertexColorType.ColorAnimated:
                // This should never be the case
                // Update: Welp now it is because of schematics
                // Thanks past me for not closing this causing a slew of issues

                total.Append(")");

                break;
        }

        return total.ToString();

    }

    public void ReceiveDataToTile(Tile tile) {

        tile.shaders = shader.Clone(tile.isQuad);

    }

}

public class ColorPreset {

    public string name;
    public VertexColorType type;
    // This value is both solid mono and normal mono
    public int monoValue;
    public XRGB555 colorValue;

    public ColorPreset(string name,VertexColorType type, int monoValue) {
        this.name = name;
        this.type = type;
        this.monoValue = monoValue;
    }

    public ColorPreset(string name, VertexColorType type, XRGB555 colorValue) {
        this.name = name;
        this.type = type;
        this.colorValue = colorValue;
    }

    public ColorPreset() {

    }

}

public class ColorPresets {

    public static string tag = "COLOTAG";

    public string directoryName;

    public ColorPresets parent = null;
    public List<ColorPresets> subFolders = new();

    public List<ColorPreset> presets = new();

    public ColorPresets(string name, ColorPresets parent) {
        directoryName = name;
        this.parent = parent;
    }

    public ColorPresets() {
        directoryName = "";
    }

    public string Compile() {

        if (parent != null) { return ""; }

        string SavePreset(List<ColorPreset> presets) {

            var total = new StringBuilder();

            foreach (var preset in presets) {

                // All Objects:
                // (NAME(string), TYPE(int), ...

                total.Append("(\"" + preset.name + "\",");

                total.Append(((int)preset.type).ToString() + ",");

                switch (preset.type) {
                    case VertexColorType.MonoChrome:
                        // VALUE(int))

                        total.Append(preset.monoValue.ToString() + "),");

                        break;
                    case VertexColorType.DynamicMonoChrome:
                        // VALUE(int))

                        total.Append(preset.monoValue.ToString() + "),");

                        break;
                    case VertexColorType.Color:
                        // VALUE(uShort))

                        total.Append(preset.colorValue.ToUShort().ToString() + "),");

                        break;
                    case VertexColorType.ColorAnimated:
                        // This should never be the case
                        // Update: Welp now it is because of schematics
                        // Thanks past me for not closing this causing a slew of issues

                        total.Append("),");

                        break;
                }

            }

            // Removes the access comma
            if (total.Length != 0) {
                total.Remove(total.Length - 1, 1);
            }

            return total.ToString();

        }

        string SavePresets(ColorPresets presets) {

            var total = new StringBuilder();

            total.Append("{\"");

            total.Append(presets.directoryName + "\",[");

            total.Append(SavePreset(presets.presets));

            total.Append("],[");

            var comma = false;
            foreach (var subPresets in presets.subFolders) {

                if (comma) {
                    total.Append(",");
                }

                total.Append(SavePresets(subPresets));

                comma = true;

            }

            total.Append("]}");

            return total.ToString();

        }

        var total = SavePresets(this);

        return total;

    }

}