

using FCopParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public abstract class Presets {

    public static UVPresets uvPresets = new UVPresets("Texture Presets", null);
    public static ShaderPresets shaderPresets = new ShaderPresets("Shader Presets", null);
    public static ColorPresets colorPresets = new ColorPresets();

    public static void ReadFile(string fileName) {

        var numbers = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        var file = File.ReadAllText(fileName);

        void ReadUVPresets() {

            var startIndex = file.IndexOf(UVPresets.tag);

            // This most likely means its the old file format
            if (startIndex == -1) {
                uvPresets = UVPresets.ReadFileOld(fileName);
                return;
            }

            var opened = new List<UVPresets>();

            UVPreset currentUVPreset = null;

            var value = "";

            var isReadingValue = false;
            var openArray = false;
            var nextProperty = new List<string> { "NAME", "CBMP", "TRANSPARENT", "VECTOR", "MESH", "UV", "SPEED", "AUV" };
            var lookingForValue = -1;

            foreach (var i in Enumerable.Range(startIndex, file.Count())) {

                var c = file[i];

                // The only thing the value is really used for is names
                // If it's ever used for something else this might have problems...
                if (c == '\"') {
                    isReadingValue = !isReadingValue;

                    if (!isReadingValue) {

                        if (currentUVPreset == null) {

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

                // If the lookingForValue is equal to the count that means it found everything it needs.
                if (currentUVPreset != null && nextProperty.Count != lookingForValue) {

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

                    if (currentUVPreset != null) {
                        throw new TexturePresetsParseException("Cannot open new UV Preset without closing the first");
                    }

                    currentUVPreset = new UVPreset();
                    lookingForValue = 0;

                } 
                else if (c == ')') {

                    opened.Last().presets.Add(currentUVPreset);

                    currentUVPreset = null;

                    lookingForValue = -1;

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
                        uvPresets = opened[0];
                        return;
                    }

                }

            }

        }

        void ReadShaderPresets() {

            var startIndex = file.IndexOf(ShaderPresets.tag);

            if (startIndex == -1) {
                return;
            }

            var opened = new List<ShaderPresets>();

            ShaderPreset currentShaderPreset = null;

            var value = "";

            var standardProperties = new List<string> { "NAME", "TYPE", "MESHTYPE" };
            var lookingForValue = -1;
            var isReadingValue = false;
            var openArray = false;

            foreach (var i in Enumerable.Range(startIndex, file.Count())) {

                var c = file[i];

                // The only thing the value is really used for is names
                // If it's ever used for something else this might have problems...
                if (c == '\"') {
                    isReadingValue = !isReadingValue;

                    if (!isReadingValue) {

                        if (currentShaderPreset == null) {

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

                // Not equal to four because that's the count for all the data
                if (currentShaderPreset != null && lookingForValue != 4) {

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
                                throw new Exception("There should be no color animated presets");
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

                    if (currentShaderPreset != null) {
                        throw new Exception("Cannot open new shader Preset without closing the first");
                    }

                    currentShaderPreset = new ShaderPreset();
                    lookingForValue = 0;

                }
                else if (c == ')') {

                    opened.Last().presets.Add(currentShaderPreset);

                    currentShaderPreset = null;

                    lookingForValue = -1;

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

            colorPresets = new ColorPresets();

            var startIndex = file.IndexOf(ColorPresets.tag);

            if (startIndex == -1) {
                return;
            }

            var openArray = false;
            var value = "";
            foreach (var i in Enumerable.Range(startIndex, file.Count())) {

                var c = file[i];

                if (c == '[') {
                    openArray = true;
                    continue;
                }

                if (openArray) {

                    if (c == ',') {
                        colorPresets.presets.Add(new XRGB555(new BitArray(BitConverter.GetBytes(UInt16.Parse(value)))));
                        value = "";

                    }
                    else if (numbers.Contains(c)) {
                        value += c;
                    }

                }

                if (c == ']') {
                    return;
                }

            }

        }

        ReadUVPresets();
        ReadShaderPresets();
        ReadColorPresets();

    }

    public static void SaveToFile(string name) {

        var total = "";

        total += UVPresets.tag + uvPresets.Compile();
        total += ShaderPresets.tag + shaderPresets.Compile();
        total += ColorPresets.tag + colorPresets.Compile();

        File.WriteAllText("Presets/" + name + ".txt", total);

    }

}
