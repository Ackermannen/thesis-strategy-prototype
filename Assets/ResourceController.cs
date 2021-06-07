using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class ResourceController : MonoBehaviour {

    private int _grainPrice;
    private int _ironPrice;
    private int _woodPrice;
    private int _peoplePrice;

    public delegate void OnPriceChange(int newPrice, TrainCargoTypes type);
    public event OnPriceChange onPriceChange;

    private bool dealCoolDown = false;

    public int GrainPrice {
        get => this._grainPrice; set {
            if (value != this._grainPrice) {
                this._grainPrice = value;
                if (onPriceChange != null) onPriceChange(value, TrainCargoTypes.GRAIN);
            }
        }
    }
    public int IronPrice {
        get => this._ironPrice; set {
            if (value != this._ironPrice) {
                this._ironPrice = value;
                if (onPriceChange != null) onPriceChange(value, TrainCargoTypes.IRON);
            }
        }
    }
    public int WoodPrice {
        get => this._woodPrice; set {
            if (value != this._woodPrice) {
                this._woodPrice = value;
                if (onPriceChange != null) onPriceChange(value, TrainCargoTypes.WOOD);
            }
        }
    }
    public int PeoplePrice {
        get => this._peoplePrice; set {
            if (value != this._peoplePrice) {
                this._peoplePrice = value;
                if (onPriceChange != null) onPriceChange(value, TrainCargoTypes.PEOPLE);
            }
        }
    }

    public ObservableCollection<Deal> deals;

    private void Start() {
        StartCoroutine(waitUntilSplinesExist());
        PlayerDataController.onNewDay += onNewDay;
    }

    public int GetPrice(TrainCargoTypes type) {
        switch (type) {
            case TrainCargoTypes.GRAIN:
                return GrainPrice;
            case TrainCargoTypes.WOOD:
                return WoodPrice;
            case TrainCargoTypes.IRON:
                return IronPrice;
            case TrainCargoTypes.PEOPLE:
                return PeoplePrice;
            default:
                return -1;
        }
    }

    private void onNewDay(int day) {
        randomizePrizing();
    }

    IEnumerator waitUntilSplinesExist() {
        yield return new WaitUntil(() => XMLParser.parsingComplete == true);
        deals = new ObservableCollection<Deal>();
        deals.Add(generateDeal());
        deals.Add(generateDeal());
        deals.Add(generateDeal());
        deals.Add(generateDeal());
        deals.Add(generateDeal());
        deals.Add(generateDeal());
        Debug.Log("randomizing");
        randomizePrizing();
    }

    IEnumerator waitSomeTimeBeforeNextDeal() {
        dealCoolDown = true;
        yield return new WaitForSeconds(Random.Range(1F, 5F));
        deals.Add(generateDeal());
        dealCoolDown = false;
    }

    private void Update() {
        if(deals != null) {
            if (deals.Count < 6 && !dealCoolDown) {
                StartCoroutine(waitSomeTimeBeforeNextDeal());
            }
            foreach (Deal d in deals.ToList()) {
                d.Duration -= Time.deltaTime;
                if (d.Duration <= 0) {
                    deals.Remove(d);
                }
            }
        }
    }

    public void SetDealAsUsed(Deal d) {
        d.Active = false;
    }

    private void randomizePrizing() {
        GrainPrice = Random.Range(1, 10);
        IronPrice = Random.Range(1, 10);
        WoodPrice = Random.Range(1, 10);
        PeoplePrice = Random.Range(3, 7);
    }

    private Deal generateDeal() {
        Deal deal = new Deal();
        deal.cargoType = (TrainCargoTypes)Random.Range(0, 4);
        deal.trainType = deal.cargoType == TrainCargoTypes.PEOPLE ? TrainTypes.PASSENGER : TrainTypes.CARGO;

        //Target location can never be home city
        var num1 = Random.Range(1, PlayerDataController.playerCity);
        var num2 = Random.Range(PlayerDataController.playerCity + 1, XMLParser.cityNameMap.Count + 1);
        var myNum = Random.Range(0, 2) == 1 ? num1 : num2;
        deal.location = myNum;

        deal.units = Random.Range(1000, 5000);
        System.TimeSpan travelTime = (PlayerDataController.date.AddDays(Random.Range(1,3)) - PlayerDataController.date);

        deal.Duration = (float)travelTime.TotalSeconds/PlayerDataController.speed;
        deal.maxDuration = deal.Duration;
        deal.Active = true;
        
        if(deal.cargoType == TrainCargoTypes.PEOPLE) {
            deal.units = (int) (deal.units * 1.5f);
        }

        return deal;
    }
}
