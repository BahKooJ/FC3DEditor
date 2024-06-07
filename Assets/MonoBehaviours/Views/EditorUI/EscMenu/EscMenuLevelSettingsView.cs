

using UnityEngine;

public class EscMenuLevelSettingsView : MonoBehaviour {

    // - Parameters -
    public Main main;

    public void OnClickCalculateCulling() {

        foreach (var section in main.level.sections) {
            section.culling.CalculateCulling(section);
        }

    }

}