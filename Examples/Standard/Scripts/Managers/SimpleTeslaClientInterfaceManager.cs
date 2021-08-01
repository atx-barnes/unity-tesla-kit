using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using Tesla.API.Core;
using Newtonsoft.Json;
using System;

public class SimpleTeslaClientInterfaceManager : TeslaClientManager
{
    [Header("API Requests Elements")]

    public TMP_Dropdown TypeDropdown;

    public TMP_Dropdown RequestDropdown;

    [Header("Vehicle State Requests")]

    public DropdownItemVehicleRequestEvents StateRequests;

    [Header("Vehicle Command Requests")]

    public DropdownItemVehicleRequestEvents CommandRequests;

    private int request;

    /// <summary>
    /// Unity Start event.
    /// </summary>
    private void Start() {

        PopulateDropdownOptions(0);
    }

    /// <summary>
    /// Unity OnEnable event used for registering listeners to UI elements
    /// </summary>
    private void OnEnable() {

        TypeDropdown.onValueChanged.AddListener(PopulateDropdownOptions);

        RequestDropdown.onValueChanged.AddListener(RequestValueChanged);
    }

    /// <summary>
    /// Unity event for unregistering the listeners from the UI elements.
    /// </summary>
    private void OnDisable() {

        TypeDropdown.onValueChanged.RemoveListener(PopulateDropdownOptions);

        RequestDropdown.onValueChanged.RemoveListener(RequestValueChanged);
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
    /// Handle for the onValueChanged for the dropdown menu.
    /// </summary>
    /// <param name="value"></param>
    private void RequestValueChanged(int value) {

        request = value;
    }

    [Serializable]
    public class DropdownItemVehicleRequestEvents {

        public UnityEvent Event = new UnityEvent();
    }
}
