using UnityEngine;

public class IntegratedTrainController : MonoBehaviour {
    public Train train;

    public GameObject trainPanelPrefab;

    private TrainPanelData trainPanel;

    void Start() {
        train = GetComponent<Train>();
        trainPanel = Instantiate(trainPanelPrefab, WorldspaceCanvas.Instance.trainsLayer.transform).GetComponent<TrainPanelData>();
        RectTransform crt = trainPanel.GetComponent<RectTransform>();

        //Sets pivot to bottom middle
        crt.pivot = new Vector2(0f, 0f);
        crt.position += new Vector3(0, -100, -1);
        crt.transform.localScale = new Vector3(1, 1, 1);
        crt.eulerAngles = new Vector3(-10, 0, 0);

        trainPanel.transform.position = transform.position - new Vector3(0, 0, 1);
        train.PropertyChanged += (a,b) => onPropsChange(train, trainPanel);
    }

    private void onPropsChange(Train train, TrainPanelData data) {
        data.routeTo.text = train.CurrentDestination != -1 ? XMLParser.cityNameMap[train.CurrentDestination] : "No destination";
        data.unitSize.text = train.Cargo.ToString();
        data.trainType.text = train.Type == TrainTypes.CARGO ? "Cargo train" : "Passenger train";
    }

    void Update() {
        trainPanel.transform.position = transform.position - new Vector3(0,0,1);
    }

    public void updatePanel(string to, string type, string goods) {
        trainPanel.routeTo.text = to;
        trainPanel.trainType.text = type;
        trainPanel.unitSize.text = goods;
    }

    private void OnDisable() {
        if (trainPanel != null) {
            trainPanel.gameObject.SetActive(false);
        }
    }

    private void OnEnable() {
        if(trainPanel != null) {
            trainPanel.gameObject.SetActive(true);
        }
        
    }
}
