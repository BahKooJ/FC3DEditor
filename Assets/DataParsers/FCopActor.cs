

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCopParser {

    /*
     * 11 - prop
     * 14 - interactable
     * 16 - item
     * 
     * 
     */

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
        public int objectType;
        public int x;
        public int y;

        public List<FCopResource> resourceReferences = new();

        public FCopActorScript script;

        public IFFDataFile rawFile;

        public FCopActor(IFFDataFile rawFile) {

            this.rawFile = rawFile;

            FindStartChunkOffset();

            id = Utils.BytesToInt(rawFile.data.ToArray(), 8);
            objectType = Utils.BytesToInt(rawFile.data.ToArray(), 12);
            y = Utils.BytesToInt(rawFile.data.ToArray(), 16);
            x = Utils.BytesToInt(rawFile.data.ToArray(), 24);

            ParseResourceReferences();

            switch(objectType) {
                case 5:
                    script = new FCopScript5(this);
                    break;
                case 8:
                    script = new FCopScript8(this);
                    break;
                case 9:
                    script = new FCopScript9(this);
                    break;
                case 11:
                    script = new FCopScript11(this);
                    break;
                case 28:
                    script = new FCopScript28(this);
                    break;
                case 36:
                    script = new FCopScript36(this);
                    break;
                case 95:
                    script = new FCopScript95(this);
                    break;
            }


        }

        virtual public void Compile() {

            rawFile.data.RemoveRange(yOffset, 4);
            rawFile.data.InsertRange(yOffset, BitConverter.GetBytes(y));
            rawFile.data.RemoveRange(xOffset, 4);
            rawFile.data.InsertRange(xOffset, BitConverter.GetBytes(x));

            script.Compile();

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

        public static IFFDataFile AddNetrualTurretTempMethod(int id, int x, int y) {

            var file = new IFFDataFile(2, new(), "Csac", id, new() );

            file.additionalData.AddRange(BitConverter.GetBytes(1807));
            file.additionalData.AddRange(BitConverter.GetBytes(1807));
            file.additionalData.AddRange(BitConverter.GetBytes(1807));
            file.additionalData.AddRange(new List<byte>() { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x43, 0x4F });


            file.data.AddRange(new List<byte>() { 0x54, 0x43, 0x41, 0x74, 0x58, 0x00, 0x00, 0x00 });
            file.data.AddRange(BitConverter.GetBytes(id));
            file.data.AddRange(BitConverter.GetBytes(36));
            file.data.AddRange(BitConverter.GetBytes(y));
            file.data.AddRange(BitConverter.GetBytes(0));
            file.data.AddRange(BitConverter.GetBytes(x));
            file.data.AddRange(new List<byte>() {
                0x48, 0x01, 0x11, 0x00, 0xF4, 0x01, 0x00, 0x00, 0x00, 0x32, 0x03, 0x00, 0x65, 0x00, 0x00, 0x00, 0x03, 
                0x00, 0x06, 0x02, 0x03, 0x04, 0x0A, 0x00, 0x00, 0x10, 0x00, 0x10, 0x00, 0x18, 0x20, 0x00, 0x00, 0x04, 0x01, 0x00, 0x00, 
                0x02, 0x00, 0x00, 0x99, 0x09, 0x00, 0x18, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x01, 0x02, 0x01, 0x02, 0x3C, 
                0x00, 0x00, 0x04, 0x4C, 0x53, 0x52, 0x61, 0x2C, 0x00, 0x00, 0x00
            });

            file.data.AddRange(BitConverter.GetBytes(id));

            file.data.AddRange(new List<byte>() {
                0x6A, 0x62, 0x6F, 0x43, 0x1F, 0x00, 0x00, 0x00, 0x4C, 0x4C, 0x55, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x6A, 0x62, 0x6F, 0x43, 0x20, 0x00, 0x00, 0x00, 0x4C,
                0x4C, 0x55, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x43, 0x41, 0x53, 0x74, 0x30, 0x00, 0x00, 0x00
            });


            file.data.AddRange(BitConverter.GetBytes(id));
            file.data.AddRange(new List<byte>() {
                0x24, 0xFA, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            });

            return file;

        }


    }

    public interface FCopActorScript {

        public FCopActor actor { get; set; }

        public void Compile() {

        }

        public void ChangeRotation(float y) {

        }

    }

    public class FCopScript5 : FCopActorScript {

        public FCopActor actor { get; set; }

        public int textureOffset;

        public FCopScript5(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            textureOffset = Utils.BytesToShort(rawFile.data.ToArray(), 42);

        }

    }

    public class FCopScript8 : FCopActorScript {

        public FCopActor actor { get; set; }

        public Team teamHostileToThis;
        public Team miniMapColor;
        public int textureOffset;
        public Team hostileTowards;

        public ActorRotation headRotation;

        public ActorRotation baseRotation;

        public FCopScript8(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            teamHostileToThis = Utils.BytesToShort(rawFile.data.ToArray(), 36) == 1 ? Team.RED : Team.BLUE;
            miniMapColor = Utils.BytesToShort(rawFile.data.ToArray(), 38) == 1 ? Team.RED : Team.BLUE;
            textureOffset = Utils.BytesToShort(rawFile.data.ToArray(), 42);
            hostileTowards = Utils.BytesToShort(rawFile.data.ToArray(), 50) == 1 ? Team.RED : Team.BLUE;

            headRotation = new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 64));
            baseRotation = new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 78));

        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(64, 2);
            actor.rawFile.data.InsertRange(64, BitConverter.GetBytes((short)headRotation.compiledRotation));

        }

        public void ChangeRotation(float y) {

            headRotation += y;

        }

    }

    public class FCopScript9 : FCopActorScript {

        public FCopActor actor { get; set; }

        public int textureOffset;

        public FCopScript9(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            textureOffset = Utils.BytesToShort(rawFile.data.ToArray(), 42);

        }

    }

    public class FCopScript11 : FCopActorScript {

        public FCopActor actor { get; set; }

        public ActorRotation rotation;

        public FCopScript11(FCopActor actor) {
            this.actor = actor;

            rotation = new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 46));

        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(46, 2);
            actor.rawFile.data.InsertRange(46, BitConverter.GetBytes((short)rotation.compiledRotation));

        }

        public void ChangeRotation(float y) {

            rotation += y;

        }

    }

    public class FCopScript28 : FCopActorScript {

        public FCopActor actor { get; set; }

        public int textureOffset;

        public FCopScript28(FCopActor actor) {
            this.actor = actor;

            var rawFile = actor.rawFile;

            textureOffset = Utils.BytesToShort(rawFile.data.ToArray(), 42);

        }

    }

    public class FCopScript36 : FCopActorScript {

        //

        public FCopActor actor { get; set; }

        public ActorRotation headRotation;

        public ActorRotation baseRotation;


        public FCopScript36(FCopActor actor) {
            this.actor = actor;

            headRotation = new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 64));
            baseRotation = new ActorRotation().SetRotationCompiled(Utils.BytesToShort(actor.rawFile.data.ToArray(), 78));

        }

        public void Compile() {

            actor.rawFile.data.RemoveRange(64, 2);
            actor.rawFile.data.InsertRange(64, BitConverter.GetBytes((short)headRotation.compiledRotation));

        }

        public void ChangeRotation(float y) {

            headRotation += y;

        }

    }

    public class FCopScript95 : FCopActorScript {

        public FCopActor actor { get; set; }

        public int number1;
        public int number2;
        public int number3;
        public int number4;
        public int number5;

        public FCopScript95(FCopActor actor) {

            number1 = Utils.BytesToShort(actor.rawFile.data.ToArray(), 28);
            number2 = Utils.BytesToShort(actor.rawFile.data.ToArray(), 30);
            number3 = Utils.BytesToShort(actor.rawFile.data.ToArray(), 32);
            number4 = Utils.BytesToShort(actor.rawFile.data.ToArray(), 34);
            number5 = Utils.BytesToInt(actor.rawFile.data.ToArray(), 36);


        }

    }

    public enum Team {
        RED = 1,
        BLUE = 2
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

}