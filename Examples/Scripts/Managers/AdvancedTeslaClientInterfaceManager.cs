using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using Tesla.API.Core;
using Newtonsoft.Json;
using System;

public class AdvancedTeslaClientInterfaceManager : TeslaClientManager {

    #region Properties

    [Header("Vehicle Model")]

    public GameObject Model3Prefab;

    [Header("Login Elements")]

    public GameObject LoginScreen;

    [Header("Custom Client Events")]

    public UnityEvent OnVehicleInfoRetrieved = new UnityEvent();

    [Header("API Requests Elements")]

    public TMP_Dropdown TypeDropdown;

    public TMP_Dropdown RequestDropdown;

    [Header("Vehicle State Requests")]

    public DropdownItemVehicleRequestEvents StateRequests;

    public DriveStateResponseFields.Fields DriveStateFields;

    [Header("Vehicle State TMP Fields")]

    public VehicleInfoResponseFields.Fields VehicleInfoFields;

    public ChargeStateResponseFields.Fields ChargeStateFields;

    public ClimateStateResponseFields.Fields ClimateStateFields;

    [Header("Vehicle Command Requests")]

    public DropdownItemVehicleRequestEvents CommandRequests;

    public CommandRequestEvents VehicleCommandRequestEvents;

    [Header("Interface Color Customizer")]

    public List<ElementGroups> ColorElementGroups;

    public Button ToggleButton;

    public ColorThemeScriptableObject LightColor;

    public ColorThemeScriptableObject DarkColor;

    [Header("Interface Events")]

    public UnityEvent OnToggleOn = new UnityEvent();

    public UnityEvent OnToggleOff = new UnityEvent();

    public static AdvancedTeslaClientInterfaceManager Instance;

    private bool isToggle = false;

    private int request;

    #endregion

    #region Main Methods

    /// <summary>
    /// Unity OnEnable event used for registering listeners to UI elements
    /// </summary>
    private void OnEnable() {

        ToggleButton.onClick.AddListener(Toggle);

        TypeDropdown.onValueChanged.AddListener(PopulateDropdownOptions);

        RequestDropdown.onValueChanged.AddListener(RequestValueChanged);
    }

    /// <summary>
    /// Unity Start event.
    /// </summary>
    private void Start() {

        ChangeTheme(LightColor);

        PopulateDropdownOptions(0);
    }

    /// <summary>
    /// Handle for loading the vehicles once they are retrieved.
    /// </summary>
    /// <param name="vehicles"></param>
    public void LoadVehicles(List<Vehicle> vehicles) {

        foreach (Vehicle vehicle in vehicles) {

            if (vehicle is Model3) {

                Instantiate(Model3Prefab);
            }
        }
    }

    /// <summary>
    /// Changes the theme of the interface using the theme scriptable object that contains color information.
    /// </summary>
    /// <param name="themeScriptableObject"></param>
    public void ChangeTheme(ColorThemeScriptableObject themeScriptableObject) {

        var colors = themeScriptableObject.Colors;

        Camera.main.backgroundColor = themeScriptableObject.Background;

        // For each element group in element groups
        for (int i = 0; i < ColorElementGroups.Count; i++) {

            // Change the color of the text of this element groups elements
            for (int x = 0; x < ColorElementGroups[i].TextElements.Count; x++) {

                ColorElementGroups[i].TextElements[x].color = colors[i];
            }

            // Change the color of the images of this element groups elements
            for (int x = 0; x < ColorElementGroups[i].ImageElements.Count; x++) {

                ColorElementGroups[i].ImageElements[x].color = colors[i];
            }
        }
    }

    /// <summary>
    /// Populates the dropdown menu based on index value once the value has been changed.
    /// </summary>
    /// <param name="value"></param>
    public void PopulateDropdownOptions(int value) {

        RequestDropdown.ClearOptions();

        // Reset the request dropdown counter when switching to new request type
        request = 0;

        // Get the text value for the type of request the user selected using the value passed back from the onValueChanged event
        switch (TypeDropdown.options[value].text) {

            // Populate vehicle state endpoints using the GetPersistentMethodName from StateRequestEvents events list
            case "State":

                List<TMP_Dropdown.OptionData> StateOptionDatas = new List<TMP_Dropdown.OptionData>();

                string name = string.Empty;

                for (int i = 0; i < StateRequests.Event.GetPersistentEventCount(); i++) {

                    name = StateRequests.Event.GetPersistentMethodName(i);

                    StateOptionDatas.Add(new TMP_Dropdown.OptionData(name));
                }

                if (string.IsNullOrEmpty(name)) {

                    throw new UnityException("method name is empty or null. Do you have state request listeners added to the state request event?");
                }

                RequestDropdown.AddOptions(StateOptionDatas);

                break;

            // Populate vehicle command endpoints dropdown using the GetPersistentMethodName from CommandRequestEvents events list
            case "Command":

                List<TMP_Dropdown.OptionData> CommandOptionDatas = new List<TMP_Dropdown.OptionData>();

                for (int i = 0; i < CommandRequests.Event.GetPersistentEventCount(); i++) {

                    CommandOptionDatas.Add(new TMP_Dropdown.OptionData(CommandRequests.Event.GetPersistentMethodName(i)));
                }

                RequestDropdown.AddOptions(CommandOptionDatas);

                break;
        }
    }

