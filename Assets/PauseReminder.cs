using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseReminder : MonoBehaviour
{

    public TimeController controller;
    public TextMeshProUGUI target;

    public float fadeTime = 0.2f;
    public float waitTime = 5f;
    private bool hasWaited = false;

    private Coroutine activeRoutine = null;

    void Awake()
    {
        controller.OnPause.AddListener(OnPauseStateChange);
    }

    private void OnPauseStateChange() {
        if(activeRoutine != null) {
            StopCoroutine(activeRoutine);
        }

        if(controller.Paused) {
            activeRoutine = StartCoroutine(FadeTextToFullAlpha(fadeTime, target, waitTime));
        } else {
            //Don't show animation if timer didn't go all the way through
            if(hasWaited) {
                hasWaited = false;
                activeRoutine = StartCoroutine(FadeTextToZeroAlpha(fadeTime, target));
            }
            
        }
    }

    public IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI i, float waitTime) {
        yield return new WaitForSecondsRealtime(waitTime);
        i.gameObject.SetActive(true);
        hasWaited = true;
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f) {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.unscaledDeltaTime / t));
            yield return null;
        }
        activeRoutine = null;
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI i) {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f) {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.unscaledDeltaTime / t));
            yield return null;
        }
        i.gameObject.SetActive(false);
        activeRoutine = null;
    }
}
