using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogText : Singleton<LogText>
{
    [Header("Log")]
    public Canvas canvas;
    public Transform logPos;
    public GameObject logTextPrefab;
    public float moveUp;
    public int maxDisplayedMessages;
    public float displayMessageFor;

    [SerializeField] List<GameObject> logMessages = new List<GameObject>();
    GameObject latestLog;

    void Awake() {
        Instance = this;
    }

    public void Log(string text) {
        logMessages.Add(SpawnLogMessage(text, logPos));
        UpdateLog();
    }

    GameObject SpawnLogMessage(string text, Transform pos) {
        GameObject spawnedObj = Instantiate(logTextPrefab);
        spawnedObj.GetComponent<Text>().text = text;
        spawnedObj.transform.SetParent(canvas.transform, false);
        spawnedObj.transform.position = pos.position;
        spawnedObj.transform.localScale = pos.localScale;

        return spawnedObj;
    }

    void UpdateLog() {
        for (int i = 0; i < logMessages.Count-1; i++) {
            GameObject obj  =logMessages[i];
            obj.transform.position += new Vector3 (0, moveUp, 0);
        }

        while (logMessages.Count > maxDisplayedMessages) {
            RemoveLog(logMessages[0]);
        }
    }

    public void RemoveLog(GameObject logObject) {
        GameObject.Destroy(logObject);
        logMessages.Remove(logObject);
    }
}
