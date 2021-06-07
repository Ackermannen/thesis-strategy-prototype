using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaintenanceUpgradeWindow : MonoBehaviour {
    public Button eventButton;
    public RailController controller;

    // Start is called before the first frame update
    void Start() {
        eventButton.onClick.AddListener(() => onUpgrade());
    }

    private void onUpgrade() {
        PopupSystem.ShowGenericConfirmPopup($"Upgrade Maintenance Capacity?", $"Upgrading capacity allows more rails to be repaired at once," +
            $" but will cost {controller.maintenanceTeamsCost} in maintenance each day.", () => {
                controller.TotalMaintenance++;
            });
    }
}
