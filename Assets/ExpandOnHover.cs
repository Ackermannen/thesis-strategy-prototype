using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ExpandOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private RectTransform rectTransform;
    private Vector3 initialScale;
    public float scaleFactor = 1.2f;
    
    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    public void OnPointerExit(PointerEventData eventData) {
        rectTransform.localScale = initialScale;
    }

    
}
