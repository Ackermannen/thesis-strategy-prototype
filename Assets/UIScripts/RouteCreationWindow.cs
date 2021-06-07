using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RouteCreationWindow : MonoBehaviour
{
    public TMP_Dropdown trainDropdown;
    public TMP_Dropdown dealDropdown;
    public TextMeshProUGUI wrongTrainText;
    public TextMeshProUGUI implicationsText;
    public Button submitButton;
    public Tooltippable buttonTooltip;

    public RailController railController;
    public TrainController trainController;
    public ResourceController resourceController;

    public Sprite peopleIcon;
    public Sprite grainIcon;
    public Sprite ironIcon;
    public Sprite woodIcon;

    public delegate void OnActiveDealChange(Deal deal);
    public event OnActiveDealChange onActiveDealChange;

    private float profitRatio = 1; 

    private Deal _activeDeal;
    public Deal ActiveDeal { get => _activeDeal; set {
            if (value != this._activeDeal) {
                this._activeDeal = value;
                if (onActiveDealChange != null) onActiveDealChange(_activeDeal);
            }
        }
    }

    private void Start() {
        StartCoroutine(waitUntilSplinesExist());
        dealDropdown.onValueChanged.AddListener((i) => handleDealDropdownChange(i));
        trainDropdown.onValueChanged.AddListener((i) => handleTrainTypeChange(i));
        onActiveDealChange += handleActiveDealChange;
        submitButton.onClick.AddListener(handleSubmit);
        railController.onTrainJourneyCompletion += onTrainJourneyComplete;
    }

    private void onTrainJourneyComplete(TrainTypes type, TrainCargoData data, int profit) {
        handleTrainTypeChange(trainDropdown.value);
    }

    private void handleTrainTypeChange(int i) {
        if(ActiveDeal != null) {
            //if passenger train selected but there is none available
            if (i == 0 && !trainController.isThereATrain(TrainTypes.PASSENGER)) {
                submitButton.interactable = false;
                buttonTooltip.content = "No passenger train available. Buy more trains or wait...";
                //if cargo train selected but there is none available
            } else if (i == 1 && !trainController.isThereATrain(TrainTypes.CARGO)) {
                submitButton.interactable = false;
                buttonTooltip.content = "No cargo train available. Buy more trains or wait...";
            } else {
                submitButton.interactable = true;
                buttonTooltip.content = "";
            }

            if (i == 0 && ActiveDeal.trainType == TrainTypes.CARGO) {
                wrongTrainText.text = "<color=red>This cargo should be carried by a passenger train. Sending this will reduce profits by 50%</color>";
                trainDropdown.image.color = Color.red;
                profitRatio = 0.5f;
            } else if (i == 1 && ActiveDeal.trainType == TrainTypes.PASSENGER) {
                wrongTrainText.text = "<color=red>This cargo should be carried by a cargo train. Sending this will reduce profits by 50%</color>";
                trainDropdown.image.color = Color.red;
                profitRatio = 0.5f;
            } else {
                wrongTrainText.text = "";
                trainDropdown.image.color = Color.white;
                profitRatio = 1f;
            }

            //Refreshes options
            handleActiveDealChange(ActiveDeal);
        } else {
            wrongTrainText.text = "No deal available at this moment.";
            submitButton.interactable = false;
            buttonTooltip.content = "Can't send train without a deal selected.";
        }
    }

    private void handleSubmit() {
        railController.invokeNewTrain(new Connection(PlayerDataController.playerCity, ActiveDeal.location),
            trainDropdown.value == 0 ? TrainTypes.PASSENGER : TrainTypes.CARGO,
            new TrainCargoData(ActiveDeal.cargoType, ActiveDeal.units), (int)(resourceController.GetPrice(ActiveDeal.cargoType) * ActiveDeal.units * profitRatio));
        resourceController.SetDealAsUsed(ActiveDeal);
        RefreshDealDropdown();

        handleTrainTypeChange(trainDropdown.value);
        handleDealDropdownChange(0);

        PopupSystem.HideNewRoutePopup();
    }

    private void handleActiveDealChange(Deal deal) {
        if(deal != null) {
            int profit = (int) (deal.units * resourceController.GetPrice(deal.cargoType) * profitRatio);
            int costLength = railController.getRouteCost(new Connection(PlayerDataController.playerCity, deal.location));
            Rail rail = railController.getRailsToLocation(new Connection(PlayerDataController.playerCity, deal.location))[0];
            int trainsOntheRoadCost = (rail.maxHealth - rail.Health) * railController.railHealthCostModifier;

            implicationsText.text = $"Profit when arrived: <color={(profit - costLength > 0 ? "green" : "red")}>{(profit - costLength).ToString("N0")}</color> kr\n" +
                $"Daily maintenance of train on route: <color=red>-{trainsOntheRoadCost.ToString("N0")}</color>\n" + 
                $"Rail: <color={(rail.Health >= rail.maxHealth ? "white" : "red")}>{rail.Health}</color> / {rail.maxHealth}\n" + 
                $"Trains will be running at <color={(rail.Health >= rail.maxHealth ? "white" : "red")}>{Mathf.Round(railController.getTravelTimeBase(rail) / railController.getTravelTime(rail)*100)}%</color> speed";

            /*implicationsText.text = $"This trip will give you <color=green>{profit.ToString("n0")}</color> kr upon arriving. " +
            $"You will pay an up-front cost of <color=red>-{costLength.ToString("N0")}</color> kr due to the length of the journey, " +
            $"which will give you a profit of <color={(profit - costLength > 0 ? "green" : "red")}>{(profit - costLength).ToString("N0")}</color> kr, " +
            $"excluding train maintenance costs, which will be  each day given that the railroad to {XMLParser.cityNameMap[deal.location]} has {rail.Health} / {rail.maxHealth} health. " +
            $"Would you like to go ahead and send the train?";*/
        } else {
            implicationsText.text = "No deal available";
        }
    }

    private void handleDealDropdownChange(int i) {
        if(dealDropdown.options.Count > 0) {
            Debug.Log("Deal change");
            DealOptionData options = (DealOptionData)dealDropdown.options[i];
            ActiveDeal = options.deal;

            //refreshes other text
            handleTrainTypeChange(trainDropdown.value);
        } else {
            Debug.LogError("Empty dropdown");
            ActiveDeal = null;
        }
    }

    IEnumerator waitUntilSplinesExist() {
        yield return new WaitUntil(() => railController.connectionMap != null && railController.connectionMap.Count > 0);
        this.gameObject.SetActive(false);
        foreach(Deal d in resourceController.deals) {
            addAsDropdown(d);
        }
        resourceController.deals.CollectionChanged += changeInDeals;
        handleDealDropdownChange(0);
        handleTrainTypeChange(0);

    }

    private void RefreshDealDropdown() {
        dealDropdown.ClearOptions();
        foreach (Deal deal in resourceController.deals) {
            if (deal.Active) {
                addAsDropdown(deal);
            }
        }
    }

    private void changeInDeals(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
            foreach (Deal deal in e.NewItems) {
                if(deal.Active) {
                    addAsDropdown(deal);
                }
            }
        } else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) {
            RefreshDealDropdown();
            handleDealDropdownChange(0);
        }
    }

    private Sprite GetSprite(TrainCargoTypes type) {
        switch (type) {
            case TrainCargoTypes.GRAIN:
                return grainIcon;
            case TrainCargoTypes.WOOD:
                return woodIcon;
            case TrainCargoTypes.IRON:
                return ironIcon;
            case TrainCargoTypes.PEOPLE:
                return peopleIcon;
            default:
                return grainIcon;
        }
    }

    private void addAsDropdown(Deal deal) {
        DealOptionData data = new DealOptionData();
        data.image = GetSprite(deal.cargoType);
        data.deal = deal;
        

        Rail[] rails = railController.getRailsToLocation(new Connection(PlayerDataController.playerCity, deal.location));

        float travelTime = railController.getTravelTime(rails);
        TimeSpan span = new DateTime().AddSeconds(travelTime) - new DateTime();
        string timeToArrive = Math.Ceiling(span.TotalHours * PlayerDataController.speed) + "";

        if (deal.cargoType == TrainCargoTypes.PEOPLE) {
            data.text = $"{deal.units.ToString("N0")} {(deal.cargoType + "").ToLower()} - {XMLParser.cityNameMap[deal.location]}, {timeToArrive} Hours away";
        } else {
            data.text = $"{deal.units.ToString("N0")} units of {(deal.cargoType + "").ToLower()} - {XMLParser.cityNameMap[deal.location]}, {timeToArrive} Hours away";
        }
        
        dealDropdown.AddOptions(new List<TMP_Dropdown.OptionData>() { data });
    }

    public void setDealAsActive(Deal d) {
        int i = 0;
        foreach(DealOptionData option in dealDropdown.options) {
            if(option.deal == d) {
                handleDealDropdownChange(i);
                dealDropdown.value = i;

                if(d.trainType == TrainTypes.CARGO) {
                    trainDropdown.value = 1;
                } else {
                    trainDropdown.value = 0;
                }

                return;
            }
            i++;
        }

    }
}
