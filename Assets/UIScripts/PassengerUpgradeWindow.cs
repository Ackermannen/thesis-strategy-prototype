using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassengerUpgradeWindow : MonoBehaviour {
    public Button eventButton;
    public TrainController controller;
    public TotalUsed totalUsedPassenger;

    // Start is called before the first frame update
    void Start() {
        eventButton.onClick.AddListener(() => onUpgrade());
        totalUsedPassenger.total.text = controller.passengerTrains.Count + " Total";
        controller.onUsedPassengerChange += (i) => onAvailableTrainChange(totalUsedPassenger, i);
        controller.passengerTrains.CollectionChanged += totalPassengerChange;
    }

    private void onUpgrade() {
        PopupSystem.ShowGenericConfirmPopup($"Upgrade Passenger Capacity?", $"Upgrading passenger capacity allows more trains to be sent," +
            $" but will cost <color=red>{controller.passengerTrainWage}</color> kr in maintenance each day.", () => {
                controller.AddTrainCapacity(TrainTypes.PASSENGER);
            });
    }

    private void totalPassengerChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        if (controller.passengerTrains.Count >= 6) {
            eventButton.interactable = false;
        }
        totalUsedPassenger.total.text = controller.passengerTrains.Count + " Total";
    }

    private void onAvailableTrainChange(TotalUsed totalUsed, int usedTrains) {
        totalUsed.used.text = usedTrains + " Used";
    }
}
