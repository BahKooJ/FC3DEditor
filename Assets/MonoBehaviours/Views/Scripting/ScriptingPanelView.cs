
using FCopParser;
using UnityEngine;

public class ScriptingPanelView : MonoBehaviour {


    // View refs
    public Transform scriptListContent;

    // View refs prefabs
    public GameObject ScriptListItem;

    public bool isScriptTab = true;
    public int actorRPNSRefIndex;
    public FCopActor actor;
    public FCopLevel level;

    void Start() {

        //if (isScriptTab) {

        //    foreach (var script in level.rpns.code) {

        //        var listItem = Instantiate(ScriptListItem);

        //        listItem.gameObject.SetActive(true);

        //        var listItemScript = listItem.GetComponent<ScriptingButtonItemView>();

        //        listItemScript.view = this;
        //        listItemScript.value = script.offset;

        //        listItem.transform.SetParent(scriptListContent, false);

        //    }


        //}
        


    }

    public void OnDone() {
        Destroy(gameObject);
    }

    public void OnApply() {

    }

}