

using FCopParser;
using System.Linq;
using TMPro;
using UnityEngine;

public class ActorAssetReferencesItemView : MonoBehaviour {

    // - View Refs -
    public TMP_Text resourceName;
    public TMP_Text assetName;

    // - Parameters -
    [HideInInspector]
    public ActorAssetReferencesView view;
    public Main main;
    public FCopActor fcopActor;
    [HideInInspector]
    public int refIndex;

    private void Start() {

        Refresh();

    }

    void Refresh() {

        var resourceRef = fcopActor.resourceReferences[refIndex];

        switch (resourceRef.fourCC) {

            case FCopActor.FourCC.Cobj:
                var obj = main.level.objects.FirstOrDefault(o => o.DataID == resourceRef.id);

                if (obj != null) {
                    assetName.text = obj.name;
                }
                else {
                    assetName.text = "Missing";
                }

                break;
            case FCopActor.FourCC.Cnet:

                var net = main.level.navMeshes.FirstOrDefault(o => o.DataID == resourceRef.id);

                if (net != null) {
                    assetName.text = net.name;
                }
                else {
                    assetName.text = "Missing";
                }

                break;
            case FCopActor.FourCC.NULL:
                assetName.text = "None";
                break;

        }

        if (fcopActor.behavior?.assetRefNames == null) {
            resourceName.text = "Resource " + refIndex + ":";
        }
        else {
            resourceName.text = fcopActor.behavior.assetRefNames[refIndex] + ":";
        }

    }

    public void OnClick() {

        if (fcopActor.behavior?.assetRefType == null) {
            return;
        }

        var resourceRef = fcopActor.resourceReferences[refIndex];
        var type = fcopActor.behavior.assetRefType[refIndex];

        switch (type) {

            case AssetType.Object:

                MiniAssetManagerUtil.RequestAsset(AssetType.Object, main, asset => {

                    if (asset == null) {
                        fcopActor.resourceReferences[refIndex] = new FCopActor.Resource(FCopActor.FourCC.NULL, 0);
                    }
                    else {
                        fcopActor.resourceReferences[refIndex] = new FCopActor.Resource(FCopActor.FourCC.Cobj, asset.DataID);
                    }


                    Refresh();

                    view.controller.UnselectActor();
                    view.controller.RequestActorRefresh(fcopActor.DataID);
                    view.controller.SelectActorByID(fcopActor.DataID);

                });

                break;
            case AssetType.NavMesh:

                MiniAssetManagerUtil.RequestAsset(AssetType.NavMesh, main, asset => {

                    if (asset == null) {
                        fcopActor.resourceReferences[refIndex] = new FCopActor.Resource(FCopActor.FourCC.NULL, 0);
                    }
                    else {
                        fcopActor.resourceReferences[refIndex] = new FCopActor.Resource(FCopActor.FourCC.Cnet, asset.DataID);
                    }


                    Refresh();

                });

                break;

        }

    }

}