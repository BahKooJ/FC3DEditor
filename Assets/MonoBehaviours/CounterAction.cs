

using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditorInternal;

// WARNING: FOR ADDITIONAL ACTIONS DO NOT REFERENCE ANYTHING IN THE CLASS,
// THIS WILL CAUSE MEMEORY LEAKS AND DOUBLE EDITMODES!
public interface CounterAction {

    public void Action();

}

public class TileSaveStateCounterAction : CounterAction {

    Tile saveStateTile;
    public Tile tile;
    Action additionalAction;

    public TileSaveStateCounterAction(Tile tile, Action additionalAction) {
        this.tile = tile;
        this.saveStateTile = tile.Clone();

        this.additionalAction = additionalAction;
    }

    public void Action() {
        tile.ReceiveData(saveStateTile);
        additionalAction();
    }

}

public class MultiTileSaveStateCounterAction : CounterAction {

    List<TileSaveStateCounterAction> saveStateTiles = new();
    Action additionalAction;

    public MultiTileSaveStateCounterAction(List<TileSelection> items, Action additionalAction) {

        foreach (var item in items) {
            saveStateTiles.Add(new TileSaveStateCounterAction(item.tile, () => { }));
        }

        this.additionalAction = additionalAction;
    }

    public void Action() {

        foreach (var state in saveStateTiles) {
            state.Action();
        }

        additionalAction();
    }

}

public class SelectionSaveStateCounterAction : CounterAction {

    HashSet<LevelMesh> selectedSections = new();
    List<TileSelection> selectedItems = new();
    Action additionalAction;

    public SelectionSaveStateCounterAction(TileMutatingEditMode editMode, Action additionalAction) {

        this.selectedSections = new(editMode.selectedSections);
        this.selectedItems = new(editMode.selectedItems);
        this.additionalAction = additionalAction;

    }

    public void Action() {

        if (Main.editMode is TileMutatingEditMode) {

            var editMode = (TileMutatingEditMode)Main.editMode;

            editMode.selectedItems = this.selectedItems;
            editMode.selectedSections = this.selectedSections;

            additionalAction();

        }
        
    }

}

public class HeightMapSaveStateCounterAction : CounterAction {

    HeightPoints savedHeights;
    HeightPoints heights;
    Action additionalAction;

    public HeightMapSaveStateCounterAction(HeightPoints heights, Action additionalAction) {

        savedHeights = heights.Clone();
        this.heights = heights;
        this.additionalAction = additionalAction;

    }

    public void Action() {

        heights.ReceiveData(savedHeights);
        additionalAction();

    }

}

public class MultiHeightMapSaveStateCounterAction : CounterAction {

    List<HeightMapSaveStateCounterAction> saveStateHeights = new();
    Action additionalAction;

    public MultiHeightMapSaveStateCounterAction(List<HeightPoints> multiHeights, Action additionalAction) {

        foreach (var heights in multiHeights) {
            saveStateHeights.Add(new HeightMapSaveStateCounterAction(heights, () => { }));
        }

        this.additionalAction = additionalAction;
    }

    public void Action() {

        foreach (var state in saveStateHeights) {
            state.Action();
        }

        additionalAction();
    }

}

public class NonSelectiveMultiTileSaveStateCounterAction : CounterAction {

    HashSet<LevelMesh> levelMeshes = new HashSet<LevelMesh>();
    List<TileSaveStateCounterAction> saveStateTiles = new();
    Action additionalAction;

    public NonSelectiveMultiTileSaveStateCounterAction(List<TileSelection> items, Action additionalAction) {

        foreach (var item in items) {
            saveStateTiles.Add(new TileSaveStateCounterAction(item.tile, () => { }));
            levelMeshes.Add(item.section);
        }

        this.additionalAction = additionalAction;
    }

    public NonSelectiveMultiTileSaveStateCounterAction(TileSelection item, Action additionalAction) {

        saveStateTiles.Add(new TileSaveStateCounterAction(item.tile, () => { }));
        levelMeshes.Add(item.section);


        this.additionalAction = additionalAction;

    }

    public void Action() {

        foreach (var state in saveStateTiles) {
            state.Action();
        }

        foreach (var mesh in levelMeshes) {
            mesh.RefreshMesh();
        }

        additionalAction();
    }

    public void AddTileSaveState(TileSelection item) {

        var doesExist = saveStateTiles.FirstOrDefault(itItem => { return itItem.tile == item.tile; });

        if (doesExist == null) {

            saveStateTiles.Add(new TileSaveStateCounterAction(item.tile, () => { }));
            levelMeshes.Add(item.section);

        }

    }

}

