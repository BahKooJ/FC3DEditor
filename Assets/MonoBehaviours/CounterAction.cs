

using FCopParser;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public interface CounterAction {

    public void Action();

}

public class TileSaveStateCounterAction : CounterAction {

    Tile saveStateTile;
    Tile tile;
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