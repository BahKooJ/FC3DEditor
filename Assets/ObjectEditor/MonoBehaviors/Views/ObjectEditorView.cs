

using FCopParser;
using UnityEngine;

public class ObjectEditorView : MonoBehaviour {

    // - Prefabs -
    public GameObject ObjectPropertiesPanelFab;
    public GameObject PrimitivePropertyPanelFab;

    // - Unity View Refs -
    public Transform inspectorContent;

    // - Parameters -
    public ObjectEditorMain main;

    PrimitivePropertyPanelView primitivePropertyView;

    public void Init() {

        var gobj = Instantiate(ObjectPropertiesPanelFab);
        gobj.transform.SetParent(inspectorContent, false);
        var view = gobj.GetComponent<ObjectPropertiesPanelView>();
        view.main = main;

    }

    public void RefreshPrimitivePropertyView(FCopObject.Primitive primitive) {

        if (primitivePropertyView == null) {

            var gobj = Instantiate(PrimitivePropertyPanelFab);
            gobj.transform.SetParent(inspectorContent, false);
            var view = gobj.GetComponent<PrimitivePropertyPanelView>();
            view.main = main;
            view.primitive = primitive;
            primitivePropertyView = view;

        }
        else {

            primitivePropertyView.primitive = primitive;
            primitivePropertyView.Refresh();

        }

    }

}