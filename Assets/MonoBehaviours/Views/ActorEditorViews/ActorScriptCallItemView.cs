

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

        rpnsRefText1.text = actor.rpnsReferences[0].ToString();
        rpnsRefText2.text = actor.rpnsReferences[1].ToString();
        rpnsRefText3.text = actor.rpnsReferences[2].ToString();


    }

    public void OpenPanel() {

        var view = Instantiate(scriptingPanel);

        var viewScript = view.GetComponent<ScriptingPanelView>();

        viewScript.actor = actor;
        viewScript.level = FileManagerMain.level;

        view.transform.SetParent(DialogWindowUtil.canvas.transform, false);

    }

}