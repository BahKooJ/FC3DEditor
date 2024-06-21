

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickLogItemView : MonoBehaviour {

    const int lifeTime = 6;

    // - Unity Asset Refs -
    public Sprite errorIcon;
    public Sprite warningIcon;
    public Sprite infoIcon;
    public Sprite successIcon;

    // - Unity View Refs -
    public Image icon;
    public TMP_Text viewText;

    // - Parameters -
    public string message;
    public LogSeverity severity;

    float counter = 0;

    void Start() {

        switch (severity) {
            case LogSeverity.Error:
                icon.sprite = errorIcon;
                break;
            case LogSeverity.Warning:
                icon.sprite = warningIcon;
                break;
            case LogSeverity.Info:
                icon.sprite = infoIcon;
                break;
            case LogSeverity.Success:
                icon.sprite = successIcon;
                break;
        }

        viewText.text = message;

    }

    void Update() {
        
        counter += Time.deltaTime;

        if (counter > lifeTime) {
            Destroy(gameObject);
        }

    }

}