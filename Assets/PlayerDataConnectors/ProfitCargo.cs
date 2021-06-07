using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ValueWindow))]
public class ProfitCargo : MonoBehaviour {

    private ValueWindow target;
    public Sprite icon;

    void Start() {
        target = GetComponent<ValueWindow>();
        target.setValue(0);
        target.setIcon(icon);

        PlayerDataController.onNewDay += onDayChange;
    }

    //Set listener to new finance object.
    private void onDayChange(int day) {
        PlayerDataController.currentDayFinance.profit.onProfitChange.AddListener(() => SetProfit(PlayerDataController.currentDayFinance.profit));
        SetProfit(PlayerDataController.currentDayFinance.profit);
    }

    private void SetProfit(DailyFinance.Profit i) {
        target.setValue(i.CargoTrains);
    }
}
