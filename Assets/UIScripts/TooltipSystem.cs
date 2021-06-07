﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;

    public Tooltip tooltip;

    // Start is called before the first frame update
    public void Awake()
    {
        current = this;
    }

    public static void Show(string content, string header = "") {
        current.tooltip.setText(content, header);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide() {
        current.tooltip.gameObject.SetActive(false);
    }

}
