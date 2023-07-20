
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class UVPresets {

    public static UVPresets ReadFile(string fileName) {

        var file = File.ReadAllText(fileName);

        var opened = new List<UVPresets>();

        UVPreset currentUVPreset = null;

        var isReadingValue = false;
        var isUVArrayOpened = false;
        var lookingForPaletteNum = false;

        var value = "";

        foreach (var c in file) {

            // TODO: If name has '"' it won't parse correctly
            if (c == '\"') {
                isReadingValue = !isReadingValue;

                if (!isReadingValue) {

                    if (currentUVPreset != null) {

                        currentUVPreset.name = value;

                    }
                    else {

                        opened.Last().directoryName = value;

                    }

                    value = "";

                }

                continue;

            }

            if (isReadingValue) {
                value += c;
                continue;
            }

            if (lookingForPaletteNum) {

                if (new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }.Contains(c)) {
                    value += c;
                }

            }

            if (currentUVPreset != null && isUVArrayOpened) {

                if (new List<char> { '0','1','2','3','4','5','6','7','8','9' }.Contains(c)) {
                    value += c;
                }

                if (c == ',') {
                    currentUVPreset.uvs.Add(Int32.Parse(value));
                    value = "";
                }

            }

            if (c == '(') {

                if (currentUVPreset != null) {
                    throw new TexturePresetsParseException("Cannot open new UV Preset without closing the first");
                }

                currentUVPreset = new UVPreset();


            } 
            else if (c == ')') {

                if (value.Count() == 0) {
                    throw new TexturePresetsParseException("UV Preset closed without storing texture palette ID");
                }

                if (currentUVPreset.uvs.Count == 0) {
                    throw new TexturePresetsParseException("UV Preset closed without storing UVs");
                }

                currentUVPreset.texturePalette = Int32.Parse(value);

                lookingForPaletteNum = false;

                value = "";

                opened.Last().presets.Add(currentUVPreset);

                currentUVPreset = null;

            }

            if (c == '[' && currentUVPreset != null) {
                isUVArrayOpened = true;
            } 
            else if (c == ']' && currentUVPreset != null) {

                isUVArrayOpened = false;

                if (value.Count() != 0) {
                    currentUVPreset.uvs.Add(Int32.Parse(value));
                    value = "";
                }

                lookingForPaletteNum = true;

            }

            if (c == '{') {

                opened.Add(new UVPresets());

            }
            else if (c == '}') {

                var last = opened.Last();

                if (opened.Count() > 1) {

                    var beforeLast = opened[opened.Count - 2];

                    beforeLast.subFolders.Add(last);

                    last.parent = beforeLast;

                    opened.Remove(last);

                }
                else if (opened.Count() == 1) {
                    return opened[0];
                }

            }

        }

        return null;

    }

    public string directoryName;
    
    public UVPresets parent = null;
    public List<UVPresets> subFolders = new List<UVPresets>();

    public List<UVPreset> presets = new List<UVPreset>();

    public UVPresets(string name, UVPresets parent) {
        directoryName = name;
        this.parent = parent;
    }

    public UVPresets() {
        directoryName = "";
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

    public UVPreset() {

        uvs = new List<int>();
        texturePalette = 0;
        name = "";

    }

}

public class TexturePresetsParseException : Exception {

    public TexturePresetsParseException(string message): base(message) { 

    }

}