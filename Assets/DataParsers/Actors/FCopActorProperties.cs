
using System.Collections.Generic;
using System;
using System.Linq;

namespace FCopParser {

    public interface ActorProperty {

        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public int GetCompiledValue();

        public void SetCompiledValue(int value);

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

    public class ValueActorProperty : ActorProperty {
        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public int min;
        public int max;
        public int value;

        public int GetCompiledValue() {
            return value;
        }

        public void SetCompiledValue(int value) {
            this.value = value;
        }

        public void Set(int value) {

            if (value > max) {
                this.value = max;
            }
            else if (value < min) {
                this.value = min;
            }
            else {
                this.value = value;
            }

        }


        public ValueActorProperty(string name, int value, int min, int max, BitCount bitCount) {
            this.name = name;
            this.value = value;
            this.min = min;
            this.max = max;
            this.bitCount = bitCount;
            this.commonName = "";

        }

        public ValueActorProperty(string name, int value, int min, int max, BitCount bitCount, string commonName) : this(name, value, min, max, bitCount) {
            this.commonName = commonName;
        }

    }

    public class ToggleActorProperty : ActorProperty {

        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public int GetCompiledValue() {
            return value ? 1 : 0;
        }

        public void SetCompiledValue(int value) {
            this.value = value == 1;
        }

        public bool value;

        public ToggleActorProperty(string name, bool value, BitCount bitCount) {
            this.name = name;
            this.value = value;
            this.bitCount = bitCount;
            this.commonName = "";
        }

        public ToggleActorProperty(string name, bool value, BitCount bitCount, string commonName) : this(name, value, bitCount) {
            this.commonName = commonName;
        }

    }

    public class NormalizedValueProperty : ActorProperty {
        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public float min;
        public float max;
        public float multiplier;
        public float value;

        public int GetCompiledValue() {
            return (int)(value * multiplier);
        }

        public void SetCompiledValue(int value) {
            this.value = value / multiplier;
        }

        public void Set(float value) {

            if (value > max) {
                this.value = max;
            }
            else if (value < min) {
                this.value = min;
            }
            else {
                this.value = value;
            }

        }

        public NormalizedValueProperty(string name, int value, int min, int max, float multiplier, BitCount bitCount) {
            this.name = name;
            this.value = value / multiplier;
            this.min = min / multiplier;
            this.max = max / multiplier;
            this.multiplier = multiplier;
            this.bitCount = bitCount;
            this.commonName = "";
        }

        public NormalizedValueProperty(string name, int value, int min, int max, float multiplier, BitCount bitCount, string commonName): this(name, value, min, max, multiplier, bitCount) {
            this.commonName = commonName;
        }

    }

    public class EnumDataActorProperty : ActorProperty {
        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public int GetCompiledValue() {
            return Convert.ToInt32(caseValue);
        }

        public void SetCompiledValue(int value) {
            caseValue = (Enum)Enum.ToObject(caseValue.GetType(), value);
        }

        public Enum caseValue;

        public EnumDataActorProperty(string name, Enum caseValue, BitCount bitCount) {
            this.name = name;
            this.caseValue = caseValue;
            this.bitCount = bitCount;
            this.commonName = "";
        }

        public EnumDataActorProperty(string name, Enum caseValue, BitCount bitCount, string commonName) : this(name, caseValue, bitCount) {
            this.commonName = commonName;
        }


        public EnumDataActorProperty(string name, Enum caseValue, BitCount bitCount, string commonName, List<string> dictatesOverload) : this(name, caseValue, bitCount, commonName) {
            this.dictatesOverload = dictatesOverload;
        }
    }

    public class RangeActorProperty : ActorProperty {
        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public float min;
        public float max;
        public float multiplier;
        public float value;

        public int GetCompiledValue() {
            return (int)(value * multiplier);
        }

        public void SetCompiledValue(int value) {
            this.value = value / multiplier;
        }

        public void Set(float value) {

            if (value > max) {
                this.value = max;
            }
            else if (value < min) {
                this.value = min;
            }
            else {
                this.value = value;
            }

        }

