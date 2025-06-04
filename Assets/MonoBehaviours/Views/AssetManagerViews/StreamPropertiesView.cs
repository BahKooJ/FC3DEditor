

using UnityEngine;
using TMPro;
using FCopParser;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class StreamPropertiesView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text indexText;
    public Image animationPreview;

    // - Parameters -
    public FCopStream stream;
    public FCopLevel level;

    List<Sprite> frames = new();

    private void Start() {
        
        indexText.text = level.audio.soundStreams.IndexOf(stream).ToString();

        Refresh();

    }

    void Refresh() {

        if (stream.miniAnimation != null) {

            frames.Clear();

            foreach (var frame in stream.miniAnimation.framesBitmapped) {

                var bmpTexture = new Texture2D(FCopMiniAnimation.width, FCopMiniAnimation.height, TextureFormat.ARGB32, false);

                bmpTexture.filterMode = FilterMode.Point;

                bmpTexture.LoadRawTextureData(frame.ToArray());
                bmpTexture.Apply();

                frames.Add(Sprite.Create(bmpTexture, new Rect(0, 0, FCopMiniAnimation.width, FCopMiniAnimation.height), Vector2.zero));

            }

            animationPreview.sprite = frames[0];

        }

    }


    float timer = 0f;
    int frameI = 0;
    private void Update() {

        if (stream.miniAnimation != null) {

            if (timer > 0.07f) {
                if (frameI == frames.Count) {
                    frameI = 0;
                }

                animationPreview.sprite = frames[frameI];
                frameI++;
                timer = 0f;
            }
            else {
                timer += Time.deltaTime;
            }


        }

    }

    public void OnClickExportMiniAnimation() {

        if (stream.miniAnimation == null) {
            return;
        }

        OpenFileWindowUtil.SaveFile("FCEAssets", "Mini Animation", path => {
            File.WriteAllBytes(path, stream.miniAnimation.rawFile.data.ToArray());
        });

    }

    public void OnClickImportMiniAnimation() {

        OpenFileWindowUtil.OpenFile("FCEAssets", "", path => {

            //try {

                var data = File.ReadAllBytes(path);

                stream.miniAnimation = new FCopMiniAnimation(new IFFDataFile(6, data.ToList(), "canm", 1, -1));

                Refresh();

            //}
            //catch {

            //    DialogWindowUtil.Dialog("Invalid File", "Please select a valid file.");

            //}

        });

    }

}