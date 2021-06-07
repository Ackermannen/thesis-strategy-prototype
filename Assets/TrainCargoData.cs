using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrainCargoData
{
    public TrainCargoTypes type;
    public int amount;

    public TrainCargoData(TrainCargoTypes type, int amount) {
        this.type = type;
        this.amount = amount;
    }

    public override string ToString() {
        if (amount <= 0) return "";
        switch(this.type) {
            case TrainCargoTypes.PEOPLE:
                return amount + " people";
            default:
                return amount + " units of " + this.type;
        }
    }
}
