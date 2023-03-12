

using System;
using System.Collections.Generic;

namespace FCopParser {

    /*
     * 11 - prop
     * 14 - interactable
     * 16 - item
     * 
     * 
     */
    public class FCopActor {

        const int idOffset = 8;
        const int objectTypeOffset = 12;
        const int yOffset = 16;
        const int xOffset = 24;

        public int id;
        public int objectType;
        public int x;
        public int y;

        public IFFDataFile rawFile;

        public FCopActor(IFFDataFile rawFile) {

            this.rawFile = rawFile;

            id = Utils.BytesToInt(rawFile.data.ToArray(), 8);
            objectType = Utils.BytesToInt(rawFile.data.ToArray(), 12);
            y = Utils.BytesToInt(rawFile.data.ToArray(), 16);
            x = Utils.BytesToInt(rawFile.data.ToArray(), 24);


        }

        public void Compile() {

            rawFile.data.RemoveRange(yOffset, 4);
            rawFile.data.InsertRange(yOffset, BitConverter.GetBytes(y));
            rawFile.data.RemoveRange(xOffset, 4);
            rawFile.data.InsertRange(xOffset, BitConverter.GetBytes(x));

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

    public class FCopTurretActor : FCopActor {

        public int rotation;

        public FCopTurretActor(IFFDataFile rawFile) : base(rawFile) {

            rotation = Utils.BytesToShort(rawFile.data.ToArray(), 64);

        }

    }

    public class FCopBaseTurretActor : FCopActor {

        public Team teamHostileToThis;
        public Team miniMapColor;

        public Team hostileTowards;

        public int rotation;


        public FCopBaseTurretActor(IFFDataFile rawFile) : base(rawFile) {

            teamHostileToThis = Utils.BytesToShort(rawFile.data.ToArray(), 36) == 1 ? Team.RED : Team.BLUE;
            miniMapColor = Utils.BytesToShort(rawFile.data.ToArray(), 38) == 1 ? Team.RED : Team.BLUE;
            hostileTowards = Utils.BytesToShort(rawFile.data.ToArray(), 50) == 1 ? Team.RED : Team.BLUE;

            rotation = Utils.BytesToShort(rawFile.data.ToArray(), 64);

        }

    }

    public enum Team {
        RED = 1,
        BLUE = 2
    }

}