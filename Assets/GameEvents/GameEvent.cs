using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public string title;
    public string description;
    public GameEventTypes type;
    public Dictionary<GameEventOption, GameEventEffect[]> options;

    public GameEvent(string title, string description, GameEventTypes type, params KeyValuePair<GameEventOption, GameEventEffect[]>[] options) {
        this.title = title;
        this.description = description;
        this.type = type;

        this.options = new Dictionary<GameEventOption, GameEventEffect[]>();
        foreach(KeyValuePair<GameEventOption, GameEventEffect[]> kv in options) {
            this.options.Add(kv.Key, kv.Value);
        }
    }

    public void invokeOption(GameEventOption option) {
        foreach(GameEventEffect effect in options[option]) {
            effect.invokeEffect();
        }
    }
}
