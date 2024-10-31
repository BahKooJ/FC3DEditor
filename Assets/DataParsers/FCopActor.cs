

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;

namespace FCopParser {

    public class FCopActor : FCopAsset {

        public static class FourCC {

            public const string tACT = "tACT";
            public const string aRSL = "aRSL";
            public const string Cobj = "Cobj";
            public const string NULL = "NULL";
            public const string tSAC = "tSAC";
            public const string Cnet = "Cnet";

        }

        public struct Resource {

            public string fourCC;
            public int id;

            public Resource(string fourCC, int id) {
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

        public List<Resource> resourceReferences = new();

        public FCopActorBehavior behavior;

        public List<byte> tSACData = null;

        public FCopActor(IFFDataFile rawFile): base(rawFile) {

            name = "Actor " + DataID;

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

            var tSAC = offsets.FirstOrDefault(h => h.fourCCDeclaration == FourCC.tSAC);

            if (tSAC != null) {
                tSACData = rawFile.data.GetRange(tSAC.index, tSAC.chunkSize);
            }

        }

        public IFFDataFile Compile() {

            var total = new List<byte>();

            total.AddRange(BitConverter.GetBytes(DataID));
            total.AddRange(BitConverter.GetBytes(actorType));
            total.AddRange(BitConverter.GetBytes(y));
            total.AddRange(BitConverter.GetBytes(0));
            total.AddRange(BitConverter.GetBytes(x));


            var didBehaviorCompile = false;
            if (behavior != null) {
                var data = behavior.Compile();

                if (data != null) {

                    total.AddRange(data);
                    didBehaviorCompile = true;

                }

            }

            if (!didBehaviorCompile) {

                var tACT = offsets.FirstOrDefault(h => h.fourCCDeclaration == FourCC.tACT);

                total.AddRange(rawFile.data.GetRange(tACT.index + 28, tACT.chunkSize - 28));

            }

            var actSize = total.Count + 8;

            var refTotal = new List<byte>();

            foreach (var r in resourceReferences) {

                refTotal.AddRange(Encoding.ASCII.GetBytes(Reverse(r.fourCC)));
                refTotal.AddRange(BitConverter.GetBytes(r.id));

            }

            total.AddRange(Encoding.ASCII.GetBytes(Reverse(FourCC.aRSL)));
            total.AddRange(BitConverter.GetBytes(refTotal.Count + 12));
            total.AddRange(BitConverter.GetBytes(DataID));
            total.AddRange(refTotal);

            if (tSACData != null) {
                total.AddRange(tSACData);
            }

            var totalWithHeader = new List<byte>();

            totalWithHeader.AddRange(Encoding.ASCII.GetBytes(Reverse(FourCC.tACT)));
            totalWithHeader.AddRange(BitConverter.GetBytes(actSize));
            totalWithHeader.AddRange(total);

            if (totalWithHeader.Count != rawFile.data.Count) {
                throw new Exception("Compiled size doesn't equal original");
            }

            rawFile.data = totalWithHeader;

            return rawFile;

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

                resourceReferences.Add(new Resource(fourCC, id));

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

        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }
        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }

        public List<byte> Compile() {
            return null;
        }


    }

    public interface FCopObjectMutating {



    }

    public interface FCopHeightOffseting {

        public int GetHeight();

        public ActorGroundCast GetGroundCast();

    }

    public class FCopBehavior1 : FCopActorBehavior {
        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

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

