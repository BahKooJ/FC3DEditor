
using FCopParser;
using UnityEngine;

public class GlobalExplosionSelectorItemView : MonoBehaviour {

    // - Unity Refs -
    public ExplosionSelectorView view;
    public int id;

    public void OnClick() {

        view.Select(id);

    }

}