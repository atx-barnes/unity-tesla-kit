using System;

namespace Tesla.API.Core {

    [Serializable]
    public class ClimateState : State {

        public ClimateStateResponse Response { get; set; }

        [Serializable]
        public class ClimateStateResponse {

            public bool battery_heater { get; set; }
            public object battery_heater_no_power { get; set; }
            public string climate_keeper_mode { get; set; }
            public int defrost_mode { get; set; }
            public double driver_temp_setting { get; set; }
            public int fan_status { get; set; }
            public double inside_temp { get; set; }
            public bool is_auto_conditioning_on { get; set; }
            public bool is_climate_on { get; set; }
            public bool is_front_defroster_on { get; set; }
            public bool is_preconditioning { get; set; }
            public bool is_rear_defroster_on { get; set; }
            public int left_temp_direction { get; set; }
            public double max_avail_temp { get; set; }
            public double min_avail_temp { get; set; }
            public double outside_temp { get; set; }
            public double passenger_temp_setting { get; set; }
            public bool remote_heater_control_enabled { get; set; }
            public int right_temp_direction { get; set; }
            public int seat_heater_left { get; set; }
            public int seat_heater_right { get; set; }
            public bool side_mirror_heaters { get; set; }
            public long timestamp { get; set; }
            public bool wiper_blade_heater { get; set; }
        }
    }
}
