using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveWindow : MonoBehaviour
{

    public Button trigger;

    public RectTransform panel;

    public bool open = true;

    // Start is called before the first frame update
    void Start()
    {
        trigger.onClick.AddListener(togglePanel);
    }

    private void togglePanel() {
        open = !open;
        if (open) {
            panel.gameObject.SetActive(true);
        } else {
            panel.gameObject.SetActive(false);
        }
    }
}
