using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

namespace Tesla.API.Core {

    /// <summary>
    /// Base class for creating custom Tesla client managers.
    /// </summary>
    [AddComponentMenu("TeslaKit/TeslaClientManager")]
    [DisallowMultipleComponent]
    public class TeslaClientManager : MonoBehaviour {

        /// <summary>
        /// Email field for Tesla account login authentication flow.
        /// </summary>
        [Header("Login Elements")]
        public TMP_InputField Email;

        /// <summary>
        /// Password field for Tesla account login authentication flow.
        /// </summary>
        public TMP_InputField Password;

        /// <summary>
        /// Remember login input toggle for Tesla account login authentication flow. 
        /// </summary>
        public Toggle RememberLogin;

        /// <summary>
        /// Login button for Tesla account login authentication flow.
        /// </summary>
        public Button LoginButton;

        /// <summary>
        /// Optional field for displaying a response for a given client request.
        /// </summary>
        public TextMeshProUGUI Response;

        /// <summary>
        /// Event invoked when the client checks for saved user authentication. Returns true if user authentication is saved.
        /// </summary>
        [Header("Client Events")]
        public UnityEvent OnRememberLogin = new UnityEvent();

        /// <summary>
        /// Event invoked when the client checks for saved user authentication. Returns true if user authentication is saved.
        /// </summary>
        public UnityEvent OnUserAuthenticated = new UnityEvent();

        /// <summary>
        /// Event invoked when a new user has been created.
        /// </summary>
        public UserInitialized OnUserInitialized = new UserInitialized();

        /// <summary>
        /// Event invoked when vehicles have retrieved.
        /// </summary>
        public UserVehiclesRetrieved OnUserVehiclesRetrieved = new UserVehiclesRetrieved();

        /// <summary>
        /// Event invoked when vehicles have awaken.
        /// </summary>
        public UserVehiclesAwaken OnUserVehiclesAwaken = new UserVehiclesAwaken();

        /// <summary>
        /// Event invoked when authentication has reset.
        /// </summary>
        public UnityEvent OnClientCacheReset = new UnityEvent();

        /// <summary>
        /// Reference to the Tesla client instance created on awake.
        /// </summary>
        protected TeslaClient Client;

        /// <summary>
        /// Initialize Tesla client on awake.
        /// </summary>
        public virtual void Awake() {

            // Create a new client and register event handlers
            Client = new TeslaClient {

                OnRememberLogin = OnRememberLogin,

                OnAuthenticated = OnUserAuthenticated,

                OnUserInitialized = OnUserInitialized,

                OnUserVehiclesRetrieved = OnUserVehiclesRetrieved,

                OnUserVehiclesAwaken = OnUserVehiclesAwaken,

                OnClientCacheReset = OnClientCacheReset
            };
        }

        /// <summary>
        /// Used for login button.
        /// </summary>
        public void Login() {

            Client.RememberLogin = RememberLogin.isOn;

            LoginAsync();
        }

        /// <summary>
        /// Login to Tesla account.
        /// </summary>
        public virtual async void LoginAsync() {

            await Client.AuthenticateAsync(Email.text, Password.text);
        }

        /// <summary>
        /// Logout of current instance and reset the Tesla client.
        /// </summary>
        public virtual void Logout() {

            Email.text = string.Empty;

            Password.text = string.Empty;

            Client.ResetClientCache();
        }

        #region Endpoint Methods

        /// <summary>
        /// Returns the climate state of the vehicle.
        /// </summary>
        public virtual void GetClimateState() {

            Client.User.GetVehicle<Model3>(async (M3) => {

                ClimateState state = await M3.GetClimateStateAsync();

                Response.text = JsonConvert.SerializeObject(state.Response, Formatting.Indented);
            });
        }

        /// <summary>
        /// Returns the charge state of the vehicle.
        /// </summary>
        public virtual void GetChargeState() {

            Client.User.GetVehicle<Model3>(async (M3) => {

                ChargeState state = await M3.GetChargeStateAsync();

                Response.text = JsonConvert.SerializeObject(state.Response, Formatting.Indented);
            });
        }

        /// <summary>
        /// Returns vehicle information.
        /// </summary>
        public virtual void GetVehicleInfo() {

            Client.User.GetVehicle<Model3>(async (M3) => {

                VehicleInfo info = await M3.GetVehicleInfoAsync();

                Response.text = JsonConvert.SerializeObject(info.Response, Formatting.Indented);
            });
        }

        /// <summary>
        /// Returns the driving and position state of the vehicle.
        /// </summary>
        public virtual void GetDriveState() {

            Client.User.GetVehicle<Model3>(async (M3) => {

                DriveState state = await M3.GetDriveStateAsync();

                Response.text = JsonConvert.SerializeObject(state.Response, Formatting.Indented);
            });
        }

        /// <summary>
        /// Opens the charge port of the vehicle.
        /// </summary>
        public virtual void OpenChargePort() {

            Client.User.GetVehicle<Model3>(async (M3) => {

                Command command = await M3.OpenChargePortAsync();

                Response.text = JsonConvert.SerializeObject(command.Response, Formatting.Indented);
            });
        }

        /// <summary>
        /// Locks the vehicle.
        /// </summary>
        public virtual void Lock() {

            Client.User.GetVehicle<Model3>(async (M3) => {

                Command command = await M3.LockAsync();

                Response.text = JsonConvert.SerializeObject(command.Response, Formatting.Indented);
            });
        }

        /// <summary>
        /// Unlocks the vehicle.
        /// </summary>
        public virtual void Unlock() {

            Client.User.GetVehicle<Model3>(async (M3) => {

                Command command = await M3.UnlockAsync();

                Response.text = JsonConvert.SerializeObject(command.Response, Formatting.Indented);
            });
        }

        #endregion
    }
}
