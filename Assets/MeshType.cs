
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FCopParser {
    public abstract class MeshType {

        static readonly public List<int> quadMeshes = new() { 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 103, 104, 105, 106, 107, 108, 109, 110 };
        static readonly public List<int> topWallMeshes = new() { 79, 80, 81, 82, 83, 84, 85, 86, 108, 110 };
        static readonly public List<int> leftWallMeshes = new() { 71, 72, 73, 74, 75, 76, 77, 78, 107, 109 };



        static public Dictionary<int, List<TileVertex>> meshes = ReadMeshType();

        static public List<TileVertex> VerticiesFromID(int id) {

            return new List<TileVertex> (meshes[id]);

        }

        static public int? IDFromVerticies(List<TileVertex> verticies) {

            foreach (var mesh in meshes) {

                if (verticies.Count != mesh.Value.Count) {
                    continue;
                }

                var found = true;
                var index = 0;
                foreach (var vertex in mesh.Value) {

                    if (!mesh.Value.Contains(verticies[index])) {
                        found = false;
                        break;
                    }

                    index++;
                }

                if (found) {
                    return mesh.Key;
                }

            }

            return null;

        }


        static public void MeshTypesToFile() {

            string total = "";

            foreach (int type in Enumerable.Range(0,111)) {

                // type: [(heightChannel,vertexPosition)]

                total += type.ToString() + ": [";
                var verticies = VerticiesFromID(type);

                foreach (var vertex in verticies) {
                    total += "(" + vertex.heightChannel + "," + (int)vertex.vertexPosition + ")";
                }

                total += "]\n";

            }

            File.WriteAllText("MeshData.txt", total);

        }

        static public Dictionary<int, List<TileVertex>> ReadMeshType() {

            var total = new Dictionary<int, List<TileVertex>>();

            string fileText = File.ReadAllText("MeshData.txt");

            string value = "";
            bool insideArray = false;
            var openedObject = new List<int>();

            foreach (var c in fileText) {

                if (insideArray) {

                    if (c == ']') {
                        insideArray = false;
                    }

                    else if (c == ',') {
                        openedObject.Add(Int32.Parse(value));
                        value = "";
                    }

                    else if (c == ')') {
                        openedObject.Add(Int32.Parse(value));
                        value = "";
                        total.Last().Value.Add(new TileVertex(openedObject[0], (VertexPosition)openedObject[1]));
                        openedObject = new List<int>();
                    }

                    else if (c != '(' && c != ' ') {
                        value += c;
                    }

                }

                else if (c == ':') {
                    total[Int32.Parse(value)] = new List<TileVertex>();
                    value = "";
                }

                else if (c == '[') {
                    insideArray = true;
                }

                else if (c != ' ' && c != '\n') {
                    value += c;
                }

            }

            return total;

        }

    }

}