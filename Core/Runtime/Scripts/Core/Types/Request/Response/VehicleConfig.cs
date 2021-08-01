using System;

namespace Tesla.API.Core {

    [Serializable]
    public class VehicleConfig : State {

        public VehicleConfigResponse Response { get; set; }

        [Serializable]
        public class VehicleConfigResponse {

            public bool can_accept_navigation_requests { get; set; }
            public bool can_actuate_trunks { get; set; }
            public string car_special_type { get; set; }
            public string car_type { get; set; }
            public string charge_port_type { get; set; }
            public bool ece_restrictions { get; set; }
            public bool eu_vehicle { get; set; }
            public string exterior_color { get; set; }
            public bool has_air_suspension { get; set; }
            public bool has_ludicrous_mode { get; set; }
            public bool motorized_charge_port { get; set; }
            public bool plg { get; set; }
            public int rear_seat_heaters { get; set; }
            public int rear_seat_type { get; set; }
            public bool rhd { get; set; }
            public string roof_color { get; set; }
            public int seat_type { get; set; }
            public string spoiler_type { get; set; }
            public int sun_roof_installed { get; set; }
            public string third_row_seats { get; set; }
            public long timestamp { get; set; }
            public string trim_badging { get; set; }
            public bool use_range_badging { get; set; }
            public string wheel_type { get; set; }
        }
    }
}
