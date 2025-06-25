

using UnityEngine;
using UnityEngine.UI;

public class CreditsView : MonoBehaviour {

    // - Unity Refs -
    public ScrollRect scrollRect;

    float timer = 7f;
    private void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Destroy(gameObject);
        }

        if (timer > 0f) {
            timer -= Time.deltaTime;
            return;
        }

        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, scrollRect.normalizedPosition.y - 0.001f);

    }

}