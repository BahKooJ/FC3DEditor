

using System;
using System.Collections.Generic;

namespace FCopParser {

    public class TextureSnippet {

        public readonly string fourCC = "Cdcs";
        static List<byte> fourCCBytes = new List<byte>() { 115, 99, 100, 67 };

        public static List<TextureSnippet> Parse(List<byte> cdscBytes) {

            var total = new List<TextureSnippet>();

            var offset = 12;

            while (offset < cdscBytes.Count) {

                var x = cdscBytes[offset];
                offset++;
                var y = cdscBytes[offset];
                offset++;
                var width = cdscBytes[offset];
                offset++;
                var height = cdscBytes[offset];
                offset++;
                var textureID = cdscBytes[offset];
                offset++;
                var zeros = BitConverter.ToInt16(cdscBytes.ToArray(), offset);
                offset += 2;
                var id = cdscBytes[offset];
                offset++;

                total.Add(new TextureSnippet(x, y, width, height, textureID, id));

            }

            return total;

        }

        public static IFFDataFile Compile(List<TextureSnippet> textureSnippets, int nullRPNSRef) {

            var data = new List<byte>();

            foreach (var textureSnippet in textureSnippets) {

                data.AddRange(textureSnippet.Compile());
                
            }

            var total = new List<byte>();

            total.AddRange(fourCCBytes);
            total.AddRange(BitConverter.GetBytes(data.Count + 12));
            total.AddRange(BitConverter.GetBytes(textureSnippets.Count));
            total.AddRange(data);

            var dataFile = new IFFDataFile(2, total, "Cdcs", 11, nullRPNSRef);

            return dataFile;

        }

        public string name;

        public int id;
        public int x;
        public int y;
        public int width;
        public int height;
        public int texturePaletteID;

        public TextureSnippet(int x, int y, int width, int height, int texturePaletteID, int id) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.texturePaletteID = texturePaletteID;
            this.id = id;
            name = "Texture Snippet " + id;
        }

        public TextureSnippet(string name, int id, int x, int y, int width, int height, int texturePaletteID) {
            this.name = name;
            this.id = id;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.texturePaletteID = texturePaletteID;
        }

        public List<int> ConvertToUVs() {

            return new List<int> {
                (y * 256) + x,
                (y * 256) + x + width,
                ((y + height) * 256) + x + width,
                ((y + height) * 256) + x
            };

        }

        public List<byte> Compile() {

            return new List<byte> {
                (byte)x,
                (byte)y,
                (byte)width,
                (byte)height,
                (byte)texturePaletteID,
                0,
                0,
                (byte)id
            };

        }

    }

}