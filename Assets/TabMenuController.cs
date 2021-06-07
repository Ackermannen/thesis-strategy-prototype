using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private Button activeButton;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Tab tab in tabs) {
            tab.button.onClick.AddListener(() => setupToggle(tab));
        }
    }

    private void Update() {
        if(Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject()) {
            closeAllTabs();
        }

        if(activeTab != null) {
            Analytics.IncrementCounter(activeTab.name + "_time", Time.unscaledDeltaTime);
        } else {
            Analytics.IncrementCounter("untabbed_time", Time.unscaledDeltaTime);
        }
    }

    private void closeAllTabs() {
        if(activeTab) {
            activeTab.SetActive(false);
            activeTab = null;
            activeButton.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void setupToggle(Tab target) {

        //if active target exists, close that one before continouing
        if(activeTab != null) {
            if (target.tab.name == activeTab.name) {
                closeAllTabs();
                return;
            }
            activeTab.SetActive(false);
            activeButton.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        //If target wasn't active
        if(!target.tab.activeSelf) {
            activeTab = target.tab;
            activeButton = target.button;
            target.button.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
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
