

using System;
using System.Collections.Generic;

public class ActorPropertyChangeEvent {

    public static Dictionary<string, Action<ActorEditMode>> changeEventsByPropertyName = new() {
        {"Height Offset", controller => { 
            
            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.SetToCurrentPosition();

            }

        } },
        {"1st Height Offset", controller => {

            if (controller.selectedActorObject != null) {

                var aObj = controller.actorObjectsByID[controller.selectedActor.DataID];

                aObj.SetToCurrentPosition();

            }

        } },
    };


}