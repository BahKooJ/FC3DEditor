

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public ActorBehavior behaviorType;
        public int x;
        public int y;

        public List<Resource> resourceReferences = new();

        public FCopActorBehavior behavior;

        public List<byte> tSACData = null;

        public FCopActor(IFFDataFile rawFile): base(rawFile) {

            name = "Actor " + DataID;

            FindStartChunkOffset();

            behaviorType = (ActorBehavior)Utils.BytesToInt(rawFile.data.ToArray(), 12);
            y = Utils.BytesToInt(rawFile.data.ToArray(), 16);
            x = Utils.BytesToInt(rawFile.data.ToArray(), 24);

            ParseResourceReferences();

            switch (behaviorType) {
                case ActorBehavior.Player:
                    behavior = new FCopBehavior1(this);
                    break;
                case ActorBehavior.PathedEntity:
                    behavior = new FCopBehavior5(this);
                    break;
                case ActorBehavior.StationaryEntity: 
                    behavior = new FCopBehavior6(this);
                    break;
                case ActorBehavior.Turret:
                    behavior = new FCopBehavior8(this);
                    break;
                case ActorBehavior.Aircraft:
                    behavior = new FCopBehavior9(this);
                    break;
                case ActorBehavior.Elevator:
                    behavior = new FCopBehavior10(this);
                    break;
                case ActorBehavior.DynamicProp:
                    behavior = new FCopBehavior11(this);
                    break;
                case ActorBehavior.CollidableProp:
                    behavior = new FCopBehavior12(this);
                    break;
                case ActorBehavior.UniversalTrigger:
                    behavior = new FCopBehavior14(this);
                    break;
                case ActorBehavior.FloatingItem:
                    behavior = new FCopBehavior16(this);
                    break;
                case ActorBehavior.PathedTurret:
                    behavior = new FCopBehavior20(this);
                    break;
                case ActorBehavior.MovableProp:
                    behavior = new FCopBehavior25(this);
                    break;
                case ActorBehavior.Behavior26:
                    behavior = new FCopBehavior26(this);
                    break;
                case ActorBehavior.Behavior27:
                    behavior = new FCopBehavior27(this);
                    break;
                case ActorBehavior.Behavior28:
                    behavior = new FCopBehavior28(this);
                    break;
                case ActorBehavior.Behavior30:
                    behavior = new FCopBehavior30(this);
                    break;
                case ActorBehavior.Behavior31:
                    behavior = new FCopBehavior31(this);
                    break;
                case ActorBehavior.Reloader:
                    behavior = new FCopBehavior32(this);
                    break;
                case ActorBehavior.Behavior33:
                    behavior = new FCopBehavior33(this);
                    break;
                case ActorBehavior.ClaimableTurret:
                    behavior = new FCopBehavior36(this);
                    break;
                case ActorBehavior.Behavior37:
                    behavior = new FCopBehavior37(this);
                    break;
                case ActorBehavior.Trigger:
                    behavior = new FCopBehavior95(this);
                    break;
                case ActorBehavior.StaticProp:
                    behavior = new FCopBehavior96(this);
                    break;
                case ActorBehavior.PlayerWeapon:
                    behavior = new FCopBehavior99(this);
                    break;
            }

            var tSAC = offsets.FirstOrDefault(h => h.fourCCDeclaration == FourCC.tSAC);

            if (tSAC != null) {
                tSACData = rawFile.data.GetRange(tSAC.index, tSAC.chunkSize);
            }

        }

        public FCopActor(int id, int nullRPNSRef, ActorBehavior behavior, int x, int y) : base(null) {

            name = "Actor " + id;

            behaviorType = behavior;
            this.x = x;
            this.y = y;

            //switch (behavior) {
            //    case ActorBehavior.DynamicProp:
            //        resourceReferences.Add(new Resource("NULL", 0));
            //        resourceReferences.Add(new Resource("NULL", 0));

            //        rawFile = new IFFDataFile(3, new(), "Cact", id, nullRPNSRef);
            //        this.behavior = new FCopBehavior11();
            //        break;
            //}

        }

        public IFFDataFile Compile() {

            var total = new List<byte>();

            total.AddRange(BitConverter.GetBytes(DataID));
            total.AddRange(BitConverter.GetBytes((int)behaviorType));
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

            if (totalWithHeader.Count != rawFile.data.Count && rawFile.data.Count != 0) {
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

    public class FCopActorBehavior {

        public int expectedRawFileSize;
        public string[] assetRefNames;
        public AssetType[] assetRefType;

        public FCopActor actor;

        public List<ActorProperty> properties;
        public Dictionary<string, ActorProperty> propertiesByName = new();

        public bool refuseCompile = false;

        public FCopActorBehavior(FCopActor actor) {
            this.actor = actor;

            properties = new();

        }

        // 28 is the offset to the properties, it skips the fourCC and coordinates.
        protected int offset = 28;

        protected int Read16() {

            var value = BitConverter.ToInt16(actor.rawFile.data.ToArray(), offset);
            offset += 2;
            return value;

        }

        protected int Read8() {

            var value = actor.rawFile.data[offset];
            offset += 1;
            return value;

        }

        public void InitPropertiesByName() {

            foreach (var prop in properties) {
                propertiesByName[prop.name] = prop;
            }

        }

        public List<byte> Compile() {

            if (refuseCompile) {
                return null;
            }

            var total = new List<byte>();

            var floatingBits = new List<BitNumber>();

            foreach (var p in properties) {

                var bytes = p.Compile();

                if (bytes == null) {

                    switch (p.bitCount) {
                        case BitCount.Bit1:
                            floatingBits.Add(new BitNumber(1, p.GetCompiledValue()));
                            break;
                    }

                    var totalBitCount = 0;
                    foreach (var bits in floatingBits) {
                        totalBitCount += bits.bitCount;
                    }

                    if (totalBitCount % 8 == 0) {
                        var bitfield = new BitField(totalBitCount, floatingBits);

                        total.AddRange(Utils.BitArrayToByteArray(bitfield.Compile()));

                        floatingBits.Clear();

                    }

                }
                else {

                    total.AddRange(bytes);

                }

            }

            return total;

        }

    }

    public abstract class FCopEntity: FCopActorBehavior {

        public FCopEntity(FCopActor actor) : base(actor) {

            properties.AddRange(InitTags());

            properties.AddRange(InitEntityProperties());

        }

        List<ActorProperty> InitTags() {

            var data = actor.rawFile.data.GetRange(offset, 4);

            var total = new List<ActorProperty>() {
                new ToggleActorProperty("disableTargeting", (data[0] & 0x01) == 0x01, BitCount.Bit1),
                new ToggleActorProperty("unknown3", (data[0] & 0x02) == 0x02, BitCount.Bit1),
                new ToggleActorProperty("Disable Collision", (data[0] & 0x04) == 0x04, BitCount.Bit1),
                new ToggleActorProperty("unknown2", (data[0] & 0x08) == 0x08, BitCount.Bit1),
                new ToggleActorProperty("unknown1", (data[0] & 0x10) == 0x10, BitCount.Bit1),
                new ToggleActorProperty("Disable Rendering", (data[0] & 0x20) == 0x20, BitCount.Bit1),
                new ToggleActorProperty("Player Physics", (data[0] & 0x40) == 0x40, BitCount.Bit1),
                new ToggleActorProperty("Is Invincible", (data[0] & 0x80) == 0x80, BitCount.Bit1),

                new ToggleActorProperty("unknown9", (data[1] & 0x01) == 0x01, BitCount.Bit1),
                new ToggleActorProperty("hurtBySameTeam", (data[1] & 0x02) == 0x02, BitCount.Bit1),
                new ToggleActorProperty("unknown8", (data[1] & 0x04) == 0x04, BitCount.Bit1),
                new ToggleActorProperty("unknown7", (data[1] & 0x08) == 0x08, BitCount.Bit1),
                new ToggleActorProperty("Disable Destroyed Collision", (data[1] & 0x10) == 0x10, BitCount.Bit1),
                new ToggleActorProperty("unknown6", (data[1] & 0x20) == 0x20, BitCount.Bit1),
                new ToggleActorProperty("unknown5", (data[1] & 0x40) == 0x40, BitCount.Bit1),
                new ToggleActorProperty("unknown4", (data[1] & 0x80) == 0x80, BitCount.Bit1),

                new ToggleActorProperty("unknown14", (data[2] & 0x01) == 0x01, BitCount.Bit1),
                new ToggleActorProperty("disableTeam", (data[2] & 0x02) == 0x02, BitCount.Bit1),
                new ToggleActorProperty("Disable Explosion", (data[2] & 0x04) == 0x04, BitCount.Bit1),
                new ToggleActorProperty("Has Shadow", (data[2] & 0x08) == 0x08, BitCount.Bit1),
                new ToggleActorProperty("unknown13", (data[2] & 0x10) == 0x10, BitCount.Bit1),
                new ToggleActorProperty("unknown12", (data[2] & 0x20) == 0x20, BitCount.Bit1),
                new ToggleActorProperty("unknown11", (data[2] & 0x40) == 0x40, BitCount.Bit1),
                new ToggleActorProperty("unknown10", (data[2] & 0x80) == 0x80, BitCount.Bit1),

                new FillerActorProperty(0, BitCount.Bit8)

            };

            offset += 4;

            return total;

        }

        List<ActorProperty> InitEntityProperties() {

            return new List<ActorProperty>() {
                new ValueActorProperty("Health", Read16(), BitCount.Bit16),
                new ValueActorProperty("Collide Damage", Read16(), BitCount.Bit16),
                new ValueActorProperty("Team (Unknown)", Read16(), BitCount.Bit16),
                new EnumDataActorProperty("Map Icon Color", (MapIconColor)Read8(), BitCount.Bit8),
                new ValueActorProperty("u_unknown15", Read8(), BitCount.Bit8),
                new ValueActorProperty("Explosion (Unknown)", Read16(), BitCount.Bit16),
                new ValueActorProperty("UV Offset", Read16(), BitCount.Bit16)
            };

        }

        public int GetUVOffset() {
            try {
                return propertiesByName["UV Offset"].GetCompiledValue();
            } catch {
                return 0;
            }
        }

    }

    public abstract class FCopShooter : FCopEntity {

        public FCopShooter(FCopActor actor) : base(actor) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("Weapon ID", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown4", Read8(), BitCount.Bit8),
                new ValueActorProperty("u_weaponCollision", Read8(), BitCount.Bit8),
                new ValueActorProperty("unknown5 - 48", Read8(), BitCount.Bit8),
                new ValueActorProperty("unknown6 - 49", Read8(), BitCount.Bit8),
                new ValueActorProperty("attack? - 50", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown7 - 52", Read8(), BitCount.Bit8),
                new ValueActorProperty("unknown8 - 53", Read8(), BitCount.Bit8),
                new ValueActorProperty("unknown9 - 54", Read8(), BitCount.Bit8),
                new ValueActorProperty("unknown10 - 55", Read8(), BitCount.Bit8),
                new ValueActorProperty("Engage Range", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown11 - 58", Read16(), BitCount.Bit16),
            });

        }

    }

    public interface FCopObjectMutating {

        public RotationActorProperty[] GetRotations();

    }

    public interface FCopHeightOffseting {

        public int heightMultiplier { get; set; }

        public int GetHeight();

        public ActorGroundCast GetGroundCast();

    }

    public class FCopBehavior1 : FCopEntity, FCopObjectMutating {

        public FCopBehavior1(FCopActor actor) : base(actor) {

            // FIXME: for some odd reason players facing can be negative. Allow the property to be negative
            properties.Add(new RotationActorProperty("Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 0, 2, 3, 4, 5 }));

            // Implies ground cast but Future Cop won't react except with 0x01 which will crash. Leaving at default 0xFF
            properties.Add(new FillerActorProperty(0, BitCount.Bit8));
            properties.Add(new FillerActorProperty(0xFF, BitCount.Bit8));

            InitPropertiesByName();
        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { (RotationActorProperty)propertiesByName["Rotation"] };
        }

    }

    public class FCopBehavior5 : FCopShooter {

        public FCopBehavior5(FCopActor actor): base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior6 : FCopShooter {

        public FCopBehavior6(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior8 : FCopShooter, FCopObjectMutating {

        public FCopBehavior8(FCopActor actor) : base(actor) {

            assetRefNames = new string[] { "Head Object", "Object", "Base Object", "Destroyed Object" };
            assetRefType = new AssetType[] { AssetType.Object, AssetType.Object, AssetType.Object, AssetType.Object };

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("unknown10", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown11", Read16(), BitCount.Bit16),
                new RotationActorProperty("Head Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 0 }),
                new ValueActorProperty("Height Offset", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown12", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown13", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown14", Read16(), BitCount.Bit16),
                new FillerActorProperty(0, BitCount.Bit16),
                new ValueActorProperty("unknown16", Read16(), BitCount.Bit16),
                new RotationActorProperty("Base Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 2 })
            });

            InitPropertiesByName();

        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { 
                (RotationActorProperty)propertiesByName["Head Rotation"], 
                (RotationActorProperty)propertiesByName["Base Rotation"] 
            };
        }

    }

    public class FCopBehavior9 : FCopShooter {

        public FCopBehavior9(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior10 : FCopEntity {

        public FCopBehavior10(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior11 : FCopEntity, FCopHeightOffseting, FCopObjectMutating {

        public int heightMultiplier { get; set; }

        public FCopBehavior11(FCopActor actor) : base(actor) {

            heightMultiplier = 8192;

            assetRefNames = new string[] { "Object", "Destroyed Object" };
            assetRefType = new AssetType[] { AssetType.Object, AssetType.Object };

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read16(), BitCount.Bit16),
                new RotationActorProperty("Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 0, 1 }),
                new ValueActorProperty("Height Offset", Read16(), BitCount.Bit16),
                new FillerActorProperty(0, BitCount.Bit16)
            });

            InitPropertiesByName();

        }

        public int GetHeight() {
            return propertiesByName["Height Offset"].GetCompiledValue();
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)((EnumDataActorProperty)propertiesByName["Ground Cast"]).caseValue;
        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { (RotationActorProperty)propertiesByName["Rotation"] };
        }

    }

    public class FCopBehavior12 : FCopEntity {

        public FCopBehavior12(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior14 : FCopActorBehavior {

        public FCopBehavior14(FCopActor actor): base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior16 : FCopEntity {

        public FCopBehavior16(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior20 : FCopShooter {

        public FCopBehavior20(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior25 : FCopEntity {

        public FCopBehavior25(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior26 : FCopShooter {

        public FCopBehavior26(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior27 : FCopShooter {

        public FCopBehavior27(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior28 : FCopShooter {

        public FCopBehavior28(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior30 : FCopEntity {

        public FCopBehavior30(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior31 : FCopEntity {

        public FCopBehavior31(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior32 : FCopEntity {

        public FCopBehavior32(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior33 : FCopEntity {

        public FCopBehavior33(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior36 : FCopShooter, FCopObjectMutating {

        public FCopBehavior36(FCopActor actor) : base(actor) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("nt_unknown0", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown1", Read16(), BitCount.Bit16),
                new RotationActorProperty("Head Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 0 }),
                new ValueActorProperty("nt_unknown2", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown3", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown4", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown5", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown6", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown7", Read16(), BitCount.Bit16),
                new RotationActorProperty("Base Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 2 }),
                new ValueActorProperty("nt_unknown8", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown9", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown10", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown11", Read16(), BitCount.Bit16)
            });

            InitPropertiesByName();

        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] {
                (RotationActorProperty)propertiesByName["Head Rotation"],
                (RotationActorProperty)propertiesByName["Base Rotation"]
            };
        }

    }

    public class FCopBehavior37 : FCopEntity {

        public FCopBehavior37(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior95 : FCopActorBehavior {

        public FCopBehavior95(FCopActor actor): base(actor) {

            refuseCompile = true;

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("Hit Box Width", Utils.BytesToShort(actor.rawFile.data.ToArray(), 28), BitCount.Bit16),
                new ValueActorProperty("Hit Box Height", Utils.BytesToShort(actor.rawFile.data.ToArray(), 30), BitCount.Bit16),
                new ValueActorProperty("Property 3", Utils.BytesToShort(actor.rawFile.data.ToArray(), 32), BitCount.Bit16),
                new ValueActorProperty("Trigger Type", Utils.BytesToShort(actor.rawFile.data.ToArray(), 34), BitCount.Bit16),
                new ValueActorProperty("Trigger Actor", Utils.BytesToInt(actor.rawFile.data.ToArray(), 36), BitCount.Bit16)
            });

        }

    }

    public class FCopBehavior96 : FCopActorBehavior, FCopObjectMutating, FCopHeightOffseting {

        public int heightMultiplier { get; set; }

        public FCopBehavior96(FCopActor actor) : base(actor) {

            assetRefNames = new string[] { "Object" };
            assetRefType = new AssetType[] { AssetType.Object };

            heightMultiplier = 512;

            properties = new() {
                new RotationActorProperty("Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 0 }),
                new ValueActorProperty("unknown2", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown3", Read16(), BitCount.Bit16),
                new ValueActorProperty("Height Offset", Read16(), BitCount.Bit16),
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read8(), BitCount.Bit8),
                new ValueActorProperty("unknown5", Read8(), BitCount.Bit8),
                new ValueActorProperty("unknown6", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown7", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown8", Read16(), BitCount.Bit16)
            };

            InitPropertiesByName();

        }

        public int GetHeight() {
            return propertiesByName["Height Offset"].GetCompiledValue();
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)((EnumDataActorProperty)propertiesByName["Ground Cast"]).caseValue;
        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { (RotationActorProperty)propertiesByName["Rotation"] };
        }

    }

    public class FCopBehavior99 : FCopActorBehavior {

        public FCopBehavior99(FCopActor actor) : base(actor) {
            refuseCompile = true;
        }

    }

    public enum ActorBehavior {

        Player = 1,
        PathedEntity = 5,
        StationaryEntity = 6,
        Turret = 8,
        Aircraft = 9,
        Elevator = 10,
        DynamicProp = 11,
        CollidableProp = 12,
        UniversalTrigger = 14,
        FloatingItem = 16,
        PathedTurret = 20,
        MovableProp = 25,
        Behavior26 = 26,
        Behavior27 = 27,
        Behavior28 = 28,
        Behavior29 = 29,
        Behavior30 = 30,
        Behavior31 = 31,
        Reloader = 32,
        Behavior33 = 33,
        Behavior34 = 34,
        Behavior35 = 35,
        ClaimableTurret = 36,
        Behavior37 = 37,
        Behavior87 = 87,
        Behavior88 = 88,
        Behavior89 = 89,
        Behavior90 = 90,
        Behavior91 = 91,
        Behavior92 = 92,
        Behavior93 = 93,
        Behavior94 = 94,
        Trigger = 95,
        StaticProp = 96,
        Fog = 97,
        Weapon = 98,
        PlayerWeapon = 99

    }

}