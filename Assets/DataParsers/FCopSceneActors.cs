

using System.Collections.Generic;

namespace FCopParser {

    public class FCopSceneActors {

        public List<FCopActor> actors;

        public Dictionary<(int x, int y, int z), ActorNode> positionalGroupedActors = new();
        public Dictionary<int, ActorNode> behaviorGroupedActors = new();
        public List<ActorNode> scriptingGroupedActors;

        public FCopSceneActors(List<FCopActor> actors) {
            this.actors = actors;
            SortActorsByPosition();
            SortActorsByBehavior();
        }

        public void DeleteActor(FCopActor actor) {

            actors.Remove(actor);

            // TODO: Forgot this was a thing tbh, though I think it's better to remove the actual raw file.
            actor.rawFile.ignore = true;

        }

        public void SortActorsByPosition() {

            foreach (var actor in actors) { 

                if (positionalGroupedActors.ContainsKey((actor.x, actor.y, 0))) {
                    positionalGroupedActors[(actor.x, actor.y, 0)].nestedActors.Add(actor);
                    positionalGroupedActors[(actor.x, actor.y, 0)].name = "Group";
                }
                else {
                    positionalGroupedActors[(actor.x, actor.y, 0)] = new ActorNode(ActorGroupType.Position, actor.name, actor);
                }

            }

        }

        public void SortActorsByBehavior() {

            foreach (var actor in actors) {

                if (behaviorGroupedActors.ContainsKey(actor.actorType)) {
                    behaviorGroupedActors[actor.actorType].nestedActors.Add(actor);
                }
                else {
                    behaviorGroupedActors[actor.actorType] = new ActorNode(ActorGroupType.Position, "Group " + actor.actorType, actor);
                }

            }

        }

        public void Compile() {
            foreach (var actor in actors) {
                actor.Compile();
            }
        }

    }

    public class ActorNode {

        public List<FCopActor> nestedActors = new();
        public ActorGroupType groupType;
        public string name;
        public bool isNested = false;

        public ActorNode(ActorGroupType groupType, string name, FCopActor actor) {
            this.groupType = groupType;
            this.name = name;
            nestedActors.Add(actor);
        }
    }

    public enum ActorGroupType {
        Position = 0,
        Behavior = 1,
        Script = 2
    }

}