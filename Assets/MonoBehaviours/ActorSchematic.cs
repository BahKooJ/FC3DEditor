

using FCopParser;
using System.Collections.Generic;
using System.Text;

public class ActorSchematics {

    public static string tag = "ATSMTAG";

    public string directoryName;

    public ActorSchematics parent = null;
    public List<ActorSchematics> subFolders = new List<ActorSchematics>();

    public List<ActorSchematic> schematics = new List<ActorSchematic>();

    public ActorSchematics(string name, ActorSchematics parent) { 
        this.directoryName = name;
        this.parent = parent;
    }

    public ActorSchematics() {
        directoryName = "";
    }

    public string Compile() {

        if (parent != null) { return ""; }

        string SaveSchematic(List<ActorSchematic> schematics) {

            var total = new StringBuilder();

            foreach (var schem in schematics) {

                total.Append(schem.Compile());
                total.Append(",");

            }

            // Removes the access comma
            if (total.Length != 0) {
                total.Remove(total.Length - 1, 1);
            }

            return total.ToString();

        }

        string SaveSchematics(ActorSchematics schematics) {

            var total = new StringBuilder();

            total.Append("{\"");

            total.Append(schematics.directoryName + "\",[");

            total.Append(SaveSchematic(schematics.schematics));

            total.Append("],[");

            var comma = false;
            foreach (var subPresets in schematics.subFolders) {

                if (comma) {
                    total.Append(",");
                }

                total.Append(SaveSchematics(subPresets));

                comma = true;

            }

            total.Append("]}");

            return total.ToString();

        }

        var total = SaveSchematics(this);

        return total;

    }

}

public class ActorSchematic {

    public string name;
    public ActorBehavior behavior;
    // Why is this a byte array and not an actor?
    // Because of the shear number of properties an actor can have.
    // To save development time it just creates a new actor by parsing the original format.
    public List<byte> actorData = new List<byte>();

    public ActorSchematic(FCopActor actor) {
        this.name = "Actor Schematic";
        this.behavior = actor.behaviorType;
        this.actorData = actor.Compile().data;
    }

    public ActorSchematic() {

    }

    public string Compile() {

        // (NAME, TYPE, [Bytes])

        var total = new StringBuilder();

        total.Append("(");

        total.Append("\"" + name + "\",");
        total.Append(((int)behavior).ToString() + ",");

        total.Append("[");
        foreach (var b in actorData) {

            total.Append(b.ToString() + ",");

        }
        total.Remove(total.Length - 1, 1);
        total.Append("])");

        return total.ToString();

    }

}