

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

            var tACTSize = BitConverter.ToInt32(rawFile.data.ToArray(), 4);
            var propertyData = rawFile.data.GetRange(28, tACTSize - 28);

            switch (behaviorType) {
                case ActorBehavior.Player:
                    behavior = new FCopBehavior1(this, propertyData);
                    break;
                case ActorBehavior.PathedEntity:
                    behavior = new FCopBehavior5(this, propertyData);
                    break;
                case ActorBehavior.StationaryEntity: 
                    behavior = new FCopBehavior6(this, propertyData);
                    break;
                case ActorBehavior.Turret:
                    behavior = new FCopBehavior8(this, propertyData);
                    break;
                case ActorBehavior.Aircraft:
                    behavior = new FCopBehavior9(this, propertyData);
                    break;
                case ActorBehavior.Elevator:
                    behavior = new FCopBehavior10(this, propertyData);
                    break;
                case ActorBehavior.DynamicProp:
                    behavior = new FCopBehavior11(this, propertyData);
                    break;
                case ActorBehavior.CollidableProp:
                    behavior = new FCopBehavior12(this, propertyData);
                    break;
                case ActorBehavior.UniversalTrigger:
                    behavior = new FCopBehavior14(this, propertyData);
                    break;
                case ActorBehavior.FloatingItem:
                    behavior = new FCopBehavior16(this, propertyData);
                    break;
                case ActorBehavior.PathedTurret:
                    behavior = new FCopBehavior20(this, propertyData);
                    break;
                case ActorBehavior.MovableProp:
                    behavior = new FCopBehavior25(this, propertyData);
                    break;
                case ActorBehavior.Behavior26:
                    behavior = new FCopBehavior26(this, propertyData);
                    break;
                case ActorBehavior.Behavior27:
                    behavior = new FCopBehavior27(this, propertyData);
                    break;
                case ActorBehavior.Behavior28:
                    behavior = new FCopBehavior28(this, propertyData);
                    break;
                case ActorBehavior.Behavior29:
                    behavior = new FCopBehavior29(this, propertyData);
                    break;
                case ActorBehavior.Behavior30:
                    behavior = new FCopBehavior30(this, propertyData);
                    break;
                case ActorBehavior.Behavior31:
                    behavior = new FCopBehavior31(this, propertyData);
                    break;
                case ActorBehavior.Reloader:
                    behavior = new FCopBehavior32(this, propertyData);
                    break;
                case ActorBehavior.Behavior33:
                    behavior = new FCopBehavior33(this, propertyData);
                    break;
                case ActorBehavior.Behavior34:
                    behavior = new FCopBehavior33(this, propertyData);
                    break;
                case ActorBehavior.Behavior35:
                    behavior = new FCopBehavior35(this, propertyData);
                    break;
                case ActorBehavior.ClaimableTurret:
                    behavior = new FCopBehavior36(this, propertyData);
                    break;
                case ActorBehavior.Behavior37:
                    behavior = new FCopBehavior37(this, propertyData);
                    break;
                case ActorBehavior.Behavior38:
                    behavior = new FCopBehavior38(this, propertyData);
                    break;
                case ActorBehavior.Behavior87:
                    behavior = new FCopBehavior87(this, propertyData);
                    break;
                case ActorBehavior.Behavior88:
                    behavior = new FCopBehavior88(this, propertyData);
                    break;
                case ActorBehavior.Behavior89:
                    behavior = new FCopBehavior89(this, propertyData);
                    break;
                case ActorBehavior.Behavior90:
                    behavior = new FCopBehavior90(this, propertyData);
                    break;
                case ActorBehavior.Behavior91:
                    behavior = new FCopBehavior91(this, propertyData);
                    break;
                case ActorBehavior.Behavior92:
                    behavior = new FCopBehavior92(this, propertyData);
                    break;
                case ActorBehavior.Behavior93:
                    behavior = new FCopBehavior93(this, propertyData);
                    break;
                case ActorBehavior.Behavior94:
                    behavior = new FCopBehavior94(this, propertyData);
                    break;
                case ActorBehavior.Trigger:
                    behavior = new FCopBehavior95(this, propertyData);
                    break;
                case ActorBehavior.StaticProp:
                    behavior = new FCopBehavior96(this, propertyData);
                    break;
                case ActorBehavior.Fog:
                    behavior = new FCopBehavior97(this, propertyData);
                    break;
                case ActorBehavior.Weapon:
                    behavior = new FCopBehavior98(this, propertyData);
                    break;
                case ActorBehavior.PlayerWeapon:
                    behavior = new FCopBehavior99(this, propertyData);
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

            switch (behavior) {
                case ActorBehavior.StationaryEntity:
                    resourceReferences.Add(new Resource("NULL", 0));
                    resourceReferences.Add(new Resource("NULL", 0));

                    rawFile = new IFFDataFile(3, new(), "Cact", id, nullRPNSRef);
                    this.behavior = new FCopBehavior6(this, Enumerable.Repeat((byte)0, FCopBehavior6.blocks * 2).ToList());
                    break;
            }

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

        FCopActor actor;
        public List<byte> propertyData;

        public List<ActorProperty> properties;
        public Dictionary<string, ActorProperty> propertiesByName = new();
        public Dictionary<string, List<ActorProperty>> propertiesByCommonName = new();

        public bool refuseCompile = false;

        public FCopActorBehavior(FCopActor actor, List<byte> propertyData) {
            this.actor = actor;
            this.propertyData = propertyData;

            properties = new();

        }

        protected int offset = 0;

        protected int Read16() {

            var value = BitConverter.ToInt16(propertyData.ToArray(), offset);
            offset += 2;
            return value;

        }

        protected int Read8() {

            var value = propertyData[offset];
            offset += 1;
            return value;

        }

        public void InitPropertiesByName() {

            foreach (var prop in properties) {
                propertiesByName[prop.name] = prop;

                if (prop is FillerActorProperty) {
                    continue;
                }

                if (propertiesByCommonName.ContainsKey(prop.commonName)) {
                    propertiesByCommonName[prop.commonName].Add(prop);
                }
                else {
                    propertiesByCommonName[prop.commonName] = new List<ActorProperty>() { prop };
                }

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

        public FCopEntity(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(InitTags());

            properties.AddRange(InitEntityProperties());

        }

        List<ActorProperty> InitTags() {

            var data = propertyData.GetRange(offset, 4);

            var total = new List<ActorProperty>() {
                new ToggleActorProperty("disableTargeting", (data[0] & 0x01) == 0x01, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown3", (data[0] & 0x02) == 0x02, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Disable Collision", (data[0] & 0x04) == 0x04, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown2", (data[0] & 0x08) == 0x08, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown1", (data[0] & 0x10) == 0x10, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Disable Rendering", (data[0] & 0x20) == 0x20, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Player Physics", (data[0] & 0x40) == 0x40, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Is Invincible", (data[0] & 0x80) == 0x80, BitCount.Bit1, "Entity Tags"),

                new ToggleActorProperty("unknown9", (data[1] & 0x01) == 0x01, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("hurtBySameTeam", (data[1] & 0x02) == 0x02, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown8", (data[1] & 0x04) == 0x04, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown7", (data[1] & 0x08) == 0x08, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Disable Destroyed Collision", (data[1] & 0x10) == 0x10, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown6", (data[1] & 0x20) == 0x20, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown5", (data[1] & 0x40) == 0x40, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown4", (data[1] & 0x80) == 0x80, BitCount.Bit1, "Entity Tags"),

                new ToggleActorProperty("unknown14", (data[2] & 0x01) == 0x01, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("disableTeam", (data[2] & 0x02) == 0x02, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Disable Explosion", (data[2] & 0x04) == 0x04, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Has Shadow", (data[2] & 0x08) == 0x08, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown13", (data[2] & 0x10) == 0x10, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown12", (data[2] & 0x20) == 0x20, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown11", (data[2] & 0x40) == 0x40, BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("unknown10", (data[2] & 0x80) == 0x80, BitCount.Bit1, "Entity Tags"),

                new FillerActorProperty(0, BitCount.Bit8)

            };

            offset += 4;

            return total;

        }

        List<ActorProperty> InitEntityProperties() {

            return new List<ActorProperty>() {
                new ValueActorProperty("Health", Read16(), BitCount.Bit16, "Entity Properties"),
                new ValueActorProperty("Collide Damage", Read16(), BitCount.Bit16, "Entity Properties"),
                new ValueActorProperty("Team (Unknown)", Read16(), BitCount.Bit16, "Entity Properties"),
                new EnumDataActorProperty("Map Icon Color", (MapIconColor)Read8(), BitCount.Bit8, "Entity Properties"),
                new ValueActorProperty("u_unknown15", Read8(), BitCount.Bit8, "Entity Properties"),
                new ValueActorProperty("Explosion (Unknown)", Read16(), BitCount.Bit16, "Entity Properties"),
                new ValueActorProperty("UV Offset", Read16(), BitCount.Bit16, "Entity Properties")
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

        public FCopShooter(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("Weapon ID", Read16(), BitCount.Bit16, "Shooter Properties"),
                new ValueActorProperty("FOV and Fire alts?", Read8(), BitCount.Bit8, "Shooter Properties"),
                new ValueActorProperty("u_weaponCollision", Read8(), BitCount.Bit8, "Shooter Properties"),
                new ValueActorProperty("unknown5 - 48", Read8(), BitCount.Bit8, "Shooter Properties"),
                new ValueActorProperty("unknown6 - 49", Read8(), BitCount.Bit8, "Shooter Properties"),
                new ValueActorProperty("attack? - 50", Read16(), BitCount.Bit16, "Shooter Properties"),
                new ValueActorProperty("unknown7 - 52", Read8(), BitCount.Bit8, "Shooter Properties"),
                new ValueActorProperty("unknown8 - 53", Read8(), BitCount.Bit8, "Shooter Properties"),
                new ValueActorProperty("unknown9 - 54", Read8(), BitCount.Bit8, "Shooter Properties"),
                new ValueActorProperty("unknown10 - 55", Read8(), BitCount.Bit8, "Shooter Properties"),
                new ValueActorProperty("Engage Range", Read16(), BitCount.Bit16, "Shooter Properties"),
                new ValueActorProperty("unknown11 - 58", Read16(), BitCount.Bit16, "Shooter Properties"),
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

    // - Parsed -
    public class FCopBehavior1 : FCopEntity, FCopObjectMutating {

        public FCopBehavior1(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            // FIXME: for some odd reason players facing can be negative. Allow the property to be negative
            properties.Add(new RotationActorProperty("Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Y, new int[] { 0, 2, 3, 4, 5 }));

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

        public FCopBehavior5(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior6 : FCopShooter, FCopHeightOffseting, FCopObjectMutating {

        public const int assetRefCount = 2;
        public const int blocks = 24;

        public int heightMultiplier { get; set; }

        public FCopBehavior6(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            heightMultiplier = 512;

            assetRefNames = new string[] { "Object", "Destroyed Object" };
            assetRefType = new AssetType[] { AssetType.Object, AssetType.Object };

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read8(), BitCount.Bit8),
                new ValueActorProperty("6_unknown1", Read8(), BitCount.Bit8),
                new ValueActorProperty("6_unknown2", Read16(), BitCount.Bit16),
                new RotationActorProperty("Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Y, new int[] { 0 }),
                new ValueActorProperty("Height Offset", Read16(), BitCount.Bit16),
                new ValueActorProperty("6_unknown3", Read8(), BitCount.Bit8),
                new ValueActorProperty("Turn Speed", Read8(), BitCount.Bit8),
                new ValueActorProperty("6_unknown5", Read16(), BitCount.Bit16),
                new ValueActorProperty("Turn Type", Read16(), BitCount.Bit16),
                new FillerActorProperty(0, BitCount.Bit16)
            });

            InitPropertiesByName();

        }

        public int GetHeight() {
            return propertiesByName["Height Offset"].GetCompiledValue();
        }

        public ActorGroundCast GetGroundCast() {
            return ActorGroundCast.Highest;
        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { (RotationActorProperty)propertiesByName["Rotation"] };
        }

    }

    public class FCopBehavior8 : FCopShooter, FCopObjectMutating {

        public FCopBehavior8(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            assetRefNames = new string[] { "Head Object", "Object", "Base Object", "Destroyed Object" };
            assetRefType = new AssetType[] { AssetType.Object, AssetType.Object, AssetType.Object, AssetType.Object };

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("unknown10", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown11", Read16(), BitCount.Bit16),
                new RotationActorProperty("Head Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Y, new int[] { 0 }),
                new ValueActorProperty("Height Offset", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown12", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown13", Read16(), BitCount.Bit16),
                new ValueActorProperty("unknown14", Read16(), BitCount.Bit16),
                new FillerActorProperty(0, BitCount.Bit16),
                new ValueActorProperty("unknown16", Read16(), BitCount.Bit16),
                new RotationActorProperty("Base Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Y, new int[] { 2 })
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

        public FCopBehavior9(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    // - Parsed -
    public class FCopBehavior10 : FCopEntity, FCopHeightOffseting, FCopObjectMutating {

        public int heightMultiplier { get; set; }

        public FCopBehavior10(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            heightMultiplier = 8192;

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Number Of Stops", (ElevatorStops)Read8(), BitCount.Bit8),
                new EnumDataActorProperty("Starting Position", (ElevatorStartingPoint)Read8(), BitCount.Bit8),
                new ValueActorProperty("1st Height Offset", Read16(), BitCount.Bit16),
                new ValueActorProperty("2nt Height Offset", Read16(), BitCount.Bit16),
                new ValueActorProperty("3rd Height Offset", Read16(), BitCount.Bit16),
                new ValueActorProperty("1st Stop Time", Read16(), BitCount.Bit16),
                new ValueActorProperty("2nt Stop Time", Read16(), BitCount.Bit16),
                new ValueActorProperty("3rd Stop Time", Read16(), BitCount.Bit16),
                new ValueActorProperty("Up Speed", Read16(), BitCount.Bit16),
                new ValueActorProperty("Down Speed", Read16(), BitCount.Bit16),
                new RotationActorProperty("Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Y, new int[] { 0 }),
                new EnumDataActorProperty("Trigger Type", (ElevatorTrigger)Read8(), BitCount.Bit8),
                new EnumDataActorProperty("Tile Effect", (TileEffectType)Read8(), BitCount.Bit8),
                new ValueActorProperty("End Sound", Read16(), BitCount.Bit16),

            });

            InitPropertiesByName();

        }

        public int GetHeight() {
            return propertiesByName["1st Height Offset"].GetCompiledValue();
        }

        public ActorGroundCast GetGroundCast() {
            return ActorGroundCast.Highest;
        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { (RotationActorProperty)propertiesByName["Rotation"] };
        }

    }

    // - Completed -
    public class FCopBehavior11 : FCopEntity, FCopHeightOffseting, FCopObjectMutating {

        public const int assetRefCount = 2;
        public const int blocks = 12;

        public int heightMultiplier { get; set; }

        public FCopBehavior11(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            heightMultiplier = 8192;

            assetRefNames = new string[] { "Object", "Destroyed Object" };
            assetRefType = new AssetType[] { AssetType.Object, AssetType.Object };

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read16(), BitCount.Bit16),
                new RotationActorProperty("Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Y, new int[] { 0, 1 }),
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

    // - Parsed -
    public class FCopBehavior12 : FCopEntity, FCopHeightOffseting, FCopObjectMutating {

        public int heightMultiplier { get; set; }

        public FCopBehavior12(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            heightMultiplier = 512;

            properties.AddRange(new List<ActorProperty>() {
                new RotationActorProperty("Rotation Y", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Y, new int[] { 0 }),
                new RotationActorProperty("Rotation X", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.X, new int[] { 0 }),
                new ValueActorProperty("Height Offset", Read16(), BitCount.Bit16),
                new EnumDataActorProperty("Tile Effect", (TileEffectType)Read8(), BitCount.Bit8),
                new FillerActorProperty(0, BitCount.Bit8)
            });

            InitPropertiesByName();

        }

        public int GetHeight() {
            return propertiesByName["Height Offset"].GetCompiledValue();
        }

        public ActorGroundCast GetGroundCast() {
            return ActorGroundCast.Highest;
        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { 
                (RotationActorProperty)propertiesByName["Rotation Y"],
                (RotationActorProperty)propertiesByName["Rotation X"],
            };
        }

    }

    public class FCopBehavior14 : FCopActorBehavior {

        public FCopBehavior14(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior16 : FCopEntity {

        public FCopBehavior16(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior20 : FCopShooter {

        public FCopBehavior20(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior25 : FCopEntity {

        public FCopBehavior25(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior26 : FCopShooter {

        public FCopBehavior26(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior27 : FCopShooter {

        public FCopBehavior27(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior28 : FCopShooter {

        public FCopBehavior28(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior29 : FCopActorBehavior {

        public FCopBehavior29(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior30 : FCopEntity {

        public FCopBehavior30(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior31 : FCopEntity {

        public FCopBehavior31(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior32 : FCopEntity {

        public FCopBehavior32(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior33 : FCopEntity {

        public FCopBehavior33(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior34 : FCopActorBehavior {

        public FCopBehavior34(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior35 : FCopActorBehavior {

        public FCopBehavior35(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior36 : FCopShooter, FCopObjectMutating {

        public FCopBehavior36(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("nt_unknown0", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown1", Read16(), BitCount.Bit16),
                new RotationActorProperty("Head Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Y, new int[] { 0 }),
                new ValueActorProperty("nt_unknown2", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown3", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown4", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown5", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown6", Read16(), BitCount.Bit16),
                new ValueActorProperty("nt_unknown7", Read16(), BitCount.Bit16),
                new RotationActorProperty("Base Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Y, new int[] { 2 }),
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

        public FCopBehavior37(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior38 : FCopEntity {

        public FCopBehavior38(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior87 : FCopActorBehavior {

        public FCopBehavior87(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior88 : FCopActorBehavior {

        public FCopBehavior88(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior89 : FCopActorBehavior {

        public FCopBehavior89(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior90 : FCopActorBehavior {

        public FCopBehavior90(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior91 : FCopActorBehavior {

        public FCopBehavior91(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior92 : FCopActorBehavior {

        public FCopBehavior92(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior93 : FCopActorBehavior {

        public FCopBehavior93(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior94 : FCopActorBehavior {

        public FCopBehavior94(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior95 : FCopActorBehavior {

        public FCopBehavior95(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            refuseCompile = true;

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("Hit Box Width", Read16(), BitCount.Bit16),
                new ValueActorProperty("Hit Box Height", Read16(), BitCount.Bit16),
                new ValueActorProperty("Property 3", Read16(), BitCount.Bit16),
                new ValueActorProperty("Trigger Type", Read16(), BitCount.Bit16),
                new ValueActorProperty("Trigger Actor", Read16(), BitCount.Bit16)
            });

        }

    }

    public class FCopBehavior96 : FCopActorBehavior, FCopObjectMutating, FCopHeightOffseting {

        public int heightMultiplier { get; set; }

        public FCopBehavior96(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            assetRefNames = new string[] { "Object" };
            assetRefType = new AssetType[] { AssetType.Object };

            heightMultiplier = 512;

            properties = new() {
                new RotationActorProperty("Rotation Y", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Y, new int[] { 0 }),
                new RotationActorProperty("Rotation Z", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.Z, new int[] { 0 }),
                new RotationActorProperty("Rotation X", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, Axis.X, new int[] { 0 }),
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
            return new RotationActorProperty[] { 
                (RotationActorProperty)propertiesByName["Rotation Y"],
                (RotationActorProperty)propertiesByName["Rotation Z"],
                (RotationActorProperty)propertiesByName["Rotation X"]
            };
        }

    }

    public class FCopBehavior97 : FCopActorBehavior {

        public FCopBehavior97(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior98 : FCopActorBehavior {

        public FCopBehavior98(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior99 : FCopActorBehavior {

        public FCopBehavior99(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

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
        Behavior38 = 38,
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