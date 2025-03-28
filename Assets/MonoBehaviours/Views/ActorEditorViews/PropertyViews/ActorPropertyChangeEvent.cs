

using System;
using System.Collections.Generic;

public class ActorPropertyChangeEvent {

    public static Dictionary<string, Action<ActorEditMode>> changeEventsByPropertyName = new() {
        {"Height Offset", controller => { 
            
            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.SetToCurrentPosition();
                controller.selectedActorObject.RefreshPosition();

            }

        } },
        {"1st Height Offset", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.SetToCurrentPosition();
                controller.selectedActorObject.RefreshPosition();

            }

        } },
        {"Ground Cast", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.SetToCurrentPosition();
                controller.selectedActorObject.RefreshPosition();

            }

        } },
        {"Rotation", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Rotation X", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Rotation Y", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Rotation Z", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Base Rotation", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Scale X", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Scale Y", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
        {"Scale Z", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.RefreshRotation();

            }

        } },
    };

}