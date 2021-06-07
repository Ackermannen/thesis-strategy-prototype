using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataController : MonoBehaviour
{
    private static PlayerDataController current;

    public static PlayerDataController Instance { get { return current; } }

    public static PlayerInteractionState playerState;
    public static int playerCity { get; set; }

    public static int speed = 1000;

    public static DateTime date = new DateTime();
    public static DateTime timeExact = new DateTime();

    public static DailyFinance currentDayFinance;

    private static int _money = 40000;

    public static bool mouseOnNonIntrusive = false;

    public static Dictionary<string, int> moneyOverTime = new Dictionary<string, int>();

    public delegate void OnMoneyChange(int money);
    public static event OnMoneyChange onMoneyChange;
    public static int Money { get => _money; set {
            if (value != _money) {
                _money = value;
                if (onMoneyChange != null) onMoneyChange(_money);
            }
        }
    }

    private static int _dayCount = 1;
    public static int DayCount { get => _dayCount; set {
            if (value != _dayCount) {
                _dayCount = value;
                if (onNewDay != null) onNewDay(_dayCount);
            }
        }
    }

    public delegate void OnNewDay(int day);
    public static event OnNewDay onNewDay;

    public void Awake() {
        current = this;
        currentDayFinance = new DailyFinance();
    }

    private void Start() {
        onMoneyChange += OnMoneyFluctuate;
    }

    private void OnMoneyFluctuate(int money) {
        if (moneyOverTime.ContainsKey(timeExact.ToString("mm.ss"))) {
            moneyOverTime[timeExact.ToString("mm.ss")] = money;
        } else {
            moneyOverTime.Add(timeExact.ToString("mm.ss"), money);
        }
    }

    private void Update() {
        date = date.AddSeconds(Time.deltaTime * speed);
        timeExact = timeExact.AddSeconds(Time.unscaledDeltaTime);
        prepareNextDay(date.Day);
    }

    private void prepareNextDay(int day) {
        if(DayCount != day) {
            currentDayFinance = new DailyFinance();
            Analytics.IncrementCounter("day");
            DayCount = day;
        }
    }
    public static string JSONify() {
        return $"money: {Money}";
    }

}
