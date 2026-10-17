

using UnityEngine;

public class QuickLogHandler : MonoBehaviour {

    public static QuickLogHandler instance;

    public static void Log(string message, LogSeverity severity) {
        instance.AddLog(message, severity);
    }

    // - Unity Prefabs -
    public GameObject QuickLogItemPrefab;

    // - Unity View Refs -
    public Transform content;

    // - Parameters -
    public Main main;

    void Start() {
        
        instance = this;

    }

    public void AddLog(string message, LogSeverity severity) {

        var obj = Instantiate(QuickLogItemPrefab);
        obj.transform.SetParent(content.transform, false);

        var script = obj.GetComponent<QuickLogItemView>();
        script.message = message;
        script.severity = severity;

    }

}