        public RangeActorProperty(string name, int value, int min, int max, float multiplier, BitCount bitCount) {
            this.name = name;
            this.value = value / multiplier;
            this.min = min / multiplier;
            this.max = max / multiplier;
            this.multiplier = multiplier;
            this.bitCount = bitCount;
            this.commonName = "";
        }

        public RangeActorProperty(string name, int value, int min, int max, float multiplier, BitCount bitCount, string commonName) : this(name, value, min, max, multiplier, bitCount) {
            this.commonName = commonName;
        }

    }

    public class FillerActorProperty : ActorProperty {

        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public int GetCompiledValue() {
            return value;
        }

        public void SetCompiledValue(int value) {
            this.value = value;
        }

        public int value;

        public FillerActorProperty(int value, BitCount bitCount) {
            this.name = "Null";
            this.value = value;
            this.bitCount = bitCount;
            this.commonName = "";
        }

    }

    public class AssetActorProperty : ActorProperty {

        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public int GetCompiledValue() {
            return assetID;
        }

        public void SetCompiledValue(int value) {
            assetID = value;
        }

        public int assetID;
        public AssetType assetType;

        public AssetActorProperty(string name, int assetID, AssetType assetType, BitCount bitCount) {
            this.name = name;
            this.assetID = assetID;
            this.assetType = assetType;
            this.bitCount = bitCount;
            this.commonName = "";

        }

        public AssetActorProperty(string name, int assetID, AssetType assetType, BitCount bitCount, string commonName) : this(name, assetID, assetType, bitCount) {
            this.commonName = commonName;
        }

    }

    public class ExplosionActorProperty : ActorProperty {
        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public int GetCompiledValue() {
            return id;
        }

        public void SetCompiledValue(int value) {
            this.id = value;
        }

        public int id;

        public ExplosionActorProperty(string name, int id, BitCount bitCount) {
            this.name = name;
            this.id = id;
            this.bitCount = bitCount;
            this.commonName = "";

        }

        public ExplosionActorProperty(string name, int id, BitCount bitCount, string commonName) : this(name, id, bitCount) {
            this.commonName = commonName;
        }

    }

    public class SpecializedActorRefActorProperty : ActorProperty {

        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public int GetCompiledValue() {
            return id;
        }

        public void SetCompiledValue(int value) {
            this.id = value;
        }

        public int id;
        public ActorBehavior behaviorType;
        public bool specializedID;

        public SpecializedActorRefActorProperty(string name, int id, ActorBehavior behaviorType, bool specializedID, BitCount bitCount) {
            this.name = name;
            this.id = id;
            this.behaviorType = behaviorType;
            this.specializedID = specializedID;
            this.bitCount = bitCount;
            this.commonName = "";
        }

        public SpecializedActorRefActorProperty(string name, int id, ActorBehavior behaviorType, bool specializedID, BitCount bitCount, string commonName) : this(name, id, behaviorType, specializedID, bitCount) {
            this.commonName = commonName;
        }

    }

    public class OverloadedProperty : ActorProperty {

        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public List<string> dictatesOverload { get; set; }

        public List<(ActorProperty property, Func<bool> overloadCondition)> properties = new();

        public int GetCompiledValue() {
            
            return GetOverloadProperty().GetCompiledValue();

        }

        public void SetCompiledValue(int value) {
            GetOverloadProperty().SetCompiledValue(value);
        }

        public ActorProperty GetOverloadProperty() {

            foreach (var property in properties) {
                if (property.overloadCondition()) {
                    return property.property;
                }
            }

            throw new Exception("No property");

        }

        public OverloadedProperty(string name, List<(ActorProperty property, Func<bool> overloadCondition)> properties, BitCount bitCount) {
            this.name = name;
            this.properties = properties;
            this.bitCount = bitCount;
            this.commonName = "";
            UpdateCommonName();
        }

        public OverloadedProperty(string name, List<(ActorProperty property, Func<bool> overloadCondition)> properties, BitCount bitCount, string commonName) {
            this.name = name;
            this.properties = properties;
            this.bitCount = bitCount;
            this.commonName = commonName;
            UpdateCommonName();
        }

        void UpdateCommonName() {

            foreach (var prop in properties) {
                prop.property.commonName = commonName;
            }

        }

    }

    public struct ActorRotation {

        public static int maxRotation = 4096;

