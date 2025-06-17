

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ImpactSelectorItemView : MonoBehaviour {

    // - Unity Refs -
    public VideoPlayer videoPlayer;
    public RawImage reviewImage;

    // - Parameters -
    [HideInInspector]
    public VideoClip video;
    [HideInInspector]
    public int id;
    [HideInInspector]
    public ImpactSelectorView view;

    void Start() {

        var renderTexture = new RenderTexture(300, 300, 32);
        renderTexture.Create();

        videoPlayer.clip = video;
        videoPlayer.targetTexture = renderTexture;
        reviewImage.texture = renderTexture;

    }

    public void OnClick() {

        view.Select(id);

    }

}