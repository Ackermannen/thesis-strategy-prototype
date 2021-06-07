using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockMarketEffect : GameEventEffect
{
    private TrainCargoTypes type;
    private ResourceController controller;
    private bool all = false;
    private int priceModifier;

    public StockMarketEffect(ResourceController controller, TrainCargoTypes type, int priceModifier) {
        this.controller = controller;
        this.type = type;
        this.priceModifier = priceModifier;
    }

    public StockMarketEffect(ResourceController controller, int priceModifier) {
        this.controller = controller;
        this.all = true;
        this.priceModifier = priceModifier;
    }

    public override void invokeEffect() {
        if(all) {
            controller.IronPrice += priceModifier;
            if (controller.IronPrice < 1) controller.IronPrice = 1;
            controller.GrainPrice += priceModifier;
            if (controller.GrainPrice < 1) controller.GrainPrice = 1;
            controller.PeoplePrice += priceModifier;
            if (controller.PeoplePrice < 1) controller.PeoplePrice = 1;
            controller.WoodPrice += priceModifier;
            if (controller.WoodPrice < 1) controller.WoodPrice = 1;
        } else {
            switch (type) {
                case TrainCargoTypes.GRAIN:
                    controller.GrainPrice += priceModifier;
                    if (controller.GrainPrice < 1) controller.GrainPrice = 1;
                    break;
                case TrainCargoTypes.PEOPLE:
                    controller.PeoplePrice += priceModifier;
                    if (controller.PeoplePrice < 1) controller.PeoplePrice = 1;
                    break;
                case TrainCargoTypes.WOOD:
                    controller.WoodPrice += priceModifier;
                    if (controller.WoodPrice < 1) controller.WoodPrice = 1;
                    break;
                case TrainCargoTypes.IRON:
                    controller.IronPrice += priceModifier;
                    if (controller.IronPrice < 1) controller.IronPrice = 1;
                    break;
            }
        }
    }


}
