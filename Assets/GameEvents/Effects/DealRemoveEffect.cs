using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DealRemoveEffect : GameEventEffect
{
    private ResourceController controller;
    private int target;

    private bool half;

    public DealRemoveEffect(ResourceController controller, int target) {
        this.controller = controller;
        this.target = target;
    }

    public DealRemoveEffect(ResourceController controller, bool half) {
        this.controller = controller;
        target = -1;
        this.half = half;
    }

    public override void invokeEffect() {
        if(target == -1) {
            if(half) {
                int i = Random.Range(0, 2);
                foreach (Deal d in controller.deals) {
                    if (i % 2 == 0) {
                        controller.SetDealAsUsed(d);
                    }
                    i++;
                }
            } else {
                foreach (Deal d in controller.deals) {
                    controller.SetDealAsUsed(d);
                }
            }
        } else {
            IEnumerable<Deal> afflictedDeals = controller.deals.Where((x) => x.location == target);
            foreach (Deal d in afflictedDeals) {
                controller.SetDealAsUsed(d);
            }
        }
    }
}
