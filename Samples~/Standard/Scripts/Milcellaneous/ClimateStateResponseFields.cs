using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class ClimateStateResponseFields : MonoBehaviour
{
    public Fields ClimateStateFields;

    [Serializable]
    public class Fields {

        public TextMeshProUGUI TMP_battery_heater;
        public TextMeshProUGUI TMP_battery_heater_no_power;
        public TextMeshProUGUI TMP_climate_keeper_mode;
        public TextMeshProUGUI TMP_defrost_mode;
        public TextMeshProUGUI TMP_driver_temp_setting;
        public TextMeshProUGUI TMP_fan_status;
        public TextMeshProUGUI TMP_inside_temp;
        public TextMeshProUGUI TMP_is_auto_conditioning_on;
        public TextMeshProUGUI TMP_is_climate_on;
        public TextMeshProUGUI TMP_is_front_defroster_on;
        public TextMeshProUGUI TMP_is_preconditioning;
        public TextMeshProUGUI TMP_is_rear_defroster_on;
        public TextMeshProUGUI TMP_left_temp_direction;
        public TextMeshProUGUI TMP_max_avail_temp;
        public TextMeshProUGUI TMP_min_avail_temp;
        public TextMeshProUGUI TMP_outside_temp;
        public TextMeshProUGUI TMP_passenger_temp_setting;
        public TextMeshProUGUI TMP_remote_heater_control_enabled;
        public TextMeshProUGUI TMP_right_temp_direction;
        public TextMeshProUGUI TMP_seat_heater_left;
        public TextMeshProUGUI TMP_seat_heater_right;
        public TextMeshProUGUI TMP_side_mirror_heaters;
        public TextMeshProUGUI TMP_timestamp;
        public TextMeshProUGUI TMP_wiper_blade_heater;
    }
}


