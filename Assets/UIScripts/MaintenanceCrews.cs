using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintenanceCrews : MonoBehaviour
{

    public TotalUsed totalUsed;
    public RailController controller;

    // Start is called before the first frame update
    void Start()
    {

        onTotalCrewsChange(controller.TotalMaintenance);
        onCurrentActiveCrewsChange(controller.CurrentUsedMaintenance);
        controller.onChangeTotalCrews += onTotalCrewsChange;
        controller.onChangeActiveCrews += onCurrentActiveCrewsChange;
    }

    private void onCurrentActiveCrewsChange(int newVal) {
        totalUsed.used.text = newVal + " Used";
    }

    private void onTotalCrewsChange(int newVal) {
        totalUsed.total.text = newVal + " Total";
    }

    private void OnEnable() {

        //make sure numbers are up to date
        onTotalCrewsChange(controller.TotalMaintenance);
        onCurrentActiveCrewsChange(controller.CurrentUsedMaintenance);
    }
}
