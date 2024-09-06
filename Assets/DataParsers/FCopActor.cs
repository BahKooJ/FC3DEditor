

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCopParser {

    public class FCopActor {

        public static class FourCC {

            public const string tACT = "tACT";
            public const string aRSL = "aRSL";
            public const string Cobj = "Cobj";
            public const string NULL = "NULL";
            public const string tSAC = "tSAC";
            public const string Cnet = "Cnet";

        }

        public struct FCopResource {

            public string fourCC;
            public int id;

            public FCopResource(string fourCC, int id) {
                this.fourCC = fourCC;
                this.id = id;
            }

        }

        const int idOffset = 8;
        const int objectTypeOffset = 12;
        const int yOffset = 16;
        const int xOffset = 24;

        public List<ChunkHeader> offsets = new();

        public int id;
        public int actorType;
        public int x;
        public int y;

        public List<FCopResource> resourceReferences = new();

        public FCopActorBehavior behavior;

        public IFFDataFile rawFile;

        public FCopActor(IFFDataFile rawFile) {

            this.rawFile = rawFile;

            FindStartChunkOffset();

            id = Utils.BytesToInt(rawFile.data.ToArray(), 8);
            actorType = Utils.BytesToInt(rawFile.data.ToArray(), 12);
            y = Utils.BytesToInt(rawFile.data.ToArray(), 16);
            x = Utils.BytesToInt(rawFile.data.ToArray(), 24);

            ParseResourceReferences();

            switch (actorType) {
                case 1:
                    behavior = new FCopBehavior1(this);
                    break;
                case 5:
                    behavior = new FCopBehavior5(this);
                    break;
                case 8:
                    behavior = new FCopBehavior8(this);
                    break;
                case 9:
                    behavior = new FCopBehavior9(this);
                    break;
                case 11:
                    behavior = new FCopBehavior11(this);
                    break;
                case 14:
                    behavior = new FCopBehavior14(this);
                    break;
                case 28:
                    behavior = new FCopBehavior28(this);
                    break;
                case 36:
                    behavior = new FCopBehavior36(this);
                    break;
                case 95:
                    behavior = new FCopBehavior95(this);
                    break;
                case 99:
                    behavior = new FCopBehavior99(this);
                    break;
            }


        }

        virtual public void Compile() {

            rawFile.data.RemoveRange(yOffset, 4);
            rawFile.data.InsertRange(yOffset, BitConverter.GetBytes(y));
            rawFile.data.RemoveRange(xOffset, 4);
            rawFile.data.InsertRange(xOffset, BitConverter.GetBytes(x));

            if (behavior != null) {
                behavior.Compile();
            }

        }

        void ParseResourceReferences() {

            var header = offsets.First(header => {
                return header.fourCCDeclaration == FourCC.aRSL;
            });

            var bytes = rawFile.data.GetRange(header.index, header.chunkSize);

            var offset = 12;

            var refCount = (header.chunkSize - 12) / 8;

            foreach (var i in Enumerable.Range(0, refCount)) {

                var fourCC = Reverse(Encoding.Default.GetString(bytes.ToArray(), offset, 4));
                var id = BitConverter.ToInt32(bytes.ToArray(), offset + 4);

                resourceReferences.Add(new FCopResource(fourCC, id));

                offset += 8;

            }

        }


        void FindStartChunkOffset() {

            offsets.Clear();

            int offset = 0;

            while (offset < rawFile.data.Count) {

                var fourCC = BytesToStringReversed(offset, 4);
                var size = BytesToInt(offset + 4);

                offsets.Add(new ChunkHeader(offset, fourCC, size));

                offset += size;

            }

        }

        string Reverse(string s) {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        int BytesToInt(int offset) {
            return BitConverter.ToInt32(rawFile.data.ToArray(), offset);
        }

        string BytesToStringReversed(int offset, int length) {
            return Reverse(Encoding.Default.GetString(rawFile.data.ToArray(), offset, length));
        }


    }

    public interface FCopActorBehavior {

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }

        public void Compile() {

        }


    }

    public class FCopBehavior1 : FCopActorBehavior {

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }

        public ValueActorProperty unknownNumber1;
        public ValueActorProperty unknownNumber2;
        public ValueActorProperty playerHealth;
        public int unknownNumber3;
        public EnumDataActorProperty team;
        public ValueActorProperty minimapColor;
        public int unknownNumber4;
        public ValueActorProperty uvOffset;
        // FIXME: for some odd reason players facing can be negative. Allow the property to be negative
        public RotationActorProperty facing;
        public int unknownNumber5;

        public FCopBehavior1(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            unknownNumber1 = new("unknownNumber1", Utils.BytesToShort(rawFile.data.ToArray(), 28));
            unknownNumber2 = new("unknownNumber1", Utils.BytesToShort(rawFile.data.ToArray(), 30));
            playerHealth = new("Player Health", Utils.BytesToShort(rawFile.data.ToArray(), 32));
            unknownNumber3 = Utils.BytesToShort(rawFile.data.ToArray(), 34);
            team = new("Team", (PlayerTeam)Utils.BytesToShort(rawFile.data.ToArray(), 36));
            minimapColor = new("Minimap Color", Utils.BytesToShort(rawFile.data.ToArray(), 38));
            unknownNumber4 = Utils.BytesToShort(rawFile.data.ToArray(), 40);
            uvOffset = new("UV Offset", Utils.BytesToShort(rawFile.data.ToArray(), 42));
            facing = new("Facing", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 44)));
            unknownNumber5 = Utils.BytesToShort(rawFile.data.ToArray(), 46);

            properties = new() { unknownNumber1, unknownNumber2, playerHealth, team, minimapColor, uvOffset, facing };

        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(28, 2);
            actor.rawFile.data.InsertRange(28, BitConverter.GetBytes((short)unknownNumber1.value));

            actor.rawFile.data.RemoveRange(30, 2);
            actor.rawFile.data.InsertRange(30, BitConverter.GetBytes((short)unknownNumber2.value));

            actor.rawFile.data.RemoveRange(32, 2);
            actor.rawFile.data.InsertRange(32, BitConverter.GetBytes((short)playerHealth.value));

        }

    }

    public class FCopBehavior5 : FCopActorBehavior {

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public int textureOffset;

        public int offset = 44;
        public ValueActorProperty numb1;

        public FCopBehavior5(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            textureOffset = Utils.BytesToShort(rawFile.data.ToArray(), 42);

            numb1 = new("numb1", Utils.BytesToShort(rawFile.data.ToArray(), offset));

            properties = new() { numb1 };
            
        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(offset, 2);
            actor.rawFile.data.InsertRange(offset, BitConverter.GetBytes((short)numb1.value));

        }

    }

    public class FCopBehavior8 : FCopActorBehavior {

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public EnumDataActorProperty team;
        public EnumDataActorProperty miniMapColor;
        public ValueActorProperty textureOffset;
        public EnumDataActorProperty hostileTowards;

        public RotationActorProperty headRotation;

        public RotationActorProperty baseRotation;

        public FCopBehavior8(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            team = new("Team", Utils.BytesToShort(rawFile.data.ToArray(), 36) == 1 ? Team.Red : Team.Blue);
            miniMapColor = new("Minimap Color", Utils.BytesToShort(rawFile.data.ToArray(), 38) == 1 ? Team.Red : Team.Blue);
            textureOffset = new("UV Offset", Utils.BytesToShort(rawFile.data.ToArray(), 42));
            hostileTowards = new("Attacks Team", Utils.BytesToShort(rawFile.data.ToArray(), 50) == 1 ? Team.Red : Team.Blue);

            headRotation = new("Head Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 64)));
            baseRotation = new("Base Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 78)));

            properties = new() { team, miniMapColor, textureOffset, hostileTowards, headRotation, baseRotation };

        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(64, 2);
            actor.rawFile.data.InsertRange(64, BitConverter.GetBytes((short)headRotation.value.compiledRotation));

            actor.rawFile.data.RemoveRange(78, 2);
            actor.rawFile.data.InsertRange(78, BitConverter.GetBytes((short)headRotation.value.compiledRotation));

        }


    }

    public class FCopBehavior9 : FCopActorBehavior {

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public int textureOffset;

        public ValueActorProperty potentialSpawnLocation;
        public ValueActorProperty idkWhatThisIs;

        public FCopBehavior9(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            textureOffset = Utils.BytesToShort(rawFile.data.ToArray(), 42);

            potentialSpawnLocation = new("Spawn Location?", Utils.BytesToInt(actor.rawFile.data.ToArray(), 88));
            idkWhatThisIs = new("wtf is this?", Utils.BytesToShort(actor.rawFile.data.ToArray(), 64));

            properties = new() { potentialSpawnLocation, idkWhatThisIs };

        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(64, 2);
            actor.rawFile.data.InsertRange(64, BitConverter.GetBytes((short)idkWhatThisIs.value));

            actor.rawFile.data.RemoveRange(88, 4);
            actor.rawFile.data.InsertRange(88, BitConverter.GetBytes(potentialSpawnLocation.value));

        }

    }

    public class FCopBehavior11 : FCopActorBehavior {

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public RotationActorProperty rotation;

        public FCopBehavior11(FCopActor actor) {
            this.actor = actor;

            rotation = new("Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 46)));

            properties = new() { rotation };
        
        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(46, 2);
            actor.rawFile.data.InsertRange(46, BitConverter.GetBytes((short)rotation.value.compiledRotation));

        }

    }

    public class FCopBehavior14 : FCopActorBehavior {

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        ValueActorProperty number1;
        ValueActorProperty number2;
        ValueActorProperty number3;
        ValueActorProperty number4;
        ValueActorProperty number5;
        ValueActorProperty number6;
        ValueActorProperty number7;


        public FCopBehavior14(FCopActor actor) {
            this.actor = actor;

            number1 = new("Number 1", Utils.BytesToShort(actor.rawFile.data.ToArray(), 28));
            number2 = new("Number 2", Utils.BytesToShort(actor.rawFile.data.ToArray(), 30));
            number3 = new("Number 3", Utils.BytesToShort(actor.rawFile.data.ToArray(), 32));
            number4 = new("Number 4", Utils.BytesToShort(actor.rawFile.data.ToArray(), 44));
            number5 = new("Number 5", Utils.BytesToShort(actor.rawFile.data.ToArray(), 46));
            number6 = new("Number 6", Utils.BytesToShort(actor.rawFile.data.ToArray(), 48));
            number7 = new("Number 7", Utils.BytesToShort(actor.rawFile.data.ToArray(), 50));

            properties = new() { number1, number2, number3, number4, number5, number6, number7 };
        }


        public void Compile() {

        }

    }

    public class FCopBehavior28 : FCopActorBehavior {

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public int textureOffset;

        public FCopBehavior28(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            textureOffset = Utils.BytesToShort(rawFile.data.ToArray(), 42);

        }

    }

    public class FCopBehavior36 : FCopActorBehavior {


        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public RotationActorProperty headRotation;

        public RotationActorProperty baseRotation;


        public FCopBehavior36(FCopActor actor) {
            this.actor = actor;

            headRotation = new("Head Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 64)));
            baseRotation = new("Base Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 78)));

            properties = new() { headRotation, baseRotation };
        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(64, 2);
            actor.rawFile.data.InsertRange(64, BitConverter.GetBytes((short)headRotation.value.compiledRotation));

            actor.rawFile.data.RemoveRange(78, 2);
            actor.rawFile.data.InsertRange(78, BitConverter.GetBytes((short)headRotation.value.compiledRotation));

        }

    }

    public class FCopBehavior95 : FCopActorBehavior {

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public ValueActorProperty hitboxWidth;
        public ValueActorProperty hitboxHeight;
        public ValueActorProperty number3;
        public ValueActorProperty triggerType;
        public IDReferenceActorProperty actorToTest;

        public FCopBehavior95(FCopActor actor) {
            this.actor = actor;

            hitboxWidth = new ("Hit Box Width", Utils.BytesToShort(actor.rawFile.data.ToArray(), 28));
            hitboxHeight = new("Hit Box Height", Utils.BytesToShort(actor.rawFile.data.ToArray(), 30));
            number3 = new("Property 3", Utils.BytesToShort(actor.rawFile.data.ToArray(), 32));
            triggerType = new("Trigger Type", Utils.BytesToShort(actor.rawFile.data.ToArray(), 34));
            actorToTest = new("Trigger Actor", Utils.BytesToInt(actor.rawFile.data.ToArray(), 36));

            properties = new() { hitboxWidth, hitboxHeight, number3, triggerType, actorToTest };

        }

    }

    public class FCopBehavior99 : FCopActorBehavior {

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }

        public FCopBehavior99(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            properties = new();

            var propertyCount = (Utils.BytesToInt(rawFile.data.ToArray(), 4) - 28) / 2;

            var offset = 28;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Utils.BytesToShort(rawFile.data.ToArray(), offset));
                property.fileOffset = offset;
                properties.Add(property);
                offset += 2;
            }


        }

        public void Compile() {

            foreach (var p in properties) {

                var vp = (ValueActorProperty)p;

                actor.rawFile.data.RemoveRange(p.fileOffset, 2);
                actor.rawFile.data.InsertRange(p.fileOffset, BitConverter.GetBytes((short)vp.value));

            }

        }

    }

    public interface ActorProperty {

        public string name { get; set; }
        public int fileOffset { get; set; }
    }

    public class ValueActorProperty: ActorProperty { 
        public string name { get; set; }
        public int fileOffset { get; set; }

        public int value;

        public ValueActorProperty(string name, int value) {
            this.name = name;
            this.value = value;
        }

    }

    public class IDReferenceActorProperty: ActorProperty {
        public string name { get; set; }
        public int fileOffset { get; set; }

        public int value;

        public IDReferenceActorProperty(string name, int value) {
            this.name = name;
            this.value = value;
        }

    }

    public class EnumDataActorProperty: ActorProperty {
        public string name { get; set; }
        public int fileOffset { get; set; }

        public Enum caseValue;

        public EnumDataActorProperty(string name, Enum caseValue) {
            this.name = name;
            this.caseValue = caseValue;
        }

    }

    public class RangeActorProperty: ActorProperty {
        public string name { get; set; }
        public int fileOffset { get; set; }

        public int value;

        public int max;
        public int min;

        public RangeActorProperty(string name, int value, int max, int min) {
            this.name = name;
            this.value = value;
            this.max = max;
            this.min = min;
        }

    }

    public class RotationActorProperty: ActorProperty {
        public string name { get; set; }
        public int fileOffset { get; set; }

        public ActorRotation value;

        public RotationActorProperty(string name, ActorRotation value) {
            this.name = name;
            this.value = value;
        }

    }

    public struct ActorRotation {

        public static int maxRotation = 4096;

        public int compiledRotation;

        public float parsedRotation;

        public ActorRotation SetRotationDegree(float newRotation) {

            if (newRotation > 180f) {
                parsedRotation = 360f - newRotation;
            }
            if (newRotation < -180f) {
                parsedRotation = newRotation + 360f;
            }

            parsedRotation = newRotation;

            compiledRotation = (int)(newRotation / 360f * maxRotation);

            return this;

        }

        public ActorRotation SetRotationParse(float newRotation) {

            parsedRotation = newRotation;

            float degreeRotation = newRotation;

            if (newRotation < 0) {

                degreeRotation = newRotation + 360;

            }

            compiledRotation = (int)(degreeRotation / 360f * maxRotation);

            return this;

        }

        public ActorRotation SetRotationCompiled(int newRotation) {

            compiledRotation = newRotation;

            float rotationPrecentage = (float)newRotation / (float)maxRotation;

            float degreeRoation = 360f * rotationPrecentage;

            if (degreeRoation > 180f) {
                parsedRotation = 360f - degreeRoation;
            }

            parsedRotation = degreeRoation;

            return this;

        }

        public static ActorRotation operator +(ActorRotation a, float b) {
            return a.SetRotationDegree(a.parsedRotation + b);
        }

    }

    public enum Team {
        Red = 1,
        Blue = 2
    }

    public enum PlayerTeam {
        Red = 1,
        Blue = 2,
        Unknown1 = 31,
        Unknown2 = 543
    }

}