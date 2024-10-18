

using System;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {


    public class FCopSoundEffectParser {

        public Dictionary<int, List<FCopAudio>> soundEffects = new();

        public List<IFFDataFile> rawCwavs;
        public IFFDataFile rawCshdFile;

        public FCopSoundEffectParser(List<IFFDataFile> cwavs, IFFDataFile cshd) {

            rawCwavs = cwavs;
            rawCshdFile = cshd;

            IFFDataFile GetCwav(int id) {
                return rawCwavs.First(file => { return file.dataID == id; });
            }

            var soundMetaDataStructCount = BitConverter.ToInt16(cshd.data.ToArray(), 6);

            // This is here for debugging purposes to see if it's ever true
            if (soundMetaDataStructCount != cwavs.Count) {
                throw new Exception("Sound meta data and cwav count does not match");

            }

            // offset of sound meta data array
            var offset = 10;

            foreach (var i in Enumerable.Range(0, soundMetaDataStructCount)) {

                var metaDataStruct = new SoundMetaData(
                    BitConverter.ToInt16(cshd.data.ToArray(), offset),
                    BitConverter.ToInt16(cshd.data.ToArray(), offset + 2),
                    cshd.data[4],
                    cshd.data[5],
                    cshd.data[6],
                    cshd.data[7],
                    BitConverter.ToInt32(cshd.data.ToArray(), offset + 8)
                    );

                var soundEffect = new FCopAudio(GetCwav(metaDataStruct.dataID), metaDataStruct);

                if (soundEffects.ContainsKey(metaDataStruct.groupID)) {
                    soundEffects[metaDataStruct.groupID].Add(soundEffect);
                }
                else {
                    soundEffects[metaDataStruct.groupID] = new() { soundEffect };
                }


                offset += 12;

            }

        }


        public struct SoundMetaData {

            public int groupID; // 16-bit
            public int dataID; // 16-bit
            public int unknown1; // 8-bit
            public int soundLimit; // 8-bit
            public int doesLoop; // 8-bit
            public int unknown2; // 8-bit
            public int scriptingID; // 32-bit

            public SoundMetaData(int groupID, int dataID, int unknown1, int soundLimit, int doesLoop, int unknown2, int scriptingID) {
                this.groupID = groupID;
                this.dataID = dataID;
                this.unknown1 = unknown1;
                this.soundLimit = soundLimit;
                this.doesLoop = doesLoop;
                this.unknown2 = unknown2;
                this.scriptingID = scriptingID;
            }

        }

    }

    public class FCopAudio : FCopAsset {

        public bool rawDataHasHeader;
        public int bitrate;
        public int sampleRate;
        public int channelCount;
        public int groupID;
        public int scriptingID;
        public bool isLoop;
        public int soundLimit;
        public int unknown1;
        public int unknown2;

        // Cwav
        public FCopAudio(IFFDataFile rawFile, FCopSoundEffectParser.SoundMetaData soundMetaData) : base(rawFile) {
            this.groupID = soundMetaData.groupID;
            this.scriptingID = soundMetaData.scriptingID;
            this.isLoop = soundMetaData.doesLoop == 1;
            this.unknown1 = soundMetaData.unknown1;
            this.soundLimit = soundMetaData.soundLimit;
            this.unknown2 = soundMetaData.unknown2;
            this.name = "Sound Effect " + rawFile.dataID.ToString();
            this.rawDataHasHeader = true;
            this.bitrate = 16;
            this.sampleRate = 22050;

        }

        public byte[] GetFormattedAudio() {

            if (rawDataHasHeader) {
                return rawFile.data.ToArray();
            }
            else {
                return null;
            }

        } 

        public byte[] GetRawAudio() {

            if (rawDataHasHeader) {
                return rawFile.data.GetRange(44, rawFile.data.Count() - 44).ToArray();
            }
            else {
                return rawFile.data.ToArray();
            }

        }

    }


}