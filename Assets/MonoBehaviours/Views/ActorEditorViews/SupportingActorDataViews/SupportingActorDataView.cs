
using FCopParser;
using UnityEngine;

public class SupportingActorDataView : MonoBehaviour {

    // - Unity Refs -
    public TeamDataView teamDataView;

    // - Parameters -
    public FCopLevel level;

    void Start() {
        
        teamDataView.level = level;

    }

    public void OnClickDone() {

        Destroy(gameObject);

    }

}