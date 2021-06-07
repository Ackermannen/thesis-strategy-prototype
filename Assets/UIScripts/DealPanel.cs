using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DealPanel : MonoBehaviour
{
    public Image img;
    public TextMeshProUGUI location;
    public TextMeshProUGUI units;
    public TextMeshProUGUI trainType;
    public ProgressBar bar;
    public Deal targetDeal;
    public GameObject disabledCover;
    public Button newRouteButton;
}
