﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    public class FCopSceneActors {

        public FCopLevel level;
        public List<FCopActor> actors;
        public Dictionary<int, FCopActor> actorsByID = new();

        public List<ActorNode> positionalGroupedActors = new();
        public Dictionary<int, ActorNode> behaviorGroupedActors = new();

        public Dictionary<int, string> teams = new() {
            {0, "None"}
        };

        public Dictionary<int, string> scriptGroup = new() {
            {0, "None"}
        };


        public FCopSceneActors(List<FCopActor> actors, FCopLevel level) {

            this.level = level;
            this.actors = actors;

            foreach (var actor in actors) {
                actorsByID[actor.DataID] = actor;
            }

            SortActorsByPosition();
            SortActorsByBehavior();
            FindGroups();

            FindTeams();
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

        public List<FCopActor> FindActorsByBehavior(ActorBehavior behavior) {

            return actors.Where(a => a.behaviorType == behavior).ToList();

        }

        public void AddActor(FCopActor actor, ActorNode toGroup, bool AddFirst = false) {

            if (AddFirst) {
                actors.Insert(0, actor);
            }
            else {
                actors.Add(actor);
            }

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
                behaviorGroupedActors[(int)actor.behaviorType] = new ActorNode(ActorGroupType.Behavior, "Group " + actor.behaviorType, actor);
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

        public void ReorderInsideNode(FCopActor actorToReorder, int index, ActorNode group) {

            group.nestedActors.Remove(actorToReorder);
            group.nestedActors.Insert(index, actorToReorder);

        }

        public bool PositionalUngroupActor(FCopActor actor) {

            var node = positionalGroupedActors.FirstOrDefault(n => {

                if (n.nestedActors.Count == 1) {
                    return false;
                }

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

        public void FindGroups() {

            foreach (var actor in actors) {

                if (actor.behavior is FCopEntity) {

                    var groupID = actor.behavior.propertiesByName["Group"].GetCompiledValue();

                    if (!scriptGroup.ContainsKey(groupID)) {
                        scriptGroup[groupID] = "Group " + groupID;
                    }

                }

            }

        }

        public List<Type> FindAllDerivedTypesFromActorBehavior(int dataID) {

            if (!actorsByID.TryGetValue(dataID, out var actor)) {
                return new();
            }

            var types = new List<Type>();

            FindTypes(actor.behavior.GetType());

            void FindTypes(Type type) {

                types.Add(type);

                if (type != typeof(FCopActorBehavior)) {
                    FindTypes(type.BaseType);
                }

            }

            return types;

        }

        public List<Type> FindAllDerviedTypesFromGroup(int groupID) {

            var actorsByGroup = new List<FCopActor>();

            foreach (var actor in actors) {

                if (actor.behavior is FCopEntity) {

                    var groupIDToTest = actor.behavior.propertiesByName["Group"].GetCompiledValue();

                    if (groupIDToTest == groupID) {
                        actorsByGroup.Add(actor);
                    }

                }

            }

            var actorTypes = new List<List<Type>>();

            foreach (var actor in actorsByGroup) {

                actorTypes.Add(FindAllDerivedTypesFromActorBehavior(actor.DataID));

            }

            if (actorTypes.Count == 0) {
                return new();
            }

            var commonTypes = actorTypes[0];

            foreach (var types in actorTypes) {

                foreach (var commonType in new List<Type>(commonTypes)) {

                    if (!types.Contains(commonType)) {
                        commonTypes.Remove(commonType);
                    }

                }

            }

            return commonTypes;

        }

        public List<Type> FindAllDerviedTypesFromTeam(int teamID) {

            var actorsByTeam = new List<FCopActor>();

            foreach (var actor in actors) {

                if (actor.behavior is FCopEntity) {

                    var teamIDToTest = actor.behavior.propertiesByName["Team"].GetCompiledValue();

                    if (teamIDToTest == teamID) {
                        actorsByTeam.Add(actor);
                    }

                }

            }

            var actorTypes = new List<List<Type>>();

            foreach (var actor in actorsByTeam) {

                actorTypes.Add(FindAllDerivedTypesFromActorBehavior(actor.DataID));

            }

            if (actorTypes.Count == 0) {
                return new();
            }

            var commonTypes = actorTypes[0];

            foreach (var types in actorTypes) {

                foreach (var commonType in new List<Type>(commonTypes)) {

                    if (!types.Contains(commonType)) {
                        commonTypes.Remove(commonType);
                    }

                }

            }

            return commonTypes;

        }

        public List<ActorNode> CreatePositionSaveState() {

            var total = new List<ActorNode>();

            foreach (var node in positionalGroupedActors) {
                total.Add(node.Clone());
            }

            return total;

        }

        public void FindTeams() {

            var teamedActors = actors.Where(a => a.behavior is FCopEntity);

            foreach (var actor in teamedActors) {

                var teamProperties = actor.behavior.properties.Where(p => p is AssetActorProperty);

                foreach (var teamProperty in teamProperties.Cast<AssetActorProperty>()) {

                    if (!teams.ContainsKey(teamProperty.assetID) && teamProperty.assetType == AssetType.Team) {
                        teams[teamProperty.assetID] = "Team" + teamProperty.assetID;
                    }

                }

                var overloads = actor.behavior.properties.Where(o => o is OverloadedProperty);

                foreach (var overload in overloads.Cast<OverloadedProperty>()) {

                    if (overload.GetOverloadProperty() is AssetActorProperty teamProp) {

                        if (!teams.ContainsKey(teamProp.assetID) && teamProp.assetType == AssetType.Team) {
                            teams[teamProp.assetID] = "Team" + teamProp.assetID;
                        }

                    }

                }

            }

        }

        public int AddTeam() {

            int newID = 1;
            int previousID = 1;

            if (teams.Count > 0) {

                var teamsByAscending = teams.OrderBy(t => t.Key).ToList();

                previousID = teamsByAscending[0].Key;

                foreach (var team in teamsByAscending) {

                    if (team.Key == previousID + 1 || team.Key == previousID) {
                        previousID = team.Key;
                        continue;
                    }
                    else {
                        newID = previousID + 1;
                        break;
                    }

                }

                if (newID == 1) {

                    newID = teamsByAscending.Last().Key + 1;

                }

            }

            teams[newID] = "Team";

            return newID;

        }

        public int AddGroup() {

            var nextID = Utils.FindNextInt(scriptGroup.Keys.ToList());

            scriptGroup[nextID] = "Group";

            return nextID;

        }

        public void DeleteTeam(int id) {

            teams.Remove(id);

            var teamedActors = actors.Where(a => a.behavior is FCopEntity);

            foreach (var actor in teamedActors) {

                var teamProperties = actor.behavior.properties.Where(p => p is AssetActorProperty);

                foreach (var teamProperty in teamProperties.Cast<AssetActorProperty>()) {

                    if (!teams.ContainsKey(teamProperty.assetID) && teamProperty.assetType == AssetType.Team) {
                        teamProperty.assetID = 0;
                    }

                }

                var overloads = actor.behavior.properties.Where(o => o is OverloadedProperty);

                foreach (var overload in overloads.Cast<OverloadedProperty>()) {

                    if (overload.GetOverloadProperty() is AssetActorProperty teamProp) {

                        if (!teams.ContainsKey(teamProp.assetID) && teamProp.assetType == AssetType.Team) {
                            teamProp.assetID = 0;
                        }

                    }

                }

            }

        }

        public void DeleteGroup(int id) {

            scriptGroup.Remove(id);

            var groupedActors = actors.Where(a => a.behavior is FCopEntity);

            foreach (var actor in groupedActors) {

                var groupProperties = actor.behavior.propertiesByName["Group"];

                if (groupProperties.GetCompiledValue() == id) {
                    groupProperties.SetCompiledValue(0);
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

        public ActorNode Clone() {

            return new ActorNode(new(nestedActors), groupType, name);

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