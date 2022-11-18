
using UnityEngine;

class MainUI: MonoBehaviour {

    public Main controller;

    public GameObject graphicsPropertiesView;

    public GameObject activeGraphicsPropertiesView = null;

    public void OnClickGraphicsProperitesButton() {

        if (controller.selectedTile == null) { return; }

        if (activeGraphicsPropertiesView != null) {
            Destroy(activeGraphicsPropertiesView);
        } else {

            activeGraphicsPropertiesView = Instantiate(graphicsPropertiesView);

            activeGraphicsPropertiesView.GetComponent<GraphicsPropertiesView>().controller = controller;

            activeGraphicsPropertiesView.transform.SetParent(transform.parent, false);

        }

    }

}

