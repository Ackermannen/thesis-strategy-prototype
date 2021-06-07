using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueWindow : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI target;
    [SerializeField] private Image image;

    public void setIcon(Sprite icon) {
        image.sprite = icon;
    }

    public void setValue(int value) {
        if(value < 0) {
            target.color = Color.red;
        } else {
            target.color = Color.white;
        }
        target.text = value.ToString("N0");
    }
}
