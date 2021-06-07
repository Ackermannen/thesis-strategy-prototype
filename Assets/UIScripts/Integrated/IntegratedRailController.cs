using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntegratedRailController : MonoBehaviour {
    public DirectRoute route { get; private set; }

    public GameObject RailContainerPrefab;

    private GameObject dealContainer;

    public Connection connection;

    public Vector3 targetPosition;

    void Start() {
        dealContainer = Instantiate(RailContainerPrefab, WorldspaceCanvas.Instance.railsLayer.transform);
        RectTransform crt = dealContainer.GetComponent<RectTransform>();

        //Sets pivot to bottom middle
        crt.pivot = new Vector2(0.5f, 0f);
        crt.position += new Vector3(0, -100, 0);


        Debug.Log(targetPosition);
        dealContainer.transform.position = targetPosition;
    }

    void Update() {
        dealContainer.transform.position = targetPosition;
    }

    public void AddPanel(DirectRoute dr) {
        RectTransform prt = dr.GetComponent<RectTransform>();
        prt.SetParent(dealContainer.transform);
        prt.transform.localScale = new Vector3(1, 1, 1);
        prt.eulerAngles = new Vector3(-10, 0, 0);
        prt.localPosition += new Vector3(0, 1, -75);
        route = dr;
    }
}
