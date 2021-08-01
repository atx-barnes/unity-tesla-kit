using System;

namespace Tesla.API.Core {

    [Serializable]
    public class GUISettings : State {

        public GUISettingsResponse Response { get; set; }

        [Serializable]
        public class GUISettingsResponse {

            public bool gui_24_hour_time { get; set; }
            public string gui_charge_rate_units { get; set; }
            public string gui_distance_units { get; set; }
            public string gui_range_display { get; set; }
            public string gui_temperature_units { get; set; }
            public bool show_range_units { get; set; }
            public long timestamp { get; set; }
        }
    }
}
