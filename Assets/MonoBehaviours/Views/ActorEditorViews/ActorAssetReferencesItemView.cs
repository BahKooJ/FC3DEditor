

using FCopParser;
using TMPro;
using UnityEngine;

public class ActorAssetReferencesItemView : MonoBehaviour {

    // - View Refs -
    public TMP_Text text;

    // - Parameters -
    public FCopActor.Resource resourceRef;

    private void Start() {

        text.text = resourceRef.fourCC + " " + resourceRef.id;

    }

}