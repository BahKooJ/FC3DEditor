
using FCopParser;
using UnityEngine;
using UnityEngine.UI;

public class SupportingActorDataView : MonoBehaviour {

    // - Unity Refs -
    public TeamDataView teamDataView;
    public TextureSnippetsView textureSnippetsView;
    public Button teamTab;
    public Button textureTab;

    // - Parameters -
    [HideInInspector]
    public Main main;
    public FCopLevel level;

    void Start() {
        
        teamDataView.level = level;
        textureSnippetsView.level = level;
        textureSnippetsView.main = main;


    }

    public void OnClickDone() {

        Destroy(gameObject);

    }

    public void OnClickTeams() {

        var teamColors = teamTab.colors;
        teamColors.normalColor = new Color(0.5f, 1f, 0.5f);
        teamTab.colors = teamColors;

        var textureColors = textureTab.colors;
        textureColors.normalColor = new Color(1f, 1f, 1f);
        textureTab.colors = textureColors;

        teamDataView.gameObject.SetActive(true);
        textureSnippetsView.gameObject.SetActive(false);

    }

    public void OnClickTexture() {

        var teamColors = teamTab.colors;
        teamColors.normalColor = new Color(1f, 1f, 1f);
        teamTab.colors = teamColors;

        var textureColors = textureTab.colors;
        textureColors.normalColor = new Color(0.5f, 1f, 0.5f);
        textureTab.colors = textureColors;

        teamDataView.gameObject.SetActive(false);
        textureSnippetsView.gameObject.SetActive(true);

    }

}