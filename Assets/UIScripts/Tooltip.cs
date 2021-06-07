using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI header;

    public TextMeshProUGUI content;

    public LayoutElement layoutElement;

    public int characterWrapLimit;

    public RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void setText(string c, string h = "") {

        if(string.IsNullOrEmpty(h)) {
            header.gameObject.SetActive(false);
        } else {
            header.gameObject.SetActive(true);
            header.text = h;
        }

        content.text = c;
    }

    private void Update() {

        //Editor only, shows changes in layout.
        if(header != null && content != null && layoutElement != null && Application.isEditor) {
            int headerLength = header.text.Length;
            int contentLength = content.text.Length;

            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
        }


        Vector2 pos = Input.mousePosition;

        float pivotX = pos.x / Screen.width;
        float pivotY = pos.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);

        transform.position = pos;
    }
}
