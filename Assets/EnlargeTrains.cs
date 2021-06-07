using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnlargeTrains : MonoBehaviour {
    public TrainController controller;

    private void OnEnable() {
        foreach(Train t in controller.cargoTrains) {
            t.gameObject.transform.localScale = new Vector3(3,3,3);
        }
        foreach (Train t in controller.passengerTrains) {
            t.gameObject.transform.localScale = new Vector3(3,3,3);
        }
    }

    private void OnDisable() {
        foreach (Train t in controller.cargoTrains) {
            if(t != null) t.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        foreach (Train t in controller.passengerTrains) {
            if (t != null) t.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
