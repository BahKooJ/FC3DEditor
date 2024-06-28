

using UnityEngine;

public class EscMenuLevelSettingsView : MonoBehaviour {

    // - Parameters -
    public Main main;

    public void OnClickCalculateCulling() {

        foreach (var section in main.level.sections) {
            section.culling.CalculateCulling(section);
        }

        QuickLogHandler.Log("Culling successfully calculated", LogSeverity.Success);


    }

    public void OnClickDiscardAllUnusedChannels() {

        DialogWindowUtil.Dialog("Discarding All Channels", "This will set ALL unused heights to -128. This action cannot be undone.", () => {

            foreach (var section in main.level.sections) {
                section.DiscardUnusedHeights();
            }

            return true;

        });

    }

}