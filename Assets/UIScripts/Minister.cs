using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class Minister : MonoBehaviour
{
    public TextMeshProUGUI taskText;
    public ProgressBar bar;
    [NonSerialized] public int currentTask = 0;
    public Button[] tasks;
}
