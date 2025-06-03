
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FCopParser {

    // This object will act as the in-between from the IFF file and any parsers of game data/files.
    // This object is planned to convert the game files back to the IFF file format for Future Cop to read.
    public class IFFFileManager {

        public static string[] fileOrderByFourCC = new string[] { 
            "RPNS", "Cfnt", "Cshd", "Cvkh", "Cvkb", "Cpyr", "Csfx", "Cwav", "Ctos", "Cptc", "Ctil", "Cfun", "Cnet", "Cbmp", "Cdcs", "Cobj", "Cact", "Csac", "Cctr"
        };

        // Game data that are separated and turn into individual files.
        public List<IFFDataFile> files = new();

        // Sub folders/files inside the IFF file that are separated.
        public List<SubFile> subFiles = new();

        // Nothing but the music, the key is the name of the song.
        public MusicFile music = null;

        public bool isPS1 = false;

        public void Sort() {

            var newOrder = new List<IFFDataFile>();

            foreach (var fourCC in fileOrderByFourCC) {

                newOrder.AddRange(GetFiles(fourCC));

            }

            files = newOrder;

        }

        public void CreateFileList(string path) {

            var total = "";

            foreach (var file in files) {

                total += file.dataFourCC + " " +
                    file.startNumber + " " +
                    file.dataID + " " +
                    file.data.Count + " Additional Data: ";

                foreach (var data in file.rpnsReferences) {
                    total += data + " ";
                }

                foreach (var data in file.headerCodeData) {
                    total += data + " ";
                }

                foreach (var data in file.headerCode) {
                    total += data + " ";
                }

                total += "\n";

            }

            foreach (var subFile in subFiles) {

                total += "Sub File: ";
                
                total += subFile.name;

                total += "\n";

                foreach (var file in subFile.files) {

                    total += file.dataFourCC + " " +
                        file.startNumber + " " +
                        file.data.Count + " " +
                        file.dataID + " Additional Data: ";


                    foreach (var data in file.rpnsReferences) {
                        total += data + " ";
                    }

                    foreach (var data in file.headerCodeData) {
                        total += data + " ";
                    }

                    foreach (var data in file.headerCode) {
                        total += data + " ";
                    }

                    total += "\n";

                }

                total += "\n";

            }

            if (music == null) {
                File.WriteAllText(path, total);
                return;
            }

            total += "Music: ";

            total += music.name;

            File.WriteAllText(path, total);

        }

        public IFFDataFile GetFile(string fourCC, int id) {
            return files.First(file => {
                return file.dataID == id && file.dataFourCC == fourCC;
            });
        }

        public List<IFFDataFile> GetFiles(string fourCC) {
            return files.Where(file => {
                return file.dataFourCC == fourCC;
            }).ToList();
        }

        public void ReplaceFile(string fourCC, int id, List<byte> data) {

            var file = files.First(file => {
                return file.dataID == id && file.dataFourCC == fourCC;
            });

            file.data = data;

        }

        public void DeleteFile(string fourCC, int id) {

            var fileToDelete = files.First(file => {
                return file.dataID == id && file.dataFourCC == fourCC;
            });

            files.Remove(fileToDelete);

        }

    }

    // Object for storing important meta data to a game file.
    public class IFFDataFile {

        public const int rpnsRefCount = 3;
        public const int headerCodeDataCount = 2;


        public List<byte> data;
        public int startNumber;
        public string dataFourCC;
        public int dataID;

        public bool modified = false;
        public bool ignore = false;

        // Additional header data
        public List<int> rpnsReferences = new();
        public List<int> headerCodeData = new();
        public List<byte> headerCode = new();

        public string name = "";

        public IFFDataFile(int startNumber, List<byte> data, string dataFourCC, int dataID, List<int> rpnsReferences, List<int> headerCodeData, List<byte> headerCode) {
            this.startNumber = startNumber;
            this.data = data;
            this.dataFourCC = dataFourCC;
            this.dataID = dataID;
            this.rpnsReferences = rpnsReferences;
            this.headerCodeData = headerCodeData;
            this.headerCode = headerCode;

        }

        public IFFDataFile(int startNumber, List<byte> data, string dataFourCC, int dataID, int nullRPNSRef) {
            this.startNumber = startNumber;
            this.data = data;
            this.dataFourCC = dataFourCC;
            this.dataID = dataID;

            rpnsReferences.Add(nullRPNSRef);
            rpnsReferences.Add(nullRPNSRef);
            rpnsReferences.Add(nullRPNSRef);
            headerCodeData.Add(1);
            headerCodeData.Add(1);
            headerCode.AddRange(new List<byte>() { 0, 0, 67, 79 });

        }

        public IFFDataFile Clone(int newID) {
            return new IFFDataFile(startNumber, data, dataFourCC, newID, new (rpnsReferences), new (headerCodeData), new (headerCode));
        }

        public List<byte> CompileAdditionalData() {

            var additionalData = new List<byte>();

            foreach (var rpnsRef in rpnsReferences) {
                additionalData.AddRange(BitConverter.GetBytes(rpnsRef));
            }

            foreach (var i in headerCodeData) {
                additionalData.AddRange(BitConverter.GetBytes(i));
            }

            additionalData.AddRange(headerCode);

            return additionalData;

        }

    }

    public class SubFile {

        public string name = "";
        public List<IFFDataFile> files = new();

        public SubFile(byte[] encodedName) {

            foreach (var b in encodedName) {

                if (b == 0) {
                    break;
                }

                name += Encoding.Default.GetString(new byte[] { b });

            }

        }

        public SubFile(string name) {
            this.name = name;
        }

        public SubFile(string name, List<IFFDataFile> files) {
            this.name = name;
            this.files = files;
        }

        public byte[] CompileName() {

            var bytes = Encoding.ASCII.GetBytes(name).ToList();

            if (bytes.Count < 16) {

                while (bytes.Count != 16) {
                    bytes.Add(0);
                }

            }
            // This should never be the case but throwing it in just in case
            else if (bytes.Count > 16) {
                bytes = bytes.GetRange(0, 16);
            }

            return bytes.ToArray();

        }

    }

    public class MusicFile {

        public string name = "";
        public List<byte> data = new();

        public MusicFile(string name) {
            this.name = name;
        }

        public MusicFile(string name, List<byte> data) : this(name) {
            this.data = data;
        }

        public MusicFile(byte[] encodedName, List<byte> data) {

            foreach (var b in encodedName) {

                if (b == 0) {
                    break;
                }

                name += Encoding.Default.GetString(new byte[] { b });

            }

            this.data = data;

        }

        public byte[] CompileName() {

            var bytes = Encoding.ASCII.GetBytes(name).ToList();

            if (bytes.Count < 16) {

                while (bytes.Count != 16) {
                    bytes.Add(0);
                }

            }
            else if (bytes.Count > 16) {
                bytes = bytes.GetRange(0, 16);
            }

            return bytes.ToArray();

        }

    }

}