using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class VehicleInfoResponseFields : MonoBehaviour {

    public Fields VehicleInfoFields;

    [Serializable]
    public class Fields {

        public TextMeshProUGUI TMP_id;
        public TextMeshProUGUI TMP_vehicle_id;
        public TextMeshProUGUI TMP_vin;
        public TextMeshProUGUI TMP_display_name;
        public TextMeshProUGUI TMP_option_codes;
        public TextMeshProUGUI TMP_color;
        public TextMeshProUGUI[] TMP_tokens;
        public TextMeshProUGUI TMP_state;
        public TextMeshProUGUI TMP_in_service;
        public TextMeshProUGUI TMP_id_s;
        public TextMeshProUGUI TMP_calendar_enabled;
        public TextMeshProUGUI TMP_api_version;
        public TextMeshProUGUI TMP_backseat_token;
        public TextMeshProUGUI TMP_backseat_token_updated_at;
    }
}


