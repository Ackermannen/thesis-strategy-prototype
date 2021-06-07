using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class ResourcePanelController : MonoBehaviour
{

    public Sprite grainIcon;
    public Sprite ironIcon;
    public Sprite woodIcon;
    public Sprite peopleIcon;
    public Sprite waitIcon;

    public DealPanel dealPrefab;

    public ResourceController controller;

    public RailController railController;

    public IntegratedDealController[] provinceDealControllers;

    private void Start() {
        StartCoroutine(waitUntilDealsExist());
    }

    private IEnumerator waitUntilDealsExist() {
        yield return new WaitUntil(() => controller.deals != null && controller.deals.Count > 0 && railController.connectionMap != null && railController.connectionMap.Count > 0);

        provinceDealControllers = GetComponentsInChildren<IntegratedDealController>();

        foreach (Deal d in controller.deals) {
            IntegratedDealController idc = provinceDealControllers.First((c) => c.city == d.location);
            DealPanel newPanel = InstantiateDeal();


            idc.AddDeal(newPanel);
            fetchDeal(d, idc);
        }
        controller.deals.CollectionChanged += OnDealListChange;
    }

    private void OnDealListChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
            foreach(Deal deal in e.NewItems) {
                IntegratedDealController idc = provinceDealControllers.First((c) => c.city == deal.location);
                DealPanel newPanel = InstantiateDeal();
                idc.AddDeal(newPanel);
                fetchDeal(deal, idc);
            }
        } else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) {
            foreach (DealPanel deal in WorldspaceCanvas.canvas.GetComponentsInChildren<DealPanel>(true)) {
                if (deal.targetDeal != null && deal.targetDeal.Duration <= 0) {
                    Destroy(deal.gameObject);
                }
            }
        }
    }

    private DealPanel fetchDeal(Deal d, IntegratedDealController idc) {
        if(idc.deals.Count > 0) {
            //Find first panel that has no target;
            DealPanel dealPanel = idc.deals.Find((c) => c.targetDeal == null);

            Rail[] rails = railController.getRailsToLocation(new Connection(PlayerDataController.playerCity, d.location));
            float travelTimeToCity = railController.getTravelTime(rails);
            TimeSpan span = new DateTime().AddSeconds(travelTimeToCity) - new DateTime();
            string timeToArrive = Math.Ceiling(span.TotalHours * PlayerDataController.speed) + "";

            dealPanel.trainType.text = "Type: " + (d.trainType == TrainTypes.PASSENGER ? "Passenger" : "Cargo");
            dealPanel.units.text = d.cargoType == TrainCargoTypes.PEOPLE ? d.units + " People" : d.units + " Units";
            dealPanel.location.text = timeToArrive + " Hours away";
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

    private DealPanel InstantiateDeal() {
        DealPanel dealPanel = Instantiate(dealPrefab);
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

    
}
