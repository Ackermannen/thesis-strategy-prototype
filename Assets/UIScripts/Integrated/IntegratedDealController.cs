using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntegratedDealController : MonoBehaviour
{
    public List<DealPanel> deals;

    public int city;

    public GameObject DealContainerPrefab;

    private GameObject dealContainer;

    void Start()
    {
        deals = new List<DealPanel>();
        dealContainer = Instantiate(DealContainerPrefab, WorldspaceCanvas.Instance.dealsLayer.transform);
        RectTransform crt = dealContainer.GetComponent<RectTransform>();

        //Sets pivot to bottom middle
        crt.pivot = new Vector2(0.5f, 0f);
        crt.position += new Vector3(0, -100, 0);



        dealContainer.transform.position = transform.position;
    }

    void Update()
    {
        dealContainer.transform.position = transform.position;
    }

    public void AddDeal(DealPanel p) {
        RectTransform prt = p.GetComponent<RectTransform>();
        prt.SetParent(dealContainer.transform);
        prt.transform.localScale = new Vector3(1, 1, 1);
        prt.eulerAngles = new Vector3(-10, 0, 0);
        prt.localPosition += new Vector3(0, 1, -75);
        prt.pivot = new Vector2(0.5f, 0f);

        deals.Add(p);

        LayoutRebuilder.ForceRebuildLayoutImmediate(dealContainer.GetComponent<RectTransform>());
    }
}
