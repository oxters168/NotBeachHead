using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class ControllerInput : MonoBehaviour
{
    public enum Hand { left, right }
    public Hand handedness;
    private InputDevice currentDevice;
    private bool deviceFound;

    public bool gripTestInput;
    public bool triggerTestInput;

    public bool isGripping { get; private set; }
    public bool isTriggering { get; private set; }

    void Start()
    {
        InitController();
    }

    void Update()
    {
        isGripping = false;
        isTriggering = false;
        if (deviceFound)
        {
            bool gripValue;
            currentDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out gripValue);
            isGripping = gripValue;

            bool triggerValue;
            currentDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue);
            isTriggering = triggerValue;
        }
        isGripping |= gripTestInput;
        isTriggering |= triggerTestInput;
    }

    private void InitController()
    {
        var handednessCharacteristic = InputDeviceCharacteristics.Left;
        if (handedness == Hand.right)
            handednessCharacteristic = InputDeviceCharacteristics.Right;
        var desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | handednessCharacteristic | InputDeviceCharacteristics.Controller;
        var nullableDevice = GetController(desiredCharacteristics);
        if (nullableDevice != null)
        {
            deviceFound = true;
            currentDevice = (InputDevice)nullableDevice;
        }
        else
            Debug.LogError("Did not find device of given specifications");
    }
    private static InputDevice? GetController(InputDeviceCharacteristics desiredCharacteristics)
    {
        var controllersOfType = new List<InputDevice>();
        
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, controllersOfType);

        foreach (var device in controllersOfType)
        {
            Debug.Log(string.Format("Device name '{0}' has characteristics '{1}'", device.name, device.characteristics.ToString()));
        }

        InputDevice? first = null;
        if (controllersOfType.Count > 0)
            first = controllersOfType[0];
        return first;
    }
}