    /// <summary>
    /// Invokes the last selected endpoint method from the dropdown. Creates a delegate using system.reflection.
    /// </summary>
    public void Submit() {

        var targetInfo = UnityEvent.GetValidMethodInfo(this, RequestDropdown.options[request].text, new Type[0]);

        UnityAction methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), this, targetInfo) as UnityAction;

        methodDelegate.Invoke();
    }

    /// <summary>
    /// Called when vehicle has been awaken by the client, loads the initital vehicle info on the interface.
    /// </summary>
    public void InitializeVehicleInfoOnAwaken() {

        Client.User.GetVehicle<Model3>(async (M3) => {

            ClimateState climate = await M3.GetClimateStateAsync();

            ClimateStateFields.TMP_outside_temp.text = climate.Response.outside_temp.ToString() + "°";

            ChargeState charge = await M3.GetChargeStateAsync();

            ChargeStateFields.TMP_battery_level.text = charge.Response.battery_level.ToString() + "%";

            VehicleInfo vehicle = await M3.GetVehicleInfoAsync();

            VehicleInfoFields.TMP_display_name.text = vehicle.Response.display_name;

            VehicleInfoFields.TMP_vin.text = vehicle.Response.vin;

            VehicleInfoFields.TMP_state.text = vehicle.Response.state;

            DriveState drive = await M3.GetDriveStateAsync();

            DriveStateFields.TMP_latitude.text = drive.Response.latitude.ToString();

            DriveStateFields.TMP_longitude.text = drive.Response.longitude.ToString();

            DriveStateFields.TMP_heading.text = drive.Response.heading.ToString();

            DriveStateFields.TMP_timestamp.text = UnixTimeStampToDateTime(drive.Response.timestamp);

            Response.text = JsonConvert.SerializeObject(drive.Response, Formatting.Indented);

            OnVehicleInfoRetrieved?.Invoke();
        });
    }

    /// <summary>
    /// Callback for the onValueChanged for the dropdown menu.
    /// </summary>
    /// <param name="value"></param>
    private void RequestValueChanged(int value) {

        request = value;
    }

    /// <summary>
    /// Unity event for unregistering the listeners from the UI elements.
    /// </summary>
    private void OnDisable() {

        ToggleButton.onClick.RemoveListener(Toggle);

        TypeDropdown.onValueChanged.RemoveListener(PopulateDropdownOptions);

        RequestDropdown.onValueChanged.RemoveListener(RequestValueChanged);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Toggles the interface theme.
    /// </summary>
    public void Toggle() {

        isToggle = !isToggle;

        switch (isToggle) {

            case true: OnToggleOn.Invoke();

                break;

            case false: OnToggleOff.Invoke();

                break;
        }
    }

    /// <summary>
    /// Debug message to help with events in the inspector.
    /// </summary>
    /// <param name="message"></param>
    public void DebugMessage(string message) {

        Debug.Log(message);
    }

    /// <summary>
    /// Convert unix timestamp to date time https://stackoverflow.com/a/250400
    /// </summary>
    /// <param name="unixTimeStamp"></param>
    /// <returns></returns>
    public string UnixTimeStampToDateTime(double unixTimeStamp) {

        string seconds = unixTimeStamp.ToString().Remove(10);

        Double.TryParse(seconds, out double convertedTime);

        DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0);

        dateTime = dateTime.AddSeconds(convertedTime).ToLocalTime();

        return dateTime.ToShortTimeString();
    }

    #endregion

    #region Serializable Objects

    [Serializable]
    public class ElementGroups {

        public List<TextMeshProUGUI> TextElements;

        public List<Image> ImageElements;
    }


    [Serializable]
    public class DropdownItemVehicleRequestEvents {

        public UnityEvent Event = new UnityEvent();
    }

    [Serializable]
    public class CommandRequestEvents {

        public UnityEvent OnChargePortOpen = new UnityEvent();

        public UnityEvent OnVehicleUnlocked = new UnityEvent();

        public UnityEvent OnVehicleLocked = new UnityEvent();
    }

    #endregion
}
