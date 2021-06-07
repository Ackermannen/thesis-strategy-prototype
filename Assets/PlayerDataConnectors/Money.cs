using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ValueWindow))]
public class Money : MonoBehaviour
{

    private ValueWindow target;

    void Start()
    {
        target = GetComponent<ValueWindow>();
        target.setValue(PlayerDataController.Money);

        PlayerDataController.onMoneyChange += onMoneyChanging;
    }

    private void onMoneyChanging(int money) {
        target.setValue(money);
    }
}
