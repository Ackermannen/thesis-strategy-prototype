using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TabMenuController : MonoBehaviour
{
    [Serializable]
    public class Tab {
        [SerializeField] public Button button;
        [SerializeField] public GameObject tab;
    }

    public Tab[] tabs;

    private GameObject activeTab;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Tab tab in tabs) {
            tab.button.onClick.AddListener(() => setupToggle(tab));
        }

        //initial
        setupToggle(tabs[0]);
    }

    private void Update() {
        if (activeTab != null) {
            Analytics.IncrementCounter(activeTab.name + "_time", Time.unscaledDeltaTime);
        } else {
            Analytics.IncrementCounter("untabbed_time", Time.unscaledDeltaTime);
        }
    }

    private void setupToggle(Tab target) {

        //if reclick then ignore.
        if(target.tab == activeTab) {
            return;
        }

        //if active target exists, close that one before continouing
        if(activeTab != null) {
            activeTab.SetActive(false);
        }

        //If target wasn't active
        if(!target.tab.activeSelf) {
            activeTab = target.tab;
        }

        target.tab.SetActive(!target.tab.activeSelf);
    }

    /**
     * Invoke certain tab to make it open instead.
     **/
    public void invokeTab(int index) {
        Tab tab = tabs[index];

        setupToggle(tab);
    }
}
