using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoUpgradeWindow : MonoBehaviour
{
    public Button eventButton;
    public TrainController controller;
    public TotalUsed totalUsedCargo;

    // Start is called before the first frame update
    void Start()
    {
        eventButton.onClick.AddListener(() => onUpgrade());
        totalUsedCargo.total.text = controller.cargoTrains.Count + " Total";
        controller.onUsedCargoChange += (i) => onAvailableTrainChange(totalUsedCargo, i);
        controller.cargoTrains.CollectionChanged += totalCargoChange;
    }

    private void onUpgrade() {
        PopupSystem.ShowGenericConfirmPopup($"Upgrade Cargo Capacity?", $"Upgrading cargo capacity allows more trains to be sent," +
            $" but will cost <color=red>{controller.cargoTrainWage}</color> kr in maintenance each day.", () => {
                controller.AddTrainCapacity(TrainTypes.CARGO);
        });
    }
    private void totalCargoChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        if (controller.cargoTrains.Count >= 6) {
            eventButton.interactable = false;
        }
        totalUsedCargo.total.text = controller.cargoTrains.Count + " Total";
    }

    private void onAvailableTrainChange(TotalUsed totalUsed, int usedTrains) {
        totalUsed.used.text = usedTrains + " Used";
    }
}
