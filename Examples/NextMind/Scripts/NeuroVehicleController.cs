using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NextMind.Calibration;
using NextMind.Devices;
using NextMind.NeuroTags;
using NextMind;

public class NeuroVehicleController : MonoBehaviour
{
    public CalibrationManager CalibrationManager;

    public NeuroTag FrontRightDoorNeuroTag;

    public UnityEvent OnUnlock = new UnityEvent();

    public UnityEvent OnLock = new UnityEvent();

    public static NeuroVehicleController Instance;

    private AdvancedTeslaClientInterfaceManager TeslaInterfaceManager;

    private bool isToggle = false;

    private NeuroTeslaCalibrationManager _neuroTeslaCalibrationManager;

    public NeuroTeslaCalibrationManager NeuroTeslaCalibrationManager { get => _neuroTeslaCalibrationManager;  set { _neuroTeslaCalibrationManager = value;

            InitializeVehicle();
        }
    }

    private void Awake() {

        Instance = this;
    }

    /// <summary>
    /// Initializes vehicle for neuro-calibration.
    /// </summary>
    public void InitializeVehicle() {

        TeslaInterfaceManager = AdvancedTeslaClientInterfaceManager.Instance;

        CalibrationManager.onCalibrationOver.AddListener(() => {

            NeuroTeslaCalibrationManager.DebugTMP.text = "Processing Calibration Results . . .";
        });

        CalibrationManager.onCalibrationResultsAvailable.AddListener(OnReceivedResults);
    }

    /// <summary>
    /// Event callback invoked when neuro-calibration results are available.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="grade"></param>
    private void OnReceivedResults(Device device, CalibrationResults.CalibrationGrade grade) {

        Debug.Log($"Received results for {device.Name} with a grade of {grade}");

        NeuroTeslaCalibrationManager.LoadingGroup.SetActive(false);

        FrontRightDoorNeuroTag.onTriggered.AddListener(ToggleLock);
    }

    /// <summary>
    /// Event callback for the locking of the vehicle once neuro-tag is triggered by the user.
    /// </summary>
    private void ToggleLock() {

        isToggle = !isToggle;

        switch (isToggle) {

            case true:

                TeslaInterfaceManager.Unlock();

                OnUnlock.Invoke();

                break;

            case false:

                TeslaInterfaceManager.Lock();

                OnLock.Invoke();

                break;
        }
    }

    private void OnDisable() {

        FrontRightDoorNeuroTag.onTriggered.RemoveListener(ToggleLock);
    }
}
