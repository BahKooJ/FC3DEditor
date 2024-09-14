

using System.Collections.Generic;

namespace FCopParser {

    public class FCopSceneActors {

        public List<FCopActor> actors;

        public List<ActorNode> positionalGroupedActors;
        public List<ActorNode> behaviorGroupedActors;
        public List<ActorNode> scriptingGroupedActors;

        public FCopSceneActors(List<FCopActor> actors) {
            this.actors = actors;
        }

        public void DeleteActor(FCopActor actor) {

            actors.Remove(actor);

            // TODO: Forgot this was a thing tbh, though I think it's better to remove the actual raw file.
            actor.rawFile.ignore = true;

        }

        public void Compile() {
            foreach (var actor in actors) {
                actor.Compile();
            }
        }

    }

    public class ActorNode {

        public List<FCopActor> nestedActors;
        public ActorGroupType groupType;
        public string name;

    }

    public enum ActorGroupType {
        Position,
        Behavior,
        Script
    }

}