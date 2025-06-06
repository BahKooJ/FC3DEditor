

using FCopParser;
using System;
using System.Collections.Generic;

public class FCopMiniAnimation : FCopAsset {

    public const int width = 64;
    public const int height = 48;

    public int frames;
    public List<XRGB555> colorPalette = new();
    public List<List<byte>> framesBitmapped = new() { new() };

    public FCopMiniAnimation(IFFDataFile rawFile) : base(rawFile) {

        Parse(rawFile.data);

    }

    public void Parse(List<byte> data) {

        colorPalette.Clear();
        framesBitmapped = new() { new() };

        var offset = 0;
        frames = BitConverter.ToInt32(data.ToArray(), offset);
        offset += 4;

        for (int i = 0; i < 512; i += 2) {
            colorPalette.Add(new XRGB555(data.GetRange(offset, 2)));
            offset += 2;
        }

        List<List<byte>> lines = new() { new(), new(), new(), new() };

        var currentFrameDataCount = 0;
        var lineDataCount = 0;
        while (offset < data.Count + 1) {

            if (currentFrameDataCount == width * height) {
                framesBitmapped[^1].AddRange(lines[0]);
                framesBitmapped[^1].AddRange(lines[1]);
                framesBitmapped[^1].AddRange(lines[2]);
                framesBitmapped[^1].AddRange(lines[3]);
                lines = new() { new(), new(), new(), new() };

                if (framesBitmapped.Count == frames) {
                    break;
                }

                framesBitmapped.Add(new());
                currentFrameDataCount = 0;
            }

            var textureData = colorPalette[data[offset]].ToARGB32();

            if (lineDataCount == width * 4) {
                lineDataCount = 0;
            }

            lines[lineDataCount / width].AddRange(textureData);

            lineDataCount++;

            currentFrameDataCount++;
            offset++;

        }

    }

}