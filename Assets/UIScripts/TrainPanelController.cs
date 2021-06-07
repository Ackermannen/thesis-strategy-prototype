using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainPanelController : MonoBehaviour {

    public TrainPanelData trainPanelPrefab;

    public TrainController controller;

    public GameObject contentWindow;

    public GameObject lastPanel;

    public TotalUsed totalUsedCargo;

    public TotalUsed totalUsedPassenger;

    private Dictionary<Train, TrainPanelData> keyValuePairs;

    public Button addMoreCargo;
    public Button addMorePassenger;

    // Start is called before the first frame update
    void Start()
    {
        

        makeListOfTrains();

        totalUsedCargo.total.text = controller.cargoTrains.Count + " Total";
        totalUsedPassenger.total.text = controller.passengerTrains.Count + " Total";

        controller.onUsedCargoChange += (i) => onAvailableTrainChange(totalUsedCargo, i);
        controller.onUsedPassengerChange += (i) => onAvailableTrainChange(totalUsedPassenger, i);

        controller.cargoTrains.CollectionChanged += totalCargoChange;
        controller.passengerTrains.CollectionChanged += totalPassengerChange;
    }

    private void makeListOfTrains() {
        keyValuePairs = new Dictionary<Train, TrainPanelData>();

        foreach (Transform child in contentWindow.transform) {
            Destroy(child.gameObject);
        }

        foreach (Train t in controller.passengerTrains) {

            TrainPanelData trainData = Instantiate(trainPanelPrefab, contentWindow.transform);
            trainData.targetTrain = t;
            trainData.slider.bar.maxValue = t.MaxProgress;
            onPropsChange(t, trainData);

            //On future change
            t.PropertyChanged += (sender, e) => onPropsChange((Train)sender, trainData);

            keyValuePairs.Add(t, trainData);
        }

        foreach (Train t in controller.cargoTrains) {

            TrainPanelData trainData = Instantiate(trainPanelPrefab, contentWindow.transform);
            trainData.targetTrain = t;
            trainData.slider.bar.maxValue = t.Progress;
            onPropsChange(t, trainData);

            t.PropertyChanged += (sender, e) => onPropsChange((Train)sender, trainData);

            keyValuePairs.Add(t, trainData);
        }

        //Make final entry with adding buttons
        GameObject g = Instantiate(lastPanel, contentWindow.transform);
        g.GetComponentInChildren<CargoUpgradeWindow>().controller = controller;
        g.GetComponentInChildren<PassengerUpgradeWindow>().controller = controller;

        if(controller.passengerTrains.Count >= 6)
            g.GetComponentInChildren<PassengerUpgradeWindow>().GetComponent<Button>().interactable = false;

        if(controller.cargoTrains.Count >= 6)
            g.GetComponentInChildren<CargoUpgradeWindow>().GetComponent<Button>().interactable = false;
    }

    private TrainPanelData getTrainData(Train t) {
        return keyValuePairs[t];
    }

    private void totalPassengerChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        if(controller.passengerTrains.Count >= 6) {
            addMorePassenger.interactable = false;
            contentWindow.GetComponentInChildren<PassengerUpgradeWindow>().GetComponent<Button>().interactable = false;
        }
        makeListOfTrains();
        totalUsedPassenger.total.text = controller.passengerTrains.Count + " Total";
    }

    private void totalCargoChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        if (controller.cargoTrains.Count >= 6) {
            addMoreCargo.interactable = false;
            contentWindow.GetComponentInChildren<CargoUpgradeWindow>().GetComponent<Button>().interactable = false;
        }
        makeListOfTrains();
        totalUsedCargo.total.text = controller.cargoTrains.Count + " Total";
    }

    private void onAvailableTrainChange(TotalUsed totalUsed, int usedTrains) {
        totalUsed.used.text = usedTrains + " Used";
    }

    private void onPropsChange(Train train, TrainPanelData data) {
        

        data.routeTo.text = train.CurrentDestination != -1 ? XMLParser.cityNameMap[train.CurrentDestination] : "No destination";
        data.unitSize.text = train.Cargo.ToString();
        data.trainType.text = train.Type == TrainTypes.CARGO ? "Cargo train" : "Passenger train";
        data.slider.displayText.text = train.State == Train.TrainState.DISABLED ? "Unavailable" : train.State == Train.TrainState.IDLE ? "Waiting..." : "En route";
        data.slider.bar.maxValue = train.MaxProgress;
        data.slider.bar.value = train.Progress;
    }
}
