
using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActorNodeListItemView : MonoBehaviour {

    // - Prefab -
    GameObject actorNodeListItemFab;

    // - Unity Asset -
    public Sprite folderTexture;

    // - Unity Refs -
    public TMP_Text nameText;
    public Image icon;
    public GameObject pad;

    // - Parameters -
    public ActorNode node = null;
    public FCopActor actor = null;
    public SceneActorsView view;
    public bool forceGroup = false;

    public List<ActorNodeListItemView> actorNodes = new();

    int clickTimer = 0;

    private void Start() {

        actorNodeListItemFab = view.actorNodeListItemFab;

        if (node!= null ) {
            nameText.text = node.name;

            if (node.nestedActors.Count > 1 || forceGroup) {
                icon.sprite = folderTexture;
            }

        }
        else {
            nameText.text = actor.name;

            pad.SetActive(true);
        }

    }

    private void Update() {
        
        if (clickTimer > 0) {
            clickTimer--;
        }

    }

    public void OnClick() {

        if (actor != null) {

            view.controller.SelectActorByID(actor.DataID);

            if (clickTimer != 0) {
                view.controller.MoveToActor(actor.DataID);
            }

            clickTimer = 30;

            return;

        }

        if (node.nestedActors.Count == 1 && !forceGroup) {

            view.controller.SelectActorByID(node.nestedActors[0].DataID);

            if (clickTimer != 0) {
                view.controller.MoveToActor(node.nestedActors[0].DataID);
            }

        }
        else {

            if (actorNodes.Count > 0) {

                foreach (var actorNode in actorNodes) {
                    Destroy(actorNode.gameObject);
                }

                actorNodes.Clear();

            }
            else {

                var i = 0;
                foreach (var actor in node.nestedActors) {

                    var obj = Instantiate(actorNodeListItemFab);
                    obj.transform.SetParent(view.listContent, false);


                    obj.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);

                    var nodeListItem = obj.GetComponent<ActorNodeListItemView>();
                    nodeListItem.actor = actor;
                    nodeListItem.view = view;

                    actorNodes.Add(nodeListItem);

                    i++;

                }

            }


        }

        clickTimer = 30;

    }


}
