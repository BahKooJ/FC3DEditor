

using FCopParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public abstract class Presets {

    public static UVPresets uvPresets = new UVPresets("Texture Presets", null);
    public static ShaderPresets shaderPresets = new ShaderPresets("Shader Presets", null);
    public static ColorPresets colorPresets = new ColorPresets("Color Presets", null);
    public static Schematics levelSchematics = new("Level Schematics", null);
    public static ActorSchematics actorSchematics = new("Actor Schematics", null);

    static List<char> numbers = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-' };

    public static UVPresets ReadUVPresets(string fileName) {

        var file = File.ReadAllText(fileName);

        return ReadUVPresetsContent(file);

    }

    public static UVPresets ReadUVPresetsContent(string file) {

        var startIndex = file.IndexOf(UVPresets.tag);

        if (startIndex == -1) {
            return null;
        }

        var opened = new List<UVPresets>();

        var value = "";

        var isReadingValue = false;

        var i = startIndex - 1;

        while (i < file.Length) {

            i++;

            var c = file[i];

            if (c == '\"') {
                isReadingValue = !isReadingValue;

                if (!isReadingValue) {

                    opened.Last().directoryName = value;
                    value = "";

                }

                // Continue because " is a keyword and can't be used for anything else
                continue;
            }

            // If reading value all it cares is grabing the character
            if (isReadingValue) {
                value += c;
                continue;
            }

            if (c == '(') {

                opened.Last().presets.Add(ReadUVPresetObject(file, i, out i));

            }

            // Opens or closes a subfolder.
            if (c == '{') {

                opened.Add(new UVPresets());

            }
            else if (c == '}') {

                var last = opened.Last();

                if (opened.Count() > 1) {

                    // Needs to add itself to the parent
                    var beforeLast = opened[opened.Count - 2];

                    beforeLast.subFolders.Add(last);

                    last.parent = beforeLast;

                    opened.Remove(last);

                }
                else if (opened.Count() == 1) {
                    // Finished
                    return opened[0];
                }

            }

        }

        return null;

    }

    public static HeightPoints ReadHeightMapObject(string file, int index, out int newIndex) {

        var heightMapPoint = new HeightPoints(-128, -128, -128);

        var value = "";

        var properties = new List<string> { "1", "2", "3" };
        var lookingForValue = 0;

        var i = index - 1;
        while (i < file.Length) {

            i++;

            var c = file[i];

            if (lookingForValue < properties.Count) {

                switch (properties[lookingForValue]) {

                    case "1":

                        if (c == ',') {

                            heightMapPoint.SetPoint(Int32.Parse(value), 1);

                            value = "";
                            lookingForValue++;

                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "2":

                        if (c == ',') {

                            heightMapPoint.SetPoint(Int32.Parse(value), 2);

                            value = "";
                            lookingForValue++;

                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "3":

                        if (c == ')') {

                            heightMapPoint.SetPoint(Int32.Parse(value), 3);

                            value = "";
                            lookingForValue++;

                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;

                }

            }

            if (c == '(') {
                continue;
            }
            else if (c == ')') {
                newIndex = i;
                return heightMapPoint;
            }

        }

        newIndex = i;
        return heightMapPoint;

    }

    public static TileColumn ReadTileColumnObject(string file, int x, int y, Schematic schematic, int index, out int newIndex) {

        var heights = new List<HeightPoints>();

        heights.Add(schematic.GetHeightPoint(x, y));
        heights.Add(schematic.GetHeightPoint(x + 1, y));
        heights.Add(schematic.GetHeightPoint(x, y + 1));
        heights.Add(schematic.GetHeightPoint(x + 1, y + 1));

        var tileColumn = new TileColumn(x, y, new(), heights);

        var openArray = false;

        var i = index - 1;
        while (i < file.Length) {

            i++;

            var c = file[i];

            if (c == '[') {
                openArray = true;
                continue;
            }
            else if (c == ']') {
                openArray = false;
                continue;
            }

            if (openArray) {

                if (c == '(') {
                    tileColumn.tiles.Add(ReadTileObject(file, tileColumn, i, out i));
                }

            } else {

                if (c == '(') {
                    continue;
                }
                else if (c == ')') {
                    newIndex = i;
                    return tileColumn;
                }

            }

        }
        
        newIndex = i;
        return tileColumn;

    }

    public static Tile ReadTileObject(string file, TileColumn column, int index, out int newIndex) {

        Tile tile = null;

        var value = "";

        var properties = new List<string> { "MESHID", "CULLING", "EFFECTINDEX", "TEXTURES", "SHADERS" };
        var lookingForValue = -1;

        var i = index - 1;
        while (i < file.Length) {

            i++;

            var c = file[i];

            if (lookingForValue < properties.Count && lookingForValue != -1) {

                switch (properties[lookingForValue]) {

                    case "MESHID":

                        if (c == ',') {

                            tile = new Tile(column, Int32.Parse(value), 0);

                            value = "";
                            lookingForValue++;

                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "CULLING":

                        if (c == ',') {

                            tile.culling = Int32.Parse(value);

                            value = "";
                            lookingForValue++;

                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "EFFECTINDEX":

                        if (c == ',') {

                            tile.effectIndex = Int32.Parse(value);

                            value = "";
                            lookingForValue++;

                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "TEXTURES":

                        if (c == '(') {
                            ReadUVPresetObject(file, i, out i).ReceiveDataToTile(tile);
                            lookingForValue++;
                            continue;
                        }

                        break;
                    case "SHADERS":

                        if (c == '(') {
                            ReadShaderPresetObject(file, i, out i).ReceiveDataToTile(tile);
                            lookingForValue++;
                            continue;
                        }

                        break;

                }

            }

            if (c == '(') {
                lookingForValue = 0;
                continue;
            }
            else if (c == ')') {
                newIndex = i;
                return tile;
            }

        }

        newIndex = i;
        return tile;


    }

    public static UVPreset ReadUVPresetObject(string file, int index, out int newIndex) {

        var currentUVPreset = new UVPreset();

        var value = "";

        var isReadingValue = false;
        var openArray = false;
        var nextProperty = new List<string> { "NAME", "CBMP", "TRANSPARENT", "VECTOR", "MESH", "UV", "SPEED", "AUV" };
        var lookingForValue = -1;

        var i = index - 1;
        while (i < file.Length) {

            i++;

            var c = file[i];

            if (c == '\"') {
                isReadingValue = !isReadingValue;

                // Continue because " is a keyword and can't be used for anything else
                continue;
            }

            // If reading value all it cares is grabing the character
            if (isReadingValue) {
                value += c;
                continue;
            }

            if (lookingForValue < nextProperty.Count && lookingForValue != -1) {

                // Tells the next checks that an array was just opened.
                // Continue because of keyword.
                if (c == '[') {
                    openArray = true;
                    continue;
                }
                else if (c == ']') {
                    openArray = false;
                    continue;
                }

                switch (nextProperty[lookingForValue]) {

                    case "NAME":

                        // Because the name is a string if we have data it's most likely the name
                        if (value != "") {
                            currentUVPreset.name = value;
                            value = "";
                            lookingForValue++;
                        }


                        break;
                    case "CBMP":

                        if (c == ',') {
                            currentUVPreset.texturePalette = Int32.Parse(value);
                            value = "";
                            lookingForValue++;
                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "TRANSPARENT":

                        if (c == ',') {
                            currentUVPreset.isSemiTransparent = value == "1";
                            value = "";
                            lookingForValue++;
                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "VECTOR":

                        if (c == ',') {
                            currentUVPreset.isVectorAnimated = value == "1";
                            value = "";
                            lookingForValue++;
                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "MESH":

                        if (c == ',') {
                            currentUVPreset.meshID = Int32.Parse(value);
                            value = "";
                            lookingForValue++;
                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "UV":

                        if (openArray) {

                            if (c == ',') {
                                currentUVPreset.uvs.Add(Int32.Parse(value));
                                value = "";

                            }
                            else if (numbers.Contains(c)) {
                                value += c;
                            }

                        }
                        else {

                            currentUVPreset.uvs.Add(Int32.Parse(value));
                            value = "";
                            lookingForValue++;

                        }

                        break;
                    case "SPEED":

                        if (c == ',') {
                            currentUVPreset.animationSpeed = Int32.Parse(value);
                            value = "";
                            lookingForValue++;
                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "AUV":

                        if (openArray) {

                            if (c == ',') {
                                currentUVPreset.animatedUVs.Add(Int32.Parse(value));
                                value = "";

                            }
                            else if (numbers.Contains(c)) {
                                value += c;
                            }

                        }
                        else {

                            currentUVPreset.animatedUVs.Add(Int32.Parse(value));
                            value = "";
                            lookingForValue++;

                        }

                        break;

                }

            }

            if (c == '(') {
                lookingForValue = 0;
                continue;
            }
            else if (c == ')') {
                newIndex = i;
                return currentUVPreset;
            }

        }

        newIndex = i;
        return currentUVPreset;

    }

    public static ShaderPreset ReadShaderPresetObject(string file, int index, out int newIndex) {

        var currentShaderPreset = new ShaderPreset();

        var value = "";

        var standardProperties = new List<string> { "NAME", "TYPE", "MESHTYPE" };
        var lookingForValue = -1;
        var isReadingValue = false;
        var openArray = false;

        var i = index - 1;
        while (i < file.Length) {

            i++;

            var c = file[i];

            if (c == '\"') {
                isReadingValue = !isReadingValue;

                // Continue because " is a keyword and can't be used for anything else
                continue;
            }

            // If reading value all it cares is grabing the character
            if (isReadingValue) {
                value += c;
                continue;
            }

            // Not equal to four because that's the count for all the data
            if (lookingForValue != -1 && lookingForValue != 4) {

                // Tells the next checks that an array was just opened.
                // Continue because of keyword.
                if (c == '[') {
                    openArray = true;
                    continue;
                }
                else if (c == ']') {
                    openArray = false;
                    continue;
                }

                // All the common values found, time to move onto the shader data.
                if (standardProperties.Count <= lookingForValue) {

                    switch (currentShaderPreset.shader.type) {

                        case VertexColorType.MonoChrome:

                            // It checks for close par because it's the last value and not an array
                            if (c == ')') {

                                var solidMono = (MonoChromeShader)currentShaderPreset.shader;

                                solidMono.value = Byte.Parse(value);
                                value = "";
                                solidMono.isQuad = MeshType.quadMeshes.Contains(currentShaderPreset.meshID);
                                solidMono.Apply();
                                lookingForValue++;

                            }
                            else if (numbers.Contains(c)) {
                                value += c;
                            }

                            break;
                        case VertexColorType.DynamicMonoChrome:

                            var mono = (DynamicMonoChromeShader)currentShaderPreset.shader;

                            if (openArray) {

                                if (c == ',') {

                                    var listValues = mono.values.ToList();
                                    listValues.Add(Int32.Parse(value));
                                    mono.values = listValues.ToArray();
                                    value = "";

                                }
                                else if (numbers.Contains(c)) {
                                    value += c;
                                }

                            }
                            else {

                                var listValues = mono.values.ToList();
                                listValues.Add(Int32.Parse(value));
                                mono.values = listValues.ToArray();
                                mono.isQuad = MeshType.quadMeshes.Contains(currentShaderPreset.meshID);
                                mono.Apply();
                                value = "";
                                lookingForValue++;

                            }

                            break;
                        case VertexColorType.Color:

                            var color = (ColorShader)currentShaderPreset.shader;

                            if (openArray) {

                                if (c == ',') {

                                    var listValues = color.values.ToList();
                                    listValues.Add(new XRGB555(new BitArray(BitConverter.GetBytes(UInt16.Parse(value)))));
                                    color.values = listValues.ToArray();
                                    value = "";

                                }
                                else if (numbers.Contains(c)) {
                                    value += c;
                                }

                            }
                            else {

                                var listValues = color.values.ToList();
                                listValues.Add(new XRGB555(new BitArray(BitConverter.GetBytes(UInt16.Parse(value)))));
                                color.values = listValues.ToArray();
                                color.isQuad = MeshType.quadMeshes.Contains(currentShaderPreset.meshID);
                                color.Apply();
                                value = "";
                                lookingForValue++;

                            }

                            break;
                        case VertexColorType.ColorAnimated:

                            var animated = (AnimatedShader)currentShaderPreset.shader;

                            animated.isQuad = MeshType.quadMeshes.Contains(currentShaderPreset.meshID);

                            lookingForValue++;

                            break;

                    }

                }
                else {

                    switch (standardProperties[lookingForValue]) {

                        case "NAME":

                            // Because the name is a string if we have data it's most likely the name
                            if (value != "") {
                                currentShaderPreset.name = value;
                                value = "";
                                lookingForValue++;
                            }

                            break;
                        case "TYPE":

                            if (c == ',') {
                                var type = (VertexColorType)Int32.Parse(value);

                                // It doesn't know if it's a quad or not so it's just init to false
                                switch (type) {

                                    case VertexColorType.MonoChrome:
                                        currentShaderPreset.shader = new MonoChromeShader();
                                        break;
                                    case VertexColorType.DynamicMonoChrome:
                                        currentShaderPreset.shader = new DynamicMonoChromeShader();
                                        break;
                                    case VertexColorType.Color:
                                        currentShaderPreset.shader = new ColorShader();
                                        break;
                                    case VertexColorType.ColorAnimated:
                                        currentShaderPreset.shader = new AnimatedShader();
                                        break;

                                }

                                value = "";
                                lookingForValue++;
                            }
                            else if (numbers.Contains(c)) {
                                value += c;
                            }

                            break;
                        case "MESHTYPE":

                            if (c == ',') {
                                currentShaderPreset.meshID = Int32.Parse(value);
                                value = "";
                                lookingForValue++;
                            }
                            else if (numbers.Contains(c)) {
                                value += c;
                            }

                            break;

                    }

                }

            }

            if (c == '(') {
                lookingForValue = 0;
                continue;
            }
            else if (c == ')') {
                newIndex = i;
                return currentShaderPreset;
            }

        }
        
        newIndex = i;
        return currentShaderPreset;

    }

    public static Schematic ReadSchematicObject(string file, int index, out int newIndex) {

        var schematic = new Schematic("", 0, 0, new(), new());

        var value = "";

        var properties = new List<string> { "NAME", "WIDTH", "HEIGHT", "HEIGHTMAP", "TILECOLUMNS" };
        var lookingForValue = -1;
        var isReadingValue = false;
        var openArray = false;

        var columnX = 0;
        var columnY = 0;

        var i = index - 1;
        while (i < file.Length) {

            i++;

            var c = file[i];

            if (c == '\"') {
                isReadingValue = !isReadingValue;

                // Continue because " is a keyword and can't be used for anything else
                continue;
            }

            // If reading value all it cares is grabing the character
            if (isReadingValue) {
                value += c;
                continue;
            }

            // Tells the next checks that an array was just opened.
            // Continue because of keyword.
            if (c == '[') {
                openArray = true;
                continue;
            }
            else if (c == ']') {
                openArray = false;
                continue;
            }

            if (lookingForValue < properties.Count && lookingForValue != -1) {

                switch (properties[lookingForValue]) {

                    case "NAME":

                        if (value != "") {
                            schematic.name = value;
                            value = "";
                            lookingForValue++;
                        }

                        break;
                    case "WIDTH":

                        if (c == ',') {

                            schematic.width = Int32.Parse(value);

                            value = "";
                            lookingForValue++;

                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "HEIGHT":

                        if (c == ',') {

                            schematic.height = Int32.Parse(value);

                            value = "";
                            lookingForValue++;

                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "HEIGHTMAP":

                        if (openArray) {

                            if (c == '(') {
                                schematic.heightMap.Add(ReadHeightMapObject(file, i, out i));
                                continue;
                            }

                        }
                        else {
                            value = "";
                            lookingForValue++;
                        }

                        break;
                    case "TILECOLUMNS":
                        if (c == '(') {

                            schematic.tileColumns.Add(ReadTileColumnObject(file, columnX, columnY, schematic, i, out i));

                            columnX++;

                            if (columnX == schematic.width) {
                                columnY++;
                                columnX = 0;
                            }

                            continue;
                        }
                        break;

                }

            }

            if (c == '(') {
                lookingForValue = 0;
                continue;
            }
            else if (c == ')') {
                newIndex = i;
                return schematic;
            }

        }

        newIndex = i;
        return schematic;

    }

    public static ActorSchematic ReadActorSchematicObject(string file, int index, out int newIndex) {

        var schematic = new ActorSchematic();

        var value = "";

        var properties = new List<string> { "NAME", "TYPE", "BYTES" };
        var lookingForValue = -1;
        var isReadingValue = false;
        var openArray = false;

        var i = index - 1;
        while (i < file.Length) {

            i++;

            var c = file[i];

            if (c == '\"') {
                isReadingValue = !isReadingValue;

                // Continue because " is a keyword and can't be used for anything else
                continue;
            }

            // If reading value all it cares is grabing the character
            if (isReadingValue) {
                value += c;
                continue;
            }

            // Tells the next checks that an array was just opened.
            // Continue because of keyword.
            if (c == '[') {
                openArray = true;
                continue;
            }
            else if (c == ']') {
                openArray = false;
                continue;
            }

            if (lookingForValue < properties.Count && lookingForValue != -1) {

                switch (properties[lookingForValue]) {

                    case "NAME":

                        if (value != "") {
                            schematic.name = value;
                            value = "";
                            lookingForValue++;
                        }

                        break;
                    case "TYPE":

                        if (c == ',') {

                            schematic.behavior = (ActorBehavior)Int32.Parse(value);

                            value = "";
                            lookingForValue++;

                        }
                        else if (numbers.Contains(c)) {
                            value += c;
                        }

                        break;
                    case "BYTES":

                        if (openArray) {

                            if (c == ',') {
                                schematic.actorData.Add(Byte.Parse(value));
                                value = "";

                            }
                            else if (numbers.Contains(c)) {
                                value += c;
                            }

                        }
                        else {

                            schematic.actorData.Add(Byte.Parse(value));
                            value = "";
                            lookingForValue++;

                        }

                        break;

                }

            }

            if (c == '(') {
                lookingForValue = 0;
                continue;
            }
            else if (c == ')') {
                newIndex = i;
                return schematic;
            }

        }

        newIndex = i;
        return schematic;

    }

    public static void ReadFile(string fileName) {

        var file = File.ReadAllText(fileName);

        ReadString(file);

    }

    public static void ReadString(string file) {

        void ReadShaderPresets() {

            var startIndex = file.IndexOf(ShaderPresets.tag);

            if (startIndex == -1) {
                return;
            }

            var opened = new List<ShaderPresets>();

            var value = "";

            var isReadingValue = false;

            var i = startIndex - 1;

            while (i < file.Length) {

                i++;

                var c = file[i];

                // The only thing the value is really used for is names
                // If it's ever used for something else this might have problems...
                if (c == '\"') {
                    isReadingValue = !isReadingValue;

                    if (!isReadingValue) {

                        opened.Last().directoryName = value;
                        value = "";

                    }

                    // Continue because " is a keyword and can't be used for anything else
                    continue;
                }

                // If reading value all it cares is grabing the character
                if (isReadingValue) {
                    value += c;
                    continue;
                }

                if (c == '(') {

                    opened.Last().presets.Add(ReadShaderPresetObject(file, i, out i));

                }

                // Opens or closes a subfolder.
                if (c == '{') {

                    opened.Add(new ShaderPresets());

                }
                else if (c == '}') {

                    var last = opened.Last();

                    if (opened.Count() > 1) {

                        // Needs to add itself to the parent
                        var beforeLast = opened[opened.Count - 2];

                        beforeLast.subFolders.Add(last);

                        last.parent = beforeLast;

                        opened.Remove(last);

                    }
                    else if (opened.Count() == 1) {
                        // Finished
                        shaderPresets = opened[0];
                        return;
                    }

                }

            }

        }

        void ReadColorPresets() {

            var startIndex = file.IndexOf(ColorPresets.tag);

            if (startIndex == -1) {
                return;
            }

            var opened = new List<ColorPresets>();

            ColorPreset currentColorPreset = null;

            var value = "";

            var nextProperty = new List<string> { "NAME", "TYPE", "VALUE" };
            var lookingForValue = -1;
            var isReadingValue = false;

            foreach (var i in Enumerable.Range(startIndex, file.Count())) {

                var c = file[i];

                // The only thing the value is really used for is names
                // If it's ever used for something else this might have problems...
                if (c == '\"') {
                    isReadingValue = !isReadingValue;

                    if (!isReadingValue) {

                        if (currentColorPreset == null) {

                            opened.Last().directoryName = value;

                            value = "";

                        }


                    }

                    // Continue because " is a keyword and can't be used for anything else
                    continue;
                }

                // If reading value all it cares is grabing the character
                if (isReadingValue) {
                    value += c;
                    continue;
                }

                // Not equal to three because that's the count for all the data
                if (currentColorPreset != null && lookingForValue != 3) {


                    switch (nextProperty[lookingForValue]) {

                        case "NAME":

                            // Because the name is a string if we have data it's most likely the name
                            if (value != "") {
                                currentColorPreset.name = value;
                                value = "";
                                lookingForValue++;
                            }

                            break;
                        case "TYPE":

                            if (c == ',') {

                                var type = (VertexColorType)Int32.Parse(value);

                                currentColorPreset.type = type;

                                value = "";
                                lookingForValue++;
                            }
                            else if (numbers.Contains(c)) {
                                value += c;
                            }

                            break;
                        case "VALUE":

                            if (c == ')') {

                                switch (currentColorPreset.type) {
                                    case VertexColorType.MonoChrome:
                                        currentColorPreset.monoValue = Int32.Parse(value);
                                        break;
                                    case VertexColorType.DynamicMonoChrome:
                                        currentColorPreset.monoValue = Int32.Parse(value);
                                        break;
                                    case VertexColorType.Color:
                                        currentColorPreset.colorValue = new XRGB555(new BitArray(BitConverter.GetBytes(UInt16.Parse(value))));
                                        break;
                                    case VertexColorType.ColorAnimated:
                                        break;

                                }

                                value = "";
                                lookingForValue++;
                            }
                            else if (numbers.Contains(c)) {
                                value += c;
                            }

                            break;

                    }

                }



                if (c == '(') {

                    if (currentColorPreset != null) {
                        throw new Exception("Cannot open new shader Preset without closing the first");
                    }

                    currentColorPreset = new ColorPreset();
                    lookingForValue = 0;

                }
                else if (c == ')') {

                    opened.Last().presets.Add(currentColorPreset);

                    currentColorPreset = null;

                    lookingForValue = -1;

                }

                // Opens or closes a subfolder.
                if (c == '{') {

                    opened.Add(new ColorPresets());

                }
                else if (c == '}') {

                    var last = opened.Last();

                    if (opened.Count() > 1) {

                        // Needs to add itself to the parent
                        var beforeLast = opened[opened.Count - 2];

                        beforeLast.subFolders.Add(last);

                        last.parent = beforeLast;

                        opened.Remove(last);

                    }
                    else if (opened.Count() == 1) {
                        // Finished
                        colorPresets = opened[0];
                        return;
                    }

                }

            }

        }

        void ReadLevelSchematics() {

            var startIndex = file.IndexOf(Schematics.tag);

            if (startIndex == -1) {
                return;
            }

            var opened = new List<Schematics>();

            var value = "";

            var isReadingValue = false;

            var i = startIndex - 1;

            while (i < file.Length) {

                i++;

                var c = file[i];

                // The only thing the value is really used for is names
                // If it's ever used for something else this might have problems...
                if (c == '\"') {
                    isReadingValue = !isReadingValue;

                    if (!isReadingValue) {

                        opened.Last().directoryName = value;
                        value = "";

                    }

                    // Continue because " is a keyword and can't be used for anything else
                    continue;
                }

                // If reading value all it cares is grabing the character
                if (isReadingValue) {
                    value += c;
                    continue;
                }

                if (c == '(') {

                    opened.Last().schematics.Add(ReadSchematicObject(file, i, out i));

                }

                // Opens or closes a subfolder.
                if (c == '{') {

                    opened.Add(new Schematics());

                }
                else if (c == '}') {

                    var last = opened.Last();

                    if (opened.Count() > 1) {

                        // Needs to add itself to the parent
                        var beforeLast = opened[opened.Count - 2];

                        beforeLast.subFolders.Add(last);

                        last.parent = beforeLast;

                        opened.Remove(last);

                    }
                    else if (opened.Count() == 1) {
                        // Finished
                        levelSchematics = opened[0];
                        return;
                    }

                }

            }

        }

        void ReadActorSchematics() {

            var startIndex = file.IndexOf(ActorSchematics.tag);

            if (startIndex == -1) {
                return;
            }

            var opened = new List<ActorSchematics>();

            var value = "";

            var isReadingValue = false;

            var i = startIndex - 1;

            while (i < file.Length) {

                i++;

                var c = file[i];

                // The only thing the value is really used for is names
                // If it's ever used for something else this might have problems...
                if (c == '\"') {
                    isReadingValue = !isReadingValue;

                    if (!isReadingValue) {

                        opened.Last().directoryName = value;
                        value = "";

                    }

                    // Continue because " is a keyword and can't be used for anything else
                    continue;
                }

                // If reading value all it cares is grabing the character
                if (isReadingValue) {
                    value += c;
                    continue;
                }

                if (c == '(') {

                    opened.Last().schematics.Add(ReadActorSchematicObject(file, i, out i));

                }

                // Opens or closes a subfolder.
                if (c == '{') {

                    opened.Add(new ActorSchematics());

                }
                else if (c == '}') {

                    var last = opened.Last();

                    if (opened.Count() > 1) {

                        // Needs to add itself to the parent
                        var beforeLast = opened[opened.Count - 2];

                        beforeLast.subFolders.Add(last);

                        last.parent = beforeLast;

                        opened.Remove(last);

                    }
                    else if (opened.Count() == 1) {
                        // Finished
                        actorSchematics = opened[0];
                        return;
                    }

                }

            }

        }

        uvPresets = ReadUVPresetsContent(file);
        ReadShaderPresets();
        ReadColorPresets();
        ReadLevelSchematics();
        ReadActorSchematics();

    }

    public static void SaveToFile(string name) {

        File.WriteAllText("Presets/" + name + ".txt", Save());

    }

    public static string Save() {

        var total = new StringBuilder();

        total.Append(UVPresets.tag + uvPresets.Compile());
        total.Append(ShaderPresets.tag + shaderPresets.Compile());
        total.Append(ColorPresets.tag + colorPresets.Compile());
        total.Append(Schematics.tag + levelSchematics.Compile());
        total.Append(ActorSchematics.tag + actorSchematics.Compile());

        return total.ToString();

    }

}
