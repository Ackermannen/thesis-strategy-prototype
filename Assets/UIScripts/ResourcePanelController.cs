using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ResourcePanelController : MonoBehaviour
{

    //Prices labels
    public TextMeshProUGUI grainPrice;
    public TextMeshProUGUI ironPrice;
    public TextMeshProUGUI woodPrice;
    public TextMeshProUGUI peoplePrice;

    public Sprite grainIcon;
    public Sprite ironIcon;
    public Sprite woodIcon;
    public Sprite peopleIcon;
    public Sprite waitIcon;

    public GameObject dealsPanel;

    public DealPanel dealPrefab;

    public ResourceController controller;

    public RailController railController;

    public Stack<DealPanel> freePanels;

    private void Start() {
        StartCoroutine(waitUntilDealsComplete());
    }

    private IEnumerator waitUntilDealsComplete() {
        yield return new WaitUntil(() => controller.dealsComplete == true);

        grainPrice.text = controller.GrainPrice + ":-";
        ironPrice.text = controller.IronPrice + ":-";
        woodPrice.text = controller.WoodPrice + ":-";
        peoplePrice.text = controller.PeoplePrice + ":-";
        controller.onPriceChange += UpdatePrices;

        freePanels = new Stack<DealPanel>();

        foreach (Deal d in controller.deals) {
            freePanels.Push(AddDeal());
            fetchDeal(d);
        }
        controller.deals.CollectionChanged += OnDealListChange;
    }

    private void OnDealListChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
            foreach(Deal deal in e.NewItems) {
                fetchDeal(deal);
            }
        } else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) {
            foreach (DealPanel deal in dealsPanel.GetComponentsInChildren<DealPanel>()) {
                if(deal.targetDeal != null && deal.targetDeal.Duration <= 0) {
                    freePanels.Push(deal);
                    resetDeal(deal);
                }
            }
        }
    }

    private DealPanel fetchDeal(Deal d) {
        if(freePanels.Count > 0) {
            DealPanel dealPanel = freePanels.Pop();

            Rail[] rails = railController.getRailsToLocation(new Connection(PlayerDataController.playerCity, d.location));
            float travelTimeToCity = railController.getTravelTime(rails);
            TimeSpan span = new DateTime().AddSeconds(travelTimeToCity) - new DateTime();
            string timeToArrive = Math.Ceiling(span.TotalHours * PlayerDataController.speed) + "";

            dealPanel.trainType.text = "Type: " + (d.trainType == TrainTypes.PASSENGER ? "Passenger" : "Cargo");
            dealPanel.units.text = d.cargoType == TrainCargoTypes.PEOPLE ? d.units + " People" : d.units + " Units";
            dealPanel.location.text = "Destination " + XMLParser.cityNameMap[d.location] + ", " + timeToArrive + " Hours away";
            dealPanel.bar.bar.maxValue = d.maxDuration;
            dealPanel.bar.bar.value = d.Duration;
            System.TimeSpan travelTime = (PlayerDataController.date.AddSeconds(d.Duration) - PlayerDataController.date);
            dealPanel.bar.displayText.text = Math.Round(travelTime.TotalHours * PlayerDataController.speed) + " Hours left";
            dealPanel.img.sprite = GetSprite(d.cargoType);
            dealPanel.targetDeal = d;
            
            dealPanel.disabledCover.SetActive(!d.Active);
            d.onActiveStateChange += (b) => onDealActiveStateChange(b, dealPanel);
            d.onDurationChange += (i) => onTimeLeftChange(i, dealPanel);

            dealPanel.newRouteButton.onClick.AddListener(() => PopupSystem.ShowNewRoutePopup(d));

            return dealPanel;
        } else {
            Debug.LogError("No free panel exists");
            Debug.LogError(d);
            return null;
        }
    }

    private void onDealActiveStateChange(bool newState, DealPanel p) {
        Debug.Log(newState);
            p.disabledCover.SetActive(!newState);
    }

    private DealPanel AddDeal() {
        DealPanel dealPanel = Instantiate(dealPrefab, dealsPanel.transform);
        resetDeal(dealPanel);
        return dealPanel;
    }
    
    private DealPanel resetDeal(DealPanel d) {
        d.trainType.text = "";
        d.units.text = "";
        d.location.text = "";
        d.bar.bar.maxValue = 1;
        d.bar.bar.value = 1;
        d.img.sprite = waitIcon;
        d.targetDeal = null;
        d.disabledCover.SetActive(false);


        return d;
    }

    private Sprite GetSprite(TrainCargoTypes type) {
        switch(type) {
            case TrainCargoTypes.GRAIN:
                return grainIcon;
            case TrainCargoTypes.WOOD:
                return woodIcon;
            case TrainCargoTypes.IRON:
                return ironIcon;
            case TrainCargoTypes.PEOPLE:
                return peopleIcon;
            default:
                return waitIcon;
        }
    }

    private void onTimeLeftChange(float newVal, DealPanel s) {
        s.bar.bar.value = newVal;
        System.TimeSpan travelTime = (PlayerDataController.date.AddSeconds(newVal) - PlayerDataController.date);
        s.bar.displayText.text = Math.Round(travelTime.TotalHours * PlayerDataController.speed) + " Hours left";
    }

    private void UpdatePrices(int price, TrainCargoTypes type) {
        switch(type) {
            case TrainCargoTypes.GRAIN:
                grainPrice.text = price + ":-";
                break;
            case TrainCargoTypes.IRON:
                ironPrice.text = price + ":-";
                break;
            case TrainCargoTypes.WOOD:
                woodPrice.text = price + ":-";
                break;
            case TrainCargoTypes.PEOPLE:
                peoplePrice.text = price + ":-";
                break;
        }
    }
}
