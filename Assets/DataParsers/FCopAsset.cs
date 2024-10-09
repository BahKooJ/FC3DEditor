namespace FCopParser {
    public abstract class FCopAsset {

        public IFFDataFile rawFile;
        public string name;

        public int DataID {
            get { return rawFile.dataID; }
            set { rawFile.dataID = value; }
        }

        protected FCopAsset(IFFDataFile rawFile) {
            this.rawFile = rawFile;
        }

        protected FCopAsset(IFFDataFile rawFile, string name) : this(rawFile) {
            this.name = name;
        }

    }
}