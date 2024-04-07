
namespace FCopParser {
    //Objects for storing useful data regarding chunk headers.
    public class ChunkHeader {

        public int index;
        public string fourCCDeclaration;
        public int chunkSize;

        public ChunkHeader(int index, string fourCCDeclaration, int chunkSize) {
            this.index = index;
            this.fourCCDeclaration = fourCCDeclaration;
            this.chunkSize = chunkSize;
        }

        public string fourCCType = "";

        public ChunkHeader(int index, string fourCCDeclaration, int chunkSize, string fourCCType) {
            this.index = index;
            this.fourCCDeclaration = fourCCDeclaration;
            this.chunkSize = chunkSize;
            this.fourCCType = fourCCType;
        }

        public FileHeader? fileHeader = null;

        public ChunkHeader(int index, string fourCCDeclaration, int chunkSize, string fourCCType, FileHeader fileHeader) {
            this.index = index;
            this.fourCCDeclaration = fourCCDeclaration;
            this.chunkSize = chunkSize;
            this.fourCCType = fourCCType;
            this.fileHeader = fileHeader;
        }

        public byte[] subFileName;

        public ChunkHeader(int index, string fourCCDeclaration, int chunkSize, string fourCCType, byte[] subFileName) {
            this.index = index;
            this.fourCCDeclaration = fourCCDeclaration;
            this.chunkSize = chunkSize;
            this.fourCCType = fourCCType;
            this.subFileName = subFileName;
        }

        public MusicHeader musicHeader = null;

        public ChunkHeader(int index, string fourCCDeclaration, int chunkSize, string fourCCType, MusicHeader musicHeader) {
            this.index = index;
            this.fourCCDeclaration = fourCCDeclaration;
            this.chunkSize = chunkSize;
            this.fourCCType = fourCCType;
            this.musicHeader = musicHeader;
        }

    }

    public class FileHeader {

        public int startNumber;
        public string fourCCData;
        public int dataID;
        public int dataSize;
        public byte[] actData;

        public FileHeader(int startNumber, string fourCCData, int dataID, int dataSize, byte[] actData) {
            this.startNumber = startNumber;
            this.fourCCData = fourCCData;
            this.dataID = dataID;
            this.dataSize = dataSize;
            this.actData = actData;
        }

    }

    public class MusicHeader {

        public int chunkCount;
        public int chunkIteration;

        public MusicHeader(int chunkCount, int chunkIteration) {
            this.chunkCount = chunkCount;
            this.chunkIteration = chunkIteration;
        }

    }

}