using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailHurtEffect : GameEventEffect
{
    private Connection connection;
    private RailController controller;
    private int amount;

    public RailHurtEffect(RailController controller, Connection connection, int amount) {
        this.connection = connection;
        this.controller = controller;
        this.amount = amount;
    }

    public RailHurtEffect(RailController controller, int amount) {
        this.controller = controller;
        this.connection = null;
        this.amount = amount;
    }

    public override void invokeEffect() {
        if(connection != null) {
            controller.directConnections[connection].Health -= amount;
            if (controller.directConnections[connection].Health <= 0) {
                controller.directConnections[connection].Health = 1;
            } else if(controller.directConnections[connection].Health > 15) {
                controller.directConnections[connection].Health = 15;
            }
        } else {
            foreach(Rail r in controller.directConnections.Values) {
                r.Health -= amount;
                if (r.Health <= 0) {
                    r.Health = 1;
                } else if (r.Health > 15) {
                    r.Health = 15;
                }
            }
        }
    }

}
