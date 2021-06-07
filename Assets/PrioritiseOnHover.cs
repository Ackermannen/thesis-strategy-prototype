using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrioritiseOnHover : MonoBehaviour, IPointerEnterHandler {
    private void OnMouseEnter() {
        this.transform.SetAsFirstSibling();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        this.transform.SetAsLastSibling();
    }
}
