using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupSystem : MonoBehaviour {
    private static PopupSystem current;

    public Popup newRoutePopup;
    public Popup genericConfirmPopup;
    public RouteCreationWindow routeWindow;
    public GenericConfirm genericConfirm;

    //Events don't use Popup class because they can't be clicked away.
    public EventWindow eventWindow;

    // Start is called before the first frame update
    public void Awake() {
        current = this;
    }

    public static void ShowNewRoutePopup(Deal d) {
        current.newRoutePopup.gameObject.SetActive(true);
        current.newRoutePopup.setPopupPosition();
        current.routeWindow.setDealAsActive(d);
    }

    public static void ShowNewRoutePopup(TrainTypes type) {
        current.newRoutePopup.gameObject.SetActive(true);
        current.newRoutePopup.setPopupPosition();
    }

    public static void ShowGenericConfirmPopup(string title, string description, UnityAction callback) {
        current.genericConfirm.submit.onClick.RemoveAllListeners();
        current.genericConfirmPopup.gameObject.SetActive(true);
        current.genericConfirmPopup.setPopupPosition();

        current.genericConfirm.title.text = title;
        current.genericConfirm.description.text = description;
        current.genericConfirm.submit.onClick.AddListener(callback);
        current.genericConfirm.submit.onClick.AddListener(() => current.genericConfirmPopup.gameObject.SetActive(false));
    }

    public static void ShowEventPopup(string title, string description, Sprite sprite, Dictionary<GameEventOption, GameEventEffect[]> options, UnityAction<GameEventOption> callback) {
        current.eventWindow.gameObject.SetActive(true);
        current.eventWindow.SetContent(title, description, sprite);
        current.eventWindow.SetButtons(options.Keys, (i) => callback(i));
    }

    public static void HideEventPopup() {
        current.eventWindow.gameObject.SetActive(false);
        TooltipSystem.Hide();
    }

    public static void HideGenericConfirmPopup() {
        current.genericConfirmPopup.gameObject.SetActive(false);
        TooltipSystem.Hide();
    }

    public static void HideNewRoutePopup() {
        current.newRoutePopup.gameObject.SetActive(false);
        TooltipSystem.Hide();
    }

}
