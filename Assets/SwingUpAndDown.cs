using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingUpAndDown : MonoBehaviour
{
    private Vector3 initRotation;
    private void Start() {
        initRotation = this.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rot = this.transform.rotation.eulerAngles;
        float f = Mathf.PingPong(Time.time, 1f);
        rot = Vector3.Lerp(initRotation, initRotation + new Vector3(0, 90, 0), f);
        this.transform.rotation = Quaternion.Euler(rot);
    }
}
