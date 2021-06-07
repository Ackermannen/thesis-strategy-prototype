using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuaranteedNextEventEffect : GameEventEffect
{
    private GameEvent nextEvent;
    private GameEventController controller;

    public GuaranteedNextEventEffect(GameEvent nextEvent, GameEventController controller) {
        this.nextEvent = nextEvent;
        this.controller = controller;
    }

    public override void invokeEffect() {
        controller.guarranteedNextEvent = nextEvent;
    }
}
