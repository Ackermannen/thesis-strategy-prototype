using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldspaceCanvas : MonoBehaviour
{
    private static WorldspaceCanvas current;

    public static WorldspaceCanvas Instance { get { return current; } }

    public GameObject dealsLayer;
    public GameObject trainsLayer;
    public GameObject railsLayer;

    [NonSerialized] public static Canvas canvas;

    public void Awake() {
        if (current != null && current != this) {
            Destroy(this.gameObject);
        } else {
            current = this;
        }

        canvas = GetComponent<Canvas>();


        dealsLayer.transform.localScale = new Vector3(1, 1, 1);


        trainsLayer.transform.localScale = new Vector3(1, 1, 1);


        railsLayer.transform.localScale = new Vector3(1, 1, 1);
    }

    public static void SetScaleTo(float f) {
        foreach(NonIntrusiveUIElement obj in current.GetComponentsInChildren<NonIntrusiveUIElement>()) {
            obj.gameObject.transform.localScale = new Vector3(f,f,0);
        }
    }
}
