

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
                case ActorBehavior.Reloader:
                    behavior = new FCopBehavior32(this);
                    break;
                case ActorBehavior.Behavior33:
                    behavior = new FCopBehavior33(this);
                    break;
                case ActorBehavior.Behavior36:
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

            switch (behavior) {
                case ActorBehavior.DynamicProp:
                    resourceReferences.Add(new Resource("NULL", 0));
                    resourceReferences.Add(new Resource("NULL", 0));

                    rawFile = new IFFDataFile(3, new(), "Cact", id, nullRPNSRef);
                    this.behavior = new FCopBehavior11();
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

    public abstract class FCopEntity: FCopActorBehavior {

        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }
        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }

        // - Tags -
        public ToggleActorProperty isInvincible;
        public ToggleActorProperty playerPhysics;
        public ToggleActorProperty disableRendering;
        public ToggleActorProperty unknown1;
        public ToggleActorProperty unknown2;
        public ToggleActorProperty disableAllCollision;
        public ToggleActorProperty unknown3;
        public ToggleActorProperty disableTargeting;

        public ToggleActorProperty unknown4;
        public ToggleActorProperty unknown5;
        public ToggleActorProperty unknown6;
        public ToggleActorProperty disableDestroyedActorCollision;
        public ToggleActorProperty unknown7;
        public ToggleActorProperty unknown8;
        public ToggleActorProperty hurtBySameTeam;
        public ToggleActorProperty unknown9;

        public ToggleActorProperty unknown10;
        public ToggleActorProperty unknown11;
        public ToggleActorProperty unknown12;
        public ToggleActorProperty unknown13;
        public ToggleActorProperty shadows;
        public ToggleActorProperty disableExplosionEffects;
        public ToggleActorProperty disableTeam;
        public ToggleActorProperty unknown14;

        // - Properties -
        public ValueActorProperty health;
        public ValueActorProperty collideDamage;
        public ValueActorProperty u_team;
        public EnumDataActorProperty mapIconColor;
        public ValueActorProperty u_unknown15;
        public ValueActorProperty u_explosion;
        public ValueActorProperty uvOffset;

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

        public FCopEntity(FCopActor actor) {
            this.actor = actor;

            properties = new();

            properties.AddRange(InitTags());

            properties.AddRange(InitEntityProperties());

        }

        public FCopEntity() {

            properties = new();

            properties.AddRange(InitEmptyTags());

            properties.AddRange(InitEmptyEntityProperties());


        }

        List<ActorProperty> InitTags() {

            var data = actor.rawFile.data.GetRange(offset, 4);

            isInvincible = new("Is Invincible", (data[0] & 0x80) == 0x80, BitCount.Bit1);
            playerPhysics = new("Player Physics", (data[0] & 0x40) == 0x40, BitCount.Bit1);
            disableRendering = new("Disable Rendering", (data[0] & 0x20) == 0x20, BitCount.Bit1);
            unknown1 = new("unknown1", (data[0] & 0x10) == 0x10, BitCount.Bit1);
            unknown2 = new("unknown2", (data[0] & 0x08) == 0x08, BitCount.Bit1);
            disableAllCollision = new("Disable Collision", (data[0] & 0x04) == 0x04, BitCount.Bit1);
            unknown3 = new("unknown3", (data[0] & 0x02) == 0x02, BitCount.Bit1);
            disableTargeting = new("disableTargeting", (data[0] & 0x01) == 0x01, BitCount.Bit1);

            unknown4 = new("unknown4", (data[1] & 0x80) == 0x80, BitCount.Bit1);
            unknown5 = new("unknown5", (data[1] & 0x40) == 0x40, BitCount.Bit1);
            unknown6 = new("unknown6", (data[1] & 0x20) == 0x20, BitCount.Bit1);
            disableDestroyedActorCollision = new("Disable Destroyed Collision", (data[1] & 0x10) == 0x10, BitCount.Bit1);
            unknown7 = new("unknown7", (data[1] & 0x08) == 0x08, BitCount.Bit1);
            unknown8 = new("unknown8", (data[1] & 0x04) == 0x04, BitCount.Bit1);
            hurtBySameTeam = new("hurtBySameTeam", (data[1] & 0x02) == 0x02, BitCount.Bit1);
            unknown9 = new("unknown9", (data[1] & 0x01) == 0x01, BitCount.Bit1);

            unknown10 = new("unknown10", (data[2] & 0x80) == 0x80, BitCount.Bit1);
            unknown11 = new("unknown11", (data[2] & 0x40) == 0x40, BitCount.Bit1);
            unknown12 = new("unknown12", (data[2] & 0x20) == 0x20, BitCount.Bit1);
            unknown13 = new("unknown13", (data[2] & 0x10) == 0x10, BitCount.Bit1);
            shadows = new("Has Shadow", (data[2] & 0x08) == 0x08, BitCount.Bit1);
            disableExplosionEffects = new("Disable Explosion", (data[2] & 0x04) == 0x04, BitCount.Bit1);
            disableTeam = new("disableTeam", (data[2] & 0x02) == 0x02, BitCount.Bit1);
            unknown14 = new("unknown14", (data[2] & 0x01) == 0x01, BitCount.Bit1);

            offset += 4;

            // Items are reversed so they compile correctly.
            return new List<ActorProperty>() {
                disableTargeting,
                unknown3,
                disableAllCollision,
                unknown2,
                unknown1,
                disableRendering,
                playerPhysics,
                isInvincible,

                unknown9,
                hurtBySameTeam,
                unknown8,
                unknown7,
                disableDestroyedActorCollision,
                unknown6,
                unknown5,
                unknown4,

                unknown14,
                disableTeam,
                disableExplosionEffects,
                shadows,
                unknown13,
                unknown12,
                unknown11,
                unknown10,

                new FillerActorProperty(0, BitCount.Bit8)
            };

        }

        List<ActorProperty> InitEmptyTags() {

            isInvincible = new("Is Invincible", false, BitCount.Bit1);
            playerPhysics = new("Player Physics", false, BitCount.Bit1);
            disableRendering = new("Disable Rendering", false, BitCount.Bit1);
            unknown1 = new("unknown1", false, BitCount.Bit1);
            unknown2 = new("unknown2", false, BitCount.Bit1);
            disableAllCollision = new("Disable Collision", false, BitCount.Bit1);
            unknown3 = new("unknown3", false, BitCount.Bit1);
            disableTargeting = new("disableTargeting", false, BitCount.Bit1);

            unknown4 = new("unknown4", false, BitCount.Bit1);
            unknown5 = new("unknown5", false, BitCount.Bit1);
            unknown6 = new("unknown6", false, BitCount.Bit1);
            disableDestroyedActorCollision = new("Disable Destroyed Collision", false, BitCount.Bit1);
            unknown7 = new("unknown7", false, BitCount.Bit1);
            unknown8 = new("unknown8", false, BitCount.Bit1);
            hurtBySameTeam = new("hurtBySameTeam", false, BitCount.Bit1);
            unknown9 = new("unknown9", false, BitCount.Bit1);

            unknown10 = new("unknown10", false, BitCount.Bit1);
            unknown11 = new("unknown11", false, BitCount.Bit1);
            unknown12 = new("unknown12", false, BitCount.Bit1);
            unknown13 = new("unknown13", false, BitCount.Bit1);
            shadows = new("Has Shadow", false, BitCount.Bit1);
            disableExplosionEffects = new("Disable Explosion", false, BitCount.Bit1);
            disableTeam = new("disableTeam", false, BitCount.Bit1);
            unknown14 = new("unknown14", false, BitCount.Bit1);

            // Items are reversed so they compile correctly.
            return new List<ActorProperty>() {
                disableTargeting,
                unknown3,
                disableAllCollision,
                unknown2,
                unknown1,
                disableRendering,
                playerPhysics,
                isInvincible,

                unknown9,
                hurtBySameTeam,
                unknown8,
                unknown7,
                disableDestroyedActorCollision,
                unknown6,
                unknown5,
                unknown4,

                unknown14,
                disableTeam,
                disableExplosionEffects,
                shadows,
                unknown13,
                unknown12,
                unknown11,
                unknown10,

                new FillerActorProperty(0, BitCount.Bit8)
            };

        }

        List<ActorProperty> InitEntityProperties() {

            health = new("Health", Read16(), BitCount.Bit16);
            collideDamage = new("Collide Damage", Read16(), BitCount.Bit16);
            u_team = new("Team (Unknown)", Read16(), BitCount.Bit16);
            mapIconColor = new("Map Icon Color", (MapIconColor)Read8(), BitCount.Bit8);
            u_unknown15 = new("u_unknown15", Read8(), BitCount.Bit8);
            u_explosion = new("Explosion (Unknown)", Read16(), BitCount.Bit16);
            uvOffset = new("UV Offset", Read16(), BitCount.Bit16);

            return new List<ActorProperty>() {
                health, collideDamage, u_team, mapIconColor, u_unknown15, u_explosion, uvOffset
            };

        }

        List<ActorProperty> InitEmptyEntityProperties() {

            health = new("Health", 0, BitCount.Bit16);
            collideDamage = new("Collide Damage", 0, BitCount.Bit16);
            u_team = new("Team (Unknown)", 0, BitCount.Bit16);
            mapIconColor = new("Map Icon Color", MapIconColor.None, BitCount.Bit8);
            u_unknown15 = new("u_unknown15", 0, BitCount.Bit8);
            u_explosion = new("Explosion (Unknown)", 0, BitCount.Bit16);
            uvOffset = new("UV Offset", 0, BitCount.Bit16);

            return new List<ActorProperty>() {
                health, collideDamage, u_team, mapIconColor, u_unknown15, u_explosion, uvOffset
            };

        }

        public List<byte> Compile() {

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

    public interface FCopObjectMutating {

        public int GetUVOffset();

        public RotationActorProperty[] GetRotations();

    }

    public interface FCopHeightOffseting {

        public int GetHeight();

        public ActorGroundCast GetGroundCast();

    }

    public class FCopBehavior1 : FCopEntity, FCopObjectMutating {

        // FIXME: for some odd reason players facing can be negative. Allow the property to be negative
        public RotationActorProperty rotation;

        public FCopBehavior1(FCopActor actor) : base(actor) {

            rotation = new("Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 0, 2, 3, 4, 5 });

            properties.Add(rotation);
            // Implies ground cast but Future Cop won't react except with 0x01 which will crash. Leaving at default 0xFF
            properties.Add(new FillerActorProperty(0, BitCount.Bit8));
            properties.Add(new FillerActorProperty(0xFF, BitCount.Bit8));

        }

        public int GetUVOffset() {
            return uvOffset.value;
        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { rotation };
        }

    }

    public class FCopBehavior5 : FCopEntity {

        public FCopBehavior5(FCopActor actor): base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior6 : FCopEntity {

        public FCopBehavior6(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior8 : FCopEntity, FCopObjectMutating {

        public ValueActorProperty weaponID;
        public ValueActorProperty unknown4;
        public ValueActorProperty u_weaponCollision;
        public ValueActorProperty unknown5;
        public ValueActorProperty unknown6;
        public ValueActorProperty unknown7;
        public ValueActorProperty unknown8;
        public ValueActorProperty engageRange;
        public ValueActorProperty unknown9;
        public ValueActorProperty unknown10;
        public ValueActorProperty unknown11;
        public RotationActorProperty headRotation;
        public ValueActorProperty heightOffset;
        public ValueActorProperty unknown12;
        public ValueActorProperty unknown13;
        public ValueActorProperty unknown14;
        public ValueActorProperty unknown16;
        public RotationActorProperty baseRotation;

        public FCopBehavior8(FCopActor actor) : base(actor) {

            assetRefNames = new string[] { "Head Object", "Object", "Base Object", "Destroyed Object" };
            assetRefType = new AssetType[] { AssetType.Object, AssetType.Object, AssetType.Object, AssetType.Object };

            weaponID = new("Weapon ID", Read16(), BitCount.Bit16);
            unknown4 = new("unknown4", Read8(), BitCount.Bit8);
            u_weaponCollision = new("u_weaponCollision", Read8(), BitCount.Bit8);
            unknown5 = new("unknown5", Read16(), BitCount.Bit16);
            unknown6 = new("attack?", Read16(), BitCount.Bit16);
            unknown7 = new("unknown7", Read16(), BitCount.Bit16);
            unknown8 = new("unknown8", Read16(), BitCount.Bit16);
            engageRange = new("Engage Range", Read16(), BitCount.Bit16);
            unknown9 = new("unknown9", Read16(), BitCount.Bit16);
            unknown10 = new("unknown10", Read16(), BitCount.Bit16);
            unknown11 = new("unknown11", Read16(), BitCount.Bit16);
            headRotation = new("Head Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 0 });
            heightOffset = new("Height Offset", Read16(), BitCount.Bit16);
            unknown12 = new("unknown12", Read16(), BitCount.Bit16);
            unknown13 = new("unknown13", Read16(), BitCount.Bit16);
            unknown14 = new("unknown14", Read16(), BitCount.Bit16);
            // 0x00 0x00
            offset += 2;
            unknown16 = new("unknown16", Read16(), BitCount.Bit16);
            baseRotation = new("Base Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 2 });

            properties.AddRange(new List<ActorProperty>() {
                weaponID,
                unknown4,
                u_weaponCollision,
                unknown5,
                unknown6,
                unknown7,
                unknown8,
                engageRange,
                unknown9,
                unknown10,
                unknown11,
                headRotation,
                heightOffset,
                unknown12,
                unknown13,
                unknown14,
                new FillerActorProperty(0, BitCount.Bit16),
                unknown16,
                baseRotation
            });

        }

        public int GetUVOffset() {
            return uvOffset.value;
        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { headRotation, baseRotation };
        }

    }

    public class FCopBehavior9 : FCopEntity {

        public FCopBehavior9(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior10 : FCopEntity {

        public FCopBehavior10(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior11 : FCopEntity, FCopHeightOffseting, FCopObjectMutating {

        public EnumDataActorProperty groundCast;
        public RotationActorProperty rotation;
        public ValueActorProperty heightOffset;

        public FCopBehavior11(FCopActor actor) : base(actor) {

            assetRefNames = new string[] { "Object", "Destroyed Object" };
            assetRefType = new AssetType[] { AssetType.Object, AssetType.Object };

            groundCast = new("Ground Cast", (ActorGroundCast)Read16(), BitCount.Bit16);
            rotation = new("Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 0, 1 });
            heightOffset = new("Height Offset", Read16(), BitCount.Bit16);

            properties.AddRange(new List<ActorProperty>() { groundCast, rotation, heightOffset });

            // For some reason with props, there's a property that's always 0.
            properties.Add(new FillerActorProperty(0, BitCount.Bit16));

        }

        public FCopBehavior11() : base() {

            assetRefNames = new string[] { "Object", "Destroyed Object" };
            assetRefType = new AssetType[] { AssetType.Object, AssetType.Object };

            groundCast = new("Ground Cast", ActorGroundCast.Highest, BitCount.Bit16);
            rotation = new("Rotation", new ActorRotation().SetRotationCompiled(0), BitCount.Bit16, new int[] { 0, 1 });
            heightOffset = new("Height Offset", 0, BitCount.Bit16);

            properties.AddRange(new List<ActorProperty>() { groundCast, rotation, heightOffset });

            // For some reason with props, there's a property that's always 0.
            properties.Add(new FillerActorProperty(0, BitCount.Bit16));

        }

        public int GetHeight() {
            return heightOffset.value;
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)groundCast.caseValue;
        }

        public int GetUVOffset() {
            return uvOffset.value;
        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { rotation };
        }

    }

    public class FCopBehavior12 : FCopEntity {

        public FCopBehavior12(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

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

    public class FCopBehavior16 : FCopEntity {

        public FCopBehavior16(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior20 : FCopEntity {

        public FCopBehavior20(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior25 : FCopEntity {

        public FCopBehavior25(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior26 : FCopEntity {

        public FCopBehavior26(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior27 : FCopEntity {

        public FCopBehavior27(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior28 : FCopEntity {

        public FCopBehavior28(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior30 : FCopEntity {

        public FCopBehavior30(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior32 : FCopEntity {

        public FCopBehavior32(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

        }

    }

    public class FCopBehavior33 : FCopEntity {

        public FCopBehavior33(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

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

            headRotation = new("Head Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 64)), BitCount.Bit16, new int[] { 0 });
            baseRotation = new("Base Rotation", new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 78)), BitCount.Bit16, new int[] { 2 });

            properties = new() { headRotation, baseRotation };
        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(64, 2);
            actor.rawFile.data.InsertRange(64, BitConverter.GetBytes((short)headRotation.value.compiledRotation));

            actor.rawFile.data.RemoveRange(78, 2);
            actor.rawFile.data.InsertRange(78, BitConverter.GetBytes((short)headRotation.value.compiledRotation));

        }

    }

    public class FCopBehavior37 : FCopEntity {

        public FCopBehavior37(FCopActor actor) : base(actor) {

            var propertyCount = (Utils.BytesToInt(actor.rawFile.data.ToArray(), 4) - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(), BitCount.Bit16);
                property.fileOffset = offset;
                properties.Add(property);
            }

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

    public class FCopBehavior96 : FCopActorBehavior, FCopObjectMutating, FCopHeightOffseting {

        public int expectedRawFileSize { get; set; }
        public string[] assetRefNames { get; set; }
        public AssetType[] assetRefType { get; set; }

        public FCopActor actor { get; set; }
        public List<ActorProperty> properties { get; set; }

        public RotationActorProperty rotation;
        public ValueActorProperty unknown2;
        public ValueActorProperty unknown3;
        public ValueActorProperty heightOffset;
        public EnumDataActorProperty groundCast;
        public ValueActorProperty unknown5;
        public ValueActorProperty unknown6;
        public ValueActorProperty unknown7;
        public ValueActorProperty unknown8;

        public FCopBehavior96(FCopActor actor) {
            this.actor = actor;
            assetRefNames = new string[] { "Object" };
            assetRefType = new AssetType[] { AssetType.Object };

            var rawFile = actor.rawFile;
            var offset = 28;

            int Read16() {

                var value = BitConverter.ToInt16(actor.rawFile.data.ToArray(), offset);
                offset += 2;
                return value;

            }

            int Read8() {

                var value = actor.rawFile.data[offset];
                offset += 1;
                return value;

            }

            rotation = new("Rotation", new ActorRotation().SetRotationCompiled(Read16()), BitCount.Bit16, new int[] { 0 });
            unknown2 = new("unknown2", Read16(), BitCount.Bit16);
            unknown3 = new("unknown3", Read16(), BitCount.Bit16);
            heightOffset = new("Height Offset", Read16(), BitCount.Bit16);
            groundCast = new("Ground Cast", (ActorGroundCast)Read8(), BitCount.Bit8);
            unknown5 = new("unknown5", Read8(), BitCount.Bit8);
            unknown6 = new("unknown6", Read16(), BitCount.Bit16);
            unknown7 = new("unknown7", Read16(), BitCount.Bit16);
            unknown8 = new("unknown8", Read16(), BitCount.Bit16);

            properties = new() {
                rotation, 
                unknown2, 
                unknown3, 
                heightOffset,
                groundCast,
                unknown5, 
                unknown6,
                unknown7,
                unknown8
            };

        }

        public int GetHeight() {
            return heightOffset.value;
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)groundCast.caseValue;
        }

        public int GetUVOffset() {
            return 0;
        }

        public RotationActorProperty[] GetRotations() {
            return new RotationActorProperty[] { rotation };
        }

        public List<byte> Compile() {

            var total = new List<byte>();

            foreach (var p in properties) {

                total.AddRange(p.Compile());

            }

            return total;

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
                _ => null
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

    public class ToggleActorProperty: ActorProperty {

        public string name { get; set; }
        public int fileOffset { get; set; }
        public BitCount bitCount { get; set; }

        public int GetCompiledValue() {
            return value ? 1 : 0;
        }

        public bool value;

        public ToggleActorProperty(string name, bool value, BitCount bitCount) {
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

        public int[] affectedRefIndexes;
        public ActorRotation value;

        public int GetCompiledValue() {
            return value.compiledRotation;
        }

        public RotationActorProperty(string name, ActorRotation value, BitCount bitCount, int[] affectedRefIndexes) {
            this.name = name;
            this.value = value;
            this.bitCount = bitCount;
            this.affectedRefIndexes = affectedRefIndexes;
        }

    }

    public class FillerActorProperty : ActorProperty {

        public string name { get; set; }
        public int fileOffset { get; set; }
        public BitCount bitCount { get; set; }

        public int GetCompiledValue() {
            return value;
        }

        public int value;

        public FillerActorProperty(int value, BitCount bitCount) {
            this.name = "Null";
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
        Bit1 = 1,
        Bit8 = 8,
        Bit16 = 16,
        Bit32 = 32
    }

    public enum MapIconColor {
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 3,
        Cyan = 4,
        Yellow = 5,
        Magenta = 6,
        White = 7,
        Gold = 8,
        DarkRed = 9,
        DarkBlue = 10,
        DarkGreen = 11,
        DarkCyan = 12,
        DarkYellow = 13,
        DarkMagenta = 14,
        Gray = 15,
        Orange = 16,
        RedPulse = 17,
        BlueWhitePulse = 18,
        GreenPulse = 19,
        Pink = 20,
        Silver = 21,
        Sage = 22,
        FlashingRed = 23,
        FlashingBlue = 24,
        FlashingGreen = 25,
        FlashingCyan = 26,
        FlashingYellow = 27,
        FlashingMagenta = 28,
        FlashingWhite = 29,
        FlashingOrange = 30,
        FlashingGray = 31,
        BlackTri = 32,
        RedTri = 33,
        BlueTri = 34,
        GreenTri = 35,
        CyanTri = 36,
        YellowTri = 37,
        MagentaTri = 38,
        WhiteTri = 39,
        GoldTri = 40,
        DarkRedTri = 41,
        DarkBlueTri = 42,
        DarkGreenTri = 43,
        DarkCyanTri = 44,
        DarkYellowTri = 45,
        DarkMagentaTri = 46,
        GrayTri = 47,
        OrangeTri = 48,
        RedPulseTri = 49,
        BlueWhitePulseTri = 50,
        GreenPulseTri = 51,
        PinkTri = 52,
        SilverTri = 53,
        SageTri = 54,
        FlashingRedTri = 55,
        FlashingBlueTri = 56,
        FlashingGreenTri = 57,
        FlashingCyanTri = 58,
        FlashingYellowTri = 59,
        FlashingMagentaTri = 60,
        FlashingWhiteTri = 61,
        FlashingOrangeTri = 62,
        FlashingGrayTri = 63,
        BlackDiamond = 64,
        RedDiamond = 65,
        BlueDiamond = 66,
        GreenDiamond = 67,
        CyanDiamond = 68,
        YellowDiamond = 69,
        MagentaDiamond = 70,
        WhiteDiamond = 71,
        GoldDiamond = 72,
        DarkRedDiamond = 73,
        DarkBlueDiamond = 74,
        DarkGreenDiamond = 75,
        DarkCyanDiamond = 76,
        DarkYellowDiamond = 77,
        DarkMagentaDiamond = 78,
        GrayDiamond = 79,
        OrangeDiamond = 80,
        RedPulseDiamond = 81,
        BlueWhitePulseDiamond = 82,
        GreenPulseDiamond = 83,
        PinkDiamond = 84,
        SilverDiamond = 85,
        SageDiamond = 86,
        FlashingRedDiamond = 87,
        FlashingBlueDiamond = 88,
        FlashingGreenDiamond = 89,
        FlashingCyanDiamond = 90,
        FlashingYellowDiamond = 91,
        FlashingMagentaDiamond = 92,
        FlashingWhiteDiamond = 93,
        FlashingOrangeDiamond = 94,
        FlashingGrayDiamond = 95,
        CyanDiamondClone = 204,
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
        Behavior36 = 36,
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