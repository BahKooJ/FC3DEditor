
using FCopParser;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugTilePanelView : MonoBehaviour {

    //Prefabs
    public GameObject debugUVItemView;

    public TileEditMode controller;

    public TMP_Text meshIDText;
    public Transform content;
    public TMP_InputField meshField;

    public List<DebugUVItemView> debugUVItems;

    List<TileVertex> verticies;
    int id;
    Tile tile;

    public int selectedIndex = -1;

    void Update() {
        
        if (selectedIndex != -1) {

            if (Input.GetKeyUp(KeyCode.UpArrow)) {

                var vertex = verticies[selectedIndex];

                TileVertex nextVertex;

                if (selectedIndex == 0) {
                    nextVertex = verticies[verticies.Count - 1];

                    verticies[0] = nextVertex;
                    verticies[verticies.Count - 1] = vertex;

                } else {

                    nextVertex = verticies[selectedIndex - 1];

                    verticies[selectedIndex] = nextVertex;
                    verticies[selectedIndex - 1] = vertex;

                }

                MeshType.meshes[id] = new(verticies);
                tile.verticies = verticies;

                Refresh();
                TileEditMode.selectedSection.RefreshMesh();

            }
            if (Input.GetKeyUp(KeyCode.UpArrow)) {

            }


        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            MeshType.MeshTypesToFile();
        }

    }

    public void Refresh() {
        foreach (var item in debugUVItems) {
            Destroy(item.gameObject);
        }

        debugUVItems.Clear();

        if (MeshType.IDFromVerticies(tile.verticies) == null) {
            MeshType.meshes[id] = new(tile.verticies);
        }

        id = (int)MeshType.IDFromVerticies(tile.verticies);

        verticies = MeshType.VerticiesFromID(id);

        meshIDText.text = id.ToString();

        var i = 0;
        foreach (var vert in verticies) {

            var item = Instantiate(debugUVItemView);

            var script = item.GetComponent<DebugUVItemView>();

            script.text.text = vert.heightChannel + " " + vert.vertexPosition.ToString();
            script.index = i;
            script.view = this;

            debugUVItems.Add(script);

            item.transform.SetParent(content, false);

            i++;
        }
    }

    public void TileSelected(Tile tile) {
        this.tile = tile;
        Refresh();
    }

    public void FinishMeshFeild() {
        tile.culling = Int32.Parse(meshField.text);
        //TileEditMode.selectedSection.RefreshMesh();

    }


}