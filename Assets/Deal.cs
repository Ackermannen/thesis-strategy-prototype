using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deal
{
    public TrainCargoTypes cargoType;
    public int location;
    public int units;
    public TrainTypes trainType;
    public float maxDuration;
    private bool _active;


    public delegate void OnDurationChange(float newVal);
    public event OnDurationChange onDurationChange;

    public delegate void OnActiveStateChange(bool newState);
    public event OnActiveStateChange onActiveStateChange;

    private float _duration;

    public float Duration { get => this._duration; set {
            if (value != this._duration) {
                this._duration = value;

                if (onDurationChange != null) onDurationChange(_duration);
            }
        } 
    }

    public bool Active {
        get => _active; set {
            if (value != this._active) {
                this._active = value;

                if (onActiveStateChange != null) onActiveStateChange(_active);
            }
        }
    }

    public override bool Equals(object obj) {
        return obj is Deal deal &&
               cargoType == deal.cargoType &&
               location == deal.location &&
               units == deal.units &&
               trainType == deal.trainType;
    }

    public override int GetHashCode() {
        int hashCode = -2084241334;
        hashCode = hashCode * -1521134295 + cargoType.GetHashCode();
        hashCode = hashCode * -1521134295 + location.GetHashCode();
        hashCode = hashCode * -1521134295 + units.GetHashCode();
        hashCode = hashCode * -1521134295 + trainType.GetHashCode();
        return hashCode;
    }
}
