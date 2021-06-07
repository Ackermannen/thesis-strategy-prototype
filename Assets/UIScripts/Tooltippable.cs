using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltippable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public string content;
    public string header;
    public float timer = 0.5f;

    private static Coroutine delayed;

    private void Start() {
        Analytics.AddFlag(gameObject.name + "_hover", PlayerDataController.timeExact);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        delayed = StartCoroutine(delayedShow());
    }
    public void OnMouseEnter() {
        delayed = StartCoroutine(delayedShow());
    }

    public void OnPointerExit(PointerEventData eventData) {
        StopCoroutine(delayed);
        TooltipSystem.Hide();
    }

    public void OnMouseExit() {
        StopCoroutine(delayed);
        TooltipSystem.Hide();
    }
    private IEnumerator delayedShow() {
        yield return new WaitForSecondsRealtime(timer);
        TooltipSystem.Show(content, header);
        Analytics.AddFlag(gameObject.name + "_hover", PlayerDataController.timeExact);
    }
}
