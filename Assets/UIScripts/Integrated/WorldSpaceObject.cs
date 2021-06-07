using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceObject : MonoBehaviour
{
    public Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = targetPosition;
        this.transform.localScale = new Vector3(1, 1, 1);
        this.transform.eulerAngles = new Vector3(-10, 0, 0);
    }

    private void Update() {
        this.transform.position = targetPosition;
    }
}
