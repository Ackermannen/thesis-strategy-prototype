using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaintenancePanelController : MonoBehaviour
{


    public TotalUsed totalUsed;

    public GameObject routePrefab;

    public RailController controller;

    public GameObject contentWindow;

    public Button addMoreButton;

    // Start is called before the first frame update
    void Start()
    {
        foreach (KeyValuePair<Connection, Rail> kv in controller.directConnections) {
            DirectRoute route = Instantiate(routePrefab, contentWindow.transform).GetComponent<DirectRoute>();

            route.to.text = XMLParser.cityNameMap[kv.Key.end];
            route.bar.bar.maxValue = kv.Value.MaxProgress;
            route.isBeingRepaired = false;

            Rail rail = controller.connectionMap[kv.Key];

            route.health.text = rail.Health + "/" + rail.maxHealth;
            rail.onHealthChange += (i) => onHealthChange(i, rail, route);

            //Sets the buttons to disabled at first
            onHealthChange(rail.Health, rail, route);

            rail.onProgressChange += (i) => onProgressChange(i, route);

            int endCity = kv.Key.end;
            route.newRoute.onClick.AddListener(() => {
                if(kv.Value.Health < kv.Value.maxHealth) {
                    PopupSystem.ShowGenericConfirmPopup("Rail reparation", "Repairing this railroad will cost <color=red>" +
                        (controller.getRouteCost(kv.Key)/2).ToString("N0") +
                        "</color> kr and will fix your rail.", () => onConfirmRepairRail(kv.Key, route));
                }
            });

            route.setToolTipMoney(controller.getRouteCost(kv.Key)/2, 4);
        }


        onTotalCrewsChange(controller.TotalMaintenance);
        onCurrentActiveCrewsChange(controller.CurrentUsedMaintenance);
        controller.onChangeTotalCrews += onTotalCrewsChange;
        controller.onChangeActiveCrews += onCurrentActiveCrewsChange;
        checkIfButtonsShouldBeOn();
    }

    private void onConfirmRepairRail(Connection c, DirectRoute route) {
        float i = controller.RepairRail(c, 15, () => {
            route.bar.bar.value = 0;
            route.isBeingRepaired = false;
            checkIfButtonsShouldBeOn();
        });
        route.isBeingRepaired = true;
        checkIfButtonsShouldBeOn();

    }

    private void onCurrentActiveCrewsChange(int newVal) {
        totalUsed.used.text = newVal + " Used";

        checkIfButtonsShouldBeOn();
    }

    private void checkIfButtonsShouldBeOn() {
        if (controller.CurrentUsedMaintenance >= controller.TotalMaintenance) {
            foreach (DirectRoute route in contentWindow.GetComponentsInChildren<DirectRoute>()) {
                route.newRoute.interactable = false;
            }
        } else {
            foreach (DirectRoute route in contentWindow.GetComponentsInChildren<DirectRoute>()) {
                string[] hp = route.health.text.Split('/');
                if (!route.isBeingRepaired && int.Parse(hp[0]) < int.Parse(hp[1])) {
                    route.newRoute.interactable = true;
                } else {
                    route.newRoute.interactable = false;
                }
            }
        }
    }

    private void onTotalCrewsChange(int newVal) {
        totalUsed.total.text = newVal + " Total";
        if(newVal >= controller.directConnections.Count) {
            addMoreButton.interactable = false;
        }
        checkIfButtonsShouldBeOn();
    }

    private void onProgressChange(float i, DirectRoute route) {
        route.bar.bar.value = i;
        if(route.bar.bar.value >= route.bar.bar.maxValue) {
            route.bar.bar.value = 0;
            route.isBeingRepaired = false;
        }
    }

    private void onHealthChange(int newVal, Rail rail, DirectRoute route) {
        route.health.text = newVal + "/" + rail.maxHealth;

        checkIfButtonsShouldBeOn();
    }
}
