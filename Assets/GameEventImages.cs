using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventImages : MonoBehaviour
{
    public Sprite derailment;
    public Sprite economy;
    public Sprite comet;

    public Sprite GetEventImage(GameEventTypes type) {
        switch(type) {
            case GameEventTypes.DERAIL:
                return derailment;
            case GameEventTypes.ECONOMY:
                return economy;
            case GameEventTypes.COMET:
                return comet;
        }

        return derailment;
    }
}
