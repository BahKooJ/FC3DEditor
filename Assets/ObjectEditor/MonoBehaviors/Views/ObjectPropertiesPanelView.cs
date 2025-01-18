

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

    void Start() {

        nameText.text = ObjectEditorMain.fCopObject.name;

        pos0IndexText.text = PositionString(ObjectEditorMain.fCopObject.positions[0]);
        pos1IndexText.text = PositionString(ObjectEditorMain.fCopObject.positions[1]);
        pos2IndexText.text = PositionString(ObjectEditorMain.fCopObject.positions[2]);
        pos3IndexText.text = PositionString(ObjectEditorMain.fCopObject.positions[3]);


    }

    public void OnClickSetPos0() {

        HeadsUpTextUtil.HeadsUp("Select Vertex for Pos 0");

        main.requestedVertexActionCallback = v => {

            ObjectEditorMain.fCopObject.positions[0] = (byte)v.index;
            pos0IndexText.text = ObjectEditorMain.fCopObject.positions[0].ToString();
            HeadsUpTextUtil.End();
            main.ClearVertexCallback();

        };

    }

    public void OnClickSetPos1() {

        HeadsUpTextUtil.HeadsUp("Select Vertex for Pos 1");

        main.requestedVertexActionCallback = v => {

            ObjectEditorMain.fCopObject.positions[1] = (byte)v.index;
            pos1IndexText.text = ObjectEditorMain.fCopObject.positions[1].ToString();
            HeadsUpTextUtil.End();
            main.ClearVertexCallback();

        };

    }

    public void OnClickSetPos2() {

        HeadsUpTextUtil.HeadsUp("Select Vertex for Pos 2");

        main.requestedVertexActionCallback = v => {

            ObjectEditorMain.fCopObject.positions[2] = (byte)v.index;
            pos2IndexText.text = ObjectEditorMain.fCopObject.positions[2].ToString();
            HeadsUpTextUtil.End();
            main.ClearVertexCallback();

        };

    }

    public void OnClickSetPos3() {

        HeadsUpTextUtil.HeadsUp("Select Vertex for Pos 3");

        main.requestedVertexActionCallback = v => {

            ObjectEditorMain.fCopObject.positions[3] = (byte)v.index;
            pos3IndexText.text = ObjectEditorMain.fCopObject.positions[3].ToString();
            HeadsUpTextUtil.End();
            main.ClearVertexCallback();

        };

    }

    public void OnClickReset0() {

        ObjectEditorMain.fCopObject.positions[0] = 255;
        pos0IndexText.text = PositionString(ObjectEditorMain.fCopObject.positions[0]);

    }

    public void OnClickReset1() {

        ObjectEditorMain.fCopObject.positions[1] = 255;
        pos1IndexText.text = PositionString(ObjectEditorMain.fCopObject.positions[1]);

    }

    public void OnClickReset2() {

        ObjectEditorMain.fCopObject.positions[2] = 255;
        pos2IndexText.text = PositionString(ObjectEditorMain.fCopObject.positions[2]);

    }

    public void OnClickReset3() {

        ObjectEditorMain.fCopObject.positions[3] = 255;
        pos3IndexText.text = PositionString(ObjectEditorMain.fCopObject.positions[3]);

    }

    string PositionString(byte pos) {

        var total = pos.ToString();

        if (pos == 255) {
            total += " (N/A)";
        }

        return total;

    }

}