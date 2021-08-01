using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class DriveStateResponseFields : MonoBehaviour {

    public Fields DriveStateFields;

    [Serializable]
    public class Fields {

        public TextMeshProUGUI TMP_gps_as_of;
        public TextMeshProUGUI TMP_heading;
        public TextMeshProUGUI TMP_latitude;
        public TextMeshProUGUI TMP_longitude;
        public TextMeshProUGUI TMP_native_latitude;
        public TextMeshProUGUI TMP_native_location_supported;
        public TextMeshProUGUI TMP_native_longitude;
        public TextMeshProUGUI TMP_native_type;
        public TextMeshProUGUI TMP_power;
        public TextMeshProUGUI TMP_shift_state;
        public TextMeshProUGUI TMP_speed;
        public TextMeshProUGUI TMP_timestamp;
    }
}

