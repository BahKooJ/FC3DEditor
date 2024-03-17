

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public abstract class Presets {

    public static UVPresets uvPresets = new UVPresets("Texture Presets", null);
    public static ShaderPresets shaderPresets = new ShaderPresets("Shader Presets", null);

    public static void ReadFile(string fileName) {

        var numbers = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        var file = File.ReadAllText(fileName);

        void ReadUVPresets() {

            var startIndex = file.IndexOf(UVPresets.tag);

            var opened = new List<UVPresets>();

            UVPreset currentUVPreset = null;

            var value = "";

            var isReadingValue = false;
            var openArray = false;
            var nextProperty = new List<string> { "NAME", "CBMP", "MESH", "UV" };
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

        ReadUVPresets();

    }

    public static void SaveToFile(string name) {

        var total = "";

        total += UVPresets.tag + uvPresets.Compile();
        total += ShaderPresets.tag + shaderPresets.Compile();

        File.WriteAllText("TexturePresets/" + name + ".txt", total);

    }

}
