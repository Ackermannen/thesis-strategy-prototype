using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{

    [Serializable]
    public class TimeControllerColors {
        
        public Color dark;
        public Color green;
        public Color red;
        public Color darkRed;
    }

    [SerializeField] private Button[] buttons;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Image pauseButtonImage;

    [SerializeField] private Sprite pauseImg;
    [SerializeField] private Sprite playImg;
    [SerializeField] private TextMeshProUGUI currentDate;

    public float multiplier = 2F;

    public TimeControllerColors colors;
    public UnityEvent OnPause;

    private bool _paused = false;
    private bool forcePaused = false;

    public int activeIndex { get; private set; }
    public bool Paused { get => _paused; set {
            if (value != this._paused) {
                this._paused = value;

                if (OnPause != null) OnPause.Invoke();
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {

        int bIndex = 0;

        foreach(Button b in buttons) {
            int tempint = bIndex;
            b.onClick.AddListener(() => handleTimePress(tempint));
            bIndex++;
        }

        pauseButton.onClick.AddListener(handlePause);

        //Start game with timescale 1 but paused;
        handleTimePress(0);
        handlePause();
    }

    public void handleTimePress(int index) {
        if(!forcePaused) {
            if (Paused) {
                Paused = false;
            }

            Time.timeScale = 1F + index * multiplier;
            for (int i = 0; i < buttons.Length; i++) {

                if (i <= index) {
                    buttons[i].colors = getColorAsBlock(colors.green);
                } else {
                    buttons[i].colors = getColorAsBlock(colors.dark);
                }
            }

            activeIndex = index;

        }
    }

    public void handlePause() {
        if(!forcePaused) {
            if (Paused) {
                handleTimePress(activeIndex);
                pauseButtonImage.sprite = playImg;
                Paused = false;
            } else {
                Paused = true;
                pauseButtonImage.sprite = pauseImg;
                for (int i = 0; i < activeIndex + 1; i++) {
                    buttons[i].colors = getColorAsBlock(colors.red);
                }

                Time.timeScale = 0;
            }
        }
    }

    //Forces a pause state that can't be removed until forceResume() has been called.
    public void forcePause() {
            forcePaused = true;
            pauseButtonImage.sprite = pauseImg;
            for (int i = 0; i < activeIndex + 1; i++) {
                buttons[i].colors = getColorAsBlock(colors.darkRed);
            }
            Time.timeScale = 0;
    }

    public void forceResume() {
        forcePaused = false;
        handleTimePress(activeIndex);
        pauseButtonImage.sprite = playImg;
    }

    private void Update() {

        int bIndex = 0;

        foreach (Button b in buttons) {
            
            if(Input.GetKeyDown(bIndex + 1 + "")) {
                handleTimePress(bIndex);
            }
            bIndex++;
        }

        if (Input.GetKeyDown("space")) {
            handlePause();
        }

        if (Paused || forcePaused) {
            Analytics.IncrementCounter("time_spent_paused", Time.unscaledDeltaTime);
        } else {
            Analytics.IncrementCounter("time_spent_" + buttons[activeIndex].name, Time.unscaledDeltaTime);
        }

        UpdateDate();
    }

    private void UpdateDate() {
        currentDate.text = "Day: " + PlayerDataController.date.ToString("d HH:mm");
    }

    private ColorBlock getColorAsBlock(Color color) {
        ColorBlock b = new ColorBlock();

        b.normalColor = color;
        b.highlightedColor = color;
        b.pressedColor = color;
        b.selectedColor = color;
        b.colorMultiplier = 1;

        return b;
    }

}
