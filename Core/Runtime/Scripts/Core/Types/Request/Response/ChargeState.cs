using System;

namespace Tesla.API.Core {

    [Serializable]
    public class ChargeState : State {

        public ChargeStateResponse Response { get; set; }

        [Serializable]
        public class ChargeStateResponse {

            public bool battery_heater_on { get; set; }
            public int battery_level { get; set; }
            public double battery_range { get; set; }
            public int charge_current_request { get; set; }
            public int charge_current_request_max { get; set; }
            public bool charge_enable_request { get; set; }
            public double charge_energy_added { get; set; }
            public int charge_limit_soc { get; set; }
            public int charge_limit_soc_max { get; set; }
            public int charge_limit_soc_min { get; set; }
            public int charge_limit_soc_std { get; set; }
            public double charge_miles_added_ideal { get; set; }
            public double charge_miles_added_rated { get; set; }
            public bool charge_port_cold_weather_mode { get; set; }
            public bool charge_port_door_open { get; set; }
            public string charge_port_latch { get; set; }
            public double charge_rate { get; set; }
            public bool charge_to_max_range { get; set; }
            public int charger_actual_current { get; set; }
            public object charger_phases { get; set; }
            public int charger_pilot_current { get; set; }
            public int charger_power { get; set; }
            public int charger_voltage { get; set; }
            public string charging_state { get; set; }
            public string conn_charge_cable { get; set; }
            public double est_battery_range { get; set; }
            public string fast_charger_brand { get; set; }
            public bool fast_charger_present { get; set; }
            public string fast_charger_type { get; set; }
            public double ideal_battery_range { get; set; }
            public bool managed_charging_active { get; set; }
            public object managed_charging_start_time { get; set; }
            public bool managed_charging_user_canceled { get; set; }
            public int max_range_charge_counter { get; set; }
            public int minutes_to_full_charge { get; set; }
            public object not_enough_power_to_heat { get; set; }
            public bool scheduled_charging_pending { get; set; }
            public object scheduled_charging_start_time { get; set; }
            public double time_to_full_charge { get; set; }
            public long timestamp { get; set; }
            public bool trip_charging { get; set; }
            public int usable_battery_level { get; set; }
            public object user_charge_enable_request { get; set; }
        }
    }
}
