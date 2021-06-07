using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DirectRoute : MonoBehaviour
{

    public TextMeshProUGUI health;

    public Button newRoute;

    public ProgressBar bar;

    public bool isBeingRepaired;

    public void setToolTipMoney(int amount, int healthIncrease) {
        newRoute.GetComponent<Tooltippable>().content = "Repairing this railroad will cost " + amount.ToString("N0") + " kr and will repair it completely.";
    }
}
