
using UnityEngine;

public class AddTileButton : MonoBehaviour {

    public TileAddPanel view;

    public int index;

    public void OnClick() {

        view.Select(index);

    }

}