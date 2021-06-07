using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ValueWindow))]
public class ProfitLoss : MonoBehaviour {

    private ValueWindow target;

    void Start() {
        target = GetComponent<ValueWindow>();
        target.setValue(0);

        PlayerDataController.onNewDay += onDayChange;
    }

    //Set listener to new finance object.
    private void onDayChange(int day) {
        PlayerDataController.currentDayFinance.costs.onCostChange.AddListener(() => setCosts(PlayerDataController.currentDayFinance));
        PlayerDataController.currentDayFinance.profit.onProfitChange.AddListener(() => setCosts(PlayerDataController.currentDayFinance));
        setCosts(PlayerDataController.currentDayFinance);
    }

    private void setCosts(DailyFinance i) {
        int profit = i.profit.PassengerTrains + i.profit.CargoTrains;
        int costs = i.costs.CargoTrains + i.costs.PassengerTrains + i.costs.MaintenanceCrews;
        target.setValue(profit - costs);
    }
}