            unknownNumber1 = new("unknownNumber1", Utils.BytesToShort(rawFile.data.ToArray(), 28), BitCount.Bit16);
            unknownNumber2 = new("unknownNumber1", Utils.BytesToShort(rawFile.data.ToArray(), 30), BitCount.Bit16);
            playerHealth = new("Player Health", Utils.BytesToShort(rawFile.data.ToArray(), 32), BitCount.Bit16);
            unknownNumber3 = Utils.BytesToShort(rawFile.data.ToArray(), 34);
            team = new("Team", (PlayerTeam)Utils.BytesToShort(rawFile.data.ToArray(), 36), BitCount.Bit16);
            minimapColor = new("Minimap Color", Utils.BytesToShort(rawFile.data.ToArray(), 38), BitCount.Bit16);
            unknownNumber4 = Utils.BytesToShort(rawFile.data.ToArray(), 40);
            uvOffset = new("UV Offset", Utils.BytesToShort(rawFile.data.ToArray(), 42), BitCount.Bit16);
            facing = new("Facing", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 44)), BitCount.Bit16);
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
        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }

        

        public int textureOffset;

        public int debugOffset = 42;
        public ValueActorProperty debugValue;

        public FCopBehavior5(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            textureOffset = Utils.BytesToShort(rawFile.data.ToArray(), 42);

            debugValue = new("debug", Utils.BytesToShort(rawFile.data.ToArray(), debugOffset), BitCount.Bit16);

            properties = new() { };
            
        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(debugOffset, 2);
            actor.rawFile.data.InsertRange(debugOffset, BitConverter.GetBytes((short)debugValue.value));

        }

    }

    public class FCopBehavior8 : FCopActorBehavior {
        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public EnumDataActorProperty team;
        public EnumDataActorProperty miniMapColor;
        public ValueActorProperty textureOffset;
        public EnumDataActorProperty hostileTowards;

        public RotationActorProperty headRotation;

        public RotationActorProperty baseRotation;

        public int debugOffset = 76;
        public ValueActorProperty debugValue;

        public FCopBehavior8(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            team = new("Team", Utils.BytesToShort(rawFile.data.ToArray(), 36) == 1 ? Team.Red : Team.Blue, BitCount.Bit16);
            miniMapColor = new("Minimap Color", Utils.BytesToShort(rawFile.data.ToArray(), 38) == 1 ? Team.Red : Team.Blue, BitCount.Bit16);
            textureOffset = new("UV Offset", Utils.BytesToShort(rawFile.data.ToArray(), 42), BitCount.Bit16);
            hostileTowards = new("Attacks Team", Utils.BytesToShort(rawFile.data.ToArray(), 50) == 1 ? Team.Red : Team.Blue, BitCount.Bit16);

            headRotation = new("Head Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 64)), BitCount.Bit16);
            baseRotation = new("Base Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 78)), BitCount.Bit16);

            debugValue = new("debug", Utils.BytesToShort(rawFile.data.ToArray(), debugOffset), BitCount.Bit16);

            properties = new() { headRotation, baseRotation, new ValueActorProperty("boom", Utils.BytesToShort(rawFile.data.ToArray(), 40), BitCount.Bit16) };

        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(64, 2);
            actor.rawFile.data.InsertRange(64, BitConverter.GetBytes((short)headRotation.value.compiledRotation));

            actor.rawFile.data.RemoveRange(78, 2);
            actor.rawFile.data.InsertRange(78, BitConverter.GetBytes((short)headRotation.value.compiledRotation));

            actor.rawFile.data.RemoveRange(debugOffset, 2);
            actor.rawFile.data.InsertRange(debugOffset, BitConverter.GetBytes((short)debugValue.value));

        }


    }

    public class FCopBehavior9 : FCopActorBehavior {
        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public int textureOffset;

        public ValueActorProperty potentialSpawnLocation;
        public ValueActorProperty idkWhatThisIs;

        public FCopBehavior9(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            textureOffset = Utils.BytesToShort(rawFile.data.ToArray(), 42);

            potentialSpawnLocation = new("Spawn Location?", Utils.BytesToInt(actor.rawFile.data.ToArray(), 88), BitCount.Bit16);
            idkWhatThisIs = new("wtf is this?", Utils.BytesToShort(actor.rawFile.data.ToArray(), 64), BitCount.Bit16);

            properties = new() { potentialSpawnLocation, idkWhatThisIs };

        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(64, 2);
            actor.rawFile.data.InsertRange(64, BitConverter.GetBytes((short)idkWhatThisIs.value));

            actor.rawFile.data.RemoveRange(88, 4);
            actor.rawFile.data.InsertRange(88, BitConverter.GetBytes(potentialSpawnLocation.value));

        }

    }

    public class FCopBehavior11 : FCopActorBehavior, FCopHeightOffseting {
        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }

        public ValueActorProperty unknown1;
        public ValueActorProperty unknown2;
        public ValueActorProperty health;
        public ValueActorProperty collideDamage;
        public ValueActorProperty unknown3;
        public ValueActorProperty unknown4;
        public ValueActorProperty unknown5;
        public ValueActorProperty textureOffset;
        public EnumDataActorProperty groundCast;
        public RotationActorProperty rotation;
        public ValueActorProperty heightOffset;

        public FCopBehavior11(FCopActor actor) {
            this.actor = actor;

            assetRefNames = new string[] { "Object", "Destroyed Object" };
            assetRefType = new AssetType[] { AssetType.Object, AssetType.Object };

            var rawFile = actor.rawFile;
            unknown1 = new("unknown1", BitConverter.ToInt16(actor.rawFile.data.ToArray(), 28), BitCount.Bit16);
            unknown2 = new("unknown2", BitConverter.ToInt16(actor.rawFile.data.ToArray(), 30), BitCount.Bit16);
            health = new("Health", BitConverter.ToInt16(actor.rawFile.data.ToArray(), 32), BitCount.Bit16);
            collideDamage = new("Collide Damage", BitConverter.ToInt16(actor.rawFile.data.ToArray(), 34), BitCount.Bit16);
            unknown3 = new("unknown3", BitConverter.ToInt16(actor.rawFile.data.ToArray(), 36), BitCount.Bit16);
            unknown4 = new("unknown4", BitConverter.ToInt16(actor.rawFile.data.ToArray(), 38), BitCount.Bit16);
            unknown5 = new("Explosion effect", BitConverter.ToInt16(actor.rawFile.data.ToArray(), 40), BitCount.Bit16);
            textureOffset = new("Texture Offset", BitConverter.ToInt16(actor.rawFile.data.ToArray(), 42), BitCount.Bit16);
            groundCast = new("Ground Cast", (ActorGroundCast)BitConverter.ToInt16(actor.rawFile.data.ToArray(), 44), BitCount.Bit16);
            rotation = new("Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 46)), BitCount.Bit16);
            heightOffset = new("Height Offset", BitConverter.ToInt16(actor.rawFile.data.ToArray(), 48), BitCount.Bit16);

            properties = new() { unknown1, unknown2, health, collideDamage, unknown3, unknown4, unknown5, textureOffset, groundCast, rotation, heightOffset };

        }

        public int GetHeight() {
            return heightOffset.value;
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)groundCast.caseValue;
        }

        public List<byte> Compile() {

            var total = new List<byte>();

            foreach (var p in properties) {

                total.AddRange(p.Compile());

            }

            // For some reason with props, there's a property that's always 0.
            total.Add(0);
            total.Add(0);

            return total;

        }

    }

    public class FCopBehavior14 : FCopActorBehavior {
        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

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

            number1 = new("Number 1", Utils.BytesToShort(actor.rawFile.data.ToArray(), 28), BitCount.Bit16);
            number2 = new("Number 2", Utils.BytesToShort(actor.rawFile.data.ToArray(), 30), BitCount.Bit16);
            number3 = new("Number 3", Utils.BytesToShort(actor.rawFile.data.ToArray(), 32), BitCount.Bit16);
            number4 = new("Number 4", Utils.BytesToShort(actor.rawFile.data.ToArray(), 44), BitCount.Bit16);
            number5 = new("Number 5", Utils.BytesToShort(actor.rawFile.data.ToArray(), 46), BitCount.Bit16);
            number6 = new("Number 6", Utils.BytesToShort(actor.rawFile.data.ToArray(), 48), BitCount.Bit16);
            number7 = new("Number 7", Utils.BytesToShort(actor.rawFile.data.ToArray(), 50), BitCount.Bit16);

            properties = new() {  };
        }


        public void Compile() {

        }

    }

    public class FCopBehavior28 : FCopActorBehavior {
        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

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
        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public RotationActorProperty headRotation;

        public RotationActorProperty baseRotation;


        public FCopBehavior36(FCopActor actor) {
            this.actor = actor;

            headRotation = new("Head Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 64)), BitCount.Bit16);
            baseRotation = new("Base Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 78)), BitCount.Bit16);

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
        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }


        public ValueActorProperty hitboxWidth;
        public ValueActorProperty hitboxHeight;
        public ValueActorProperty number3;
        public ValueActorProperty triggerType;
        public IDReferenceActorProperty actorToTest;

        public FCopBehavior95(FCopActor actor) {
            this.actor = actor;

            hitboxWidth = new ("Hit Box Width", Utils.BytesToShort(actor.rawFile.data.ToArray(), 28), BitCount.Bit16);
            hitboxHeight = new("Hit Box Height", Utils.BytesToShort(actor.rawFile.data.ToArray(), 30), BitCount.Bit16);
            number3 = new("Property 3", Utils.BytesToShort(actor.rawFile.data.ToArray(), 32), BitCount.Bit16);
            triggerType = new("Trigger Type", Utils.BytesToShort(actor.rawFile.data.ToArray(), 34), BitCount.Bit16);
            actorToTest = new("Trigger Actor", Utils.BytesToInt(actor.rawFile.data.ToArray(), 36));

            properties = new() {  };

        }

    }

    public class FCopBehavior99 : FCopActorBehavior {
        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }

        public FCopBehavior99(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            properties = new();

            var propertyCount = (Utils.BytesToInt(rawFile.data.ToArray(), 4) - 28) / 2;

            var offset = 28;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Utils.BytesToShort(rawFile.data.ToArray(), offset), BitCount.Bit16);
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
        public BitCount bitCount { get; set; }

        public int GetCompiledValue();

        public List<byte> Compile() {

            var value = GetCompiledValue();

            return bitCount switch {
                BitCount.Bit8 => new() { (byte)value },
                BitCount.Bit16 => BitConverter.GetBytes((short)value).ToList(),
                BitCount.Bit32 => BitConverter.GetBytes(value).ToList(),
                _ => BitConverter.GetBytes((short)value).ToList(),
            };

        }

    }

    public class ValueActorProperty: ActorProperty { 
        public string name { get; set; }
        public int fileOffset { get; set; }
        public BitCount bitCount { get; set; }

        public int GetCompiledValue() {
            return value;
        }

        public int value;

        public ValueActorProperty(string name, int value, BitCount bitCount) {
            this.name = name;
            this.value = value;
            this.bitCount = bitCount;
        }

    }

    public class IDReferenceActorProperty: ActorProperty {
        public string name { get; set; }
        public int fileOffset { get; set; }
        public BitCount bitCount { get; set; }

        public int GetCompiledValue() {
            return value;
        }

        public int value;

        public IDReferenceActorProperty(string name, int value) {
            this.name = name;
            this.value = value;
        }

    }

    public class EnumDataActorProperty: ActorProperty {
        public string name { get; set; }
        public int fileOffset { get; set; }
        public BitCount bitCount { get; set; }

        public int GetCompiledValue() {
            return (int)(ActorGroundCast)caseValue;
        }

        public Enum caseValue;

        public EnumDataActorProperty(string name, Enum caseValue, BitCount bitCount) {
            this.name = name;
            this.caseValue = caseValue;
            this.bitCount = bitCount;
        }

    }

    public class RangeActorProperty: ActorProperty {
        public string name { get; set; }
        public int fileOffset { get; set; }
        public BitCount bitCount { get; set; }

        public int GetCompiledValue() {
            return value;
        }

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
        public BitCount bitCount { get; set; }

        public ActorRotation value;

        public int GetCompiledValue() {
            return value.compiledRotation;
        }

        public RotationActorProperty(string name, ActorRotation value, BitCount bitCount) {
            this.name = name;
            this.value = value;
            this.bitCount = bitCount;
        }

    }

    public struct ActorRotation {

        public static int maxRotation = 4096;

        public int compiledRotation;

        public float parsedRotation;

        public ActorRotation SetRotationDegree(float newRotation) {

            var negative = newRotation < 0;

            var newAbsRotation = MathF.Abs(newRotation);

            if (newAbsRotation > 180f) {
                parsedRotation = 360f - newAbsRotation;
            }
            if (newAbsRotation < -180f) {
                parsedRotation = newAbsRotation + 360f;
            }

            parsedRotation = newAbsRotation;

            compiledRotation = (int)(newAbsRotation / 360f * maxRotation);

            if (negative) {
                parsedRotation = -parsedRotation;
                compiledRotation = -compiledRotation;
            }

            return this;

        }


        public ActorRotation SetRotationCompiled(int newRotation) {

            var negative = newRotation < 0;

            var newAbsRotation = Math.Abs(newRotation);

            compiledRotation = newAbsRotation;

            float rotationPrecentage = (float)newAbsRotation / (float)maxRotation;

            float degreeRoation = 360f * rotationPrecentage;

            if (degreeRoation > 180f) {
                parsedRotation = 360f - degreeRoation;
            }

            parsedRotation = degreeRoation;

            if (negative) {
                parsedRotation = -parsedRotation;
                compiledRotation = -compiledRotation;
            }

            return this;

        }

        public static ActorRotation operator +(ActorRotation a, float b) {
            return a.SetRotationDegree(a.parsedRotation + b);
        }

    }

    public enum BitCount {
        Bit8 = 8,
        Bit16 = 16,
        Bit32 = 32
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

    public enum ActorGroundCast {
        Highest = 0,
        Lowest = 1,
        Default = 255

    }

}