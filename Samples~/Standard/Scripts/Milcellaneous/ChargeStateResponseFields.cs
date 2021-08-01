using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class ChargeStateResponseFields : MonoBehaviour {

    public Fields ChargeStateFields;

    [Serializable]
    public class Fields {

        public TextMeshProUGUI TMP_battery_heater_on;
        public TextMeshProUGUI TMP_battery_level;
        public TextMeshProUGUI TMP_battery_range;
        public TextMeshProUGUI TMP_charge_current_request;
        public TextMeshProUGUI TMP_charge_current_request_max;
        public TextMeshProUGUI TMP_charge_enable_request;
        public TextMeshProUGUI TMP_charge_energy_added;
        public TextMeshProUGUI TMP_charge_limit_soc;
        public TextMeshProUGUI TMP_charge_limit_soc_max;
        public TextMeshProUGUI TMP_charge_limit_soc_min;
        public TextMeshProUGUI TMP_charge_limit_soc_std;
        public TextMeshProUGUI TMP_charge_miles_added_ideal;
        public TextMeshProUGUI TMP_charge_miles_added_rated;
        public TextMeshProUGUI TMP_charge_port_cold_weather_mode;
        public TextMeshProUGUI TMP_charge_port_door_open;
        public TextMeshProUGUI TMP_charge_port_latch;
        public TextMeshProUGUI TMP_charge_rate;
        public TextMeshProUGUI TMP_charge_to_max_range;
        public TextMeshProUGUI TMP_charger_actual_current;
        public TextMeshProUGUI TMP_charger_phases;
        public TextMeshProUGUI TMP_charger_pilot_current;
        public TextMeshProUGUI TMP_charger_power;
        public TextMeshProUGUI TMP_charger_voltage;
        public TextMeshProUGUI TMP_charging_state;
        public TextMeshProUGUI TMP_conn_charge_cable;
        public TextMeshProUGUI TMP_est_battery_range;
        public TextMeshProUGUI TMP_fast_charger_brand;
        public TextMeshProUGUI TMP_fast_charger_present;
        public TextMeshProUGUI TMP_fast_charger_type;
        public TextMeshProUGUI TMP_ideal_battery_range;
        public TextMeshProUGUI TMP_managed_charging_active;
        public TextMeshProUGUI TMP_managed_charging_start_time;
        public TextMeshProUGUI TMP_managed_charging_user_canceled;
        public TextMeshProUGUI TMP_max_range_charge_counter;
        public TextMeshProUGUI TMP_minutes_to_full_charge;
        public TextMeshProUGUI TMP_not_enough_power_to_heat;
        public TextMeshProUGUI TMP_scheduled_charging_pending;
        public TextMeshProUGUI TMP_scheduled_charging_start_time;
        public TextMeshProUGUI TMP_time_to_full_charge;
        public TextMeshProUGUI TMP_timestamp;
        public TextMeshProUGUI TMP_trip_charging;
        public TextMeshProUGUI TMP_usable_battery_level;
        public TextMeshProUGUI TMP_user_charge_enable_request;
    }
}



