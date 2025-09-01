

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
            public const string Cact = "Cact";
            public const string Csac = "Csac";

            public static List<byte> tSACBytes = new() { 67, 65, 83, 116 };
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
        public FCopActorSpawning spawningProperties = null;

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
                case ActorBehavior.StationaryTurret:
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
                case ActorBehavior.WalkableProp:
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
                case ActorBehavior.PathedMultiTurret:
                    behavior = new FCopBehavior28(this, propertyData);
                    break;
                case ActorBehavior.Teleporter:
                    behavior = new FCopBehavior29(this, propertyData);
                    break;
                case ActorBehavior.InterchangingEntity:
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
                    behavior = new FCopBehavior34(this, propertyData);
                    break;
                case ActorBehavior.MapObjectiveNodes:
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
                case ActorBehavior.VisualEffects87:
                    behavior = new FCopBehavior87(this, propertyData);
                    break;
                case ActorBehavior.VisualEffects88:
                    behavior = new FCopBehavior88(this, propertyData);
                    break;
                case ActorBehavior.VisualEffects89:
                    behavior = new FCopBehavior89(this, propertyData);
                    break;
                case ActorBehavior.VisualEffects90:
                    behavior = new FCopBehavior90(this, propertyData);
                    break;
                case ActorBehavior.ActorExplosion:
                    behavior = new FCopBehavior91(this, propertyData);
                    break;
                case ActorBehavior.VisualEffects92:
                    behavior = new FCopBehavior92(this, propertyData);
                    break;
                case ActorBehavior.ParticleEmitter:
                    behavior = new FCopBehavior93(this, propertyData);
                    break;
                case ActorBehavior.VisualEffects94:
                    behavior = new FCopBehavior94(this, propertyData);
                    break;
                case ActorBehavior.Trigger:
                    behavior = new FCopBehavior95(this, propertyData);
                    break;
                case ActorBehavior.StaticProp:
                    behavior = new FCopBehavior96(this, propertyData);
                    break;
                case ActorBehavior.Texture:
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
                spawningProperties = new FCopActorSpawning(rawFile.data.GetRange(tSAC.index, tSAC.chunkSize));
            }

        }

        public FCopActor(int id, int nullRPNSRef, ActorBehavior behavior, int x, int y) : base(null) {

            name = "Actor " + id;

            behaviorType = behavior;
            this.x = x;
            this.y = y;

            void InitResourceAndRawFile(int resourceCount) {

                foreach (var i in Enumerable.Range(0, resourceCount)) {
                    resourceReferences.Add(new Resource("NULL", 0));
                }

                rawFile = new IFFDataFile(3, new(), "Cact", id, nullRPNSRef);

            }

            switch (behavior) {
                case ActorBehavior.PathedEntity:
                    InitResourceAndRawFile(FCopBehavior5.assetRefCount);
                    this.behavior = new FCopBehavior5(this, new());
                    break;
                case ActorBehavior.StationaryEntity:
                    InitResourceAndRawFile(FCopBehavior6.assetRefCount);
                    this.behavior = new FCopBehavior6(this, new());
                    break;
                case ActorBehavior.StationaryTurret:
                    InitResourceAndRawFile(FCopBehavior8.assetRefCount);
                    this.behavior = new FCopBehavior8(this, new());
                    break;
                case ActorBehavior.Aircraft:
                    InitResourceAndRawFile(FCopBehavior9.assetRefCount);
                    this.behavior = new FCopBehavior9(this, new());
                    break;
                case ActorBehavior.Elevator:
                    InitResourceAndRawFile(FCopBehavior10.assetRefCount);
                    this.behavior = new FCopBehavior10(this, new());
                    break;
                case ActorBehavior.DynamicProp:
                    InitResourceAndRawFile(FCopBehavior11.assetRefCount);
                    this.behavior = new FCopBehavior11(this, new());
                    break;
                case ActorBehavior.WalkableProp:
                    InitResourceAndRawFile(FCopBehavior12.assetRefCount);
                    this.behavior = new FCopBehavior12(this, new());
                    break;
                case ActorBehavior.FloatingItem:
                    InitResourceAndRawFile(FCopBehavior16.assetRefCount);
                    this.behavior = new FCopBehavior16(this, new());
                    break;
                case ActorBehavior.PathedTurret:
                    InitResourceAndRawFile(FCopBehavior20.assetRefCount);
                    this.behavior = new FCopBehavior20(this, new());
                    break;
                case ActorBehavior.MovableProp:
                    InitResourceAndRawFile(FCopBehavior25.assetRefCount);
                    this.behavior = new FCopBehavior25(this, new());
                    break;
                case ActorBehavior.PathedMultiTurret:
                    InitResourceAndRawFile(FCopBehavior28.assetRefCount);
                    this.behavior = new FCopBehavior28(this, new());
                    break;
                case ActorBehavior.Teleporter:
                    InitResourceAndRawFile(FCopBehavior29.assetRefCount);
                    this.behavior = new FCopBehavior29(this, new());
                    break;
                case ActorBehavior.Reloader:
                    InitResourceAndRawFile(FCopBehavior32.assetRefCount);
                    this.behavior = new FCopBehavior32(this, new());
                    break;
                case ActorBehavior.ClaimableTurret:
                    InitResourceAndRawFile(FCopBehavior36.assetRefCount);
                    this.behavior = new FCopBehavior36(this, new());
                    break;
                case ActorBehavior.Trigger:
                    InitResourceAndRawFile(FCopBehavior95.assetRefCount);
                    this.behavior = new FCopBehavior95(this, new());
                    break;
                case ActorBehavior.StaticProp:
                    InitResourceAndRawFile(FCopBehavior96.assetRefCount);
                    this.behavior = new FCopBehavior96(this, new());
                    break;
                case ActorBehavior.Texture:
                    InitResourceAndRawFile(FCopBehavior97.assetRefCount);
                    this.behavior = new FCopBehavior97(this, new());
                    break;
                case ActorBehavior.Weapon:
                    InitResourceAndRawFile(FCopBehavior98.assetRefCount);
                    this.behavior = new FCopBehavior98(this, new());
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

            if (spawningProperties != null) {
                total.AddRange(spawningProperties.Compile());
            }

            var totalWithHeader = new List<byte>();

            totalWithHeader.AddRange(Encoding.ASCII.GetBytes(Reverse(FourCC.tACT)));
            totalWithHeader.AddRange(BitConverter.GetBytes(actSize));
            totalWithHeader.AddRange(total);

            if (totalWithHeader.Count != rawFile.data.Count && rawFile.data.Count != 0) {
                //throw new Exception("Compiled size doesn't equal original");
            }

            rawFile.data = totalWithHeader;

            // Makes sure the fourCC is correct.
            if (spawningProperties != null) {
                rawFile.dataFourCC = FourCC.Csac;
            }
            else {
                rawFile.dataFourCC = FourCC.Cact;
            }

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

    public class FCopActorSpawning {

        const int size = 48;
        const float respawnTimerMulitpler = 60f;

        public int dataID; // 32bit
        public float respawnTime;
        public bool randomFirstSpawnTime;
        public int maxActiveActors;
        public bool disableRespawn;
        public bool infiniteRespawns;
        public int maxRespawns;

        public FCopActorSpawning(List<byte> propertyData) {

            dataID = BitConverter.ToInt32(propertyData.ToArray(), 8);

            var respawnRaw = BitConverter.ToInt16(propertyData.ToArray(), 12);
            
            randomFirstSpawnTime = respawnRaw > 0;
            respawnTime = MathF.Abs(respawnRaw) / respawnTimerMulitpler;

            maxActiveActors = BitConverter.ToInt16(propertyData.ToArray(), 14);
            disableRespawn = BitConverter.ToInt16(propertyData.ToArray(), 16) == 1;

            var maxRespawnsRaw = BitConverter.ToInt16(propertyData.ToArray(), 18);

            infiniteRespawns = maxRespawnsRaw == -1;
            maxRespawns = maxRespawnsRaw;

        }

        public FCopActorSpawning(int dataID) {

            dataID = 0;
            respawnTime = 5;
            randomFirstSpawnTime = false;
            maxActiveActors = 1;
            disableRespawn = true;
            infiniteRespawns = false;
            maxRespawns = 1;

        }

        public List<byte> Compile() {

            var total = new List<byte>();

            total.AddRange(FCopActor.FourCC.tSACBytes);
            total.AddRange(BitConverter.GetBytes(size));
            total.AddRange(BitConverter.GetBytes(dataID));

            var respawnTimeCompiled = (int)(respawnTime * respawnTimerMulitpler);

            if (!randomFirstSpawnTime) {
                respawnTimeCompiled = -respawnTimeCompiled;
            }

            total.AddRange(BitConverter.GetBytes((short)respawnTimeCompiled));

            total.AddRange(BitConverter.GetBytes((short)maxActiveActors));

            total.AddRange(BitConverter.GetBytes((short)(disableRespawn ? 1 : 0)));

            if (infiniteRespawns) {
                total.AddRange(BitConverter.GetBytes((short)-1));
            }
            else {
                total.AddRange(BitConverter.GetBytes((short)maxRespawns));
            }

            foreach (var i in Enumerable.Range(0, 28)) {
                total.Add(0);
            }

            return total;

        }

    }

    public class FCopActorBehavior {

        public static Dictionary<Type, ActorBehavior> behaviorsByType = new() {
            { typeof(FCopBehavior1), ActorBehavior.Player },
            { typeof(FCopBehavior5), ActorBehavior.PathedEntity },
            { typeof(FCopBehavior6), ActorBehavior.StationaryEntity },
            { typeof(FCopBehavior8), ActorBehavior.StationaryTurret },
            { typeof(FCopBehavior9), ActorBehavior.Aircraft },
            { typeof(FCopBehavior10), ActorBehavior.Elevator },
            { typeof(FCopBehavior11), ActorBehavior.DynamicProp },
            { typeof(FCopBehavior12), ActorBehavior.WalkableProp },
            { typeof(FCopBehavior14), ActorBehavior.UniversalTrigger },
            { typeof(FCopBehavior16), ActorBehavior.FloatingItem },
            { typeof(FCopBehavior20), ActorBehavior.PathedTurret },
            { typeof(FCopBehavior25), ActorBehavior.MovableProp },
            { typeof(FCopBehavior26), ActorBehavior.Behavior26 },
            { typeof(FCopBehavior27), ActorBehavior.Behavior27 },
            { typeof(FCopBehavior28), ActorBehavior.PathedMultiTurret },
            { typeof(FCopBehavior29), ActorBehavior.Teleporter },
            { typeof(FCopBehavior30), ActorBehavior.InterchangingEntity },
            { typeof(FCopBehavior31), ActorBehavior.Behavior31 },
            { typeof(FCopBehavior32), ActorBehavior.Reloader },
            { typeof(FCopBehavior33), ActorBehavior.Behavior33 },
            { typeof(FCopBehavior34), ActorBehavior.Behavior34 },
            { typeof(FCopBehavior35), ActorBehavior.MapObjectiveNodes },
            { typeof(FCopBehavior36), ActorBehavior.ClaimableTurret },
            { typeof(FCopBehavior37), ActorBehavior.Behavior37 },
            { typeof(FCopBehavior38), ActorBehavior.Behavior38 },
            { typeof(FCopBehavior87), ActorBehavior.VisualEffects87 },
            { typeof(FCopBehavior88), ActorBehavior.VisualEffects88 },
            { typeof(FCopBehavior89), ActorBehavior.VisualEffects89 },
            { typeof(FCopBehavior90), ActorBehavior.VisualEffects90 },
            { typeof(FCopBehavior91), ActorBehavior.ActorExplosion },
            { typeof(FCopBehavior92), ActorBehavior.VisualEffects92 },
            { typeof(FCopBehavior93), ActorBehavior.ParticleEmitter },
            { typeof(FCopBehavior94), ActorBehavior.VisualEffects94 },
            { typeof(FCopBehavior95), ActorBehavior.Trigger },
            { typeof(FCopBehavior96), ActorBehavior.StaticProp },
            { typeof(FCopBehavior97), ActorBehavior.Texture },
            { typeof(FCopBehavior98), ActorBehavior.Weapon },
            { typeof(FCopBehavior99), ActorBehavior.PlayerWeapon },

        };

        public int expectedRawFileSize;
        public ActorAssetReference[] assetReferences;
        public string[] callbackNames = new string[] {
            "On Callback 1",
            "On Callback 2",
            "On Callback 3"
        };

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

        int Read32() {

            var value = BitConverter.ToInt32(propertyData.ToArray(), offset);
            offset += 4;
            return value;

        }

        protected int Read32(int defaultValue) {

            if (propertyData.Count == 0) {
                return defaultValue;
            }
            return Read32();

        }

        int Read16() {

            var value = BitConverter.ToInt16(propertyData.ToArray(), offset);
            offset += 2;
            return value;

        }

        protected int Read16(int defaultValue) {

            if (propertyData.Count == 0) {
                return defaultValue;
            }
            return Read16();

        }

        protected int Read16NoIt(int defaultValue) {

            if (propertyData.Count == 0) {
                return defaultValue;
            }
            var value = BitConverter.ToInt16(propertyData.ToArray(), offset);
            return value;

        }

        int Read8() {

            var value = propertyData[offset];
            offset += 1;
            return value;

        }

        protected int Read8(int defaultValue) {

            if (propertyData.Count == 0) {
                return defaultValue;
            }
            return Read8();

        }

        int ReadS8() {

            var value = (sbyte)propertyData[offset];
            offset += 1;
            return value;

        }

        protected int ReadS8(int defaultValue) {

            if (propertyData.Count == 0) {
                return defaultValue;
            }
            return ReadS8();

        }

        protected int Read8NoIt(int defaultValue) {

            if (propertyData.Count == 0) {
                return defaultValue;
            }
            var value = propertyData[offset];
            return value;

        }

        protected bool Read1(byte compare, bool defaultValue) {

            if (propertyData.Count != 0) {

                var data = propertyData[offset];

                return (data & compare) == compare;

            }
            else {

                return defaultValue;

            }


        }

        public void InitPropertiesByName(List<string> commonNameSort = null) {

            propertiesByName.Clear();
            propertiesByCommonName.Clear();

            if (commonNameSort != null) {

                foreach (var commonName in commonNameSort) {

                    propertiesByCommonName[commonName] = new();

                }

            }

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
                        case BitCount.Bit3:
                            floatingBits.Add(new BitNumber(3, p.GetCompiledValue()));
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

            if (floatingBits.Count != 0) {
                throw new Exception("Actor Behavor " + actor.behaviorType.ToString() + " compilation still has floating bits!");
            }

            return total;

        }

    }

    public struct ActorAssetReference {

        public string name;
        public AssetType type;
        public int dependantRefIndex;
        public int positionIndex;

        public ActorAssetReference(string name, AssetType type) : this() {
            this.name = name;
            this.type = type;
            this.dependantRefIndex = -1;
            this.positionIndex = -1;
        }

        public ActorAssetReference(string name, AssetType type, int dependantRefIndex, int positionIndex) : this(name, type) {
            this.dependantRefIndex = dependantRefIndex;
            this.positionIndex = positionIndex;
        }

    }

    public interface FCopObjectMutating {

        public ObjectMutation[] GetMutations();

    }

    public struct ObjectMutation {

        public int refIndex;
        public int uvOffset;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public float scaleX;
        public float scaleY;
        public float scaleZ;

        public ObjectMutation(int refIndex, int uvOffset, float rotationX, float rotationY, float rotationZ, float scaleX, float scaleY, float scaleZ) {
            this.refIndex = refIndex;
            this.uvOffset = uvOffset;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.scaleZ = scaleZ;
        }

        public ObjectMutation(int refIndex, int uvOffset) : this() {
            this.refIndex = refIndex;
            this.uvOffset = uvOffset;
            this.rotationX = 0;
            this.rotationY = 0;
            this.rotationZ = 0;
            this.scaleX = 1;
            this.scaleY = 1;
            this.scaleZ = 1;
        }

        public ObjectMutation(int refIndex, int uvOffset, float rotationY) : this(refIndex, uvOffset) {
            this.rotationY = rotationY;
        }

        public ObjectMutation(int refIndex, int uvOffset, float rotationX, float rotationY, float rotationZ) : this(refIndex, uvOffset, rotationX) {
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
        }

    }

    public interface FCopHeightOffsetting {

        public void SetHeight(float height);

        public float GetHeight();

        public ActorProperty GetHeightProperty();

        public ActorGroundCast GetGroundCast();

    }

    public interface SpecializedID {
        public int GetID();
    }

    public abstract class FCopEntity: FCopActorBehavior {

        public FCopEntity(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(InitTags());

            properties.AddRange(InitEntityProperties());

        }

        List<ActorProperty> InitTags() {

            var total = new List<ActorProperty>() {
                new ToggleActorProperty("Disable Actor Targeting", Read1(0x01, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Unknown", Read1(0x02, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Disable Collision", Read1(0x04, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Always Active", Read1(0x08, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Disable Map Icon", Read1(0x10, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Disable Rendering", Read1(0x20, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Player Physics", Read1(0x40, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Is Invincible", Read1(0x80, false), BitCount.Bit1, "Entity Tags"),

            };
            offset++;
            total.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Always Interactable", Read1(0x01, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Actor Collision", Read1(0x02, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Strong Pushback", Read1(0x04, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Unknown 2", Read1(0x08, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Disable Destroyed Collision", Read1(0x10, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Obstruct Actor Path", Read1(0x20, false), BitCount.Bit1, "Entity Tags"),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Unknown 3", Read1(0x80, false), BitCount.Bit1, "Entity Tags"),

            });
            offset++;
            total.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Unknown 4", Read1(0x01, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Disable Player Targeting", Read1(0x02, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Disable Explosion", Read1(0x04, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Has Shadow", Read1(0x08, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Enable Third Callback", Read1(0x10, false), BitCount.Bit1, "Entity Tags"),
                new ToggleActorProperty("Unknown (Scripting)", Read1(0x20, false), BitCount.Bit1, "Entity Tags"),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),

                new FillerActorProperty(0, BitCount.Bit8)

            });

            offset += 2;

            return total;

        }

        List<ActorProperty> InitEntityProperties() {

            return new List<ActorProperty>() {
                new ValueActorProperty("Health", Read16(0), 0, 30000, BitCount.Bit16, "Entity Properties"),
                new ValueActorProperty("Collide Damage", Read16(0), 0, 30000, BitCount.Bit16, "Entity Properties"),
                new AssetActorProperty("Team", Read8(0), AssetType.Team, BitCount.Bit8, "Entity Properties"),
                new AssetActorProperty("Group", Read8(0), AssetType.ScriptGroup, BitCount.Bit8, "Entity Properties"),
                new EnumDataActorProperty("Map Icon Color", (MapIconColor)Read8(0), BitCount.Bit8, "Entity Properties"),
                new ValueActorProperty("Target Priority", Read8(0), 0, 127, BitCount.Bit8, "Entity Properties"),
                new ExplosionActorProperty("Explosion", Read8(0), BitCount.Bit8, "Entity Properties"),
                new AssetActorProperty("Ambient Sound", Read8(0), AssetType.WavSound, BitCount.Bit8, "Entity Properties"),
                new ValueActorProperty("UV Offset X", Read8(0), 0, 255, BitCount.Bit8, "Entity Properties"),
                new ValueActorProperty("UV Offset Y", Read8(0), 0, 255, BitCount.Bit8, "Entity Properties")

            };

        }

        public int GetUVOffset() {
            try {

                var x = propertiesByName["UV Offset X"].GetCompiledValue();
                var y = propertiesByName["UV Offset Y"].GetCompiledValue();

                return (y * 256) + x;
            } catch {
                return 0;
            }
        }

    }

    public abstract class FCopShooter : FCopEntity {

        public FCopShooter(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new SpecializedActorRefActorProperty("Weapon", Read16(0), ActorBehavior.Weapon, true, BitCount.Bit16, "Shooter Properties")
            });

            properties.AddRange(new List<ActorProperty>() {

                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Prevent Back Shooting", Read1(0x02, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("Shoot When Facing", Read1(0x04, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("STag Unknown", Read1(0x08, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("STag Unknown 2", Read1(0x10, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("Fire Alternations", Read1(0x20, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("Target Priority", Read1(0x40, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("STag Unknown 3", Read1(0x80, false), BitCount.Bit1, "Shooter Tags"),

            });
            offset++;
            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Disabled", Read1(0x01, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("Weapon Actor Collision", Read1(0x02, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("Attackable Weapon", Read1(0x04, false), BitCount.Bit1, "Shooter Tags"),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("STag Unknown 4", Read1(0x10, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("STag Unknown 5", Read1(0x20, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("Allow Switch Target", Read1(0x40, false), BitCount.Bit1, "Shooter Tags"),
                new ToggleActorProperty("STag Unknown 6", Read1(0x80, false), BitCount.Bit1, "Shooter Tags"),

            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Acquiring Type", (AcquiringType)Read8(0), BitCount.Bit8, "Shooter Properties"),
                new EnumDataActorProperty("Target Type", (TargetType)Read8(1), BitCount.Bit8, "Shooter Properties", new() { "OverloadAttack" }),

                new OverloadedProperty("OverloadAttack", new() {
                    (new AssetActorProperty("Attack Team", Read16NoIt(0), AssetType.Team, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type"].GetCompiledValue() == TargetType.Team),
                    (new AssetActorProperty("Attack Group", Read16NoIt(0), AssetType.ScriptGroup, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type"].GetCompiledValue() == TargetType.Group),
                    (new AssetActorProperty("Attack Actor", Read16NoIt(0), AssetType.Actor, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type"].GetCompiledValue() == TargetType.Actor),
                    (new EnumDataActorProperty("Attack Behavior", (ActorBehavior)Read16NoIt(1), BitCount.Bit16), () => (TargetType)propertiesByName["Target Type"].GetCompiledValue() == TargetType.BehaviorType),
                    (new ValueActorProperty("Attack", Read16(1), short.MinValue, short.MaxValue, BitCount.Bit16), () => true),
                }, BitCount.Bit16, "Shooter Properties"),

                new NormalizedValueProperty("Detection FOV?", Read16(4096), 0, 4096, 4096f / 360f, BitCount.Bit16, "Shooter Properties"),
                new NormalizedValueProperty("Shooting FOV?", Read16(4096), 0, 4096, 4096f / 360f, BitCount.Bit16, "Shooter Properties"),
                new NormalizedValueProperty("Engage Range", Read16(6144), 0, short.MaxValue, 512f, BitCount.Bit16, "Shooter Properties"),
                new NormalizedValueProperty("Targeting Delay", Read16(32), 0, 320, 32f, BitCount.Bit16, "Shooter Properties"),
            });

        }

    }

    public abstract class FCopTurret : FCopShooter, FCopObjectMutating, FCopHeightOffsetting {

        public FCopTurret(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read8(0), BitCount.Bit8, "Turret Properties"),
                new EnumDataActorProperty("Facing Target Type", (TargetType)Read8(1), BitCount.Bit8, "Turret Properties", new() { "FacingOverloadAttack" }),

                new OverloadedProperty("FacingOverloadAttack", new() {
                    (new AssetActorProperty("Face Team", Read16NoIt(0), AssetType.Team, BitCount.Bit16), () => (TargetType)propertiesByName["Facing Target Type"].GetCompiledValue() == TargetType.Team),
                    (new AssetActorProperty("Face Group", Read16NoIt(0), AssetType.ScriptGroup, BitCount.Bit16), () => (TargetType)propertiesByName["Facing Target Type"].GetCompiledValue() == TargetType.Group),
                    (new AssetActorProperty("Face Actor", Read16NoIt(0), AssetType.Actor, BitCount.Bit16), () => (TargetType)propertiesByName["Facing Target Type"].GetCompiledValue() == TargetType.Actor),
                    (new EnumDataActorProperty("Face Behavior", (ActorBehavior)Read16NoIt(1), BitCount.Bit16), () => (TargetType)propertiesByName["Facing Target Type"].GetCompiledValue() == TargetType.BehaviorType),
                    (new ValueActorProperty("Face", Read16(1), short.MinValue, short.MaxValue, BitCount.Bit16), () => true),
                }, BitCount.Bit16, "Turret Properties"),
                new RangeActorProperty("Rotation", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16, "Turret Properties"),
                new NormalizedValueProperty("Height Offset", Read16(0), short.MinValue, short.MaxValue, 512f, BitCount.Bit16, "Turret Properties"),
                new NormalizedValueProperty("Turn Speed", Read16(1024), short.MinValue, short.MaxValue, 512f, BitCount.Bit16, "Turret Properties"),
                new NormalizedValueProperty("Face Engage Range", Read16(6144), 0, short.MaxValue, 512f, BitCount.Bit16, "Turret Properties"),

            });

            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Use Shooter Data for Facing", Read1(0x01, false), BitCount.Bit1, "Turret Tags"),
                new ToggleActorProperty("Look at Target X-Axis", Read1(0x02, false), BitCount.Bit1, "Turret Tags"),
                new ToggleActorProperty("Use Turret Data for Facing", Read1(0x04, false), BitCount.Bit1, "Turret Tags"),
                new ToggleActorProperty("Spin Z Axis", Read1(0x08, false), BitCount.Bit1, "Turret Tags"),
                new ToggleActorProperty("Walkable", Read1(0x10, false), BitCount.Bit1, "Turret Tags"),
                new ToggleActorProperty("135 Degrees Forward Facing", Read1(0x20, false), BitCount.Bit1, "Turret Tags"),
                new ToggleActorProperty("TTag Unknown 1", Read1(0x40, false), BitCount.Bit1, "Turret Tags"),
                new ToggleActorProperty("TTag Unknown 2", Read1(0x80, false), BitCount.Bit1, "Turret Tags"),

                new FillerActorProperty(0, BitCount.Bit8)

            });
            offset += 2;

        }

        public void SetHeight(float height) {
            ((NormalizedValueProperty)propertiesByName["Height Offset"]).Set(height);
        }

        public float GetHeight() {
            return ((NormalizedValueProperty)propertiesByName["Height Offset"]).value;
        }

        public ActorProperty GetHeightProperty() {
            return propertiesByName["Height Offset"];
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)((EnumDataActorProperty)propertiesByName["Ground Cast"]).caseValue;
        }

        public virtual ObjectMutation[] GetMutations() {
            return new ObjectMutation[] {

                new ObjectMutation(0, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value)

            };
        }

    }

    public abstract class FCopPathedEntity : FCopShooter, FCopHeightOffsetting {

        public FCopPathedEntity(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Enable Backtrack", Read1(0x01, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("PTag ignore1", Read1(0x02, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("PTag ignore2", Read1(0x04, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("Disable Path Obstruction", Read1(0x08, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("Start As Landed", Read1(0x10, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("Roll On Turns", Read1(0x20, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("Disable Pathing", Read1(0x40, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("PTag Unknown1", Read1(0x80, false), BitCount.Bit1, "Pathing Tags"),

            });
            offset++;
            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Lock X Rotation", Read1(0x01, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("PTag Unknown2", Read1(0x02, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("Disable Spin To Backtrack", Read1(0x04, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("Disable Ease", Read1(0x08, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("Lock All Rotations", Read1(0x10, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("Fall On Death", Read1(0x20, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("Walkable", Read1(0x40, false), BitCount.Bit1, "Pathing Tags"),
                new ToggleActorProperty("Despawn On Path End", Read1(0x80, false), BitCount.Bit1, "Pathing Tags"),

            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {

                new NormalizedValueProperty("Move Speed", Read16(1024), 0, short.MaxValue, 1024f, BitCount.Bit16, "Pathing Properties"),
                new NormalizedValueProperty("Height Offset", Read16(0), short.MinValue, short.MaxValue, 512f, BitCount.Bit16, "Pathing Properties"),
                new RangeActorProperty("Minimum Speed Multiplier", Read16(4096), 0, 8192, 4096f, BitCount.Bit16, "Pathing Properties"),
                new NormalizedValueProperty("Acceleration", Read16(4096), 0, 8192, 4096f, BitCount.Bit16, "Pathing Properties"),
                new NormalizedValueProperty("Unknown Multiplier 70", Read16(4096), 0, short.MaxValue, 4096f, BitCount.Bit16, "Pathing Properties"),
                new NormalizedValueProperty("Unknown Multiplier 72", Read16(4096), 0, short.MaxValue, 4096f, BitCount.Bit16, "Pathing Properties"),
                new NormalizedValueProperty ("Unknown Multiplier 74", Read16(4096), 0, short.MaxValue, 4096f, BitCount.Bit16, "Pathing Properties"),
                new ValueActorProperty("Unknown 76", Read16(1), short.MinValue, short.MaxValue, BitCount.Bit16, "Pathing Properties"),
                new ValueActorProperty("Unknown 78", Read8(4), 0, 255, BitCount.Bit8, "Pathing Properties"),
                new FillerActorProperty(Read8(0), BitCount.Bit8),

            });

            InitPropertiesByName();

        }

        public void SetHeight(float height) {
            ((NormalizedValueProperty)propertiesByName["Height Offset"]).Set(height);
        }

        public float GetHeight() {
            return ((NormalizedValueProperty)propertiesByName["Height Offset"]).value;
        }

        public ActorProperty GetHeightProperty() {
            return propertiesByName["Height Offset"];
        }

        public ActorGroundCast GetGroundCast() {
            return ActorGroundCast.Highest;
        }

    }

    // - Parsed -
    public class FCopBehavior1 : FCopEntity, FCopObjectMutating {

        public FCopBehavior1(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object 1", AssetType.Object),
                new ActorAssetReference("None", AssetType.None),
                new ActorAssetReference("Object 2", AssetType.Object),
                new ActorAssetReference("Object 3", AssetType.Object),
                new ActorAssetReference("Object 4", AssetType.Object),
                new ActorAssetReference("Object 5", AssetType.Object),
            };

            properties.Add(new RangeActorProperty("Rotation", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16));

            // Implies ground cast but Future Cop won't react except with 0x01 which will crash. Leaving at default 0xFF
            properties.Add(new FillerActorProperty(Read8(0), BitCount.Bit8));
            properties.Add(new FillerActorProperty(Read8(0xFF), BitCount.Bit8));

            InitPropertiesByName();
        }



        public ObjectMutation[] GetMutations() {

            return new ObjectMutation[] {

                new ObjectMutation(0, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(2, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(3, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(4, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(5, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value)

            };

        }

    }

    // - Animation Dependant, Implemented -
    public class FCopBehavior5 : FCopPathedEntity {

        public const int assetRefCount = 3;

        public FCopBehavior5(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object),
                new ActorAssetReference("Nav Mesh", AssetType.NavMesh),
            };

            properties.AddRange(new List<ActorProperty>() {

                new ValueActorProperty("Move Animation (Unk)", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("Sub Model (Unknown)", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("5_Unknown", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8)

            });

            InitPropertiesByName();

        }

    }

    // Ok instead of have the "Turret" Object the parent object may just be behavior 6.
    // The only issues is from what I can tell, the only property is always zero which
    // makes it hard to actually confirm that.
    // - Completed -
    public class FCopBehavior6 : FCopTurret {

        public const int assetRefCount = 2;
        public const int blocks = 24;

        public FCopBehavior6(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object)
            };

            properties.AddRange(new List<ActorProperty>() {
                new FillerActorProperty(Read16(0), BitCount.Bit16)
            });

            InitPropertiesByName();

        }

    }

    // - Completed -
    public class FCopBehavior8 : FCopTurret {

        public const int assetRefCount = 4;

        public FCopBehavior8(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] { 
                new ActorAssetReference("Head Object", AssetType.Object, 2, 0),
                new ActorAssetReference("None", AssetType.None),
                new ActorAssetReference("Base Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object),
            };

            properties.AddRange(new List<ActorProperty>() {
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                // This value is only ever used in M3A, either 1 or 0, making it a fill.
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new RangeActorProperty("Base Rotation", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16)
            });

            InitPropertiesByName();

        }

        public override ObjectMutation[] GetMutations() {

            return new ObjectMutation[] {

                new ObjectMutation(0, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(2, GetUVOffset(), ((RangeActorProperty)propertiesByName["Base Rotation"]).value)

            };

        }

    }

    // - Completed -
    public class FCopBehavior9 : FCopShooter, FCopHeightOffsetting {

        public const int assetRefCount = 2;

        public FCopBehavior9(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("None", AssetType.None)
            };

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("Unknown 1", Read8(3), 1, 3, BitCount.Bit8),
                new EnumDataActorProperty("Aircraft Target Type", (TargetType)Read8(1), BitCount.Bit8, "", new() { "OverloadAircraftAttack" }),

                new OverloadedProperty("OverloadAircraftAttack", new() {
                    (new AssetActorProperty("Aircraft Attack Team", Read16NoIt(0), AssetType.Team, BitCount.Bit16), () => (TargetType)propertiesByName["Aircraft Target Type"].GetCompiledValue() == TargetType.Team),
                    (new AssetActorProperty("Aircraft Attack Group", Read16NoIt(0), AssetType.ScriptGroup, BitCount.Bit16), () => (TargetType)propertiesByName["Aircraft Target Type"].GetCompiledValue() == TargetType.Group),
                    (new AssetActorProperty("Aircraft Attack Actor", Read16NoIt(0), AssetType.Actor, BitCount.Bit16), () => (TargetType)propertiesByName["Aircraft Target Type"].GetCompiledValue() == TargetType.Actor),
                    (new EnumDataActorProperty("Aircraft Attack Behavior", (ActorBehavior)Read16NoIt(1), BitCount.Bit16), () => (TargetType)propertiesByName["Aircraft Target Type"].GetCompiledValue() == TargetType.BehaviorType),
                    (new ValueActorProperty("Aircraft Attack", Read16(1), short.MinValue, short.MaxValue, BitCount.Bit16), () => true),
                }, BitCount.Bit16),

                new EnumDataActorProperty("Target Acquisition", (AircraftTargetAcquire)Read8(1), BitCount.Bit8),
                new EnumDataActorProperty("Spawn Type", (AircraftSpawnType)Read8(0), BitCount.Bit8),
                new NormalizedValueProperty("Target Detection Range", Read16(28672), 0, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("Min Distance From Target", Read16(512), 0, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("Unknown 2", Read16(1024), 0, short.MaxValue, 1024f, BitCount.Bit16),
                new NormalizedValueProperty("Height Offset", Read16(512), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("Time To Descend", Read16(1024), 0, short.MaxValue, 1024f, BitCount.Bit16),
                new NormalizedValueProperty("Turn Rate", Read16(4096), 0, short.MaxValue, 4096f, BitCount.Bit16),
                new NormalizedValueProperty("Move Speed", Read16(1024), 0, short.MaxValue, 1024f, BitCount.Bit16),
                new NormalizedValueProperty("Orbit Area Width", Read16(22528), 0, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("Orbit Area Height", Read16(22528), 0, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("Engage Time", Read16(5120), 0, short.MaxValue, 256f, BitCount.Bit16),
                new NormalizedValueProperty("Engage Cooldown", Read16(1280), 0, short.MaxValue, 256f, BitCount.Bit16),
                new NormalizedValueProperty("Spawn Pos X", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16),
                new NormalizedValueProperty("Spawn Pos Y", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16),

            });
            
            InitPropertiesByName();

        }

        public void SetHeight(float height) {
            ((NormalizedValueProperty)propertiesByName["Height Offset"]).Set(height);
        }

        public float GetHeight() {
            return ((NormalizedValueProperty)propertiesByName["Height Offset"]).value;
        }

        public ActorProperty GetHeightProperty() {
            return propertiesByName["Height Offset"];
        }

        public ActorGroundCast GetGroundCast() {
            return ActorGroundCast.Highest;
        }

    }

    // - Completed -
    public class FCopBehavior10 : FCopEntity, FCopHeightOffsetting, FCopObjectMutating {

        public const int assetRefCount = 2;
        public const int blocks = 20;

        public FCopBehavior10(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object)
            };

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Number Of Stops", (ElevatorStops)Read8(2), BitCount.Bit8),
                new EnumDataActorProperty("Starting Position", (ElevatorStartingPoint)Read8(2), BitCount.Bit8),
                new NormalizedValueProperty("1st Height Offset", Read16(0), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("2nt Height Offset", Read16(600), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("3rd Height Offset", Read16(800), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("1st Stop Time", Read16(256), 0, short.MaxValue, 256f, BitCount.Bit16),
                new NormalizedValueProperty("2nt Stop Time", Read16(256), 0, short.MaxValue, 256f, BitCount.Bit16),
                new NormalizedValueProperty("3rd Stop Time", Read16(256), 0, short.MaxValue, 256f, BitCount.Bit16),
                new NormalizedValueProperty("Up Speed", Read16(128), 0, short.MaxValue, 128f, BitCount.Bit16),
                new NormalizedValueProperty("Down Speed", Read16(128), 0, short.MaxValue, 128f, BitCount.Bit16),
                new RangeActorProperty("Rotation", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new EnumDataActorProperty("Trigger Type", (ElevatorTrigger)Read8(0), BitCount.Bit8),
                new EnumDataActorProperty("Tile Effect", (TileEffectType)Read8(0), BitCount.Bit8),
                new AssetActorProperty("End Sound", Read16(0), AssetType.WavSound, BitCount.Bit16),

            });

            InitPropertiesByName();

        }

        public void SetHeight(float height) {
            ((NormalizedValueProperty)propertiesByName["1st Height Offset"]).Set(height);
        }

        public float GetHeight() {
            return ((NormalizedValueProperty)propertiesByName["1st Height Offset"]).value;
        }

        public ActorProperty GetHeightProperty() {
            return propertiesByName["1st Height Offset"];
        }

        public ActorGroundCast GetGroundCast() {
            return ActorGroundCast.Highest;
        }

        public ObjectMutation[] GetMutations() {

            return new ObjectMutation[] {

                new ObjectMutation(0, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value)

            };

        }

    }

    // - Completed -
    public class FCopBehavior11 : FCopEntity, FCopHeightOffsetting, FCopObjectMutating {

        public const int assetRefCount = 2;
        public const int blocks = 12;

        public FCopBehavior11(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object)
            };

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read16(0), BitCount.Bit16),
                new RangeActorProperty("Rotation", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new NormalizedValueProperty("Height Offset", Read16(0), short.MinValue, short.MaxValue, 8192f, BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16)
            });

            InitPropertiesByName();

        }

        public void SetHeight(float height) {
            ((NormalizedValueProperty)propertiesByName["Height Offset"]).Set(height);
        }

        public float GetHeight() {
            return ((NormalizedValueProperty)propertiesByName["Height Offset"]).value;
        }

        public ActorProperty GetHeightProperty() {
            return propertiesByName["Height Offset"];
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)((EnumDataActorProperty)propertiesByName["Ground Cast"]).caseValue;
        }

        public ObjectMutation[] GetMutations() {

            return new ObjectMutation[] {

                new ObjectMutation(0, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(1, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value)


            };

        }

    }

    // - Completed -
    public class FCopBehavior12 : FCopEntity, FCopHeightOffsetting, FCopObjectMutating {

        public int heightMultiplier { get; set; }

        public const int assetRefCount = 2;
        public const int blocks = 12;

        public FCopBehavior12(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object)
            };

            properties.AddRange(new List<ActorProperty>() {
                new RangeActorProperty("Rotation Y", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new RangeActorProperty("Rotation X", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new NormalizedValueProperty("Height Offset", Read16(0), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),
                new EnumDataActorProperty("Tile Effect", (TileEffectType)Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8)
            });

            InitPropertiesByName();

        }

        public void SetHeight(float height) {
            ((NormalizedValueProperty)propertiesByName["Height Offset"]).Set(height);
        }

        public float GetHeight() {
            return ((NormalizedValueProperty)propertiesByName["Height Offset"]).value;
        }

        public ActorProperty GetHeightProperty() {
            return propertiesByName["Height Offset"];
        }

        public ActorGroundCast GetGroundCast() {
            return ActorGroundCast.Highest;
        }

        public ObjectMutation[] GetMutations() {

            return new ObjectMutation[] {

                new ObjectMutation(0, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation X"]).value, ((RangeActorProperty)propertiesByName["Rotation Y"]).value, 0),

            };

        }

    }
    
    // Breifly Observed (Not simple)
    public class FCopBehavior14 : FCopActorBehavior {

        public FCopBehavior14(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {

                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Unknown 1", Read1(0x02, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Unknown 2", Read1(0x08, false), BitCount.Bit1),
                new FillerActorProperty(1, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Unknown 3", Read1(0x80, false), BitCount.Bit1),
            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Unknown 4", Read1(0x01, false), BitCount.Bit1),
                new ToggleActorProperty("Unknown 5", Read1(0x02, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {
                new FillerActorProperty(Read8(2), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("Unknown 6", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new NormalizedValueProperty("Interact Radius", Read16(0), 0, short.MaxValue, 256f, BitCount.Bit16),
                new ValueActorProperty("Unknown 7", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),

            });

            properties.AddRange(new List<ActorProperty>() {

                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Enabled", Read1(0x02, false), BitCount.Bit1),
                new ToggleActorProperty("Trigger By Action", Read1(0x04, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Script Disable", Read1(0x10, false), BitCount.Bit1),
                new ToggleActorProperty("Run Script Callback", Read1(0x20, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Trigger Type", (TriggerType)Read8(1), BitCount.Bit8, "", new() { "OverloadTrigger" }),
                new OverloadedProperty("OverloadTrigger", new() {
                    (new EnumDataActorProperty("Triggering Behavior", (ActorBehavior)Read16NoIt(0), BitCount.Bit16), () => (TriggerType)propertiesByName["Trigger Type"].GetCompiledValue() == TriggerType.BehaviorType),
                    (new AssetActorProperty("Triggering Actor", Read16(0), AssetType.Actor, BitCount.Bit16), () => (TriggerType)propertiesByName["Trigger Type"].GetCompiledValue() == TriggerType.Actor),
                }, BitCount.Bit16),
            });

            InitPropertiesByName();

        }

    }

    // - Completed -
    public class FCopBehavior16 : FCopEntity {

        public const int assetRefCount = 2;

        public FCopBehavior16(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Claim",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("None", AssetType.None)
            };

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(1), BitCount.Bit8),
                new FillerActorProperty(Read8(1), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(81), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(51), BitCount.Bit8),
                new FillerActorProperty(Read8(3), BitCount.Bit8),
                new FillerActorProperty(Read8(51), BitCount.Bit8),
                new FillerActorProperty(Read8(3), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(8), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(99), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),

            });

            // 60

            properties.AddRange(new List<ActorProperty>() {
                new ToggleActorProperty("Reload Gun", Read1(0x01, false), BitCount.Bit1),
                new ToggleActorProperty("Reload Heavy", Read1(0x02, false), BitCount.Bit1),
                new ToggleActorProperty("Reload Special", Read1(0x04, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Power Up Gun", Read1(0x10, false), BitCount.Bit1),
                new ToggleActorProperty("Power Up Heavy", Read1(0x20, false), BitCount.Bit1),
                new ToggleActorProperty("Power Up Special", Read1(0x40, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
            });
            offset++;

            properties.Add(new FillerActorProperty(Read8(0), BitCount.Bit8));

            properties.AddRange(new List<ActorProperty>() {
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Restore Health", Read1(0x02, false), BitCount.Bit1),
                new ToggleActorProperty("Invisibility", Read1(0x04, false), BitCount.Bit1),
                new ToggleActorProperty("Invincibility", Read1(0x08, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1)
            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {
                new FillerActorProperty(Read8(3), BitCount.Bit8),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new NormalizedValueProperty("Rotation Speed", Read16(1024), short.MinValue, short.MaxValue, 1024f, BitCount.Bit16),
            });

            InitPropertiesByName();

        }

    }

    // - Implemented, One Unknown, One Explosion Dependant -
    public class FCopBehavior20 : FCopPathedEntity, FCopObjectMutating {

        public const int assetRefCount = 4;

        public FCopBehavior20(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Base Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object),
                new ActorAssetReference("Nav Mesh", AssetType.NavMesh),
                new ActorAssetReference("Head Object", AssetType.Object, 0, 0),
            };

            properties.AddRange(new List<ActorProperty>() {

                new NormalizedValueProperty("Turn Speed", Read16(64), 0, 256, 64f, BitCount.Bit16),
                new RangeActorProperty("Head Rotation", Read16(0), -1, 4096, 4096f / 360f, BitCount.Bit16),

            });

            properties.AddRange(new List<ActorProperty>() {

                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Thruster Behavior Override", Read1(0x02, false), BitCount.Bit1, "Pathed Turret Tags"),
                new ToggleActorProperty("Spin Head (No Engaging)", Read1(0x04, false), BitCount.Bit1, "Pathed Turret Tags"),
                new ToggleActorProperty("Shoot With Base Object", Read1(0x08, false), BitCount.Bit1, "Pathed Turret Tags"),
                new ToggleActorProperty("Look at Target X-Axis", Read1(0x10, false), BitCount.Bit1, "Pathed Turret Tags"),
                new ToggleActorProperty("Lock Head", Read1(0x20, false), BitCount.Bit1, "Pathed Turret Tags"),
                new ToggleActorProperty("Targetable Head Object", Read1(0x40, false), BitCount.Bit1, "Pathed Turret Tags"),
                new ToggleActorProperty("PTTag Unknown", Read1(0x80, false), BitCount.Bit1, "Pathed Turret Tags"),

            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {

                new ExplosionActorProperty("Secondary Explosion", Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8)

            });

            InitPropertiesByName();

        }

        public ObjectMutation[] GetMutations() {

            return new ObjectMutation[] {

                new ObjectMutation(3, GetUVOffset(), ((RangeActorProperty)propertiesByName["Head Rotation"]).value)

            };

        }

    }

    // - Completed -
    public class FCopBehavior25 : FCopEntity, FCopHeightOffsetting, FCopObjectMutating {

        public const int assetRefCount = 2;
        public const int blocks = 16;

        public FCopBehavior25(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object)
            };

            properties.Add(new EnumDataActorProperty("Move Axis", (MoveablePropMoveAxis)Read8(0), BitCount.Bit8));

            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Start in End Position", Read1(0x01, false), BitCount.Bit1),
                new ToggleActorProperty("Looping", Read1(0x02, false), BitCount.Bit1),
                new ToggleActorProperty("Walkable", Read1(0x04, false), BitCount.Bit1),
                new ToggleActorProperty("Enabled", Read1(0x08, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),

            });

            offset++;

            properties.AddRange(new List<ActorProperty>() {

                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read8(0), BitCount.Bit8),
                new AssetActorProperty("Start Sound", Read8(0), AssetType.WavSound, BitCount.Bit8),
                new NormalizedValueProperty("Height Offset", Read16(0), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),
                new RangeActorProperty("Rotation", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new NormalizedValueProperty("Ending Position Offset", Read16(0), short.MinValue, short.MaxValue, 8192f, BitCount.Bit16),
                new RangeActorProperty("Ending Rotation", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new NormalizedValueProperty("Position Speed", Read16(0), 0, short.MaxValue, 8192f, BitCount.Bit16),
                new NormalizedValueProperty("Rotation Speed", Read16(0), 0, short.MaxValue, 1024f, BitCount.Bit16),

            });

            InitPropertiesByName();

        }

        public void SetHeight(float height) {
            ((NormalizedValueProperty)propertiesByName["Height Offset"]).Set(height);
        }

        public float GetHeight() {
            return ((NormalizedValueProperty)propertiesByName["Height Offset"]).value;
        }

        public ActorProperty GetHeightProperty() {
            return propertiesByName["Height Offset"];
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)((EnumDataActorProperty)propertiesByName["Ground Cast"]).caseValue;
        }

        public ObjectMutation[] GetMutations() {

            return new ObjectMutation[] {

                new ObjectMutation(0, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(1, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value)

            };

        }

    }

    // Observed, very animation dependant
    public class FCopBehavior26 : FCopPathedEntity {

        public FCopBehavior26(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object),
                new ActorAssetReference("Nav Mesh", AssetType.NavMesh),
            };

            properties.AddRange(new List<ActorProperty>() {

                new ValueActorProperty("Unknown 80", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("Sub Model (Unknown)", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("Unknown 82", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("Unknown 83", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("Move Animation (Unk)", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("Unknown 86", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("Unknown 87", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8)

            });

            InitPropertiesByName();

        }

    }

    // Breifly Observed, very animation dependant
    public class FCopBehavior27 : FCopShooter {

        public FCopBehavior27(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object)
            };

            properties.AddRange(new List<ActorProperty>() {

                new ValueActorProperty("60", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("61", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("62", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new RangeActorProperty("Rotation", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new ValueActorProperty("66", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("68", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("70", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("72", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("74", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("76", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("78", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("79", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("80", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("81", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("82", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("83", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("84", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("85", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("86", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),
                new ValueActorProperty("87", Read8(0), short.MinValue, short.MaxValue, BitCount.Bit8),

            });

            InitPropertiesByName();

        }

    }

    // - Implemented, One Unknown, Explosion Dependant, Many unknows from other types (Shooter, Path Turret) -
    public class FCopBehavior28 : FCopPathedEntity {

        public const int assetRefCount = 7;

        public FCopBehavior28(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Base Object", AssetType.Object),
                new ActorAssetReference("Destroyed Object", AssetType.Object),
                new ActorAssetReference("Nav Mesh", AssetType.NavMesh),
                new ActorAssetReference("Head Object 1", AssetType.Object, 0, 0),
                new ActorAssetReference("Head Object 2", AssetType.Object, 0, 1),
                new ActorAssetReference("Head Object 3", AssetType.Object, 0, 2),
                new ActorAssetReference("Head Object 4", AssetType.Object, 0, 3),
            };

            properties.AddRange(new List<ActorProperty>() {

                new NormalizedValueProperty("Turn Speed", Read16(64), 0, 256, 64f, BitCount.Bit16),
                new ValueActorProperty("Unknown", Read16(0), 0, 1, BitCount.Bit16),

            });

            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Independent Object (H1)", Read1(0x01, false), BitCount.Bit1, "Turret Properties Head 1"),
                new ToggleActorProperty("Thruster Behavior Override (H1)", Read1(0x02, false), BitCount.Bit1, "Turret Properties Head 1"),
                new ToggleActorProperty("Spin Head (No Engaging) (H1)", Read1(0x04, false), BitCount.Bit1, "Turret Properties Head 1"),
                new ToggleActorProperty("Shoot With Base Object (H1)", Read1(0x08, false), BitCount.Bit1, "Turret Properties Head 1"),
                new ToggleActorProperty("Look at Target X-Axis (H1)", Read1(0x10, false), BitCount.Bit1, "Turret Properties Head 1"),
                new ToggleActorProperty("Lock Head (H1)", Read1(0x20, false), BitCount.Bit1, "Turret Properties Head 1"),
                new ToggleActorProperty("Targetable Head Object (H1)", Read1(0x40, false), BitCount.Bit1, "Turret Properties Head 1"),
                new ToggleActorProperty("PTTag Unknown (H1)", Read1(0x80, false), BitCount.Bit1, "Turret Properties Head 1"),

            });
            offset++;
            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Independent Object (H2)", Read1(0x01, false), BitCount.Bit1, "Turret Properties Head 2"),
                new ToggleActorProperty("Thruster Behavior Override (H2)", Read1(0x02, false), BitCount.Bit1, "Turret Properties Head 2"),
                new ToggleActorProperty("Spin Head (No Engaging) (H2)", Read1(0x04, false), BitCount.Bit1, "Turret Properties Head 2"),
                new ToggleActorProperty("Shoot With Base Object (H2)", Read1(0x08, false), BitCount.Bit1, "Turret Properties Head 2"),
                new ToggleActorProperty("Look at Target X-Axis (H2)", Read1(0x10, false), BitCount.Bit1, "Turret Properties Head 2"),
                new ToggleActorProperty("Lock Head (H2)", Read1(0x20, false), BitCount.Bit1, "Turret Properties Head 2"),
                new ToggleActorProperty("Targetable Head Object (H2)", Read1(0x40, false), BitCount.Bit1, "Turret Properties Head 2"),
                new ToggleActorProperty("PTTag Unknown (H2)", Read1(0x80, false), BitCount.Bit1, "Turret Properties Head 2"),

            });
            offset++;
            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Independent Object (H3)", Read1(0x01, false), BitCount.Bit1, "Turret Properties Head 3"),
                new ToggleActorProperty("Thruster Behavior Override (H3)", Read1(0x02, false), BitCount.Bit1, "Turret Properties Head 3"),
                new ToggleActorProperty("Spin Head (No Engaging) (H3)", Read1(0x04, false), BitCount.Bit1, "Turret Properties Head 3"),
                new ToggleActorProperty("Shoot With Base Object (H3)", Read1(0x08, false), BitCount.Bit1, "Turret Properties Head 3"),
                new ToggleActorProperty("Look at Target X-Axis (H3)", Read1(0x10, false), BitCount.Bit1, "Turret Properties Head 3"),
                new ToggleActorProperty("Lock Head (H3)", Read1(0x20, false), BitCount.Bit1, "Turret Properties Head 3"),
                new ToggleActorProperty("Targetable Head Object (H3)", Read1(0x40, false), BitCount.Bit1, "Turret Properties Head 3"),
                new ToggleActorProperty("PTTag Unknown (H3)", Read1(0x80, false), BitCount.Bit1, "Turret Properties Head 3"),

            });
            offset++;
            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Independent Object (H4)", Read1(0x01, false), BitCount.Bit1, "Turret Properties Head 4"),
                new ToggleActorProperty("Thruster Behavior Override (H4)", Read1(0x02, false), BitCount.Bit1, "Turret Properties Head 4"),
                new ToggleActorProperty("Spin Head (No Engaging) (H4)", Read1(0x04, false), BitCount.Bit1, "Turret Properties Head 4"),
                new ToggleActorProperty("Shoot With Base Object (H4)", Read1(0x08, false), BitCount.Bit1, "Turret Properties Head 4"),
                new ToggleActorProperty("Look at Target X-Axis (H4)", Read1(0x10, false), BitCount.Bit1, "Turret Properties Head 4"),
                new ToggleActorProperty("Lock Head (H4)", Read1(0x20, false), BitCount.Bit1, "Turret Properties Head 4"),
                new ToggleActorProperty("Targetable Head Object (H4)", Read1(0x40, false), BitCount.Bit1, "Turret Properties Head 4"),
                new ToggleActorProperty("PTTag Unknown (H4)", Read1(0x80, false), BitCount.Bit1, "Turret Properties Head 4"),

            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {

                new ExplosionActorProperty("Head 1 Explosion", Read8(0), BitCount.Bit8, "Turret Properties Head 1"),
                new ExplosionActorProperty("Head 2 Explosion", Read8(0), BitCount.Bit8, "Turret Properties Head 2"),
                new ExplosionActorProperty("Head 3 Explosion", Read8(0), BitCount.Bit8, "Turret Properties Head 3"),
                new ExplosionActorProperty("Head 4 Explosion", Read8(0), BitCount.Bit8, "Turret Properties Head 4"),

            });

            #region Head 2

            properties.AddRange(new List<ActorProperty>() {
               new SpecializedActorRefActorProperty("Weapon (H2)", Read16(0), ActorBehavior.Weapon, true, BitCount.Bit16, "Shooter Properties Head 2")
            });

            properties.AddRange(new List<ActorProperty>() {

                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Prevent Back Shooting (H2)", Read1(0x02, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("Shoot When Facing (H2)", Read1(0x04, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("STag Unknown (H2)", Read1(0x08, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("STag Unknown 2 (H2)", Read1(0x10, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("Fire Alternations (H2)", Read1(0x20, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("Target Priority (H2)", Read1(0x40, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("STag Unknown 3 (H2)", Read1(0x80, false), BitCount.Bit1, "Shooter Tags Head 2"),

            });
            offset++;
            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Disabled (H2)", Read1(0x01, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("Weapon Actor Collision (H2)", Read1(0x02, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("Attackable Weapon (H2)", Read1(0x04, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("STag Unknown 4 (H2)", Read1(0x10, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("STag Unknown 5 (H2)", Read1(0x20, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("Allow Switch Target (H2)", Read1(0x40, false), BitCount.Bit1, "Shooter Tags Head 2"),
                new ToggleActorProperty("STag Unknown 6 (H2)", Read1(0x80, false), BitCount.Bit1, "Shooter Tags Head 2"),

            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Acquiring Type (H2)", (AcquiringType)Read8(0), BitCount.Bit8, "Shooter Properties Head 2"),
                new EnumDataActorProperty("Target Type (H2)", (TargetType)Read8(1), BitCount.Bit8, "Shooter Properties Head 2", new() { "OverloadAttack2" }),

                new OverloadedProperty("OverloadAttack2", new() {
                    (new AssetActorProperty("Attack Team (H2)", Read16NoIt(0), AssetType.Team, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H2)"].GetCompiledValue() == TargetType.Team),
                    (new AssetActorProperty("Attack Group (H2)", Read16NoIt(0), AssetType.ScriptGroup, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H2)"].GetCompiledValue() == TargetType.Group),
                    (new AssetActorProperty("Attack Actor (H2)", Read16NoIt(0), AssetType.Actor, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H2)"].GetCompiledValue() == TargetType.Actor),
                    (new EnumDataActorProperty("Attack Behavior (H2)", (ActorBehavior)Read16NoIt(1), BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H2)"].GetCompiledValue() == TargetType.BehaviorType),
                    (new ValueActorProperty("Attack (H2)", Read16(1), short.MinValue, short.MaxValue, BitCount.Bit16), () => true),
                }, BitCount.Bit16, "Shooter Properties Head 2"),

                new NormalizedValueProperty("Detection FOV? (H2)", Read16(4096), 0, 4096, 4096f / 360f, BitCount.Bit16, "Shooter Properties Head 2"),
                new NormalizedValueProperty("Shooting FOV? (H2)", Read16(4096), 0, 4096, 4096f / 360f, BitCount.Bit16, "Shooter Properties Head 2"),
                new NormalizedValueProperty("Engage Range (H2)", Read16(6144), 0, short.MaxValue, 512f, BitCount.Bit16, "Shooter Properties Head 2"),
                new NormalizedValueProperty("Targeting Delay (H2)", Read16(32), 0, 320, 32f, BitCount.Bit16, "Shooter Properties Head 2"),
            });

            #endregion

            #region Head 3

            properties.AddRange(new List<ActorProperty>() {
                new SpecializedActorRefActorProperty("Weapon (H3)", Read16(0), ActorBehavior.Weapon, true, BitCount.Bit16, "Shooter Properties Head 3")
            });

            properties.AddRange(new List<ActorProperty>() {

                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Prevent Back Shooting (H3)", Read1(0x02, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("Shoot When Facing (H3)", Read1(0x04, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("STag Unknown (H3)", Read1(0x08, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("STag Unknown 2 (H3)", Read1(0x10, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("Fire Alternations (H3)", Read1(0x20, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("Target Priority (H3)", Read1(0x40, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("STag Unknown 3 (H3)", Read1(0x80, false), BitCount.Bit1, "Shooter Tags Head 3"),

            });
            offset++;
            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Disabled (H3)", Read1(0x01, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("Weapon Actor Collision (H3)", Read1(0x02, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("Attackable Weapon (H3)", Read1(0x04, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("STag Unknown 4 (H3)", Read1(0x10, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("STag Unknown 5 (H3)", Read1(0x20, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("Allow Switch Target (H3)", Read1(0x40, false), BitCount.Bit1, "Shooter Tags Head 3"),
                new ToggleActorProperty("STag Unknown 6 (H3)", Read1(0x80, false), BitCount.Bit1, "Shooter Tags Head 3"),

            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Acquiring Type (H3)", (AcquiringType)Read8(0), BitCount.Bit8, "Shooter Properties Head 3"),
                new EnumDataActorProperty("Target Type (H3)", (TargetType)Read8(1), BitCount.Bit8, "Shooter Properties Head 3", new() { "OverloadAttack3" }),

                new OverloadedProperty("OverloadAttack3", new() {
                    (new AssetActorProperty("Attack Team (H3)", Read16NoIt(0), AssetType.Team, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H3)"].GetCompiledValue() == TargetType.Team),
                    (new AssetActorProperty("Attack Group (H3)", Read16NoIt(0), AssetType.ScriptGroup, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H3)"].GetCompiledValue() == TargetType.Group),
                    (new AssetActorProperty("Attack Actor (H3)", Read16NoIt(0), AssetType.Actor, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H3)"].GetCompiledValue() == TargetType.Actor),
                    (new EnumDataActorProperty("Attack Behavior (H3)", (ActorBehavior)Read16NoIt(1), BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H3)"].GetCompiledValue() == TargetType.BehaviorType),
                    (new ValueActorProperty("Attack (H3)", Read16(1), short.MinValue, short.MaxValue, BitCount.Bit16), () => true),
                }, BitCount.Bit16, "Shooter Properties Head 3"),

                new NormalizedValueProperty("Detection FOV? (H3)", Read16(4096), 0, 4096, 4096f / 360f, BitCount.Bit16, "Shooter Properties Head 3"),
                new NormalizedValueProperty("Shooting FOV? (H3)", Read16(4096), 0, 4096, 4096f / 360f, BitCount.Bit16, "Shooter Properties Head 3"),
                new NormalizedValueProperty("Engage Range (H3)", Read16(6144), 0, short.MaxValue, 512f, BitCount.Bit16, "Shooter Properties Head 3"),
                new NormalizedValueProperty("Targeting Delay (H3)", Read16(32), 0, 320, 32f, BitCount.Bit16, "Shooter Properties Head 3"),
            });

            #endregion

            #region Head 4

            properties.AddRange(new List<ActorProperty>() {
                new SpecializedActorRefActorProperty("Weapon (H4)", Read16(0), ActorBehavior.Weapon, true, BitCount.Bit16, "Shooter Properties Head 4")
            });

            properties.AddRange(new List<ActorProperty>() {

                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Prevent Back Shooting (H4)", Read1(0x02, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("Shoot When Facing (H4)", Read1(0x04, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("STag Unknown (H4)", Read1(0x08, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("STag Unknown 2 (H4)", Read1(0x10, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("Fire Alternations (H4)", Read1(0x20, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("Target Priority (H4)", Read1(0x40, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("STag Unknown 3 (H4)", Read1(0x80, false), BitCount.Bit1, "Shooter Tags Head 4"),

            });
            offset++;
            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Disabled (H4)", Read1(0x01, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("Weapon Actor Collision (H4)", Read1(0x02, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("Attackable Weapon (H4)", Read1(0x04, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("STag Unknown 4 (H4)", Read1(0x10, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("STag Unknown 5 (H4)", Read1(0x20, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("Allow Switch Target (H4)", Read1(0x40, false), BitCount.Bit1, "Shooter Tags Head 4"),
                new ToggleActorProperty("STag Unknown 6 (H4)", Read1(0x80, false), BitCount.Bit1, "Shooter Tags Head 4"),

            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Acquiring Type (H4)", (AcquiringType)Read8(0), BitCount.Bit8, "Shooter Properties Head 4"),
                new EnumDataActorProperty("Target Type (H4)", (TargetType)Read8(1), BitCount.Bit8, "Shooter Properties Head 4", new() { "OverloadAttack4" }),

                new OverloadedProperty("OverloadAttack4", new() {
                    (new AssetActorProperty("Attack Team (H4)", Read16NoIt(0), AssetType.Team, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H4)"].GetCompiledValue() == TargetType.Team),
                    (new AssetActorProperty("Attack Group (H4)", Read16NoIt(0), AssetType.ScriptGroup, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H4)"].GetCompiledValue() == TargetType.Group),
                    (new AssetActorProperty("Attack Actor (H4)", Read16NoIt(0), AssetType.Actor, BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H4)"].GetCompiledValue() == TargetType.Actor),
                    (new EnumDataActorProperty("Attack Behavior (H4)", (ActorBehavior)Read16NoIt(1), BitCount.Bit16), () => (TargetType)propertiesByName["Target Type (H4)"].GetCompiledValue() == TargetType.BehaviorType),
                    (new ValueActorProperty("Attack (H4)", Read16(1), short.MinValue, short.MaxValue, BitCount.Bit16), () => true),
                }, BitCount.Bit16, "Shooter Properties Head 4"),

                new NormalizedValueProperty("Detection FOV? (H4)", Read16(4096), 0, 4096, 4096f / 360f, BitCount.Bit16, "Shooter Properties Head 4"),
                new NormalizedValueProperty("Shooting FOV? (H4)", Read16(4096), 0, 4096, 4096f / 360f, BitCount.Bit16, "Shooter Properties Head 4"),
                new NormalizedValueProperty("Engage Range (H4)", Read16(6144), 0, short.MaxValue, 512f, BitCount.Bit16, "Shooter Properties Head 4"),
                new NormalizedValueProperty("Targeting Delay (H4)", Read16(32), 0, 320, 32f, BitCount.Bit16, "Shooter Properties Head 4"),
            });

            #endregion

            InitPropertiesByName(new() {
                "Entity Tags",
                "Entity Properties",
                "Shooter Tags",
                "Shooter Properties",
                "Pathing Tags",
                "Pathing Properties",
                "Turret Properties Head 1",
                "Turret Properties Head 2",
                "Shooter Tags Head 2",
                "Shooter Properties Head 2",
                "Turret Properties Head 3",
                "Shooter Tags Head 3",
                "Shooter Properties Head 3",
                "Turret Properties Head 4",
                "Shooter Tags Head 4",
                "Shooter Properties Head 4"
            });


        }

    }

    // - Completed -
    public class FCopBehavior29 : FCopEntity {

        public const int assetRefCount = 2;

        public FCopBehavior29(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Teleport",
                "None",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("None", AssetType.None),
                new ActorAssetReference("None", AssetType.None)
            };

            properties.AddRange(new List<ActorProperty>() {

                new NormalizedValueProperty("X", Read32(0), 0, int.MaxValue, 4096f, BitCount.Bit32),
                new NormalizedValueProperty("Y", Read32(0), 0, int.MaxValue, 4096f, BitCount.Bit32),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(1), BitCount.Bit8),
                new FillerActorProperty(Read8(1), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new NormalizedValueProperty("Trigger Radius", Read16(0), 0, short.MaxValue, 512f, BitCount.Bit16),
                new FillerActorProperty(Read8(204), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8)

            });

            InitPropertiesByName();

        }

    }

    // - Implemented..?, Very script dependent -
    public class FCopBehavior30 : FCopTurret {

        public FCopBehavior30(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On New Second"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object 1", AssetType.Object),
                new ActorAssetReference("Object 2", AssetType.Object),
                new ActorAssetReference("Object 3", AssetType.Object),
                new ActorAssetReference("Object 4", AssetType.Object),
                new ActorAssetReference("Object 5", AssetType.Object)
            };

            properties.Add(new FillerActorProperty(Read16(0), BitCount.Bit16));

            InitPropertiesByName();

        }

        public override ObjectMutation[] GetMutations() {
            return new ObjectMutation[] {

                new ObjectMutation(0, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(1, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(2, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(3, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(4, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value)

            };
        }

    }

    // Low Priority
    public class FCopBehavior31 : FCopEntity {

        public FCopBehavior31(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    // - Completed, One Unknown -
    public class FCopBehavior32 : FCopEntity, FCopObjectMutating, FCopHeightOffsetting {

        public const int assetRefCount = 3;

        public FCopBehavior32(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On Interact"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Base Object", AssetType.Object),
                new ActorAssetReference("None", AssetType.None),
                new ActorAssetReference("Item Object", AssetType.Object)
            };

            properties.AddRange(new List<ActorProperty>() {
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(1), BitCount.Bit8),
                new FillerActorProperty(Read16(1), BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new NormalizedValueProperty("Open Radius", Read16(0), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),

            });

            properties.AddRange(new List<ActorProperty>() {
                new ToggleActorProperty("Reload Gun", Read1(0x01, false), BitCount.Bit1),
                new ToggleActorProperty("Reload Heavy", Read1(0x02, false), BitCount.Bit1),
                new ToggleActorProperty("Reload Special", Read1(0x04, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Power Up Gun", Read1(0x10, false), BitCount.Bit1),
                new ToggleActorProperty("Power Up Heavy", Read1(0x20, false), BitCount.Bit1),
                new ToggleActorProperty("Power Up Special", Read1(0x40, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
            });
            offset++;

            properties.Add(new FillerActorProperty(Read8(0), BitCount.Bit8));

            properties.AddRange(new List<ActorProperty>() {
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Restore Health", Read1(0x02, false), BitCount.Bit1),
                new ToggleActorProperty("Invisibility", Read1(0x04, false), BitCount.Bit1),
                new ToggleActorProperty("Invincibility", Read1(0x08, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1)
            });
            offset++;

            properties.Add(new FillerActorProperty(Read8(0), BitCount.Bit8));

            properties.AddRange(new List<ActorProperty>() {
                new RangeActorProperty("Rotation", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new ValueActorProperty("Unknown", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
            });

            InitPropertiesByName();

        }

        public void SetHeight(float height) {

        }

        public float GetHeight() {
            return 0f;
        }

        public ActorProperty GetHeightProperty() {
            return null;
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)((EnumDataActorProperty)propertiesByName["Ground Cast"]).caseValue;
        }

        public ObjectMutation[] GetMutations() {

            return new ObjectMutation[] {

                new ObjectMutation(0, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(1, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value),
                new ObjectMutation(2, GetUVOffset(), ((RangeActorProperty)propertiesByName["Rotation"]).value)


            };

        }

    }

    // Low Priority
    public class FCopBehavior33 : FCopEntity {

        public FCopBehavior33(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    // TODO, Delayed Trigger?
    public class FCopBehavior34 : FCopActorBehavior {

        public FCopBehavior34(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    // - Completed - (One unknown)
    public class FCopBehavior35 : FCopActorBehavior {

        public const int assetRefCount = 2;
        public const int blocks = 32;

        public FCopBehavior35(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("None", AssetType.None),
                new ActorAssetReference("None", AssetType.None)
            };

            properties.Add(new ValueActorProperty("Unknown", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16));

            properties.AddRange(new List<ActorProperty>() {

                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new FillerActorProperty(Read16(1), BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),
                new FillerActorProperty(Read16(255), BitCount.Bit16),
                new FillerActorProperty(Read16(0), BitCount.Bit16),

            });

            properties.AddRange(new List<ActorProperty>() {
                new ToggleActorProperty("Show Arrow Node 1", Read1(0x01, false), BitCount.Bit1, "Node 1 Properties"),
                new ToggleActorProperty("Show Satellite Node 1", Read1(0x02, false), BitCount.Bit1, "Node 1 Properties"),
                new ToggleActorProperty("Show Minimap Node 1", Read1(0x04, false), BitCount.Bit1, "Node 1 Properties"),
                new ToggleActorProperty("Show Arrow Node 2", Read1(0x08, false), BitCount.Bit1, "Node 2 Properties"),
                new ToggleActorProperty("Show Satellite Node 2", Read1(0x10, false), BitCount.Bit1, "Node 2 Properties"),
                new ToggleActorProperty("Show Minimap Node 2", Read1(0x20, false), BitCount.Bit1, "Node 2 Properties"),
                new ToggleActorProperty("Show Arrow Node 3", Read1(0x40, false), BitCount.Bit1, "Node 3 Properties"),
                new ToggleActorProperty("Show Satellite Node 3", Read1(0x80, false), BitCount.Bit1, "Node 3 Properties"),

            });
            offset++;
            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Show Minimap Node 3", Read1(0x01, false), BitCount.Bit1, "Node 3 Properties"),
                new ToggleActorProperty("Show Arrow Node 4", Read1(0x02, false), BitCount.Bit1, "Node 4 Properties"),
                new ToggleActorProperty("Show Satellite Node 4", Read1(0x04, false), BitCount.Bit1, "Node 4 Properties"),
                new ToggleActorProperty("Show Minimap Node 4", Read1(0x08, false), BitCount.Bit1, "Node 4 Properties"),
                new ToggleActorProperty("Show Arrow Node 5", Read1(0x10, false), BitCount.Bit1, "Node 5 Properties"),
                new ToggleActorProperty("Show Satellite Node 5", Read1(0x20, false), BitCount.Bit1, "Node 5 Properties"),
                new ToggleActorProperty("Show Minimap Node 5", Read1(0x40, false), BitCount.Bit1, "Node 5 Properties"),
                new ToggleActorProperty("Show Arrow Node 6", Read1(0x80, false), BitCount.Bit1, "Node 6 Properties"),

            });
            offset++;
            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Show Satellite Node 6", Read1(0x01, false), BitCount.Bit1, "Node 6 Properties"),
                new ToggleActorProperty("Show Minimap Node 6", Read1(0x02, false), BitCount.Bit1, "Node 6 Properties"),
                new ToggleActorProperty("Show Arrow Node 7", Read1(0x04, false), BitCount.Bit1, "Node 7 Properties"),
                new ToggleActorProperty("Show Satellite Node 7", Read1(0x08, false), BitCount.Bit1, "Node 7 Properties"),
                new ToggleActorProperty("Show Minimap Node 7", Read1(0x10, false), BitCount.Bit1, "Node 7 Properties"),
                new ToggleActorProperty("Show Arrow Node 8", Read1(0x20, false), BitCount.Bit1, "Node 8 Properties"),
                new ToggleActorProperty("Show Satellite Node 8", Read1(0x40, false), BitCount.Bit1, "Node 8 Properties"),
                new ToggleActorProperty("Show Minimap Node 8", Read1(0x80, false), BitCount.Bit1, "Node 8 Properties"),

                new FillerActorProperty(0, BitCount.Bit8)
            });

            offset += 2;

            properties.AddRange(new List<ActorProperty>() {

                new EnumDataActorProperty("Map Icon Color 1", (MapIconColor)Read8(0), BitCount.Bit8, "Node 1 Properties"),
                new EnumDataActorProperty("Map Icon Color 2", (MapIconColor)Read8(0), BitCount.Bit8, "Node 2 Properties"),
                new EnumDataActorProperty("Map Icon Color 3", (MapIconColor)Read8(0), BitCount.Bit8, "Node 3 Properties"),
                new EnumDataActorProperty("Map Icon Color 4", (MapIconColor)Read8(0), BitCount.Bit8, "Node 4 Properties"),
                new EnumDataActorProperty("Map Icon Color 5", (MapIconColor)Read8(0), BitCount.Bit8, "Node 5 Properties"),
                new EnumDataActorProperty("Map Icon Color 6", (MapIconColor)Read8(0), BitCount.Bit8, "Node 6 Properties"),
                new EnumDataActorProperty("Map Icon Color 7", (MapIconColor)Read8(0), BitCount.Bit8, "Node 7 Properties"),
                new EnumDataActorProperty("Map Icon Color 8", (MapIconColor)Read8(0), BitCount.Bit8, "Node 8 Properties"),

                new NormalizedValueProperty("Node 1 X", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 1 Properties"),
                new NormalizedValueProperty("Node 1 Y", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 1 Properties"),
                new NormalizedValueProperty("Node 2 X", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 2 Properties"),
                new NormalizedValueProperty("Node 2 Y", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 2 Properties"),
                new NormalizedValueProperty("Node 3 X", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 3 Properties"),
                new NormalizedValueProperty("Node 3 Y", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 3 Properties"),
                new NormalizedValueProperty("Node 4 X", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 4 Properties"),
                new NormalizedValueProperty("Node 4 Y", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 4 Properties"),
                new NormalizedValueProperty("Node 5 X", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 5 Properties"),
                new NormalizedValueProperty("Node 5 Y", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 5 Properties"),
                new NormalizedValueProperty("Node 6 X", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 6 Properties"),
                new NormalizedValueProperty("Node 6 Y", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 6 Properties"),
                new NormalizedValueProperty("Node 7 X", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 7 Properties"),
                new NormalizedValueProperty("Node 7 Y", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 7 Properties"),
                new NormalizedValueProperty("Node 8 X", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 8 Properties"),
                new NormalizedValueProperty("Node 8 Y", Read16(0), 0, short.MaxValue, 16f, BitCount.Bit16, "Node 8 Properties"),

            });

            InitPropertiesByName();

        }

    }

    // - Completed -
    public class FCopBehavior36 : FCopBehavior8 {

        public FCopBehavior36(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Hurt",
                "On Death",
                "On Interact"
            };

            properties.AddRange(new List<ActorProperty>() {
                new AssetActorProperty("1st Interact Team", Read8(0), AssetType.Team, BitCount.Bit8),
                new AssetActorProperty("2nt Interact Team", Read8(0), AssetType.Team, BitCount.Bit8),
                new EnumDataActorProperty("First Map Icon Color", (MapIconColor)Read8(0), BitCount.Bit8),
                new EnumDataActorProperty("Second Map Icon Color", (MapIconColor)Read8(0), BitCount.Bit8),
                new ValueActorProperty("Interact UV Offset X", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("Interact UV Offset Y", Read8(0), 0, 255, BitCount.Bit8),
                new NormalizedValueProperty("Trigger Radius", Read16(0), 0, short.MaxValue, 512f, BitCount.Bit16)
            });

            InitPropertiesByName();

        }

    }

    // Low Priority, Sky Captain
    public class FCopBehavior37 : FCopEntity {

        public FCopBehavior37(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    // Low Priority, Actor Controller?
    public class FCopBehavior38 : FCopEntity {

        public FCopBehavior38(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public class FCopBehavior87 : FCopActorBehavior {

        public FCopBehavior87(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("28", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("29", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("30", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("34", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("36", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("38", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("40", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("41", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("42", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("43", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("45", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("48", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("50", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("54", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("56", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("58", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("60", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("62", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("64", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("65", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("66", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("67", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("68", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("69", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("71", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("72", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("73", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("74", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("76", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("78", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("80", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("82", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("84", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("86", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("88", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("90", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("94", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
            });

            InitPropertiesByName();

        }

    }

    public class FCopBehavior88 : FCopActorBehavior {

        public FCopBehavior88(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("28", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("29", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("30", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("34", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("36", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("38", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("41", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("54", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("56", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("58", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("60", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("62", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("64", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("67", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("68", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("69", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("70", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("72", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("74", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("78", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("80", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("82", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
            });

            InitPropertiesByName();

        }

    }

    public class FCopBehavior89 : FCopActorBehavior {

        public FCopBehavior89(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("28", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("29", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("30", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("34", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("36", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("40", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("41", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("42", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("43", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("44", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("45", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("46", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("47", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("48", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("50", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("58", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("60", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("62", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("64", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("66", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("68", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("70", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("71", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("72", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("73", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("74", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("75", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("76", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("77", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("78", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("79", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("80", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("81", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("82", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("84", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("86", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("88", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("90", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("92", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("94", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),

            });

            InitPropertiesByName();

        }

    }

    public class FCopBehavior90 : FCopActorBehavior {

        public FCopBehavior90(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("28", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("29", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("30", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("34", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("38", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("40", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("41", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("42", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("43", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("44", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("45", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("46", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("47", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("58", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("60", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("61", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("62", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("64", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("66", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("68", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("70", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("74", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("76", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("78", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("80", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("81", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("82", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),

            });

            InitPropertiesByName();

        }

    }

    public class FCopBehavior91 : FCopActorBehavior, SpecializedID {

        public FCopBehavior91(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("ID", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("29 Ref", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("30", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("32", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("33", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("36 Ref?", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("37", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("38", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("39", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("40 Ref?", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("41", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("42", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("43", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("44 Ref?", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("45", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("46", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("47", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("48 Ref?", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("49", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("50", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("51", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("52 Ref?", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("53", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("54", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("55", Read8(0), 0, 255, BitCount.Bit8),

            });

            InitPropertiesByName();

        }

        public int GetID() {
            return propertiesByName["ID"].GetCompiledValue();
        }
    }

    public class FCopBehavior92 : FCopActorBehavior {

        public FCopBehavior92(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("28", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("29", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("30", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("34", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("36", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("44", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("45", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("46", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("47", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("48", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("50", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("54", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("58", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("60", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("62", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("64", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("66", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("68", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("70", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("72", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("74", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("75", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("76", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("77", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("78", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("79", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("80", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("84", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("86", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("88", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("90", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),

            });

            InitPropertiesByName();

        }

    }

    public class FCopBehavior93 : FCopActorBehavior {

        public FCopBehavior93(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("28", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("29", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("30", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("32", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("34", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("36", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("38", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("40", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("41", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("42", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("43", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("44", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("45", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("46", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("47", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("48", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("50", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("54", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("56", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("58", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("60", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("62", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("64", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("66", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("68", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("70", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("73", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("74", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("75", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("77", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("78", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("79", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("80", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("82", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("84", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("86", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("88", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("90", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("92", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("94", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
            });

            InitPropertiesByName();

        }

    }

    public class FCopBehavior94 : FCopActorBehavior {

        public FCopBehavior94(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("28", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("29", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("30", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("32", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("34", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("36", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(255), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("40", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("41", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("42", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("43", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("44", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("45", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("46", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("47", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("48", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("50", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("58", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("60", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("62", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("64", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("66", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("68", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("70", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("72", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("74", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("76", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("78", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("80", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("82", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("84", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("86", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("88", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("89", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("90", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("91", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("92", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("93", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("94", Read8(0), 0, 255, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new ValueActorProperty("96", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("98", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("99", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("100", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("102", Read8(0), 0, 255, BitCount.Bit8),
                new ValueActorProperty("103", Read8(0), 0, 255, BitCount.Bit8),

            });

            InitPropertiesByName();

        }

    }

    // - Completed, Two Unknowns -
    public class FCopBehavior95 : FCopActorBehavior, FCopHeightOffsetting {

        public const int assetRefCount = 1;

        public FCopBehavior95(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            callbackNames = new string[] {
                "On Trigger",
                "None",
                "None"
            };

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("None", AssetType.None)
            };

            properties.AddRange(new List<ActorProperty>() {

                new NormalizedValueProperty("Width Area", Read16(512), 0, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("Length Area", Read16(512), 0, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("Height Area", Read16(512), 0, short.MaxValue, 512f, BitCount.Bit16),
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read8(0), BitCount.Bit8),


            });

            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Unknown", Read1(0x01, false), BitCount.Bit1, "Trigger Tags"),
                new ToggleActorProperty("Can Retrigger", Read1(0x02, false), BitCount.Bit1, "Trigger Tags"),
                new ToggleActorProperty("Trigger By Action", Read1(0x04, false), BitCount.Bit1, "Trigger Tags"),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Disable Trigger", Read1(0x10, false), BitCount.Bit1, "Trigger Tags"),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Unknown (Crowd Control)", Read1(0x40, false), BitCount.Bit1, "Trigger Tags"),
                new FillerActorProperty(0, BitCount.Bit1)

            });
            offset++;

            // This one is a little confusing, so if "Triggering Actor" is -1 it will only target the player, otherwise it references an actor.
            // For a more use friendly way, I may an overload that doesn't read any actual property data.
            properties.AddRange(new List<ActorProperty>() {

                new EnumDataActorProperty("Actor Triggering Type", Read16NoIt(0) > -1 ? TriggeringActorType.Actor : TriggeringActorType.Player, BitCount.NA, "", new() { "TriggeringActorOverload" }),
                new OverloadedProperty("TriggeringActorOverload", new() {
                    (new AssetActorProperty("Triggering Actor", Read16NoIt(0), AssetType.Actor, BitCount.Bit16), () => (TriggeringActorType)propertiesByName["Actor Triggering Type"].GetCompiledValue() == TriggeringActorType.Actor),
                    (new FillerActorProperty(-1, BitCount.Bit16), () => (TriggeringActorType)propertiesByName["Actor Triggering Type"].GetCompiledValue() == TriggeringActorType.Player),
                }, BitCount.Bit16),

            });

            offset += 2;

            properties.AddRange(new List<ActorProperty>() {

                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8)

            });

            InitPropertiesByName();

        }

        public void SetHeight(float height) {

        }

        public float GetHeight() {
            return 0f;
        }

        public ActorProperty GetHeightProperty() {
            return null;
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)((EnumDataActorProperty)propertiesByName["Ground Cast"]).caseValue;
        }

    }

    // - Completed, Three Unknowns -
    public class FCopBehavior96 : FCopActorBehavior, FCopObjectMutating, FCopHeightOffsetting {

        public const int assetRefCount = 1;

        public FCopBehavior96(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("Object", AssetType.Object)
            };

            properties = new() {

                new RangeActorProperty("Rotation Y", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new RangeActorProperty("Rotation Z", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new RangeActorProperty("Rotation X", Read16(0), -4096, 4096, 4096f / 360f, BitCount.Bit16),
                new NormalizedValueProperty("Height Offset", Read16(0), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read8(0), BitCount.Bit8),

            };

            properties.AddRange(new List<ActorProperty>() {

                new ToggleActorProperty("Unknown (Rendering Order)", Read1(0x01, false), BitCount.Bit1, "Tags"),
                new ToggleActorProperty("Unknown1", Read1(0x02, false), BitCount.Bit1, "Tags"),
                new ToggleActorProperty("Unknown2", Read1(0x04, false), BitCount.Bit1, "Tags"),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Disable Rendering", Read1(0x10, false), BitCount.Bit1, "Tags"),
                new ToggleActorProperty("Disable Animation", Read1(0x20, false), BitCount.Bit1, "Tags"),
                new ToggleActorProperty("Reverse Animation", Read1(0x40, false), BitCount.Bit1, "Tags"),
                new FillerActorProperty(0, BitCount.Bit1)

            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {

                new RangeActorProperty("Animation Speed", Read8(0), 0, 32, 1f, BitCount.Bit8),
                new RangeActorProperty("Scale X", Read8(64), 0, 127, 64f, BitCount.Bit8),
                new RangeActorProperty("Scale Y", Read8(64), 0, 127, 64f, BitCount.Bit8),
                new RangeActorProperty("Scale Z", Read8(64), 0, 127, 64f, BitCount.Bit8),
                new RangeActorProperty("Spin Speed", ReadS8(0), -16, 16, 1f, BitCount.Bit8),
                new RangeActorProperty("Spin Angle", Read8(0), 0, 180, 1f, BitCount.Bit8)


            });

            InitPropertiesByName();

        }

        public void SetHeight(float height) {
            ((NormalizedValueProperty)propertiesByName["Height Offset"]).Set(height);
        }

        public float GetHeight() {
            return ((NormalizedValueProperty)propertiesByName["Height Offset"]).value;
        }

        public ActorProperty GetHeightProperty() {
            return propertiesByName["Height Offset"];
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)((EnumDataActorProperty)propertiesByName["Ground Cast"]).caseValue;
        }

        public ObjectMutation[] GetMutations() {

            return new ObjectMutation[] {

                new ObjectMutation(0, 0, 
                ((RangeActorProperty)propertiesByName["Rotation X"]).value, 
                ((RangeActorProperty)propertiesByName["Rotation Y"]).value,
                ((RangeActorProperty)propertiesByName["Rotation Z"]).value,
                ((RangeActorProperty)propertiesByName["Scale X"]).value,
                ((RangeActorProperty)propertiesByName["Scale Y"]).value,
                ((RangeActorProperty)propertiesByName["Scale Z"]).value)

            };

        }

    }

    // - Completed -
    public class FCopBehavior97 : FCopActorBehavior, FCopHeightOffsetting {

        public const int assetRefCount = 4;

        public FCopBehavior97(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            assetReferences = new ActorAssetReference[] {
                new ActorAssetReference("None", AssetType.None),
                new ActorAssetReference("None", AssetType.None),
                new ActorAssetReference("None", AssetType.None),
                new ActorAssetReference("None", AssetType.None)
            };

            properties.AddRange(new List<ActorProperty>() {

                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Transparent", Read1(0x02, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new ToggleActorProperty("Additive", Read1(0x10, false), BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1),
                new FillerActorProperty(0, BitCount.Bit1)

            });
            offset++;

            properties.AddRange(new List<ActorProperty>() {

                new AssetActorProperty("Texture Snippet", Read8(0), AssetType.TextureSnippet, BitCount.Bit8),
                new NormalizedValueProperty("Height Offset", Read16(0), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("Width", Read16(512), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),
                new NormalizedValueProperty("Height", Read16(512), short.MinValue, short.MaxValue, 512f, BitCount.Bit16),
                new RangeActorProperty("Rotation Y", Read16(0), 0, 4096, 4096f / 360f, BitCount.Bit16),
                new RangeActorProperty("Rotation X", Read16(0), 0, 4096, 4096f / 360f, BitCount.Bit16),
                new RangeActorProperty("Rotation Z", Read16(0), 0, 4096, 4096f / 360f, BitCount.Bit16),
                new EnumDataActorProperty("Ground Cast", (ActorGroundCast)Read8(0), BitCount.Bit8),
                new RangeActorProperty("Red", Read8(128), 0, 128, 128f, BitCount.Bit8),
                new RangeActorProperty("Green", Read8(128), 0, 128, 128f, BitCount.Bit8),
                new RangeActorProperty("Blue", Read8(128), 0, 128, 128f, BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8),
                new FillerActorProperty(Read8(0), BitCount.Bit8)

            });

            InitPropertiesByName();

        }

        public void SetHeight(float height) {
            ((NormalizedValueProperty)propertiesByName["Height Offset"]).Set(height);
        }

        public float GetHeight() {
            return ((NormalizedValueProperty)propertiesByName["Height Offset"]).value;
        }

        public ActorProperty GetHeightProperty() {
            return propertiesByName["Height Offset"];
        }

        public ActorGroundCast GetGroundCast() {
            return (ActorGroundCast)((EnumDataActorProperty)propertiesByName["Ground Cast"]).caseValue;
        }

    }

    // - Implemented, TODO: Effects -
    public class FCopBehavior98 : FCopActorBehavior, SpecializedID {

        public const int assetRefCount = 4;

        public FCopBehavior98(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            assetReferences = new ActorAssetReference[] {

                new ActorAssetReference("Object", AssetType.Object),
                new ActorAssetReference("None", AssetType.None),
                new ActorAssetReference("None", AssetType.None),
                new ActorAssetReference("None", AssetType.None)

            };

            properties.AddRange(new List<ActorProperty>() {
                new ValueActorProperty("Weapon ID", Read8(0), 0, 255, BitCount.Bit8),
                new EnumDataActorProperty("Type", (WeaponType)Read8(1), BitCount.Bit8, "", new() { "OverloadMaxRange", "Overload52", "OverloadBlastRadius" }),
                new ValueActorProperty("Ammo Count", Read16(-1), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("Reload Count", Read16(100), short.MinValue, short.MaxValue, BitCount.Bit16),
                new ValueActorProperty("Burst Shot Count", Read16(0), 0, 100, BitCount.Bit16),
                new NormalizedValueProperty("Fire Delay", Read16(16), 0, short.MaxValue, 16f, BitCount.Bit16),
                new NormalizedValueProperty("Burst Fire Delay", Read16(16), 0, short.MaxValue, 16f, BitCount.Bit16),
                new ValueActorProperty("Damage", Read16(5), 0, short.MaxValue, BitCount.Bit16),

                new OverloadedProperty("OverloadBlastRadius", new() {
                    (new NormalizedValueProperty("Blast Radius (MUST SET!)", Read16NoIt(256), 0, short.MaxValue, 512f, BitCount.Bit16), () => (WeaponType)propertiesByName["Type"].GetCompiledValue() == WeaponType.Grenade),
                    (new NormalizedValueProperty("Shield Radius", Read16NoIt(0), 0, short.MaxValue, 512f, BitCount.Bit16), () => (WeaponType)propertiesByName["Type"].GetCompiledValue() == WeaponType.Shield),
                    (new NormalizedValueProperty("Blast Radius", Read16(0), 0, short.MaxValue, 512f, BitCount.Bit16), () => true),
                }, BitCount.Bit16),

                new ValueActorProperty("Unknown 44", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),
                new NormalizedValueProperty("Velocity", Read16(4096), 0, short.MaxValue, 1024f, BitCount.Bit16),
                new ValueActorProperty("Unknown 48", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16),

                new OverloadedProperty("OverloadMaxRange", new() {
                    (new NormalizedValueProperty("Gravity", Read16NoIt(6144), 0, short.MaxValue, 512f,  BitCount.Bit16), () => (WeaponType)propertiesByName["Type"].GetCompiledValue() == WeaponType.Mortar),
                    (new NormalizedValueProperty("Gravity", Read16NoIt(6144), 0, short.MaxValue, 512f,  BitCount.Bit16), () => (WeaponType)propertiesByName["Type"].GetCompiledValue() == WeaponType.Bomb),
                    (new NormalizedValueProperty("Gravity", Read16NoIt(6144), 0, short.MaxValue, 512f,  BitCount.Bit16), () => (WeaponType)propertiesByName["Type"].GetCompiledValue() == WeaponType.Grenade),
                    (new NormalizedValueProperty("Max Range", Read16(6144), 0, short.MaxValue, 512f,  BitCount.Bit16), () => true),
                }, BitCount.Bit16),

                new OverloadedProperty("Overload52", new() {
                    (new ValueActorProperty("Fuse Time", Read16NoIt(0), short.MinValue, short.MaxValue, BitCount.Bit16), () => (WeaponType)propertiesByName["Type"].GetCompiledValue() == WeaponType.Grenade),
                    (new ValueActorProperty("Unknown 52", Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16), () => true),
                }, BitCount.Bit16),

                new ImpactActorProperty("Impact Effect", Read8(0), BitCount.Bit8),
                new ImpactActorProperty("Weapon Effects", Read8(0), BitCount.Bit8),
                new AssetActorProperty("Shoot Sound", Read8(0), AssetType.WavSound, BitCount.Bit8),
                new ValueActorProperty("Unknown 57", Read8(0), 0, 255, BitCount.Bit8),
                new AssetActorProperty("Echo Sound", Read8(0), AssetType.WavSound, BitCount.Bit8),
                new ValueActorProperty("Unknown 59", Read8(0), 0, 255, BitCount.Bit8),
            });

            InitPropertiesByName();

        }

        public int GetID() {
            return propertiesByName["Weapon ID"].GetCompiledValue();
        }

    }

    public class FCopBehavior99 : FCopActorBehavior {

        public FCopBehavior99(FCopActor actor, List<byte> propertyData) : base(actor, propertyData) {

            var propertyCount = (propertyData.Count - offset) / 2;

            foreach (var i in Enumerable.Range(0, propertyCount)) {
                var property = new ValueActorProperty("value " + offset.ToString(), Read16(0), short.MinValue, short.MaxValue, BitCount.Bit16);
                properties.Add(property);
            }

            InitPropertiesByName();

        }

    }

    public enum ActorBehavior {

        Player = 1,
        PathedEntity = 5,
        StationaryEntity = 6,
        StationaryTurret = 8,
        Aircraft = 9,
        Elevator = 10,
        DynamicProp = 11,
        WalkableProp = 12,
        UniversalTrigger = 14,
        FloatingItem = 16,
        PathedTurret = 20,
        MovableProp = 25,
        Behavior26 = 26,
        Behavior27 = 27,
        PathedMultiTurret = 28,
        Teleporter = 29,
        InterchangingEntity = 30,
        Behavior31 = 31,
        Reloader = 32,
        Behavior33 = 33,
        Behavior34 = 34,
        MapObjectiveNodes = 35,
        ClaimableTurret = 36,
        Behavior37 = 37,
        Behavior38 = 38,
        VisualEffects87 = 87,
        VisualEffects88 = 88,
        VisualEffects89 = 89,
        VisualEffects90 = 90,
        ActorExplosion = 91,
        VisualEffects92 = 92,
        ParticleEmitter = 93,
        VisualEffects94 = 94,
        Trigger = 95,
        StaticProp = 96,
        Texture = 97,
        Weapon = 98,
        PlayerWeapon = 99

    }

}