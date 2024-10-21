
// This object is what indexes the IFF files determines where chunks are located.
// It stores chunk information with the ChunkHeader object.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCopParser {
    public class IFFParser {

        // ---Constants---

        // consants for important fourCCs.
        // Other fourCCs can be ignored as they're just a description of what the item is.
        // However it is import to test for these specifically to know how to parse the chunk.
        static class FourCC {
            public const string CTRL = "CTRL";
            public const string SHOC = "SHOC";
            public const string FILL = "FILL";
            public const string SHDR = "SHDR";
            public const string SDAT = "SDAT";
            public const string SWVR = "SWVR";
            public const string MSIC = "MSIC";

            public readonly static List<byte> CTRLbytes = new() { 76, 82, 84, 67 };
            public readonly static List<byte> SHOCbytes = new() { 67, 79, 72, 83 };
            public readonly static List<byte> SDATbytes = new() { 84, 65, 68, 83 };
            public readonly static List<byte> SHDRbytes = new() { 82, 68, 72, 83 };
            public readonly static List<byte> FILLbytes = new() { 76, 76, 73, 70 };
            public readonly static List<byte> SWVRbytes = new() { 82, 86, 87, 83 };
            public readonly static List<byte> FILEbytes = new() { 69, 76, 73, 70 };
            public readonly static List<byte> MSICbytes = new() { 67, 73, 83, 77 };

            // PS1
            public const string VAGB = "VAGB";
            public const string VAGM = "VAGM";
            public const string CANM = "CANM";

            public readonly static List<byte> VAGBbytes = new() { 66, 71, 65, 86 };
            public readonly static List<byte> VAGMbytes = new() { 77, 71, 65, 86 };
            public readonly static List<byte> CANMbytes = new() { 77, 78, 65, 67 };
            public readonly static List<byte> MDECbytes = new() { 67, 69, 68, 77 };

        }

        // The normal length of a chunk header.
        public const int chunkHeaderLength = 20;

        // The maximum size of a SDAT chunk
        public const int dataChunkSize = 4096;

        // The maximum size of a SDAT chunk not including the size of the header
        public const int dataChunkSizeWithoutHeader = 4076;

        // The length of the MSIC chunk
        const int musicHeaderLength = 28;

        // The maximum size of a MSIC chunk
        const int musicChunkSize = 24540;

        // The maximum size of a VAGM chunk (PS1)
        const int vagmMusicChunkSize = 24476;

        // The maximum size of a MSIC chunk not including the size of the header
        const int musicChunkSizeWithoutHeader = 24512;

        // The maximum size of a VAGM chunk not including the size of the header (PS1)
        const int vagmMusicChunkSizeWithoutHeader = 24448;

        // These are unused
        const int musicLoopNumberIncrease = 65536;
        const int randomMusicNumber = 12256;

        // All chunks in the IFF file must fill a perfeck 24KB size.
        // If a chunk exends a 24KB section, a FILL chunk must be made to fill in the remaining space
        public const int iffFileSectionSize = 24576;

        // ---Parsing---

        public IFFFileManager parsedData;

        // Bytes of the file IFF file
        public byte[] bytes = Array.Empty<byte>();

        // This stores all the offsets or index of chunks as well as useful information regarding them with the ChunkHeader object.
        public List<ChunkHeader> offsets = new();

        // If given bytes, the object knows the data needs to be parsed
        public IFFParser(byte[] bytes) {
            this.bytes = bytes;
            FindStartChunkOffset();
            parsedData = Parse();
        }

        public IFFParser(IFFFileManager parsedData) {
            this.parsedData = parsedData;
            this.parsedData.Sort();
        }

        // Grabs all the files/data and coverts them into their own files,
        // separating the data and chuncks allowing for other programs to parse the data freely.
        // Returns the IFFFileManage object store the individual files.
        IFFFileManager Parse() {

            var fileMananger = new IFFFileManager();

            IFFDataFile file = null;
            var dataChunksToAdd = 0;

            SubFile openedSubFile = null;

            foreach (ChunkHeader header in offsets) {

                if (header.fourCCDeclaration == FourCC.SWVR) {

                    if (openedSubFile != null) {

                        if (openedSubFile.files.Count != 0) {
                            fileMananger.subFiles.Add(openedSubFile);
                        }

                    }

                    openedSubFile = new SubFile(header.subFileName);

                } 
                else if (header.fourCCDeclaration == FourCC.MSIC) {

                    if (fileMananger.music == null) {

                        fileMananger.music = new MusicFile(openedSubFile.name);

                        fileMananger.music.data.AddRange(CopyOfRange(header.index + 28, header.index + header.chunkSize).ToList());

                    } 
                    else {

                        //todo: Magic number 28 is the size of the music header, there are two numbers after the header that are unknown
                        //update: I know what they are now but I'm too lazy to refactor. They are 3 numbers, chunk count, chunk iteration, and the size divided by 2

                        fileMananger.music.data.AddRange(CopyOfRange(header.index + 28, header.index + header.chunkSize).ToList());

                    }

                }
                // PS1
                else if (header.fourCCDeclaration == FourCC.VAGB) {

                    if (openedSubFile == null) {
                        throw new Exception("This should only be inside of of a SWVR?");
                    }

                    var vagbFile = new IFFDataFile(0,
                        CopyOfRange(header.index + 20, header.index + header.chunkSize).ToList(),
                        header.fourCCDeclaration, 0, 0
                        );

                    openedSubFile.files.Add(vagbFile);

                }
                else if (header.fourCCDeclaration == FourCC.CANM) {

                    if (openedSubFile == null) {
                        throw new Exception("This should only be inside of of a SWVR?");
                    }

                    var camnFile = new IFFDataFile(0,
                        CopyOfRange(header.index + 20, header.index + header.chunkSize).ToList(),
                        header.fourCCDeclaration, 0, 0
                        );

                    openedSubFile.files.Add(camnFile);

                }
                else if (header.fourCCDeclaration == FourCC.VAGM) {

                    fileMananger.isPS1 = true;

                    if (fileMananger.music == null) {

                        fileMananger.music = new MusicFile(openedSubFile.name);

                        fileMananger.music.data.AddRange(CopyOfRange(header.index + 28, header.index + header.chunkSize).ToList());

                    }
                    else {

                        //todo: Magic number 28 is the size of the music header, there are two numbers after the header that are unknown
                        //update: I know what they are now but I'm too lazy to refactor. They are 3 numbers, chunk count, chunk iteration, and the size divided by 2

                        fileMananger.music.data.AddRange(CopyOfRange(header.index + 28, header.index + header.chunkSize).ToList());

                    }

                }

                if (header.fourCCType == FourCC.SHDR) {

                    if (file == null && header.fileHeader != null) {

                        List<int> rpnsReferences = new();
                        List<int> headerCodeData = new();
                        List<byte> headerCode = new();

                        var offset = 0;

                        foreach (var i in Enumerable.Range(0, IFFDataFile.rpnsRefCount)) {

                            rpnsReferences.Add(Utils.BytesToInt(header.fileHeader.actData.ToArray(), offset));
                            offset += 4;

                        }

                        foreach (var i in Enumerable.Range(0, IFFDataFile.headerCodeDataCount)) {

                            headerCodeData.Add(Utils.BytesToInt(header.fileHeader.actData.ToArray(), offset));
                            offset += 4;
                        }

                        headerCode = header.fileHeader.actData.ToList().GetRange(offset, header.fileHeader.actData.Count() - offset);

                        file = new IFFDataFile(
                            header.fileHeader.startNumber, 
                            new List<byte>(), 
                            header.fileHeader.fourCCData, 
                            header.fileHeader.dataID,
                            rpnsReferences, headerCodeData, headerCode
                        );

                        dataChunksToAdd = DataChunksBySize(header.fileHeader.dataSize);

                    }

                } 
                else if (header.fourCCType == FourCC.SDAT && file != null && dataChunksToAdd != 0) {

                    file.data.AddRange(CopyOfRange(header.index + chunkHeaderLength, header.index + header.chunkSize).ToList());
                    dataChunksToAdd--;

                    if (dataChunksToAdd == 0) {

                        if (openedSubFile != null) {

                            openedSubFile.files.Add(file);

                            file = null;

                        } 
                        else {

                            fileMananger.files.Add(file);
                            file = null;

                        }

                    }

                }

            }


            return fileMananger;

        }

        public void Compile() {

            List<byte> compiledFile = new();

            // The files size starts off as 24 to fill the space for a future CTRL chunk
            var current24kSectionSize = 24;

            void FILLCheck(int nextDataSize) {

                if (current24kSectionSize == iffFileSectionSize) {
                    current24kSectionSize = 0;
                    return;
                }

                if (current24kSectionSize + nextDataSize > iffFileSectionSize) {

                    var difference = iffFileSectionSize - current24kSectionSize;

                    if (difference == 4) {
                        compiledFile.AddRange(FourCC.FILLbytes);
                        current24kSectionSize = 0;
                        return;
                    }

                    if (difference < 4) {
                        compiledFile.AddRange(FourCC.FILLbytes.GetRange(0, difference));
                        current24kSectionSize = 0;
                        return;
                    }

                    if (difference <= 8) {
                        compiledFile.AddRange(FourCC.FILLbytes);
                        var size = BitConverter.GetBytes(difference).ToList();
                        compiledFile.AddRange(size.GetRange(0, difference - 4));
                        current24kSectionSize = 0;
                        return;
                    }

                    compiledFile.AddRange(FourCC.FILLbytes);
                    compiledFile.AddRange(BitConverter.GetBytes(difference));

                    foreach (int i in Enumerable.Range(0, difference - 8)) {
                        compiledFile.Add(0);
                    }

                    current24kSectionSize = 0;

                }

            }

            void FILLRemain() {

                if (current24kSectionSize == iffFileSectionSize) {
                    current24kSectionSize = 0;
                    return;
                }

                var difference = iffFileSectionSize - current24kSectionSize;

                // TODO: Change to <= 8 check
                if (difference == 4) {
                    compiledFile.AddRange(FourCC.FILLbytes);
                }

                compiledFile.AddRange(FourCC.FILLbytes);
                compiledFile.AddRange(BitConverter.GetBytes(difference));

                foreach (int i in Enumerable.Range(0, difference - 8)) {
                    compiledFile.Add(0);
                }

                current24kSectionSize = 0;


            }

            void CompileDataFile(IFFDataFile file) {

                List<byte> dataHeader = new();

                var dataSize = file.data.Count();

                var additionalData = file.CompileAdditionalData();

                var headerSize = 36 + additionalData.Count();

                dataHeader.AddRange(FourCC.SHOCbytes);
                dataHeader.AddRange(BitConverter.GetBytes(headerSize));
                dataHeader.AddRange(new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 });
                dataHeader.AddRange(FourCC.SHDRbytes);
                dataHeader.AddRange(BitConverter.GetBytes(file.startNumber));
                dataHeader.AddRange(Encoding.ASCII.GetBytes(Reverse(file.dataFourCC)));
                dataHeader.AddRange(BitConverter.GetBytes(file.dataID));
                dataHeader.AddRange(BitConverter.GetBytes(dataSize));
                dataHeader.AddRange(additionalData);

                FILLCheck(headerSize);
                current24kSectionSize += headerSize;
                compiledFile.AddRange(dataHeader);

                var chunkedDataOffset = 0;

                foreach (int i in Enumerable.Range(0, DataChunksBySize(dataSize))) {

                    if (chunkedDataOffset + dataChunkSizeWithoutHeader > dataSize) {

                        var chunkHeader = new List<byte>();

                        var chunkSize = dataSize - chunkedDataOffset;

                        chunkHeader.AddRange(FourCC.SHOCbytes);
                        chunkHeader.AddRange(BitConverter.GetBytes(chunkSize + chunkHeaderLength));
                        chunkHeader.AddRange(new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 });
                        chunkHeader.AddRange(FourCC.SDATbytes);

                        FILLCheck(chunkSize + chunkHeaderLength);
                        current24kSectionSize += chunkSize + chunkHeaderLength;
                        compiledFile.AddRange(chunkHeader);
                        compiledFile.AddRange(file.data.GetRange(chunkedDataOffset, chunkSize));

                    } else {

                        var chunkHeader = new List<byte>();

                        chunkHeader.AddRange(FourCC.SHOCbytes);
                        chunkHeader.AddRange(BitConverter.GetBytes(dataChunkSize));
                        chunkHeader.AddRange(new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 });
                        chunkHeader.AddRange(FourCC.SDATbytes);

                        FILLCheck(dataChunkSize);
                        current24kSectionSize += dataChunkSize;
                        compiledFile.AddRange(chunkHeader);
                        compiledFile.AddRange(file.data.GetRange(chunkedDataOffset, dataChunkSizeWithoutHeader));

                        chunkedDataOffset += dataChunkSizeWithoutHeader;

                    }

                }

            }

            int dataFileSize;
            int subFileSize;
            int musicSize;

            foreach (var file in parsedData.files) {

                if (file.ignore) { continue; }

                CompileDataFile(file);

            }

            FILLRemain();

            dataFileSize = compiledFile.Count() + 24;

            foreach (var subFile in parsedData.subFiles) {

                List<byte> subFileHeader = new();

                subFileHeader.AddRange(FourCC.SWVRbytes);
                subFileHeader.AddRange(BitConverter.GetBytes(36));
                subFileHeader.AddRange(new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 });
                subFileHeader.AddRange(FourCC.FILEbytes);
                subFileHeader.AddRange(subFile.CompileName());

                // No need to check for fills, because the remainder was already filled
                compiledFile.AddRange(subFileHeader);
                current24kSectionSize += 36;

                foreach (var file in subFile.files) {

                    if (file.dataFourCC == FourCC.VAGB) {

                        var chunkHeader = new List<byte>();

                        chunkHeader.AddRange(FourCC.VAGBbytes);
                        chunkHeader.AddRange(BitConverter.GetBytes(20 + file.data.Count));
                        chunkHeader.AddRange(new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 });
                        chunkHeader.AddRange(FourCC.VAGBbytes);



                        if (subFile.files.Count > 1) {
                            FILLCheck(chunkHeader.Count + file.data.Count);
                        }

                        compiledFile.AddRange(chunkHeader);
                        compiledFile.AddRange(file.data);

                        current24kSectionSize += chunkHeader.Count + file.data.Count;

                    } 
                    else if (file.dataFourCC == FourCC.CANM) {

                        var chunkHeader = new List<byte>();

                        chunkHeader.AddRange(FourCC.CANMbytes);
                        chunkHeader.AddRange(BitConverter.GetBytes(20 + file.data.Count));
                        // In all my time looking at these files I've never seen these 8 bytes be something other than 0.
                        // Does it matter though?
                        chunkHeader.AddRange(new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 });
                        chunkHeader.AddRange(FourCC.MDECbytes);

                        if (subFile.files.Count > 1) {
                            FILLCheck(chunkHeader.Count + file.data.Count);
                        }

                        compiledFile.AddRange(chunkHeader);
                        compiledFile.AddRange(file.data);

                        current24kSectionSize += chunkHeader.Count + file.data.Count;

                    }
                    else {

                        CompileDataFile(file);

                    }

                }

                FILLRemain();

            }

            // The CTRL isn't part of the file yet, but it's still counted for in dataFileSize. To get an accurate size, 24 is added to the subFile Size
            subFileSize = compiledFile.Count() - dataFileSize + 24;

            List<byte> musicfileHeader = new();

            musicfileHeader.AddRange(FourCC.SWVRbytes);
            musicfileHeader.AddRange(BitConverter.GetBytes(36));
            musicfileHeader.AddRange(new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 });
            musicfileHeader.AddRange(FourCC.FILEbytes);
            musicfileHeader.AddRange(parsedData.music.CompileName());

            compiledFile.AddRange(musicfileHeader);
            current24kSectionSize += 36;

            var musicDataSize = parsedData.music.data.Count();

            var chunkedMusicOffset = 0;

            var chunkIteration = 0;

            var currentMusicChunkSize = parsedData.isPS1 ? vagmMusicChunkSize : musicChunkSize;
            var currentMusicChunkSizeWithoutHeader = parsedData.isPS1 ? vagmMusicChunkSizeWithoutHeader : musicChunkSizeWithoutHeader;

            var musicChunkAmount = DataChunksBySize(musicDataSize, currentMusicChunkSize, musicHeaderLength);

            void MakeMusicHeader(int chunkSize) {

                var chunkHeader = new List<byte>();

                if (parsedData.isPS1) {

                    chunkHeader.AddRange(FourCC.VAGMbytes);
                    chunkHeader.AddRange(BitConverter.GetBytes(chunkSize));
                    chunkHeader.AddRange(new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 });
                    chunkHeader.AddRange(FourCC.VAGMbytes);
                    chunkHeader.AddRange(BitConverter.GetBytes((ushort)musicChunkAmount));
                    chunkHeader.AddRange(BitConverter.GetBytes((ushort)chunkIteration));
                    chunkHeader.AddRange(BitConverter.GetBytes((chunkSize - musicHeaderLength) / 2));

                }
                else {

                    chunkHeader.AddRange(FourCC.MSICbytes);
                    chunkHeader.AddRange(BitConverter.GetBytes(chunkSize));
                    chunkHeader.AddRange(new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 });
                    chunkHeader.AddRange(FourCC.MSICbytes);
                    chunkHeader.AddRange(BitConverter.GetBytes((ushort)musicChunkAmount));
                    chunkHeader.AddRange(BitConverter.GetBytes((ushort)chunkIteration));
                    chunkHeader.AddRange(BitConverter.GetBytes((chunkSize - musicHeaderLength) / 2));

                }

                compiledFile.AddRange(chunkHeader);

                chunkIteration++;

            }

            foreach (int i in Enumerable.Range(0, musicChunkAmount)) {

                if (chunkedMusicOffset + currentMusicChunkSizeWithoutHeader > musicDataSize) {

                    var chunkSize = musicDataSize - chunkedMusicOffset;

                    FILLCheck(chunkSize + musicHeaderLength);
                    current24kSectionSize += chunkSize + musicHeaderLength;
                    MakeMusicHeader(chunkSize + musicHeaderLength);
                    compiledFile.AddRange(parsedData.music.data.GetRange(chunkedMusicOffset, chunkSize));

                } else {

                    FILLCheck(currentMusicChunkSize);
                    current24kSectionSize += currentMusicChunkSize;
                    MakeMusicHeader(currentMusicChunkSize);
                    compiledFile.AddRange(parsedData.music.data.GetRange(chunkedMusicOffset, currentMusicChunkSizeWithoutHeader));

                    chunkedMusicOffset += currentMusicChunkSizeWithoutHeader;

                }

            }

            FILLRemain();

            musicSize = compiledFile.Count() - (dataFileSize + subFileSize);

            var ctrlChunk = new List<byte>();

            ctrlChunk.AddRange(FourCC.CTRLbytes);
            ctrlChunk.AddRange(BitConverter.GetBytes(24));
            ctrlChunk.AddRange(new List<byte>() { 0, 0, 0, 0 });
            ctrlChunk.AddRange(BitConverter.GetBytes(musicSize));
            ctrlChunk.AddRange(BitConverter.GetBytes(subFileSize));
            ctrlChunk.AddRange(BitConverter.GetBytes(dataFileSize));

            ctrlChunk.AddRange(compiledFile);

            bytes = ctrlChunk.ToArray();

            FindStartChunkOffset();

        }

        // ---Indexing---

        void FindStartChunkOffset() {

            offsets.Clear();

            int offset = 0;
            int current24kSectionSize = 0;

            if (BytesToStringReversed(offset, 4) != FourCC.CTRL) {
                throw new InvalidFileException();
            }

            while (offset < bytes.Length) {

                var fourCC = BytesToStringReversed(offset, 4);
                var size = BytesToInt(offset + 4);

                if (fourCC == FourCC.FILL) {
                    var difference = iffFileSectionSize - current24kSectionSize;

                    offsets.Add(new ChunkHeader(offset, fourCC, difference));

                    offset += difference;

                    current24kSectionSize = 0;

                    continue;

                } 
                else {

                    current24kSectionSize += size;

                    if (current24kSectionSize == iffFileSectionSize) {
                        current24kSectionSize = 0;
                    }

                }

                if (fourCC == FourCC.SHOC) {

                    var fourCCType = BytesToStringReversed(offset + 16, 4);

                    if (fourCCType == FourCC.SDAT) {

                        offsets.Add(new ChunkHeader(offset, fourCC, size, fourCCType));

                    } 
                    else if (fourCCType == FourCC.SHDR) {

                        var offsetOfHeader = offset + chunkHeaderLength;

                        var startNumber = BytesToInt(offsetOfHeader);
                        var fourCCData = BytesToStringReversed(offsetOfHeader + 4, 4);
                        var dataID = BytesToInt(offsetOfHeader + 8);
                        var dataSize = BytesToInt(offsetOfHeader + 12);
                        var remainingData = CopyOfRange(offsetOfHeader + 16, offset + size);

                        var fileHeader = new FileHeader(startNumber, fourCCData, dataID, dataSize, remainingData.ToArray());

                        offsets.Add(new ChunkHeader(offset, fourCC, size, fourCCType, fileHeader));

                    } 
                    else if (fourCCType == FourCC.MSIC || fourCCType == FourCC.VAGM) {

                        var offsetOfHeader = offset + chunkHeaderLength;

                        var chunkCount = BitConverter.ToInt16(bytes, offsetOfHeader);
                        var chunkIteration = BitConverter.ToInt16(bytes, offsetOfHeader + 2);

                        var musicHeader = new MusicHeader(chunkCount, chunkIteration);

                        offsets.Add(new ChunkHeader(offset, fourCC, size, fourCCType, musicHeader));
                    }
                    else {

                        offsets.Add(new ChunkHeader(offset, fourCC, size, fourCCType));

                    }

                } 
                else if (fourCC == FourCC.SWVR) {

                    var fourCCType = BytesToStringReversed(offset + 16, 4);

                    //todo: Sub file chunks length has not been proven to be constitant, and if it is refactor to a constant.

                    var fileNameOffset = offset + 20;

                    offsets.Add(new ChunkHeader(offset, fourCC, size, fourCCType, CopyOfRange(fileNameOffset, fileNameOffset + 16)));


                } 
                else {

                    offsets.Add(new ChunkHeader(offset, fourCC, size));

                }

                offset += size;


            }

        }

        // ---Utils---

        public static string Reverse(string s) {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static int DataChunksBySize(int size, int chunkSize = 4096, int headerLength = 20) {

            var total = size / (chunkSize - headerLength);
            if (size % (chunkSize - headerLength) != 0) {
                total++;
            }
            return total;

        }

        byte[] CopyOfRange(int start, int end) {

            var length = end - start;

            var total = new byte[length];

            Array.Copy(bytes, start, total, 0, length);

            return total;

        }

        int BytesToInt(int offset) {
            return BitConverter.ToInt32(bytes, offset);
        }


        string BytesToString(int offset, int length) {
            return Encoding.Default.GetString(bytes, offset, length);
        }

        string BytesToStringReversed(int offset, int length) {
            return Reverse(Encoding.Default.GetString(bytes, offset, length));
        }

    }

}