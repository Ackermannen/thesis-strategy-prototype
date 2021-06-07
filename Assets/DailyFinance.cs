using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DailyFinance
{

    public class Costs {
        private int _maintenance = 0;
        private int _cargoTrains = 0;
        private int _passengerTrains = 0;

        public UnityEvent onCostChange = new UnityEvent();

        public int MaintenanceCrews { get => _maintenance; set {
                if (value != this._maintenance) {
                    this._maintenance = value;
                    onCostChange.Invoke();
                }
            }
        }
        public int CargoTrains { get => _cargoTrains; set {
                if (value != this._cargoTrains) {
                    this._cargoTrains = value;
                    onCostChange.Invoke();
                }
            }
        }
        public int PassengerTrains { get => _passengerTrains; set {
                if (value != this._passengerTrains) {
                    this._passengerTrains = value;
                    onCostChange.Invoke();
                }
            }
        }
    }
    public Costs costs;

    public class Profit {
        private int _cargoTrains = 0;
        private int _passengerTrains = 0;

        public UnityEvent onProfitChange = new UnityEvent();

        public int CargoTrains { get => _cargoTrains; set {
                if (value != this._cargoTrains) {
                    this._cargoTrains = value;
                    onProfitChange.Invoke();
                }
            }
        }
        public int PassengerTrains { get => _passengerTrains; set {
                if (value != this._passengerTrains) {
                    this._passengerTrains = value;
                    onProfitChange.Invoke();
                }
            }
        }
    }

    public Profit profit;

    public DailyFinance() {
        costs = new Costs();
        profit = new Profit();
    }
}
