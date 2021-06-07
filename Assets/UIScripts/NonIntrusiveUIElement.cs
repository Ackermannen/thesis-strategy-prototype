using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NonIntrusiveUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public void OnPointerEnter(PointerEventData eventData) {
        PlayerDataController.mouseOnNonIntrusive = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        PlayerDataController.mouseOnNonIntrusive = false;
    }
}
