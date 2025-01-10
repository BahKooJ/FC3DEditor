

using TMPro;
using UnityEngine;

public class ObjectPropertiesPanelView : MonoBehaviour {

    // - Unity View Refs -
    public TMP_Text nameText;

    public TMP_Text pos0IndexText;
    public TMP_Text pos1IndexText;
    public TMP_Text pos2IndexText;
    public TMP_Text pos3IndexText;

    // - Parameters -
    public ObjectEditorMain main;

    public void Init() {

        nameText.text = ObjectEditorMain.fCopObject.name;

        pos0IndexText.text = ObjectEditorMain.fCopObject.positions[0].ToString();
        pos1IndexText.text = ObjectEditorMain.fCopObject.positions[1].ToString();
        pos2IndexText.text = ObjectEditorMain.fCopObject.positions[2].ToString();
        pos3IndexText.text = ObjectEditorMain.fCopObject.positions[3].ToString();


    }

}