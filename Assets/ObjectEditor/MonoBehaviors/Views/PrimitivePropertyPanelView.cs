

using FCopParser;
using TMPro;
using UnityEngine;

public class PrimitivePropertyPanelView : MonoBehaviour {

    // - Unity View Refs -
    public TMP_Text unknown1Text;
    public TMP_Text textureEnabledText;
    public TMP_Text unknown2Text;
    public TMP_Text isReflectiveText;
    public TMP_Text gouraudShadingText;
    public TMP_Text vertexColorModeText;
    public TMP_Text visabilityModeText;
    public TMP_Text vertexColorSemiTransText;

    // - Parameters -
    [HideInInspector]
    public ObjectEditorMain main;
    public FCopObject.Primitive primitive;

    private void Start() {
        
        Refresh();

    }

    public void Refresh() {

        unknown1Text.text = primitive.unknown1.ToString();
        textureEnabledText.text = primitive.textureEnabled.ToString();
        unknown2Text.text = primitive.unknown2.ToString();
        isReflectiveText.text = primitive.isReflective.ToString();
        gouraudShadingText.text = primitive.material.gouraudShading.ToString();
        vertexColorModeText.text = primitive.material.colorMode.ToString();
        visabilityModeText.text = primitive.material.visabilityMode.ToString();
        vertexColorSemiTransText.text = primitive.material.vertexColorSemiTransparent.ToString();

    }

}