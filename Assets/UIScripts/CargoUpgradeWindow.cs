using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoUpgradeWindow : MonoBehaviour
{
    public Button eventButton;
    public TrainController controller;

    // Start is called before the first frame update
    void Start()
    {
        eventButton.onClick.AddListener(() => onUpgrade());
    }

    private void onUpgrade() {
        PopupSystem.ShowGenericConfirmPopup($"Upgrade Cargo Capacity?", $"Upgrading cargo capacity allows more trains to be sent," +
            $" but will cost {controller.cargoTrainWage} in maintenance each day.", () => {
                controller.AddTrainCapacity(TrainTypes.CARGO);
        });
    }
}
