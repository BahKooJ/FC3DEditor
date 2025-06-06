
using FCopParser;
using UnityEngine;
using UnityEngine.UI;

public class SupportingActorDataView : MonoBehaviour {

    // - Unity Refs -
    public TeamDataView teamDataView;
    public GroupDataView groupDataView;
    public TextureSnippetsView textureSnippetsView;
    public Image teamTab;
    public Image groupTab;
    public Image textureTab;

    // - Parameters -
    [HideInInspector]
    public Main main;
    public FCopLevel level;

    void Start() {
        
        teamDataView.level = level;
        groupDataView.level = level;
        textureSnippetsView.level = level;
        textureSnippetsView.main = main;

        teamTab.color = Main.selectedColor;

    }

    public void OnClickDone() {

        Destroy(gameObject);

    }

    public void OnClickTeams() {

        teamTab.color = Main.selectedColor;
        groupTab.color = Main.mainColor;
        textureTab.color = Main.mainColor;

        teamDataView.gameObject.SetActive(true);
        groupDataView.gameObject.SetActive(false);
        textureSnippetsView.gameObject.SetActive(false);

    }

    public void OnClickGroup() {

        teamTab.color = Main.mainColor;
        groupTab.color = Main.selectedColor;
        textureTab.color = Main.mainColor;

        teamDataView.gameObject.SetActive(false);
        groupDataView.gameObject.SetActive(true);
        textureSnippetsView.gameObject.SetActive(false);

    }

    public void OnClickTexture() {

        teamTab.color = Main.mainColor;
        groupTab.color = Main.mainColor;
        textureTab.color = Main.selectedColor;

        teamDataView.gameObject.SetActive(false);
        groupDataView.gameObject.SetActive(false);
        textureSnippetsView.gameObject.SetActive(true);

    }

}