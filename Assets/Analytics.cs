using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Analytics : MonoBehaviour
{
    public static Dictionary<string, List<string>> flags = new Dictionary<string, List<string>>();
    public static Dictionary<string, float> counters = new Dictionary<string, float>();

    private static Analytics current;
    public static Analytics Instance { get { return current; } }

    public TMP_InputField ConsoleLog;

    // Start is called before the first frame update
    void Awake()
    {
        if (current != null && current != this) {
            Destroy(this.gameObject);
        } else {
            current = this;
        }

    }
    private void Start() {
        List<GameObject> rootObjectsInScene = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjectsInScene);

        for (int i = 0; i < rootObjectsInScene.Count; i++) {
            Button[] allComponents = rootObjectsInScene[i].GetComponentsInChildren<Button>(true);
            foreach (Button b in allComponents) {
                AddFlag(b.gameObject.name + "_click", new DateTime());
                b.onClick.AddListener(() => {
                    AddFlag(b.gameObject.name + "_click", PlayerDataController.timeExact);
                });
            }
        }
    }

    public static void AddFlag(string s, DateTime dt) {
        if(!flags.ContainsKey(s)) {
            flags.Add(s, new List<string>() { dt.ToString("mm.ss") });
        } else {
            if(dt.Second != 0f)
                flags[s].Add(dt.ToString("mm.ss"));
        }
    }

    public static void IncrementCounter(string s) {
        if (!counters.ContainsKey(s)) {
            counters.Add(s, 1);
        } else {
            counters[s]++;
        }
    }
    public static void IncrementCounter(string s, float i) {
        if (!counters.ContainsKey(s)) {
            counters.Add(s, i);
        } else {
            counters[s] = counters[s] + i;
        }
    }

    private void Update() {
        if(Input.GetKeyDown("9")) {
            var str = ToString();
            Debug.Log(ToString());
            GUIUtility.systemCopyBuffer = str;
            ConsoleLog.gameObject.SetActive(!ConsoleLog.gameObject.activeSelf);
            ConsoleLog.text = str;
        }
    }

    public override string ToString() {
        return "{\"flags\": " + JsonConvert.SerializeObject(flags) + ", \"counters\": " +  JsonConvert.SerializeObject(counters) + ", \"player_data\": " + JsonConvert.SerializeObject(PlayerDataController.moneyOverTime) + "}";
    }
}
