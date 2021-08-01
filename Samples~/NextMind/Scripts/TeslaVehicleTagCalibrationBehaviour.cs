using System.Collections;
using System.Collections.Generic;
using NextMind.Calibration;
using NextMind.NeuroTags;
using UnityEngine;

public class TeslaVehicleTagCalibrationBehaviour : TagCalibrationBehaviour {

    public override void OnInitialize(NeuroTag tag) {

        tag.gameObject.SetActive(true);

        return;
    }

    public override IEnumerator OnStartCalibrating(NeuroTag tag) {

        Debug.Log($"Start NeuroTag Calibration For: {tag.name}");

        yield break;
    }

    public override IEnumerator OnEndCalibrating(NeuroTag tag) {

        Debug.Log($"Ending NeuroTag Calibration For: {tag.name}");

        yield break;
    }
}
