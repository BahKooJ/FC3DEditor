

using System.Collections.Generic;
using UnityEngine;

public class TileEffectsView : MonoBehaviour {

    // - View Refs -
    public List<TileEffectChannelView> channelViewItems;

    public TileEditMode controller;

    public bool refuseCallbacks = false;

    void Start() {

        foreach (var channel in channelViewItems) {
            channel.parentView = this;
            channel.CreateDropdown();
        }

        Refresh();

    }

    public void Refresh() {

        refuseCallbacks = true;

        if (!controller.HasSelection) {
            controller.view.CloseTileEffectsPanel();
            return;
        }

        foreach (var item in channelViewItems) {

            item.UnCheck();

            item.SetDropdown(controller.FirstItem.section.section.tileEffects[item.channel]);

        }

        channelViewItems[controller.FirstTile.effectIndex].Check();

        refuseCallbacks = false;

    }

}