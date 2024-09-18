
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
    public TMP_InputField nameField;
    public ContextMenuHandler contextMenu;
    public Image icon;
    public Image background;
    public GameObject pad;

    // - Parameters -
    public ActorNode node = null;
    public FCopActor actor = null;
    public ActorNode parent = null;
    public SceneActorsView view;
    public bool forceGroup = false;

    public List<ActorNodeListItemView> actorNodes = new();
    public Dictionary<int, ActorNodeListItemView> actorNodesByID = null;


    int clickTimer = 0;

    private void Start() {

        contextMenu.items = new() {
            ("Rename", Rename),
            ("Group", StartGroup),
            ("Ungroup", Ungroup),
            ("Delete", Delete)

        };

        actorNodeListItemFab = view.actorNodeListItemFab;

        if (node != null) {

            if (node.nestedActors.Count == 1) {
                nameText.text = node.nestedActors[0].name;
            }
            else {
                nameText.text = node.name;

            }

            if (node.nestedActors.Count > 1 || forceGroup) {
                icon.sprite = folderTexture;
            }

        }
        else {
            nameText.text = actor.name;

            pad.SetActive(true);
        }

        RefreshSelection(true);

    }

    private void Update() {
        
        if (clickTimer > 0) {
            clickTimer--;
        }

    }

    public void RefreshName() {

        if (node != null) {

            if (node.nestedActors.Count == 1) {
                nameText.text = node.nestedActors[0].name;
            }
            else {
                nameText.text = node.name;

            }
        }
        else {
            nameText.text = actor.name;
        }

    }

    public void RefreshSelection(bool jump) {

        if (node != null) {

            if (node.nestedActors.Count > 1) {
                background.color = Main.mainColor;
                return;
            }

        }

        if (forceGroup) {
            background.color = Main.mainColor;
            return; 
        }

        if (view.controller.selectedActor != null) {

            if (node != null) {

                if (view.controller.selectedActor.DataID == node.nestedActors[0].DataID) {
                    background.color = Main.selectedColor;

                    if (jump) {
                        JumpViewToListItem();
                    }

                    return;
                }

            }
            else {

                if (view.controller.selectedActor.DataID == actor.DataID) {
                    background.color = Main.selectedColor;

                    if (jump) {
                        JumpViewToListItem();
                    }

                    return;
                }

            }

        }

        background.color = Main.mainColor;

    }

    void JumpViewToListItem() {

        var normalizedPos = ((decimal)transform.GetSiblingIndex()) / ((decimal)transform.parent.childCount - 10);

        view.contentScrollview.verticalNormalizedPosition = (float)(1 - normalizedPos);

    }

    void Rename() {

        nameText.gameObject.SetActive(false);
        nameField.gameObject.SetActive(true);

        nameField.text = nameText.text;
        nameField.Select();

    }

    void Ungroup() {

        if (actor != null) {

            view.controller.UngroupActor(actor);

        }


    }

    void Delete() {

        if (node != null) {

            if (node.nestedActors.Count == 1 && !forceGroup) {
                view.controller.DeleteByID(node.nestedActors[0].DataID);
            }

        }
        else {

            view.controller.DeleteByID(actor.DataID);

        }

    }

    void StartGroup() {

        if (node != null) {

            if (node.nestedActors.Count == 1 && !forceGroup) {
                view.controller.StartGroup(node.nestedActors[0]);
            }

        }
        else {

            view.controller.StartGroup(actor);

        }

    }

    public void OpenGroup() {

        if (actorNodes.Count > 0) {

            foreach (var actorNode in actorNodes) {
                Destroy(actorNode.gameObject);
            }

            actorNodes.Clear();
            actorNodesByID = null;

        }
        else {

            actorNodesByID = new();

            foreach (var actor in node.nestedActors) {

                var obj = Instantiate(actorNodeListItemFab);
                obj.transform.SetParent(view.listContent, false);


                obj.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);

                var nodeListItem = obj.GetComponent<ActorNodeListItemView>();
                nodeListItem.actor = actor;
                nodeListItem.parent = node;
                nodeListItem.view = view;

                actorNodes.Add(nodeListItem);

                actorNodesByID[actor.DataID] = nodeListItem;

            }

        }

    }

    // - Unity Events -

    public void OnClick() {

        if (view.controller.actorToGroup != null) {

            if (forceGroup) {
                return;
            }

            if (node != null) {

                view.controller.GroupActor(node);

            }
            else {

                if (parent != null) {

                    view.controller.GroupActor(parent);

                }

            }

            return;

        }

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

            OpenGroup();

        }

        clickTimer = 30;

    }

    public void OnFinishNameType() {

        if (node != null) {

            if (node.nestedActors.Count == 1) {
                view.controller.RenameActor(node.nestedActors[0], nameField.text);
            }
            else {

                node.name = nameField.text;

                nameText.text = node.name;

            }
        }
        else {

            view.controller.RenameActor(actor, nameField.text);

        }

        nameText.gameObject.SetActive(true);
        nameField.gameObject.SetActive(false);
        Main.ignoreAllInputs = false;

    }

    public void OnStartNameType() {

        Main.ignoreAllInputs = true;

    }

}
