using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Tesla.API.Core {

    /// <summary>
    /// Vehicle class that holds state and command endpoints. For new endpoints add extension method(s) to this class.
    /// </summary>
    public class Vehicle {

        #region Properties

        /// <summary>
        /// Vehicle id used for all vehicle state/command requests not to be confused with vehicle_id which is different.
        /// </summary>
        public long id;

        public long Id { get => id; private set => id = value; }

        /// <summary>
        /// If the vehicle is currently being woken up.
        /// </summary>
        private bool isWaking = false;

        public bool IsWaking { get => isWaking; private set => isWaking = value; }

        /// <summary>
        /// Action delegate invoked when vehicle have been awaken.
        /// </summary>
        public Action OnVehicleAwaken { get; set; }

        /// <summary>
        /// Vehicle reponse information.
        /// </summary>
        private VehiclesInfo.VehiclesInfoResponse vehicleInfo;

        public VehiclesInfo.VehiclesInfoResponse VehicleInfo { get => vehicleInfo; set {

                vehicleInfo = value;

                Id = vehicleInfo.id;
            }
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Vehicle constructor
        /// </summary>
        /// <param name="vehicleInfo"></param>
        public Vehicle(VehiclesInfo.VehiclesInfoResponse vehicleInfo) {

            VehicleInfo = vehicleInfo;
        }

        /// <summary>
        /// Vehicle request method for all vehicle api endpoint requests. Will check if vehicle is awake before making request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public async Task<T> RequestAsync<T>(string endpoint) {

            await CheckVehicleStatusForRequestAsync();

            return await TeslaClient.Instance.ClientRequestAsync<T>(endpoint);
        }

        /// <summary>
        /// Checks if the vehicle is awake before making a vehicle request.
        /// </summary>
        /// <returns></returns>
        private async Task CheckVehicleStatusForRequestAsync() {

            VehicleInfo info = await TeslaClient.Instance.ClientRequestAsync<VehicleInfo>($"/api/1/vehicles/{Id}");

            switch (info.Response.state) {

                case "asleep":

                    await WakeVehicleAsnyc();

                    break;

                case "online":

                    if (TeslaClient.Instance.DebugLevel == TeslaClient.LogLevel.Vehicles)

                        Debug.Log($"Vehicle {info.Response.display_name} State: {info.Response.state}");

                    break;
            }
        }

        /// <summary>
        /// Wake vehicle async will wait until the vehicle is awake or timeout is reached.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task WakeVehicleAsnyc() {

            if (!IsWaking) {

                IsWaking = true;

                if (TeslaClient.Instance.DebugLevel == TeslaClient.LogLevel.Client) Debug.Log($"Waking up vehicle...");

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

                await TeslaClient.Instance.ClientRequestAsync<Wake>($"/api/1/vehicles/{Id}/wake_up");

                sw.Start();

                while (IsWaking) {

                    if (sw.Elapsed.Seconds < TeslaClient.Instance.VehicleWakeTimeout) {

                        VehicleInfo info = await TeslaClient.Instance.ClientRequestAsync<VehicleInfo>($"/api/1/vehicles/{Id}");

                        if (info.Response.state == "online") {

                            OnVehicleAwaken?.Invoke();

                            if (TeslaClient.Instance.DebugLevel == TeslaClient.LogLevel.Client) Debug.Log("Vehicle is now ONLINE and can receive requests");

                            IsWaking = false;

                            break;
                        }

                    } else if (sw.Elapsed.Seconds >= TeslaClient.Instance.VehicleWakeTimeout) {

                        Debug.LogWarning("Timeout reached to wake vehicle");

                        IsWaking = false;

                        throw new TeslaException("Failed to Wake Vehicle: Timeout reached to wake vehicle");
                    }
                }

            } else {

                Debug.LogWarning("Vehicle is still waking up please wait.");
            }
        }

        /// <summary>
        /// Utility method for checking if vehicle is awake. This method does NOT need to be used to make vehicle requests or to see if a request can be made
        /// to a vehicle, that is already handled by the RequestAsync() in Vehicle which automatically checks to see if vehicle
        /// is awake to make requests and if it not it will try and awaken it.
        /// </summary>
        /// <returns></returns>
        public async Task IsVehicleAwake(Action<bool> onCheckComplete) {

            VehicleInfo info = await TeslaClient.Instance.ClientRequestAsync<VehicleInfo>($"/api/1/vehicles/{Id}");

            switch (info.Response.state) {

                case "online": onCheckComplete.Invoke(true);

                    break;

                case "asleep": onCheckComplete.Invoke(false);

                    break;
            }
        }

        #endregion

        #region Vehicle API Endpoints

        #region State Endpoints

        /// <summary>
        /// Returns the climate state of the vehicle.
        /// </summary>
        public async Task<ClimateState> GetClimateStateAsync() {

            return await RequestAsync<ClimateState>($"/api/1/vehicles/{id}/data_request/climate_state");
        }

        /// <summary>
        /// Returns the charge state of the vehicle.
        /// </summary>
        public async Task<ChargeState> GetChargeStateAsync() {

            return await RequestAsync<ChargeState>($"/api/1/vehicles/{id}/data_request/charge_state");
        }

        /// <summary>
        /// Returns the driving and position state of the vehicle.
        /// </summary>
        public async Task<DriveState> GetDriveStateAsync() {

            return await RequestAsync<DriveState>($"/api/1/vehicles/{id}/data_request/drive_state");
        }

        /// <summary>
        /// Returns vehicle information.
        /// </summary>
        public async Task<VehicleInfo> GetVehicleInfoAsync() {

            return await RequestAsync<VehicleInfo>($"/api/1/vehicles/{id}");
        }

        /// <summary>
        /// Returns various information about the GUI settings of the car, such as unit format and range display.
        /// </summary>
        /// <returns></returns>
        public async Task<GUISettings> GetGUISettingsAsync() {

            return await RequestAsync<GUISettings>($"/api/1/vehicles/{id}/data_request/gui_settings");
        }

        /// <summary>
        /// Returns the vehicle's configuration information including model, color, badging and wheels.
        /// </summary>
        /// <returns></returns>
        public async Task<VehicleConfig> GetVehicleConfig() {

            return await RequestAsync<VehicleConfig>($"/api/1/vehicles/{id}/data_request/vehicle_config");
        }

        #endregion

        #region Command Endpoints

        /// <summary>
        /// Wakes up the vehicle.
        /// </summary>
        public async Task<Wake> WakeAsync() {

            return await RequestAsync<Wake>($"/api/1/vehicles/{id}/wake_up");
        }

        /// <summary>
        /// Unlock vehicle.
        /// </summary>
        public async Task<Command> UnlockAsync() {

            return await RequestAsync<Command>($"/api/1/vehicles/{id}/command/door_unlock");
        }

        /// <summary>
        /// Lock vehicle.
        /// </summary>
        public async Task<Command> LockAsync() {

            return await RequestAsync<Command>($"/api/1/vehicles/{id}/command/door_lock");
        }

        /// <summary>
        /// Set the charge limit.
        /// </summary>
        /// <param name="percent"></param>
        public async Task<Command> SetChargeLimitAsync(int percent) {

            return await RequestAsync<Command>($"/api/1/vehicles/{id}/command/set_charge_limit?percent={percent}");
        }

        /// <summary>
        /// Honks the horn twice.
        /// </summary>
        /// <returns></returns>
        public async Task<Command> HonkHorn() {

            return await RequestAsync<Command>($"/api/1/vehicles/{id}/command/honk_horn");
        }

        /// <summary>
        /// Flashes the headlights once.
        /// </summary>
        /// <returns></returns>
        public async Task<Command> FlashLights() {

            return await RequestAsync<Command>($"/api/1/vehicles/{id}/command/flash_lights");
        }

        /// <summary>
        /// Enables keyless driving. There is a two minute window after issuing the command to start driving the car. The password for the authenticated tesla.com account.
        /// </summary>
        /// <returns></returns>
        public async Task<Command> RemoteStart(string password) {

            return await RequestAsync<Command>($"/api/1/vehicles/{id}/command/remote_start_drive?password={password}");
        }

        /// <summary>
        /// Sets the maximum speed allowed when Speed Limit Mode is active. The speed limit in MPH. Must be between 50-90.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<Command> SetSpeedLimit(int limit) {

            return await RequestAsync<Command>($"/api/1/vehicles/{id}/command/speed_limit_set_limit?limit_mph={limit}");
        }

        /// <summary>
        /// Activates Speed Limit Mode at the currently set speed. The existing PIN, if previously set, or a new 4 digit PIN.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public async Task<Command> ActivateSpeedLimit(int pin) {

            return await RequestAsync<Command>($"/api/1/vehicles/{id}/command/speed_limit_activate?pin={pin}");
        }

        /// <summary>
        /// Deactivates Speed Limit Mode if it is currently active. The 4 digit PIN used to activate Speed Limit Mode.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public async Task<Command> DeactivateSpeedLimit(int pin) {

            return await RequestAsync<Command>($"/api/1/vehicles/{id}/command/speed_limit_deactivate?pin={pin}");
        }

        /// <summary>
        /// Clears the currently set PIN for Speed Limit Mode. The 4 digit PIN used to activate Speed Limit Mode.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public async Task<Command> ClearSpeedLimitPin(int pin) {

            return await RequestAsync<Command>($"/api/1/vehicles/{id}/command/speed_limit_clear_pin?pin{pin}");
        }

        #endregion

        #endregion
    }
}