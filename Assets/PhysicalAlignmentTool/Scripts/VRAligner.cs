using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class VRAligner : MonoBehaviour
{

    [SerializeField]
    private AlignmentGizmo _gizmo;

    private bool isTranslating;

    private Vector3 lastPosition;
    private AlignmentGizmoHandle HoveredHandle;

    private List<InputDevice> _devices = new List<InputDevice>();
    private InputDevice device;

    void Start()
    {
        //_gizmo = GetComponentInChildren<AlignmentGizmo>();
        
    }
    
    private void Update()
    {
        if (isTranslating && _gizmo.gameObject.activeSelf)
        {
            Vector3 delta = transform.position - lastPosition;
            //delta.x = delta.x * -1;
            //delta.z = delta.z * -1;
            //print(delta);
            lastPosition = transform.position;
            
            _gizmo.Translate(delta);
            
        }
        
        
    }
    
    void SelectTracker(AlignmentTracker tracker)
    {
        _gizmo.SetTrackerTarget(tracker);
    }

    public void GrabHoveredGizmo()
    {
        if (HoveredHandle)
        {
            //We dont need to deselect because every selection overrides the active axis handle
            //print("SelectHandle: " + HoveredHandle.gameObject.name);
            HoveredHandle.SelectHandle();
            isTranslating = true;
            lastPosition = transform.position;
        }
    }

    public void ReleaseGizmo()
    {
        isTranslating = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        HoveredHandle = other.GetComponent<AlignmentGizmoHandle>();
        //print("Hovered Handle" + HoveredHandle.gameObject.name);
    }
    
    public void RaycastSelection()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, 10f);
        Debug.DrawRay(transform.position,transform.forward,Color.red,10f);

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            AlignmentTracker hitTracker = hitObject.GetComponent<AlignmentTracker>();
            print("Raycasted");
            print(hitTracker);

            if (hitTracker)
            {
                _gizmo.SetTrackerTarget(hitTracker);
                break;
            }
            else
            {
                _gizmo.ResetTrackerTarget();
            }
        }
    }
}
