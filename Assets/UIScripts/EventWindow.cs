using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class EventWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private GameObject buttonLocation;
    [SerializeField] private Image image;

    public void SetContent(string title, string description, Sprite sprite) {
        this.title.text = title;
        this.description.text = description;
        image.sprite = sprite;
    }

    internal void SetButtons(Dictionary<GameEventOption, GameEventEffect[]>.KeyCollection keys, Action<GameEventOption> p) {

        //Clear before countinuing
        foreach (Transform child in buttonLocation.transform) {
            Destroy(child.gameObject);
        }

        foreach (GameEventOption data in keys) {
            Button b = Instantiate(buttonPrefab, buttonLocation.transform);
            b.GetComponentInChildren<TextMeshProUGUI>().text = data.flavorText;
            b.GetComponentInChildren<Tooltippable>().content = data.tooltipText;
            b.onClick.AddListener(() => p(data));
        }
    }


}
