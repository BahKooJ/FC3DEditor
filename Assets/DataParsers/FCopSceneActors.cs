﻿

using System;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    public class FCopSceneActors {

        public FCopLevel level;
        public List<FCopActor> actors;
        public Dictionary<int, FCopActor> actorsByID = new();

        public List<ActorNode> positionalGroupedActors = new();
        public Dictionary<int, ActorNode> behaviorGroupedActors = new();
        public List<ActorNode> scriptingGroupedActors;

        public FCopSceneActors(List<FCopActor> actors, FCopLevel level) {

            this.level = level;
            this.actors = actors;

            foreach (var actor in actors) {
                actorsByID[actor.DataID] = actor;
            }

            SortActorsByPosition();
            SortActorsByBehavior();
        }

        public void DeleteActor(FCopActor actor) {

            actors.Remove(actor);
            level.fileManager.files.Remove(actor.rawFile);

            var posNode = ActorNodeByIDPositional(actor.DataID);

            if (posNode.nestedActors.Count == 1) {
                positionalGroupedActors.Remove(posNode);
            }
            else {
                posNode.nestedActors.Remove(actor);
            }

            var behNode = ActorNodeByIDBehavior(actor.DataID);

            if (behNode.nestedActors.Count == 1) {
                behaviorGroupedActors.Remove(behNode.nestedActors[0].actorType);
            }
            else {
                behNode.nestedActors.Remove(actor);
            }
            
            // TODO: script group

        }

        public void DeleteActor(int id) {

            DeleteActor(actorsByID[id]);

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
            node.nestedActors.Remove(actor);

            actor.x += 10;
            actor.y += 10;

            positionalGroupedActors.Insert(indexOfNode, new ActorNode(ActorGroupType.Position, actor.name, actor));
            return true;

        }

        public bool PositionalGroupActor(FCopActor actor, ActorNode toGroup) {

            if (toGroup.groupType != ActorGroupType.Position) return false;

            ActorNode node;

            node = positionalGroupedActors.FirstOrDefault(n => {

                if (n.nestedActors.Count == 1) {
                    return n.nestedActors[0].DataID == actor.DataID;
                }
                return false;

            });

            if (node == null) {

                node = positionalGroupedActors.FirstOrDefault(n => {

                    foreach (var nestedN in n.nestedActors) {

                        if (nestedN.DataID == actor.DataID) {
                            return true;
                        }

                    }

                    return false;

                });

                if (node == null) return false;

                node.nestedActors.Remove(actor);

            }
            else {

                positionalGroupedActors.Remove(node);

            }

            actor.x = toGroup.nestedActors[0].x;
            actor.y = toGroup.nestedActors[0].y;

            toGroup.nestedActors.Add(actor);
            return true;

        }

        public ActorNode ActorNodeByIDPositional(int id) {

            ActorNode node;

            node = positionalGroupedActors.FirstOrDefault(n => {

                if (n.nestedActors.Count == 1) {
                    return n.nestedActors[0].DataID == id;
                }
                return false;

            });

            node ??= positionalGroupedActors.FirstOrDefault(n => {

                    foreach (var nestedN in n.nestedActors) {

                        if (nestedN.DataID == id) {
                            return true;
                        }

                    }

                    return false;

            });

            return node;

        }

        public ActorNode ActorNodeByIDBehavior(int id) {

            ActorNode node;

            node = behaviorGroupedActors.FirstOrDefault(pair => {

                if (pair.Value.nestedActors.Count == 1) {
                    return pair.Value.nestedActors[0].DataID == id;
                }
                return false;

            }).Value;

            node ??= behaviorGroupedActors.FirstOrDefault(pair => {

                foreach (var nestedN in pair.Value.nestedActors) {

                    if (nestedN.DataID == id) {
                        return true;
                    }

                }

                return false;

            }).Value;

            return node;

        }

        public void SortActorsByPosition() {

            Dictionary<(int x, int y, int z), ActorNode> dicPositionalGroupedActors = new();

            foreach (var actor in actors) { 

                if (dicPositionalGroupedActors.ContainsKey((actor.x, actor.y, 0))) {
                    var value = dicPositionalGroupedActors[(actor.x, actor.y, 0)];
                    value.nestedActors.Add(actor);
                    value.name = "Group";
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