

using System.Collections.Generic;

namespace FCopParser {

    public class FCopSceneActors {

        public List<FCopActor> actors;

        public List<ActorNode> positionalGroupedActors;
        public List<ActorNode> behaviorGroupedActors;
        public List<ActorNode> scriptingGroupedActors;

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