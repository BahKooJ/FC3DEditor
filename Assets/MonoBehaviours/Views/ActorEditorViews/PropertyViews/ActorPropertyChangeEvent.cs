

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