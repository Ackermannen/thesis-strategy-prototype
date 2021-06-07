using BezierSolution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    public int cargoInitial = 1;
    public int passengerInitial = 1;
    public int cargoTrainWage = 100;
    public int passengerTrainWage = 150;

    [SerializeField] private GameObject cargoTrain;
    [SerializeField] private GameObject passengerTrain;

    public ObservableCollection<Train> cargoTrains { get; private set; }
    public ObservableCollection<Train> passengerTrains { get; private set; }

    //Event listeners for current amount of trains being used at the moment.
    public delegate void OnUsedCargoChange(int newVal);
    public event OnUsedCargoChange onUsedCargoChange;

    public delegate void OnUsedPassengerChange(int newVal);
    public event OnUsedPassengerChange onUsedPassengerChange;

    private int _currentlyUsedCargoTrains;
    public int CurrentlyUsedCargoTrains { get => _currentlyUsedCargoTrains; set {
            if (value != this._currentlyUsedCargoTrains) {
                this._currentlyUsedCargoTrains = value;

                if (onUsedCargoChange != null) onUsedCargoChange(_currentlyUsedCargoTrains);
            }
        }
    }

    private int _currentlyUsedPassengerTrains;
    public int CurrentlyUsedPassengerTrains {
        get => _currentlyUsedPassengerTrains; set {
            if (value != this._currentlyUsedPassengerTrains) {
                this._currentlyUsedPassengerTrains = value;

                if (onUsedPassengerChange != null) onUsedPassengerChange(_currentlyUsedPassengerTrains);
            }
        }
    }

    private void Awake() {
        PlayerDataController.onNewDay += onDayChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        cargoTrains = new ObservableCollection<Train>();
        passengerTrains = new ObservableCollection<Train>();
        
        CurrentlyUsedPassengerTrains = 0;
        CurrentlyUsedCargoTrains = 0;

        for (int i = 0; i < cargoInitial; i++) {
            Train newCargoTrain = Instantiate(cargoTrain, transform).AddComponent<Train>();
            newCargoTrain.gameObject.SetActive(false);
            newCargoTrain.Type = TrainTypes.CARGO;
            cargoTrains.Add(newCargoTrain);
        }

        for (int i = 0; i < passengerInitial; i++) {
            Train newPassengerTrain = Instantiate(passengerTrain, transform).AddComponent<Train>();
            newPassengerTrain.gameObject.SetActive(false);
            newPassengerTrain.Type = TrainTypes.PASSENGER;
            passengerTrains.Add(newPassengerTrain);
        }

    }

    public void AddTrainCapacity(TrainTypes type) {
        if(type == TrainTypes.CARGO) {
            Train newCargoTrain = Instantiate(cargoTrain, transform).AddComponent<Train>();
            newCargoTrain.gameObject.SetActive(false);
            newCargoTrain.Type = TrainTypes.CARGO;
            cargoTrains.Add(newCargoTrain);
        } else {
            Train newPassengerTrain = Instantiate(passengerTrain, transform).AddComponent<Train>();
            newPassengerTrain.gameObject.SetActive(false);
            newPassengerTrain.Type = TrainTypes.PASSENGER;
            passengerTrains.Add(newPassengerTrain);
        }
    }

    private void onDayChange(int day) {
        //Cost -> wage * cargoTrains + wage * usedCargoTrains, aka pay double for trains in use.
        int cargoCost = cargoTrainWage * cargoTrains.Count + cargoTrainWage * CurrentlyUsedCargoTrains;
        PlayerDataController.currentDayFinance.costs.CargoTrains += cargoCost;
        int passengerCost = passengerTrainWage * passengerTrains.Count + passengerTrainWage * CurrentlyUsedPassengerTrains;
        PlayerDataController.currentDayFinance.costs.PassengerTrains += passengerCost;
        Debug.LogFormat("Train costs are {0}", cargoCost + passengerCost);
        PlayerDataController.Money -= (cargoCost + passengerCost);
    }

    public bool isThereATrain(TrainTypes type) {
        switch(type) {
            case TrainTypes.CARGO:
                return (cargoTrains.Count - CurrentlyUsedCargoTrains) > 0;
            case TrainTypes.PASSENGER:
                return (passengerTrains.Count - CurrentlyUsedPassengerTrains) > 0;
        }
        return false;
    }

    public void returnTrain(Train train) {
        train.setTrainData(0, TrainCargoTypes.GRAIN, Train.TrainState.IDLE, -1);
        train.gameObject.SetActive(false);

        if(train.Type == TrainTypes.CARGO) {
            CurrentlyUsedCargoTrains--;
        } else {
            CurrentlyUsedPassengerTrains--;
        }
    }

    public Train getTrain(TrainTypes type) {
        switch(type) {
            case TrainTypes.CARGO:
                Train t = useCargoTrain();
                if(t == null) throw new NullReferenceException("No available cargo trains");
                CurrentlyUsedCargoTrains++;
                return t;
            case TrainTypes.PASSENGER:
                Train d = usePassengerTrain();
                if (d == null) throw new NullReferenceException("No available passenger trains");
                CurrentlyUsedPassengerTrains++;
                return d;
            default:
                throw new ArgumentException("TrainType is neither CARGO nor PASSENGER");
        }
    }

    private Train[] getIdleTrains(TrainTypes type) {
        List<Train> trains = new List<Train>();

        if(type == TrainTypes.CARGO) {
            foreach (Train t in cargoTrains) {
                if (t.State == Train.TrainState.IDLE) {
                    trains.Add(t);
                }
            }
        } else {
            foreach (Train t in passengerTrains) {
                if (t.State == Train.TrainState.IDLE) {
                    trains.Add(t);
                }
            }
        }

        return trains.ToArray();
    }

    private Train usePassengerTrain() {
        Train[] idleTrains = getIdleTrains(TrainTypes.PASSENGER);

        if(idleTrains.Length > 0) {
            idleTrains[0].State = Train.TrainState.ENROUTE;
            idleTrains[0].gameObject.SetActive(true);
            return idleTrains[0];
        }
        return null;
    }

    private Train useCargoTrain() {
        Train[] idleTrains = getIdleTrains(TrainTypes.CARGO);

        if (idleTrains.Length > 0) {
            idleTrains[0].State = Train.TrainState.ENROUTE;
            idleTrains[0].gameObject.SetActive(true);
            return idleTrains[0];
        }
        return null;
    }
}
