
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FCopParser;
using static NodeSelectorViewUtil;

public class NodeSelectorViewItem : MonoBehaviour {

    // - Unity Assets -
    public Sprite openExpressionSprite;
    public Sprite expressionFillSprite;
    public Sprite closeExpressionSprite;

    // - Unity Refs -
    public TMP_Text nodeNameText;
    public TMP_Text typeText;
    public TMP_Text descriptionText;
    public Image openBracketImage;
    public Image fillImage;
    public Image closeBracketImage;

    // - Parameter -
    public NodeCreatorData creatorData;
    [HideInInspector]
    public NodeSelectorView view;

    void Start() {

        nodeNameText.text = creatorData.name;
        typeText.text = creatorData.returnType.ToString();
        descriptionText.text = creatorData.description;

        if (creatorData.returnType != ScriptDataType.Void) {
            openBracketImage.sprite = openExpressionSprite;
            fillImage.sprite = expressionFillSprite;
            closeBracketImage.sprite = closeExpressionSprite;
        }

    }

}