using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConsoleLog : MonoBehaviour {
    public Text txt;
    public string cachedText;
    public List<string> cachedStrings;
    public int lineLimit = 10;

    public bool useFilter = false;
    public string filter;

    void OnEnable() {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {

        if (!useFilter) {
            AddLog(logString);
        } else if (logString.Contains(filter)) {
            AddLog(logString);
        }
    }

    protected virtual void AddLog(string logString) {
        cachedStrings.Add(logString);
        if (cachedStrings.Count > lineLimit) {
            cachedStrings.RemoveAt(0);
        }

        cachedText = "";
        for (int i = 0; i < cachedStrings.Count; i++) {
            cachedText += cachedStrings[i] + "\n";
        }
    }

    void Update() {
        txt.text = cachedText;
    }
}
