
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshUtils {

    // This code is... very verbose
    static public List<Vector3> FlatScreenVerticies(List<TileVertex> tileVertices, float meshSize) {

        var vertices = new List<Vector3>();

        var isWall = false;

        var lowestHeight = 10;
        var positions = new List<VertexPosition>();
        foreach (var vert in tileVertices) {

            if (lowestHeight > vert.heightChannel) {
                lowestHeight = vert.heightChannel;
            }

            if (positions.Contains(vert.vertexPosition)) {
                isWall = true;
            }

            positions.Add(vert.vertexPosition);

        }

        if (isWall) {

            var isTop = positions.Contains(VertexPosition.TopLeft) && positions.Contains(VertexPosition.TopRight);
            var isLeft = positions.Contains(VertexPosition.TopLeft) && positions.Contains(VertexPosition.BottomLeft);
            var isDiagnal =
                positions.Contains(VertexPosition.TopLeft) && positions.Contains(VertexPosition.BottomRight) ||
                positions.Contains(VertexPosition.BottomLeft) && positions.Contains(VertexPosition.TopRight);


            foreach (var point in tileVertices) {

                switch (point.vertexPosition) {

                    case VertexPosition.TopLeft:

                        if (point.heightChannel == lowestHeight) {

                            vertices.Add(new Vector3(0, -meshSize));

                        }
                        else {

                            vertices.Add(new Vector3(x: 0, y: 0));

                        }

                        break;
                    case VertexPosition.TopRight:

                        if (point.heightChannel == lowestHeight) {

                            vertices.Add(new Vector3(meshSize, -meshSize));

                        }
                        else {

                            vertices.Add(new Vector3(meshSize, 0));

                        }

                        break;
                    case VertexPosition.BottomLeft:

                        if (isDiagnal) {

                            if (point.heightChannel == lowestHeight) {

                                vertices.Add(new Vector3(0, -meshSize));

                            }
                            else {

                                vertices.Add(new Vector3(x: 0, y: 0));

                            }

                        }
                        else {

                            if (point.heightChannel == lowestHeight) {

                                vertices.Add(new Vector3(meshSize, -meshSize));

                            }
                            else {

                                vertices.Add(new Vector3(meshSize, 0));

                            }

                        }

                        break;
                    case VertexPosition.BottomRight:

                        if (point.heightChannel == lowestHeight) {

                            vertices.Add(new Vector3(meshSize, -meshSize));

                        }
                        else {

                            vertices.Add(new Vector3(meshSize, 0));

                        }

                        break;
                }

            }

            return vertices;

        }

        foreach (var point in tileVertices) {

            switch (point.vertexPosition) {

                case VertexPosition.TopLeft:
                    vertices.Add(new Vector3(x: 0, y: 0));

                    break;
                case VertexPosition.TopRight:
                    vertices.Add(new Vector3(meshSize, 0));

                    break;
                case VertexPosition.BottomLeft:
                    vertices.Add(new Vector3(0, -meshSize));

                    break;
                case VertexPosition.BottomRight:
                    vertices.Add(new Vector3(meshSize, -meshSize));

                    break;
            }

        }

        return vertices;

    }

}