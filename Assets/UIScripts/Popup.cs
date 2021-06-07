using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Popup : MonoBehaviour {

    public RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void setPopupPosition() {
        Vector2 pos = Input.mousePosition;
        float pivotX = pos.x / Screen.width;
        float pivotY = pos.y / Screen.height;
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = pos;
    }

    private void Update() {
        if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition)) {
            if((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))) {
                PopupSystem.HideNewRoutePopup();
                PopupSystem.HideGenericConfirmPopup();
            }
            
        }
    }
}
