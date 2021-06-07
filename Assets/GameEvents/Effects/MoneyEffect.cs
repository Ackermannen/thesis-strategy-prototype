using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyEffect : GameEventEffect {

    private int amount;
    public MoneyEffect(int amount) {
        this.amount = amount;
    }

    public override void invokeEffect() {
        PlayerDataController.Money += amount;
    }
}
