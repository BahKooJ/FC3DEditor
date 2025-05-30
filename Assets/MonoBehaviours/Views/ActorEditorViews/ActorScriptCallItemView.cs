

using FCopParser;
using TMPro;
using UnityEngine;

public class ActorScriptCallItemView : MonoBehaviour {

    //Prefabs
    public GameObject scriptingPanel;

    // View Refs
    public TMP_Text rpnsRefText1;
    public TMP_Text rpnsRefText2;
    public TMP_Text rpnsRefText3;

    public ActorEditMode controller;
    public FCopActor actor;

    void Start() {

        rpnsRefText1.text = actor.rawFile.rpnsReferences[0].ToString();
        rpnsRefText2.text = actor.rawFile.rpnsReferences[1].ToString();
        rpnsRefText3.text = actor.rawFile.rpnsReferences[2].ToString();

    }

    void OpenPanel(int index) {

        var view = Instantiate(scriptingPanel);

        var viewScript = view.GetComponent<ScriptingPanelView>();

        viewScript.level = FileManagerMain.level;

        view.transform.SetParent(DialogWindowUtil.canvas.transform, false);

        viewScript.SelectScript(actor.rawFile.rpnsReferences[index]);

    }

    public void OnClickRPNSRef1() {
        OpenPanel(0);
    }

    public void OnClickRPNSRef2() {
        OpenPanel(1);

    }

    public void OnClickRPNSRef3() {
        OpenPanel(2);

    }

}