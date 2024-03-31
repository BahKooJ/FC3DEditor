

using FCopParser;
using System;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

public class ShaderDebug : MonoBehaviour {

    public TileEditMode controller;

    // View refs
    public TMP_Text graphicsTypeText;
    public TMP_InputField shaderDataText;
    public TMP_InputField extraShaderDataText;

    void Start() {
        Refresh();
    }

    public void Refresh() {

        if (TileEditMode.selectedTiles.Count == 0) {
            return;
        }

        //graphicsTypeText.text = TileEditMode.selectedTiles[0].graphics.graphicsType.ToString();
        //shaderDataText.text = TileEditMode.selectedTiles[0].graphics.lightingInfo.ToString();

        var total = "";

        //if (TileEditMode.selectedTiles[0].graphics.graphicsType == 1) {
        //    foreach (var i in ((DynamicMonoChromeShader)TileEditMode.selectedTiles[0].shaders).values) {
        //        total += i.ToString() + " ";
        //    }
        //}

        //foreach (var meta in TileEditMode.selectedTiles[0].graphicsMetaData) {

        //    foreach (var b in meta.data) {
        //        total += b.ToString() + " ";
        //    }

        //}

        extraShaderDataText.text = total;

    }

    public void FinishShaderData() {
        //TileEditMode.selectedTiles[0].graphics.lightingInfo = Int32.Parse(shaderDataText.text);
    }

    public void FinishChromeData() {

        return;

        //if (TileEditMode.selectedTiles[0].graphics.graphicsType != 1) {
        //    TileEditMode.selectedTiles[0].shaders = new FCopParser.TileShaders(new int[] { 0,0,0,0 });
        //    TileEditMode.selectedTiles[0].graphics.graphicsType = 1;
        //}

        //var num = "";
        //var i = 0;
        //foreach (var c in extraShaderDataText.text) {

        //    if (c == ' ') {
        //        TileEditMode.selectedTiles[0].shaders.monoChrome[i] = Int32.Parse(num);
        //        num = "";
        //        i++;
        //    } else {
        //        num += c;
        //    }

        //}

        //if (num != "") {
        //    TileEditMode.selectedTiles[0].shaders.monoChrome[i] = Int32.Parse(num);
        //}

    }

    public void Experiment() {

        //var i = 0;
        //var nextCorner = 0;
        //foreach (var column in TileEditMode.selectedSection.section.tileColumns) {
        //    column.tiles[0].shaders = new FCopParser.TileShaders(new int[] { 3, 3, 3, 3 });
        //    column.tiles[0].graphics.graphicsType = 1;
        //    column.tiles[0].shaders.monoChrome[nextCorner] = i;
        //    i++;

        //    if (i == 64) {
        //        nextCorner++;
        //        i = 0;
        //    }

        //}

    }

}