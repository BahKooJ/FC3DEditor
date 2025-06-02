using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCopParser {

    public class FCopRPNS {

        // Uses the script offset as it's ID or key.
        // The key is only updated on a new file load.
        // Even if the file is compiled the key stays the same.
        // The new offset is stored on FCopScript.
        public Dictionary<int, FCopScript> codeByOffset = new();
        public List<FCopScript> code = new();

        public List<byte> bytes = new();

        public IFFDataFile rawFile;

        public FCopRPNS(IFFDataFile rawFile) {

            this.rawFile = rawFile;

            bytes = rawFile.data;

            var i = 0;
            while (i < bytes.Count) {

                var script = new FCopScript(i, bytes);
                codeByOffset[i] = script;
                code.Add(script);
                i = script.terminationOffset;

            }

        }

        public FCopRPNS(List<byte> ncfcBytes) {

            var arrayBytes = ncfcBytes.ToArray();

            var i = 0;
            while (i < ncfcBytes.Count) {

                var offset = BitConverter.ToInt32(arrayBytes, i);
                i += 4;
                var nameSize = BitConverter.ToInt32(arrayBytes, i);
                i += 4;
                var name = Encoding.ASCII.GetString(ncfcBytes.GetRange(i, nameSize).ToArray());
                i += nameSize;
                var commentSize = BitConverter.ToInt32(arrayBytes, i);
                i += 4;
                var comment = Encoding.ASCII.GetString(ncfcBytes.GetRange(i, commentSize).ToArray());
                i += commentSize;
                var codeSize = BitConverter.ToInt32(arrayBytes, i);
                i += 4;
                var code = ncfcBytes.GetRange(i, codeSize);
                i += codeSize;

                var script = new FCopScript(0, code) {
                    name = name,
                    comment = comment,
                    offset = offset
                };

                codeByOffset[offset] = script;
                this.code.Add(script);

            }

            this.rawFile = new IFFDataFile(2, new(), "RPNS", 1, -1);

        }

        public void AddScript() {

            var keys = codeByOffset.Keys.ToList();

            var nextKey = Utils.FindNextInt(keys);

            var script = new FCopScript(nextKey);
            codeByOffset[nextKey] = script;
            code.Insert(0, script);

        }

        public void RemoveScript(int offset) {
            codeByOffset.Remove(offset);
            code.RemoveAll(script => script.offset == offset);
        }

        public IFFDataFile Compile() {

            var total = new List<byte>();

            var offset = 0;
            foreach (var line in code) {

                var compiledLine = line.Compile(offset, false);

                total.AddRange(compiledLine);

                offset += compiledLine.Count;

            }

            rawFile.data = total;

            return rawFile;

        }

        public List<byte> CompileNCFC() {

            var scriptData = new List<byte>();

            var offset = 0;
            foreach (var script in code) {

                scriptData.AddRange(BitConverter.GetBytes(offset));
                scriptData.AddRange(BitConverter.GetBytes(script.name.Length));
                scriptData.AddRange(Encoding.ASCII.GetBytes(script.name));
                scriptData.AddRange(BitConverter.GetBytes(script.comment.Length));
                scriptData.AddRange(Encoding.ASCII.GetBytes(script.comment));

                var compiledBytes = script.Compile(offset, true);

                scriptData.AddRange(BitConverter.GetBytes(compiledBytes.Count));
                scriptData.AddRange(compiledBytes);

                offset += compiledBytes.Count;

            }

            var total = new List<byte>();

            total.AddRange(Encoding.ASCII.GetBytes("SCPTRPNS"));
            total.AddRange(BitConverter.GetBytes(scriptData.Count + 16));
            total.AddRange(BitConverter.GetBytes(code.Count));
            total.AddRange(scriptData);

            return total;

        }

        public void ResetKeys() {

            var newOrder = new List<FCopScript>();
            foreach (var line in codeByOffset) {
                newOrder.Add(line.Value);
            }

            codeByOffset.Clear();

            foreach (var line in newOrder) {
                codeByOffset.Add(line.offset, line);
            }

        }

    }

}