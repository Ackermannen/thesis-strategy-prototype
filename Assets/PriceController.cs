using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PriceController : MonoBehaviour
{

    //Prices labels
    public TextMeshProUGUI grainPrice;
    public TextMeshProUGUI ironPrice;
    public TextMeshProUGUI woodPrice;
    public TextMeshProUGUI peoplePrice;

    public ResourceController controller;

    public Sprite grainIcon;
    public Sprite ironIcon;
    public Sprite woodIcon;
    public Sprite peopleIcon;


    // Start is called before the first frame update
    void Start()
    {
        grainPrice.text = controller.GrainPrice + ":-";
        ironPrice.text = controller.IronPrice + ":-";
        woodPrice.text = controller.WoodPrice + ":-";
        peoplePrice.text = controller.PeoplePrice + ":-";
        controller.onPriceChange += UpdatePrices;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdatePrices(int price, TrainCargoTypes type) {
        switch (type) {
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
