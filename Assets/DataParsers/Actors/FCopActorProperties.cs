
using System.Collections.Generic;
using System;
using System.Linq;

namespace FCopParser {

    public interface ActorProperty {

        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
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

    public class ValueActorProperty : ActorProperty {
        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }
        public int GetCompiledValue() {
            return value;
        }

        public void SafeSetSigned(int newValue) {

            var maxValue = (int)((Math.Pow(2, (int)bitCount) - 1) / 2);
            var minValue = -(int)(Math.Pow(2, (int)bitCount) / 2);

            value = newValue;

            if (value > maxValue) {
                value = maxValue;
            }
            if (value < minValue) {
                value = minValue;
            }

        }

        public int value;

        public ValueActorProperty(string name, int value, BitCount bitCount) {
            this.name = name;
            this.value = value;
            this.bitCount = bitCount;
            this.commonName = "";

        }

        public ValueActorProperty(string name, int value, BitCount bitCount, string commonName) : this(name, value, bitCount) {
            this.commonName = commonName;
        }

    }

    public class ToggleActorProperty : ActorProperty {

        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }

        public int GetCompiledValue() {
            return value ? 1 : 0;
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

    public class IDReferenceActorProperty : ActorProperty {
        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }

        public int GetCompiledValue() {
            return value;
        }

        public int value;

        public IDReferenceActorProperty(string name, int value) {
            this.name = name;
            this.value = value;
            this.commonName = "";

        }



    }

    public class EnumDataActorProperty : ActorProperty {
        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }

        public int GetCompiledValue() {
            return Convert.ToInt32(caseValue);
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

    }

    public class RangeActorProperty : ActorProperty {
        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }

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
            this.commonName = "";

        }



    }

    public class RotationActorProperty : ActorProperty {
        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }

        public int[] affectedRefIndexes;
        public ActorRotation value;
        public Axis axis;

        public int GetCompiledValue() {
            return value.compiledRotation;
        }

        public RotationActorProperty(string name, ActorRotation value, BitCount bitCount, Axis axis, int[] affectedRefIndexes) {
            this.name = name;
            this.value = value;
            this.bitCount = bitCount;
            this.axis = axis;
            this.affectedRefIndexes = affectedRefIndexes;
            this.commonName = "";

        }

        public RotationActorProperty(string name, ActorRotation value, BitCount bitCount, Axis axis, int[] affectedRefIndexes, string commonName) : this(name, value, bitCount, axis, affectedRefIndexes) {
            this.commonName = commonName;
        }

    }

    public class FillerActorProperty : ActorProperty {

        public string name { get; set; }
        public BitCount bitCount { get; set; }
        public string commonName { get; set; }

        public int GetCompiledValue() {
            return value;
        }

        public int value;

        public FillerActorProperty(int value, BitCount bitCount) {
            this.name = "Null";
            this.value = value;
            this.bitCount = bitCount;
            this.commonName = "";
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

    public enum BitCount {
        NA = -1,
        Bit1 = 1,
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
        Middle = 3,
        Default = 255

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

}