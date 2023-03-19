
using System.Collections.Generic;

namespace FCopParser {

    // This object will act as the in-between from the IFF file and any parsers of game data/files.
    // This object is planned to convert the game files back to the IFF file format for Future Cop to read.
    public class IFFFileManager {

        // Game data that are separated and turn into individual files.
        public List<IFFDataFile> files = new List<IFFDataFile>();

        // Sub folders/files inside the IFF file that are separated.
        public Dictionary<byte[], List<IFFDataFile>> subFiles = new Dictionary<byte[], List<IFFDataFile>>();

        // Nothing but the music, the key is the name of the song.
        public KeyValuePair<byte[], List<byte>>? music = null;

    }

    // Object for storing important meta data to a game file.
    public class IFFDataFile {

        public List<byte> data;
        public int startNumber;
        public string dataFourCC;
        public int dataID;
        public List<byte> additionalData;
        public bool modified = false;
        public bool ignore = false;

        public IFFDataFile(int startNumber, List<byte> data, string dataFourCC, int dataID, List<byte> additionalData) {
            this.startNumber = startNumber;
            this.data = data;
            this.dataFourCC = dataFourCC;
            this.dataID = dataID;
            this.additionalData = additionalData;
        }

        public IFFDataFile Clone(int newID) {
            return new IFFDataFile(startNumber, data, dataFourCC, newID, additionalData);
        }

    }

}