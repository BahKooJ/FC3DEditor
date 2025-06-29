﻿

using System;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    public class FCopAudioParser {

        public static List<FCopAudio> ParseGlblDataSounds(List<IFFDataFile> cwavs, IFFDataFile cshd) {

            List<FCopAudio> soundEffects = new();

            IFFDataFile GetCwav(int id) {
                return cwavs.First(file => { return file.dataID == id; });
            }

            var soundMetaDataStructCount = BitConverter.ToInt16(cshd.data.ToArray(), 6);

            // offset of sound meta data array
            var offset = BitConverter.ToInt16(cshd.data.ToArray(), 8);

            foreach (var i in Enumerable.Range(0, soundMetaDataStructCount)) {

                var metaDataStruct = new SoundMetaData(
                    BitConverter.ToInt16(cshd.data.ToArray(), offset),
                    BitConverter.ToInt16(cshd.data.ToArray(), offset + 2),
                    cshd.data[offset + 4],
                    cshd.data[offset + 5],
                    cshd.data[offset + 6],
                    cshd.data[offset + 7],
                    BitConverter.ToInt16(cshd.data.ToArray(), offset + 8)
                    );

                var soundEffect = new FCopAudio(GetCwav(metaDataStruct.dataID), metaDataStruct);
                soundEffect.isGlobalData = true;

                soundEffects.Add(soundEffect);

                offset += 12;

            }

            return soundEffects;

        }

        public List<FCopAudio> globalSoundEffects = new();
        public List<FCopAudio> soundEffects = new();
        public List<FCopStream> soundStreams = new();
        public FCopAudio music;

        IFFDataFile cshd;

        public FCopAudioParser(List<IFFDataFile> cwavs, IFFDataFile cshd, List<SubFile> subFiles, MusicFile musicFile) {

            this.cshd = cshd;

            IFFDataFile GetCwav(int id) {
                return cwavs.First(file => { return file.dataID == id; });
            }

            var soundMetaDataStructCount = BitConverter.ToInt16(cshd.data.ToArray(), 6);

            if (soundMetaDataStructCount != cwavs.Count) {
                throw new Exception("Sound meta data and Cwav count does not match");

            }

            // offset of sound meta data array
            var offset = 10;

            foreach (var i in Enumerable.Range(0, soundMetaDataStructCount)) {

                var metaDataStruct = new SoundMetaData(
                    BitConverter.ToInt16(cshd.data.ToArray(), offset),
                    BitConverter.ToInt16(cshd.data.ToArray(), offset + 2),
                    cshd.data[offset + 4],
                    cshd.data[offset + 5],
                    cshd.data[offset + 6],
                    cshd.data[offset + 7],
                    BitConverter.ToInt32(cshd.data.ToArray(), offset + 8)
                    );

                var soundEffect = new FCopAudio(GetCwav(metaDataStruct.dataID), metaDataStruct);

                soundEffects.Add(soundEffect);

                offset += 12;

            }

            foreach (var stream in subFiles) {

                soundStreams.Add(new FCopStream(stream));

            }

            music = new FCopAudio(musicFile);

        }

        public IFFDataFile CompileSoundHeader() {

            var total = new List<byte>();

            total.AddRange(BitConverter.GetBytes((short)4));
            total.AddRange(BitConverter.GetBytes((short)1));
            total.AddRange(BitConverter.GetBytes((short)50));
            total.AddRange(BitConverter.GetBytes((short)soundEffects.Count));
            total.AddRange(BitConverter.GetBytes((short)10));

            foreach (var wave in soundEffects) {

                total.AddRange(BitConverter.GetBytes((short)wave.groupID));
                total.AddRange(BitConverter.GetBytes((short)wave.DataID));
                total.Add((byte)wave.unknown1);
                total.Add((byte)wave.soundLimit);
                total.Add((byte)(wave.isLoop ? 1 : 0));
                total.Add((byte)wave.unknown2);
                total.AddRange(BitConverter.GetBytes(wave.scriptingID));

            }

            total.Add(32);
            total.Add(32);

            cshd.data = total;

            return cshd;

        }

        // Ctos is a sort of header file for SWVR chunks, it has offsets to all SWVR chunks relative to the start of the data.
        // Meaning that the first SWVR chunk would be at offset 0.
        // HOWEVER these offsets are made after compression, so it includes fills, chunks and all that garbage.
        // So in order to properly make a Ctos it needs to mimic making a IFF file
        public IFFDataFile CreateCtos(int emptyRPNSOffset) {

            int swvrHeaderSize = 36;
            int shdrHeaderSize = 60;

            var i = 0;
            var current24kSectionSize = 0;

            void FILLCheck(int nextDataSize) {

                if (current24kSectionSize == IFFParser.iffFileSectionSize) {
                    current24kSectionSize = 0;
                    return;
                }

                if (current24kSectionSize + nextDataSize > IFFParser.iffFileSectionSize) {

                    var difference = IFFParser.iffFileSectionSize - current24kSectionSize;

                    i += difference;

                    current24kSectionSize = 0;

                }

            }

            void FILLRemain() {

                if (current24kSectionSize == IFFParser.iffFileSectionSize) {
                    current24kSectionSize = 0;
                    return;
                }

                var difference = IFFParser.iffFileSectionSize - current24kSectionSize;

                i += difference;

                current24kSectionSize = 0;

            }

            void AddToIterator(int value) {
                FILLCheck(value);
                current24kSectionSize += value;
                i += value;
            }

            var offsets = new List<int>();

            var subFiles = CompileStreams();

            foreach (var subFile in subFiles) {

                offsets.Add(i);

                AddToIterator(swvrHeaderSize);

                foreach (var file in subFile.files) {

                    AddToIterator(shdrHeaderSize);

                    var dataSize = file.data.Count;

                    var dataChunkCount = IFFParser.DataChunksBySize(dataSize);

                    var chunkedDataOffset = 0;

                    foreach (int _ in Enumerable.Range(0, dataChunkCount)) {

                        if (chunkedDataOffset + IFFParser.dataChunkSizeWithoutHeader > dataSize) {

                            var chunkHeader = new List<byte>();

                            var chunkSize = dataSize - chunkedDataOffset;

                            AddToIterator(chunkSize + IFFParser.chunkHeaderLength);

                        }
                        else {

                            AddToIterator(IFFParser.dataChunkSize);

                            chunkedDataOffset += IFFParser.dataChunkSizeWithoutHeader;

                        }

                    }


                }

                FILLRemain();

            }

            var total = new List<byte>();

            // FourCC SOTt
            total.AddRange(new List<byte>() { 83, 79, 84, 116 });
            // Size
            total.AddRange(BitConverter.GetBytes(16 + (offsets.Count * 12)));
            total.AddRange(BitConverter.GetBytes(1));
            total.AddRange(BitConverter.GetBytes(offsets.Count));

            foreach (var offset in offsets) {

                total.AddRange(BitConverter.GetBytes(offset));
                total.AddRange(BitConverter.GetBytes(0));
                total.AddRange(BitConverter.GetBytes(0));

            }

            return new IFFDataFile(2, total, "Ctos", 1, emptyRPNSOffset);

        }

        public FCopAudio AddWave(byte[] newData, int emptyOffset) {

            var maxID = soundEffects.Max(s => s.DataID);
            var maxGroup = soundEffects.Max(s => s.groupID);
            var maxScript = soundEffects.Max(s => s.scriptingID);

            var rawFile = new IFFDataFile(2, newData.ToList(), "Cwav", maxID + 1, emptyOffset);

            var sound = new FCopAudio(rawFile, new SoundMetaData(maxGroup + 1, maxID + 1, 7, 3, 0, 0, maxScript + 1));

            soundEffects.Add(sound);

            return sound;

        }

        public void RemoveWave(int id) {

            soundEffects.RemoveAll(sound => sound.DataID == id);

        }

        public void ImportWave(int id, byte[] newData) {

            var wave = soundEffects.First(wav => wav.DataID == id);

            wave.rawFile.data = newData.ToList();

        }

        public FCopStream AddStream(byte[] newData) {

            var rawFile = new IFFDataFile(5, newData.ToList(), "snds", 0, -1);

            var stream = new FCopStream(rawFile, "New Stream");

            soundStreams.Add(stream);

            return stream;

        }

        public void RemoveStream(int index) {

            soundStreams.RemoveAt(index);

        }

        public List<SubFile> CompileStreams() {

            var total = new List<SubFile>();

            foreach (var stream in soundStreams) {

                var subFile = new SubFile(stream.name);

                if (stream.miniAnimation != null) {

                    subFile.files.Add(stream.miniAnimation.rawFile);

                }

                stream.sound.rawFile.name = stream.name;

                subFile.files.Add(stream.sound.rawFile);

                total.Add(subFile);

            }

            return total;

        }

        public MusicFile CompileMusic() {

            return new MusicFile(music.name, music.rawFile.data);

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
        public bool rawDataHasSize;
        public int bitrate;
        public int sampleRate;
        public int channelCount;

        // Cwav
        public int groupID;
        public int scriptingID;
        public bool isLoop;
        public int soundLimit;
        public int unknown1;
        public int unknown2;

        // Cwav
        public FCopAudio(IFFDataFile rawFile, FCopAudioParser.SoundMetaData soundMetaData) : base(rawFile) {
            this.groupID = soundMetaData.groupID;
            this.scriptingID = soundMetaData.scriptingID;
            this.isLoop = soundMetaData.doesLoop == 1;
            this.unknown1 = soundMetaData.unknown1;
            this.soundLimit = soundMetaData.soundLimit;
            this.unknown2 = soundMetaData.unknown2;
            this.name = rawFile.name == "" ? "Sound Effect " + rawFile.dataID.ToString() : rawFile.name;
            this.rawDataHasHeader = true;
            this.rawDataHasSize = false;
            this.bitrate = 16;
            this.sampleRate = 22050;
            this.channelCount = 1;

        }

        public FCopAudio(IFFDataFile rawFile, string streamName) : base(rawFile) {
            this.name = streamName;
            this.rawDataHasHeader = false;
            this.rawDataHasSize = true;
            this.bitrate = 8;
            this.sampleRate = 22050;
            this.channelCount = 1;

        }

        public FCopAudio(MusicFile musicFile) : base(null) {

            rawFile = new IFFDataFile(0, musicFile.data, "MSIC", 1, -1);
            this.name = musicFile.name;
            this.rawDataHasHeader = false;
            this.rawDataHasSize = false;
            this.bitrate = 8;
            this.sampleRate = 14212;
            this.channelCount = 2;

        }

        public byte[] GetFormattedAudio() {

            if (rawDataHasHeader) {
                return rawFile.data.ToArray();
            }
            else {
                var bytesPerBlock = channelCount * bitrate / 8;
                var BytePerSec = bytesPerBlock * sampleRate;

                if (rawDataHasSize) {
                    return new WaveParser(1, channelCount, sampleRate, BytePerSec, bytesPerBlock, bitrate, rawFile.data.GetRange(4, rawFile.data.Count - 4)).Compile();
                }
                else {
                    return new WaveParser(1, channelCount, sampleRate, BytePerSec, bytesPerBlock, bitrate, rawFile.data).Compile();
                }

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

    // SWVR
    public class FCopStream : FCopAsset {

        public FCopAudio sound;
        public FCopMiniAnimation miniAnimation = null;

        public FCopStream(SubFile subFile) : base(null, subFile.name) {

            name = subFile.name;

            var rawSound = subFile.files.FirstOrDefault(file => file.dataFourCC == "snds");
            
            var rawAnm = subFile.files.FirstOrDefault(file => file.dataFourCC == "canm");

            if (rawSound != null) {
                sound = new FCopAudio(rawSound, subFile.name);

                if (rawSound.name != "") {
                    name = rawSound.name;
                    sound.name = rawSound.name;
                }

            }

            if (rawAnm != null) {
                miniAnimation = new FCopMiniAnimation(rawAnm);
            }

        }

        public FCopStream(IFFDataFile rawFile, string name) : base(null, name) {

            this.name = name;
            sound = new FCopAudio(rawFile, name);

        }

    }

}