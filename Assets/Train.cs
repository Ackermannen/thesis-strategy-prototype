using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Train : MonoBehaviour, INotifyPropertyChanged {
    public enum TrainState {
        IDLE = 0,
        ENROUTE = 1,
        DISABLED = 2,
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(PropertyChangedEventArgs e) {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null) {
            handler(this, e);
        }
            
    }

    protected void OnPropertyChanged([CallerMemberName]string propertyName = "") {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    [SerializeField]
    private TrainState _state = TrainState.IDLE;

    public TrainState State {
        get {
            return this._state;
        }

        set {
            if(value != this._state) {
                this._state = value;
                OnPropertyChanged();
            }
        }
    }

    [SerializeField]
    private TrainTypes _type;
    public TrainTypes Type {
        get {
            return this._type;
        }

        set {
            if (value != this._type) {
                this._type = value;
                OnPropertyChanged();
            }
        }
    }

    [SerializeField]
    private TrainCargoData _cargo = new TrainCargoData(TrainCargoTypes.GRAIN, 0);
    public TrainCargoData Cargo { get => _cargo; set {
            if (value != this._cargo) {
                this._cargo = value;
                OnPropertyChanged();
            }
        }
    }

    [SerializeField]
    private int _currentDestination = -1;
    public int CurrentDestination { get => _currentDestination; set {
            if (value != this._currentDestination) {
                this._currentDestination = value;
                OnPropertyChanged();
            }
        }
    }


    [SerializeField]
    private float _progress = 0;
    public float Progress {
        get => _progress; set {
            if (value != this._progress) {
                this._progress = value;
                OnPropertyChanged();
            }
        }
    }


    [SerializeField]
    private float _maxProgress = 0;
    public float MaxProgress {
        get => _maxProgress; set {
            if (value != this._maxProgress) {
                this._maxProgress = value;
                OnPropertyChanged();
            }
        }
    }

    public void setTrainData(int unitSize, TrainCargoTypes cargoType, TrainState state, int currentDestination) {
        this.Cargo = new TrainCargoData(cargoType, unitSize);
        this.State = state;

        this.CurrentDestination = currentDestination;
    }

    public override bool Equals(object obj) {
        return obj is Train train &&
               base.Equals(obj) &&
               EqualityComparer<GameObject>.Default.Equals(gameObject, train.gameObject);
    }

    public override int GetHashCode() {
        int hashCode = -1963758822;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<GameObject>.Default.GetHashCode(gameObject);
        return hashCode;
    }
}
