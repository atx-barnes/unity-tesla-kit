using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NextMind.Calibration;
using TMPro;
using NextMind.Devices;
using NextMind;

public class NeuroTeslaCalibrationManager : MonoBehaviour
{
    public AdvancedTeslaClientInterfaceManager TeslaInterfaceManager;

    public GameObject LoadingGroup;

    public TextMeshProUGUI DebugTMP;

    private CalibrationManager calibrationManager;

    private NeuroVehicleController neuroVehicleController;

    private bool vehiclesAwaken;

    /// <summary>
    /// Retrieves the calibration manager once vehicle is retrieve by the client.
    /// </summary>
    public void GetCalibrationManager() {

        neuroVehicleController = NeuroVehicleController.Instance;

        calibrationManager = neuroVehicleController.CalibrationManager;

        neuroVehicleController.NeuroTeslaCalibrationManager = this;

        TeslaInterfaceManager.OnUserVehiclesAwaken.AddListener((awaken) => {

            vehiclesAwaken = awaken;
        });
    }

    /// <summary>
    /// Starts calibration once calibrate button is clicked.
    /// </summary>
    public void StartCalibration() {

        if(vehiclesAwaken) {

            StartCoroutine(StartCalibrationWhenReady());

        } else {

            Debug.LogWarning("Please wait until vehicles are fully awaken before calibrating");
        }
    }

    /// <summary>
    /// Starts the calibration when headset is ready to be calibrated.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartCalibrationWhenReady() {

        yield return new WaitUntil(NeuroManager.Instance.IsReady);

        calibrationManager.SetNeuroTagBehaviour(new TeslaVehicleTagCalibrationBehaviour());

        calibrationManager.StartCalibration();

        LoadingGroup.SetActive(true);

        DebugTMP.text = "Neuro-Calibration In Progress . . .";
    }
}
