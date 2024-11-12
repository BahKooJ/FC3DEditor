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
        
        public int FindNextID() {

            var actorsByAscending = actors.OrderBy(a => a.DataID).ToList();


            var previousID = actorsByAscending[0].DataID;
            foreach (var actor in actorsByAscending) {

                if (actor.DataID == previousID + 1 || actor.DataID == previousID) {
                    previousID = actor.DataID;
                    continue;
                }
                else {
                    return previousID + 1;
                }

            }

            return actorsByAscending.Last().DataID + 1;

        }

        public void AddActor(FCopActor actor, ActorNode toGroup) {

            actors.Add(actor);
            actorsByID[actor.DataID] = actor;

            if (toGroup != null) {
                PositionalGroupActor(actor, toGroup);
            }
            else {
                positionalGroupedActors.Add(new ActorNode(ActorGroupType.Position, actor.name, actor));
            }

            if (behaviorGroupedActors.ContainsKey((int)actor.behaviorType)) {
                behaviorGroupedActors[(int)actor.behaviorType].nestedActors.Add(actor);
            }
            else {
                behaviorGroupedActors[(int)actor.behaviorType] = new ActorNode(ActorGroupType.Position, "Group " + (int)actor.behaviorType, actor);
            }

        }

        public void DeleteActor(FCopActor actor) {

            actors.Remove(actor);
            actorsByID.Remove(actor.DataID);

            var posNode = ActorNodeByIDPositional(actor.DataID);

            if (posNode.nestedActors.Count == 1) {
                positionalGroupedActors.Remove(posNode);
            }
            else {
                posNode.nestedActors.Remove(actor);
            }

            var behNode = ActorNodeByIDBehavior(actor.DataID);

            if (behNode.nestedActors.Count == 1) {
                behaviorGroupedActors.Remove((int)behNode.nestedActors[0].behaviorType);
            }
            else {
                behNode.nestedActors.Remove(actor);
            }
            
            // TODO: script group

        }

        public void DeleteActor(int id) {

            DeleteActor(actorsByID[id]);

        }

        public void ReorderPositionalGroup(int index, int newIndex) {

            var movingItem = positionalGroupedActors[index];
            positionalGroupedActors.RemoveAt(index);
            positionalGroupedActors.Insert(newIndex, movingItem);

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

            // Makes sure there's no dupes.
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

                if (node != null) {

                    node.nestedActors.Remove(actor);

                }

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

        public void SetPositionalGroup(List<ActorGroup> actorGroups) {

            positionalGroupedActors = new();

            foreach (var group in actorGroups) {

                var actors = new List<FCopActor>();

                foreach (var id in group.actorIDs) {
                    actors.Add(actorsByID[id]);
                }

                positionalGroupedActors.Add(new ActorNode(actors, ActorGroupType.Position, group.name));

            }

        }

        public void SortActorsByBehavior() {

            foreach (var actor in actors) {

                if (behaviorGroupedActors.ContainsKey((int)actor.behaviorType)) {
                    behaviorGroupedActors[(int)actor.behaviorType].nestedActors.Add(actor);
                }
                else {
                    behaviorGroupedActors[(int)actor.behaviorType] = new ActorNode(ActorGroupType.Behavior, "Group " + actor.behaviorType, actor);
                }

            }

        }

        public List<IFFDataFile> Compile() {

            var total = new List<IFFDataFile>();

            foreach (var actor in actors) {
                total.Add(actor.Compile());
            }

            return total;

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

        public ActorNode(List<FCopActor> nestedActors, ActorGroupType groupType, string name) {
            this.nestedActors = nestedActors;
            this.groupType = groupType;
            this.name = name;
        }

    }

    public enum ActorGroupType {
        Position = 0,
        Behavior = 1,
        Script = 2
    }

    public struct ActorGroup {

        public string name;
        public ActorGroupType groupType;
        public List<int> actorIDs;

        public ActorGroup(string name, ActorGroupType groupType, List<int> actorIDs) {
            this.name = name;
            this.groupType = groupType;
            this.actorIDs = actorIDs;
        }

    }

}