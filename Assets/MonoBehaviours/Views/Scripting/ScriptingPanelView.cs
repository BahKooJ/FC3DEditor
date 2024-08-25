
using FCopParser;
using UnityEngine;

public class ScriptingPanelView : MonoBehaviour {


    // - Unity Refs -
    public Transform scriptListContent;
    public VisualScriptingWindowView scriptingWindow;

    // - Prefabs -
    public GameObject ScriptListItem;

    public bool isScriptTab = true;
    public int actorRPNSRefIndex;
    public FCopActor actor;
    public FCopLevel level;

    void Start() {

        if (isScriptTab) {

            foreach (var script in level.scripting.rpns.code) {

                var listItem = Instantiate(ScriptListItem);

                listItem.gameObject.SetActive(true);

                var listItemScript = listItem.GetComponent<ScriptingButtonItemView>();

                listItemScript.view = this;
                listItemScript.value = script.Key;

                listItem.transform.SetParent(scriptListContent, false);

            }

        }

    }

    public void SelectScript(FCopScript script) {

        scriptingWindow.script = script;
        scriptingWindow.Init();

    }

    public void OnDone() {
        Destroy(gameObject);
    }

    public void OnApply() {

    }

}