        public int compiledRotation;

        public float parsedRotation;

        public ActorRotation SetRotationDegree(float newRotation) {

            parsedRotation = newRotation;

            compiledRotation = (int)(newRotation / 360f * maxRotation);

            return this;

        }


        public ActorRotation SetRotationCompiled(int newRotation) {

            float rotationPrecentage = (float)newRotation / (float)maxRotation;

            float degreeRoation = 360f * rotationPrecentage;

            compiledRotation = newRotation;
            parsedRotation = degreeRoation;

            return this;

        }

        public static ActorRotation operator +(ActorRotation a, float b) {
            return a.SetRotationDegree(a.parsedRotation + b);
        }

    }

    public abstract class FCopExplosion {

        public static Dictionary<int, string> globalExplosions = new() {
            {10, "X1 Alpha"},
            {20, "Alive Flaming Crumble"},
            {21, "Alive Flaming Steam Disappear"},
            {22, "Small Green Flame Explosion"},
            {23, "Flaming Crumble"},
            {24, "Particle Shockwave"},
            {25, "Small Shockwave"},
            {26, "Small Explosion"},
            {27, "Red Shockwave With Launch"},
            {28, "Crumble"},
            {50, "Small Spark Explosion"},
            {91, "Infantry Explosion"},
            {99, "Fall Large Flash Explosion"},
            {100, "Multi-Shockwave Explosion"},
            {101, "Flaming Crumble Dupe"},
            {102, "Energy Explosion"},
            {109, "Launch Crumble"},
            {110, "Dynamic Infantry Death"},
        };

    }

    public enum BitCount {
        NA = -1,
        Bit1 = 1,
        Bit3 = 3,
        Bit5 = 5,
        Bit8 = 8,
        Bit16 = 16,
        Bit32 = 32,
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

    public enum MapIconColorObjective {
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
        FlashingGray = 31
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

    public enum PlayerCameraType {
        Standard = 0,
        CloseUp = 1,
        StandardSide = 2,
        CloseUpSide = 3,
        Sky = 4,
        CrowdControl = 5,
        CrowdControlSide = 6
    }

    public enum ActorGroundCast {
        Highest = 0,
        Lowest = 1,
        Middle = 3,
        NoCast = 255

    }

    public enum TargetType {
        ShootNoTarget = 0,
        PlayerOnly = 1,
        Actor = 2,
        NoTarget = 3,
        Team = 4
    }

    public enum AcquiringType {
        ForceFirstPlayer = 0,
        NormalIgnoreWalls = 1,
        Random = 2,
        Normal = 3
    }


    public enum ElevatorStops {
        Two = 2,
        Three = 3,
    }

    public enum ElevatorStartingPoint {
        First = 1,
        Second = 2,
        Third = 3
    }

    public enum ElevatorTrigger {
        Implied = 0,
        ActionOnly = 1,
        Unknown2 = 2,
        Unknown3 = 3,
        Unknown4 = 4
    }

    public enum ElevatorMoveType {
        NextStop = 0,
        FirstStop = 1,
        SecondStop = 2,
        ThirdStop = 3,
        FirstStopJump = 11
    }

    public enum MoveablePropMoveAxis {
        RotationY = 0,
        PositionZ = 1,
        PositionX = 2,
        PositionY = 3,
        RotationX = 4,
        RotationZ = 5
    }

    public enum AircraftSpawnType {

        EaseTakeoffActorPos = 0,
        EaseTakeoffRandom = 1,
        AirActorPos = 2,
        AirRandom = 3,
        VTOLActorPos = 4,
        VTOLRandom = 5,
        EaseTakeoffSpawnPos = 8,
        AirSpawnPos = 10,
        VTOLSpawnPos = 12

    }

    public enum TriggeringActorType {
        Player = 0,
        Actor = 1
    }

    public enum WeaponType {

        Direct = 1,
        Lead = 2,
        Homing = 3,
        Mortar = 4,
        Bomb = 6,
        DirectDupe = 7,
        Grenade = 9,
        Unknown = 10,
        Arch = 11,
        Bullet = 12,
        Shield = 13,
        Flame = 14,
        Laser = 17,
        VerticalHoming = 19,
        ClusterMortar = 20

    }

}