

using System;
using System.Collections.Generic;
using FCopParser;

public class ActorPropertyChangeEvent {

    static void RefreshActorEditPosition(ActorEditMode controller, ActorProperty prop) {

        foreach (var node in controller.actorEditingNodes) {

            if (node.controlledProperties.Contains(prop)) {

                node.SetToPosition();

                if (controller.selectedActorEditingNode != null) {
                    controller.selectedActorEditingNode.RefreshPosition();
                }

                break;

            }

        }

    }

    static void RefreshActorEditingNodes(ActorEditMode controller) {

        foreach (var node in controller.actorEditingNodes) {
            
            node.Refresh();

        }

    }

    static void RefreshActor(ActorEditMode controller) {

        controller.actorObjectsByID[controller.selectedActor.DataID].Refresh();

    }

    public static Dictionary<string, Action<ActorEditMode, ActorProperty>> changeEventsByPropertyName = new() {
        {"Height Offset", (controller, prop) => { 
            
            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.SetToCurrentPosition();
                controller.selectedActorObject.RefreshPosition();

            }

        } },
        {"1st Height Offset", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.SetToCurrentPosition();
                controller.selectedActorObject.RefreshPosition();

            }

        } },
        {"Ground Cast", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.SetToCurrentPosition();
                controller.selectedActorObject.RefreshPosition();

            }

        } },
        {"Rotation", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Rotation X", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Rotation Y", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Rotation Z", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Base Rotation", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Head Rotation", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Scale X", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Scale Y", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Scale Z", (controller, prop) => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"X", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Y", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Width Area", (controller, prop) => {

            RefreshActorEditingNodes(controller);

        } },
        {"Length Area", (controller, prop) => {

            RefreshActorEditingNodes(controller);

        } },
        {"Height Area", (controller, prop) => {

            RefreshActorEditingNodes(controller);

        } },
        {"Width", (controller, prop) => {

            RefreshActor(controller);

        } },
        {"Height", (controller, prop) => {

            RefreshActor(controller);

        } },
        {"Transparent", (controller, prop) => {

            RefreshActor(controller);

        } },
        {"Additive", (controller, prop) => {

            RefreshActor(controller);

        } },
        {"Red", (controller, prop) => {

            RefreshActor(controller);

        } },
        {"Green", (controller, prop) => {

            RefreshActor(controller);

        } },
        {"Blue", (controller, prop) => {

            RefreshActor(controller);

        } },
        {"Texture Snippet", (controller, prop) => {

            RefreshActor(controller);

        } },
        {"Thruster Behavior Override", (controller, prop) => {

            RefreshActor(controller);

        } },
        #region MapNodes
        {"Node 1 X", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 1 Y", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 2 X", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 2 Y", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 3 X", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 3 Y", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 4 X", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 4 Y", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 5 X", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 5 Y", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 6 X", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 6 Y", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 7 X", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 7 Y", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 8 X", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        {"Node 8 Y", (controller, prop) => {

            RefreshActorEditPosition(controller, prop);

        } },
        #endregion
    };

}