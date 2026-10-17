
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FCopParser {
    public abstract class MeshType {

        public static readonly List<int> quadMeshes = new() { 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 103, 104, 105, 106, 107, 108, 109, 110 };
        public static readonly List<int> wallMeshes = new() { 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110 };
        public static readonly List<int> topWallMeshes = new() { 79, 80, 81, 82, 83, 84, 85, 86, 108, 110 };
        public static readonly List<int> leftWallMeshes = new() { 71, 72, 73, 74, 75, 76, 77, 78, 107, 109 };
        public static readonly List<int> diagonalTLeftBRightWallMeshes = new() { 95, 96, 97, 98, 99, 100, 101, 102, 105, 106 };
        public static readonly List<int> diagonalBLeftTRightWallMeshes = new() { 87, 88, 89, 90, 91, 92, 93, 94, 103, 104 };

        public static readonly List<int> diagonalBLeftTRightQuadWallMeshes = new() { 103, 104 };
        public static readonly List<int> diagonalTLeftBRightQuadWallMeshes = new() { 105, 106 };
        public static readonly List<int> diagonalBLeftTRightTriWallMeshes = new() { 87, 88, 89, 90, 91, 92, 93, 94 };
        public static readonly List<int> diagonalTLeftBRightTriWallMeshes = new() { 95, 96, 97, 98, 99, 100, 101, 102 };



        public static readonly List<int> bottomRightTriangles = new() { 0, 2,  4, 16, 18, 25, 27, 33, 35, 36, 38, 44, 46, 53, 55 };
        public static readonly List<int> bottomLeftTriangles = new()  { 6, 8, 10, 12, 14, 21, 23, 28, 30, 41, 43, 48, 50, 56, 58 };

        public static readonly List<int> topRightTriangles = new()    { 7, 9, 11, 13, 15, 20, 22, 29, 31, 40, 42, 49, 51, 57, 59 };




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