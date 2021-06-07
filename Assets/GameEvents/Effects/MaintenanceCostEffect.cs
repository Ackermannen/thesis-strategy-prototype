using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintenanceCostEffect : GameEventEffect
{
    private int initialCost;
    private RailController railController;
    private TrainController trainController;
    private int multiplier;
    private float debuffDuration;
    private TrainTypes type;
    public MaintenanceCostEffect(RailController controller, int multiplier, float time) {
        initialCost = controller.maintenanceTeamsCost;
        railController = controller;
        this.multiplier = multiplier;
        debuffDuration = time;
    }

    public MaintenanceCostEffect(TrainController controller, int multiplier, float time, TrainTypes type) {
        trainController = controller;
        this.multiplier = multiplier;
        debuffDuration = time;
        initialCost = type == TrainTypes.CARGO ? controller.cargoTrainWage : controller.passengerTrainWage;
        this.type = type;
    }

    public override void invokeEffect() {
        if(railController != null) {
            railController.StartCoroutine(TemporaryMaintenanceDebuff());
        } else if(trainController != null) {
            if(type == TrainTypes.CARGO) {
               trainController.StartCoroutine(TemporaryCargoDebuff());
            } else if(type == TrainTypes.PASSENGER) {
                trainController.StartCoroutine(TemporaryPassengerDebuff());
            } else {
                Debug.LogError("INVALID TRAIN TYPING");
            }
            
        }
    }

    private IEnumerator TemporaryPassengerDebuff() {
        trainController.passengerTrainWage = initialCost * multiplier;
        yield return new WaitForSeconds(debuffDuration);
        trainController.passengerTrainWage = initialCost;
    }

    private IEnumerator TemporaryCargoDebuff() {
        trainController.cargoTrainWage = initialCost * multiplier;
        yield return new WaitForSeconds(debuffDuration);
        trainController.cargoTrainWage = initialCost;
    }

    private IEnumerator TemporaryMaintenanceDebuff() {
        railController.maintenanceTeamsCost = initialCost * multiplier;
        yield return new WaitForSeconds(debuffDuration);
        railController.maintenanceTeamsCost = initialCost;
    }
}
