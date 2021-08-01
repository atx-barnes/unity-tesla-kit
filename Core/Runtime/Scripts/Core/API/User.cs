using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Tesla.API.Core {

    public sealed class User {

        #region Properties

        /// <summary>
        /// List of this users vehicles.
        /// </summary>
        private List<Vehicle> vehicles;

        public List<Vehicle> Vehicles {

            set {

                vehicles = value;

                vehiclesRetrieved = true;
            }

            get => vehicles;
        }

        /// <summary>
        /// Checks if vehicles are retrived.
        /// </summary>
        private bool vehiclesRetrieved = false;

        public bool VehiclesRetrieved { private set { vehiclesRetrieved = value; } get => vehiclesRetrieved; }

        /// <summary>
        /// Event invoked when vehicles have retrieved.
        /// </summary>
        public UserVehiclesRetrieved OnVehiclesRetrieved;

        /// <summary>
        /// Event invoked when vehicles have awaken
        /// </summary>
        public UserVehiclesAwaken OnUserVehiclesAwaken;

        #endregion

        #region Main Methods

        /// <summary>
        /// Returns the first found or default vehicle type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void GetVehicle<T>(Func<Vehicle, Task> onComplete) where T : Vehicle {

            if (VehiclesRetrieved) {

                var vehicle = vehicles.OfType<T>().FirstOrDefault();

                onComplete?.Invoke(vehicle);

            } else {

                Debug.LogWarning("Vehicles are still being retrieved by the client please wait.");
            }
        }

        /// <summary>
        /// Returns the vehicle(s) for a given user and parents each vehicle to the user gameobject.
        /// </summary>
        /// <param name="onRetrievalComplete"></param>
        /// <returns></returns>
        public async Task RetrieveVehiclesAsync(bool wake) {

            if(!VehiclesRetrieved) {

                VehiclesInfo vehicleInfo;

                vehicleInfo = await TeslaClient.Instance.ClientRequestAsync<VehiclesInfo>("/api/1/vehicles");

                List<Vehicle> vehicles = new List<Vehicle>();

                foreach (VehiclesInfo.VehiclesInfoResponse info in vehicleInfo.Response) {

                    // Digit 4 Make / Line. More info https://vpic.nhtsa.dot.gov/mid/home/displayfile/9d71d5c8-a6ff-45de-88ae-8ea1da8dd324
                    string modelChar = info.vin[3].ToString();

                    switch (modelChar) {

                        case "S": vehicles.Add(new ModelS (info));

                            break;

                        case "3": vehicles.Add(new Model3(info));

                            break;

                        case "X": vehicles.Add(new ModelX(info));

                            break;

                        case "Y": vehicles.Add(new ModelY(info));

                            break;
                    }
                }

                Vehicles = vehicles;

                OnVehiclesRetrieved?.Invoke(Vehicles);

                if (TeslaClient.Instance.DebugLevel == TeslaClient.LogLevel.Client) Debug.Log("Retrieved User Vehicles");

                if(wake) {

                    await AwakeVehiclesAsync();
                }

            } else {

                if (TeslaClient.Instance.DebugLevel == TeslaClient.LogLevel.Vehicles)

                    Debug.Log("Vehicles have already been retrieved for this instance");
            }
        }

        /// <summary>
        /// Wakes up all user vehicles if not already awaken
        /// </summary>
        /// <param name="OnComplete"></param>
        private async Task AwakeVehiclesAsync() {

            List<Task> wakeTasks = new List<Task>();

            foreach (var vehicle in vehicles) {

                wakeTasks.Add(vehicle.WakeVehicleAsnyc());
            }

            Task whenAllWakeTask = Task.WhenAll(wakeTasks);

            while(!whenAllWakeTask.IsCompleted) {

                await Task.Delay(100);
            }

            OnUserVehiclesAwaken?.Invoke(true);

            if (TeslaClient.Instance.DebugLevel == TeslaClient.LogLevel.Client) Debug.Log("User Vehicles Awaken Successfully");
        }

        #endregion
    }
}
