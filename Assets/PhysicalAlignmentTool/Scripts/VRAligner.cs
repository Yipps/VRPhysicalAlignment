using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class VRAligner : MonoBehaviour
{

    private AlignmentGizmo _gizmo;

    private bool isTranslating;

    private Vector3 lastPosition;
    private AlignmentGizmoHandle HoveredHandle;

    private List<InputDevice> _devices = new List<InputDevice>();
    private InputDevice device;

    void Start()
    {
        _gizmo = GetComponentInChildren<AlignmentGizmo>();

        
        
    }

    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, _devices);
        device = _devices.FirstOrDefault();
    }
    private void OnEnable()
    {
        if (!device.isValid)
        {
            GetDevice();
        }
    }

    private void Update()
    {
        if (isTranslating && _gizmo.gameObject.activeSelf)
        {
            Vector3 delta = transform.position - lastPosition;
            lastPosition = transform.position;
            
            _gizmo.Translate(delta);
        }
        
        
        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, leftHandDevices);

        if(leftHandDevices.Count == 1)
        {
            UnityEngine.XR.InputDevice device = leftHandDevices[0];
            Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.role.ToString()));
        }
        else if(leftHandDevices.Count > 1)
        {
            Debug.Log("Found more than one left hand!");
        }
        
        bool triggerValue;
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
        {
            Debug.Log("Trigger button is pressed.");
        }
    }
    
    void SelectTracker(AlignmentTracker tracker)
    {
        _gizmo.SetTrackerTarget(tracker);
    }

    void GrabHoveredGizmo()
    {
        if (HoveredHandle)
        {
            //We dont need to deselect because every selection overrides the active axis handle
            HoveredHandle.SelectHandle();
            isTranslating = true;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        HoveredHandle = GetComponent<AlignmentGizmoHandle>();
        print(HoveredHandle);
    }
}
