using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ValueWindow))]
public class LossMaintenanceCrews : MonoBehaviour {

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
        PlayerDataController.currentDayFinance.costs.onCostChange.AddListener(() => setCosts(PlayerDataController.currentDayFinance.costs));
        setCosts(PlayerDataController.currentDayFinance.costs);
    }

    private void setCosts(DailyFinance.Costs i) {
        target.setValue(-i.MaintenanceCrews);
    }
}
