


using TMPro;
using UnityEngine;

public class DebugUVItemView : MonoBehaviour {

    public TMP_Text text;

    public int index;
    public DebugTilePanelView view;

    public void OnClicked() {
        view.selectedIndex = index;
    }

}