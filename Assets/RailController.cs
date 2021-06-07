using BezierSolution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RailController : MonoBehaviour {


    public BezierWalker train;

    private MeshCollider[] colliders;

    public Dictionary<Connection, Rail> connectionMap;

    [NonSerialized] public Connection[] connections;

    [NonSerialized] public Dictionary<Connection, Rail> directConnections;

    private Dictionary<Train, Rail> trainLocations;

    private List<Rail> currentlymaintainedRails = new List<Rail>();

    public TrainController trainController;

    public delegate void OnUsedMaintenanceChange(int newVal);
    public event OnUsedMaintenanceChange onChangeActiveCrews;

    public delegate void OnTotalMaintenanceChange(int newVal);
    public event OnTotalMaintenanceChange onChangeTotalCrews;

    public delegate void OnTrainJourneyCompletion(TrainTypes type, TrainCargoData data, int money);
    public event OnTrainJourneyCompletion onTrainJourneyCompletion;

    private int _totalMaintenance = 1;

    public float speedModifier = 0.5f;

    private int _currentUsedMaintenance = 0;

    public int maintenanceTeamsCost = 100;
    public int railHealthCostModifier = 100;
    public int railLengthCostModifier = 100;

    public int TotalMaintenance { get => _totalMaintenance; set {
            if (value != this._totalMaintenance) {
                this._totalMaintenance = value;

                if (onChangeTotalCrews != null) onChangeTotalCrews(_totalMaintenance);
            }
        }
    }
    public int CurrentUsedMaintenance { get => _currentUsedMaintenance; set {
            if (value != this._currentUsedMaintenance) {
                this._currentUsedMaintenance = value;

                if (onChangeActiveCrews != null) onChangeActiveCrews(_currentUsedMaintenance);
            }
        }
    }

    private void Awake() {
        PlayerDataController.onNewDay += onDayChange;
        trainLocations = new Dictionary<Train, Rail>();
    }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(waitUntilSplinesExist());
    }

    IEnumerator waitUntilSplinesExist() {
        Debug.Log("Waiting...");
        yield return new WaitUntil(() => XMLParser.parsingComplete == true);

        connections = XMLParser.connectionMap.Keys.ToArray();
        colliders = gameObject.GetComponentsInChildren<MeshCollider>();
        connectionMap = XMLParser.connectionMap;
        

        directConnections = getDirectConnections(PlayerDataController.playerCity);
        foreach(Rail r in directConnections.Values) {
            r.MaxProgress = getTravelTime(r);
        }

        Debug.Log("Parsing is done");
    }

    private void onDayChange(int day) {
        float cost = 0;
        //For every train out on the track
        foreach (KeyValuePair<Train, Rail> kv in trainLocations) {
            int trainsOntheRoadCost = (kv.Value.maxHealth - kv.Value.Health) * railHealthCostModifier;
            if(kv.Key.Type == TrainTypes.CARGO) {
                PlayerDataController.currentDayFinance.costs.CargoTrains += trainsOntheRoadCost;
            } else {
                PlayerDataController.currentDayFinance.costs.PassengerTrains += trainsOntheRoadCost;
            }
            cost += trainsOntheRoadCost;
        }

        foreach (Rail r in directConnections.Values) {
            r.Health--;
            if(r.Health <= 0) {
                r.Health = 1;
            }
        }

        cost += maintenanceTeamsCost * TotalMaintenance;
        PlayerDataController.currentDayFinance.costs.MaintenanceCrews += maintenanceTeamsCost * TotalMaintenance;

        Debug.LogFormat("Rail costs are {0}", cost);
        PlayerDataController.Money -= (int) cost;
    }

    public int getRouteCost(Connection c) {
        Rail[] rails;

        try {

            rails = new Rail[] { connectionMap[c] };
            //Brittle and will only work with one extra connection, fine for experiment.
        } catch (KeyNotFoundException) {
            int opposingPoint = c.start == PlayerDataController.playerCity ? c.end : c.start;
            Connection c2 = findConnectionByInt(opposingPoint);

            int playerNeighbour = c2.start == opposingPoint ? c2.end : c2.start;
            Connection c3 = findConnectionByInt(playerNeighbour);

            rails = new Rail[] { connectionMap[c3], connectionMap[c2] };
        }

        float cost = 0;
        foreach(Rail r in rails) {
            //Cost dependent on rail length
            cost += Vector2.Distance(r.spline[0].position, r.spline[1].position) * railLengthCostModifier;
        }

        return (int) cost;
    }

    public Rail[] getRailsToLocation(Connection c) {
        Rail[] rails;

        try {

            rails = new Rail[] { connectionMap[c] };
            //Brittle and will only work with one extra connection, fine for experiment.
        } catch (KeyNotFoundException) {
            int opposingPoint = c.start == PlayerDataController.playerCity ? c.end : c.start;
            Connection c2 = findConnectionByInt(opposingPoint);

            int playerNeighbour = c2.start == opposingPoint ? c2.end : c2.start;
            Connection c3 = findConnectionByInt(playerNeighbour);

            rails = new Rail[] { connectionMap[c3], connectionMap[c2] };
        }


        return rails;
    }

    private Dictionary<Connection, Rail> getDirectConnections(int cityIndex) {
        Dictionary<Connection, Rail> dc = new Dictionary<Connection, Rail>();
        foreach (KeyValuePair< Connection, Rail > r in connectionMap) {
            if(r.Key.start == cityIndex || r.Key.end == cityIndex) {
                dc.Add(r.Key, r.Value);
            }
        }

        return dc;
    }

    public float RepairRail(Connection c, int amount, UnityAction callback) {
        if (CurrentUsedMaintenance >= TotalMaintenance) {
            Debug.LogWarning("Can't repair rail. No maintenance crew available");
            return -1;
        }
            

        Rail r = connectionMap[c];
        float time = -1;
        if (r != null) {
            int money = getRouteCost(c)/2;
            PlayerDataController.Money -= money;
            PlayerDataController.currentDayFinance.costs.MaintenanceCrews += money;
            r.Progress = 0.0f;
            //MaxProgress is also time
            StartCoroutine(WaitAndFixRail(r.MaxProgress, r, amount, callback));
            CurrentUsedMaintenance++;
        } else {
            Debug.LogError("Repair could not finish, rail does not exist in dictionary");
        }

        return time;
    }

    private IEnumerator WaitAndFixRail(float waitTime, Rail r, int amount, UnityAction callback) {
        currentlymaintainedRails.Add(r);
        r.hammer.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        r.hammer.SetActive(false);
        r.Health = r.Health + amount;

        if(r.Health > r.maxHealth) {
            r.Health = r.maxHealth;
        }
        CurrentUsedMaintenance--;
        currentlymaintainedRails.Remove(r);
        Analytics.IncrementCounter("rail_fixed");
        callback.Invoke();
    }

    private void Update() {
        foreach(Rail r in currentlymaintainedRails) {
            r.Progress += Time.deltaTime;
        }

        foreach (Train t in trainLocations.Keys) {
            t.Progress -= Time.deltaTime;
        }
    }

    //Finds first best connection that connects to player city.
    private Connection findConnectionByInt(int point) {
        foreach(Connection c in connections) {
            if(c.start == point || c.end == point) {
                return c;
            }
        }
        return null;
    }

    public float getTravelTime(Rail rail) {
        return Vector2.Distance(rail.spline[0].position, rail.spline[1].position) * rail.maxHealth / rail.Health / speedModifier;
    }

    public float getTravelTimeBase(Rail rail) {
        return Vector2.Distance(rail.spline[0].position, rail.spline[1].position) / speedModifier;
    }

    public float getTravelTime(Rail[] rails) {
        float distance = 0;

        foreach(Rail r in rails) {
            distance += Vector2.Distance(r.spline[0].position, r.spline[1].position) * r.maxHealth / r.Health / speedModifier;
        }
        return distance;
    }

    public float getDistance(Vector2 p1, Vector2 p2) {
        return Vector2.Distance(p1, p2);
    }

    public void invokeNewTrain(Connection c, TrainTypes type, TrainCargoData cargo, int profit) {

        Rail[] splines;
        UnityEvent onComplete = new UnityEvent();
        Connection costableConnection = null;

        try {

            splines = new Rail[] { connectionMap[c] };
            costableConnection = c;
            //Brittle and will only work with one extra connection, fine for experiment.
        } catch (KeyNotFoundException) {
            int opposingPoint = c.start == PlayerDataController.playerCity ? c.end : c.start;
            Connection c2 = findConnectionByInt(opposingPoint);

            int playerNeighbour = c2.start == opposingPoint ? c2.end : c2.start;
            Connection c3 = findConnectionByInt(playerNeighbour);

            splines = new Rail[] { connectionMap[c3], connectionMap[c2] };
            costableConnection = c3;
        }

        try {
            Train newTrain = trainController.getTrain(type);
            trainLocations.Add(newTrain, connectionMap[costableConnection]);
            int money = getRouteCost(c);
            PlayerDataController.Money -= money;
            if(type == TrainTypes.CARGO) {
                PlayerDataController.currentDayFinance.costs.CargoTrains += money;
            } else {
                PlayerDataController.currentDayFinance.costs.PassengerTrains += money;
            }
            


            newTrain.setTrainData(cargo.amount, cargo.type, Train.TrainState.ENROUTE, c.end);

            BezierWalkerWithTime walker = newTrain.GetComponent<BezierWalkerWithTime>();
            walker.spline = splines[0].spline;

            onComplete.AddListener(() => {
                Debug.LogFormat("Train trip done. type: {0}, cargo: {1} {2}", type, cargo.type, cargo.amount);
                onComplete.RemoveAllListeners();
                trainController.returnTrain(newTrain);
                walker.NormalizedT = 0;

                //Makes event signaling the train is done.
                onTrainJourneyCompletion(type, cargo, profit);
                PlayerDataController.Money += profit;

                //Analytics
                Analytics.IncrementCounter("deal_completed");

                if (type == TrainTypes.CARGO) {
                    PlayerDataController.currentDayFinance.profit.CargoTrains += profit;
                } else {
                    PlayerDataController.currentDayFinance.profit.PassengerTrains += profit;
                }

                if(trainLocations.ContainsKey(newTrain)) {
                    trainLocations.Remove(newTrain);
                }
            });

            float totalTravelTime = 0;

            if (splines.Length > 1) {
                UnityEvent continueMoving = new UnityEvent();
                totalTravelTime += getTravelTime(splines[0]);
                totalTravelTime += getTravelTime(splines[1]);
                continueMoving.AddListener(() => {

                    //trainLocations.Remove(newTrain);

                    walker.spline = splines[1].spline;
                    walker.travelTime = getTravelTime(splines[1]);
                    walker.NormalizedT = 0;
                    Vector3 diff2 = splines[1].spline[0].position - splines[1].spline[1].position;
                    float rotationZ2 = Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg;
                    newTrain.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ2);
                    continueMoving.RemoveAllListeners();
                    walker.onPathCompleted = onComplete;
                });

                walker.onPathCompleted = continueMoving;
            } else {
                totalTravelTime += getTravelTime(splines[0]);
                walker.onPathCompleted = onComplete;
            }
            newTrain.MaxProgress = totalTravelTime;
            newTrain.Progress = totalTravelTime;

            Vector3 difference = splines[0].spline[0].position - splines[0].spline[1].position;
            walker.travelTime = getTravelTime(splines[0]);
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            newTrain.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        } catch (NullReferenceException e) {
            Debug.LogWarning(e);
        }
        
    }

    private int getClickedRail(Vector3 mousePos) {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo)) {
            GameObject hitObject = hitInfo.transform.gameObject;

            int index = 0;
            foreach (MeshCollider mesh in colliders) {
                if (hitObject.transform == mesh.transform) {
                    return index;
                }
                index++;
            } 
        }

        return -1;
    }
}
