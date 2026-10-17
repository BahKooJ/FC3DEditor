
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {
    public class FCopGlobalData {

        public List<FCopAudio> soundEffects;
        public IFFFileManager fileManager;

        public FCopGlobalData(byte[] bytes) {

            var iffFile = new IFFParser(bytes);
            fileManager = iffFile.parsedData;

            List<IFFDataFile> GetFiles(string fourCC) {

                return fileManager.files.Where(file => {

                    return file.dataFourCC == fourCC;

                }).ToList();

            }

            IFFDataFile GetFile(string fourCC) {

                return fileManager.files.First(file => {

                    return file.dataFourCC == fourCC;

                });

            }

            var rawCwavs = GetFiles("Cwav");
            var rawCshd = GetFile("Cshd");

            soundEffects = FCopAudioParser.ParseGlblDataSounds(rawCwavs, rawCshd);

        }

    }
}