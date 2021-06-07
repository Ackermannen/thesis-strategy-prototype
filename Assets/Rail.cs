﻿using BezierSolution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
    public BezierSpline spline;

    private int _health;

    public int maxHealth;


    public delegate void OnHealthChange(int newVal);
    public event OnHealthChange onHealthChange;
    public int Health { get => _health; set {
            if (value != this._health) {
                this._health = value;

                if (onHealthChange != null) onHealthChange(_health);
            }
        }
    }

    public float MaxProgress;

    private float _progress;
    public delegate void OnProgressChange(float newVal);
    public event OnProgressChange onProgressChange;
    public float Progress {
        get => _progress; set {
            if (value != this._progress) {
                this._progress = value;

                if (onProgressChange != null) onProgressChange(_progress);
            }
        }
    }
}
