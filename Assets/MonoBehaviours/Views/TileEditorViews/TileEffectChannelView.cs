

using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileEffectChannelView : MonoBehaviour {

    // - View Refs -
    public Toggle toggle;
    public TMP_Dropdown dropdown;
    public int channel;

    // - Pars -
    public TileEffectsView parentView;

    void Start() {



    }

    public void CreateDropdown() {

        dropdown.ClearOptions();

        foreach (var i in Enumerable.Range(0, byte.MaxValue)) {

            dropdown.options.Add(new TMP_Dropdown.OptionData(i.ToString()));

        }

    }

    public void Check() {

        toggle.isOn = true;

    }

    public void UnCheck() {

        toggle.isOn = false;

    }

    public void OnToggle() {

        if (parentView.refuseCallbacks) {
            return;
        }

        parentView.controller.ChangeTileEffectIndex(channel);

        parentView.Refresh();

    }

    public void OnChangeDropdown() {

        if (parentView.refuseCallbacks) {
            return;
        }

        parentView.controller.ChaneTileEffect(channel, dropdown.value);

    }

}