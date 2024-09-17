

using System;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    public class FCopSceneActors {

        public List<FCopActor> actors;
        public Dictionary<int, FCopActor> actorsByID = new();

        public List<ActorNode> positionalGroupedActors = new();
        public Dictionary<int, ActorNode> behaviorGroupedActors = new();
        public List<ActorNode> scriptingGroupedActors;

        public FCopSceneActors(List<FCopActor> actors) {

            this.actors = actors;

            foreach (var actor in actors) {
                actorsByID[actor.DataID] = actor;
            }

            SortActorsByPosition();
            SortActorsByBehavior();
        }

        public void DeleteActor(FCopActor actor) {

            actors.Remove(actor);

            // TODO: Forgot this was a thing tbh, though I think it's better to remove the actual raw file.
            actor.rawFile.ignore = true;

        }

        public bool PositionalUngroupActor(FCopActor actor) {

            var node = positionalGroupedActors.FirstOrDefault(n => {

                foreach (var nestedN in n.nestedActors) {

                    if (nestedN.DataID == actor.DataID) {
                        return true;
                    }

                }

                return false;

            });

            if (node == null) return false;

            var indexOfNode = positionalGroupedActors.IndexOf(node);
            var didRemove = node.nestedActors.Remove(actor);

            // Debug
            if (!didRemove) {
                throw new Exception("Actor was not removed");
            }

            actor.x += 10;
            actor.y += 10;

            positionalGroupedActors.Insert(indexOfNode, new ActorNode(ActorGroupType.Position, actor.name, actor));
            return true;

        }

        public void SortActorsByPosition() {

            Dictionary<(int x, int y, int z), ActorNode> dicPositionalGroupedActors = new();

            foreach (var actor in actors) { 

                if (dicPositionalGroupedActors.ContainsKey((actor.x, actor.y, 0))) {
                    dicPositionalGroupedActors[(actor.x, actor.y, 0)].nestedActors.Add(actor);
                    dicPositionalGroupedActors[(actor.x, actor.y, 0)].name = "Group";
                }
                else {
                    dicPositionalGroupedActors[(actor.x, actor.y, 0)] = new ActorNode(ActorGroupType.Position, actor.name, actor);
                }

            }

            positionalGroupedActors = dicPositionalGroupedActors.Values.ToList();

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