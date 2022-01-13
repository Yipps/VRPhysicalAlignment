using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.WSA;

public class AlignmentGizmo : MonoBehaviour
{
    [SerializeField]
    private bool[] _activeAxes = new bool[3];
    private AlignmentTracker[] _focusedTrackers;
    private bool _isPivotCenter = true;
    private AlignmentGizmoHandle[] _gizmoHandles;

    private void OnEnable()
    {
        _gizmoHandles = GetComponentsInChildren<AlignmentGizmoHandle>();
        foreach (AlignmentGizmoHandle alignmentGizmoHandle in _gizmoHandles)
        {
            alignmentGizmoHandle.OnHandleSelected += SetAxes;
        }
        
        //gameObject.SetActive((false));
    }

    private void OnDisable()
    {
        foreach (AlignmentGizmoHandle alignmentGizmoHandle in _gizmoHandles)
        {
            alignmentGizmoHandle.OnHandleSelected -= SetAxes;
        }
    }

    // Start is called before the first frame update
    public void Translate(Vector3 delta)
    {
        //zero out any unactive axis
        
        Vector3 directionVector = Vector3.zero;
        for (int i = 0; i < 3; i++)
        {
            // if (!_activeAxes[i])
            //     delta[i] = 0;
            if (_activeAxes[i])
            {
                if (i == 0)
                {
                    directionVector = transform.right;
                    print("(x) Forward " + directionVector);
                }else if (i == 1)
                {
                    directionVector = transform.up;
                    print("(y) Up " + directionVector);
                }else if (i == 2)
                {
                    directionVector = transform.forward;
                    print("(z) Forward " + directionVector);
                }
            }
        }
        
        delta = Vector3.Project(delta, directionVector);

        foreach (AlignmentTracker focusedTracker in _focusedTrackers)
        {
            focusedTracker.transform.Translate(delta,Space.World);
        }
        
        transform.Translate(delta,Space.World);
        
        
    }

    public void SetTrackerTarget(AlignmentTracker[] selectionList)
    {
        //If no valid target reset selection
        if (selectionList.Length < 1)
        {
            ResetTrackerTarget();
            return;
        }

        Vector3 selectionCenter = Vector3.zero;
        
        foreach (AlignmentTracker tracker in selectionList)
        {
            Vector3 trackerCenter = tracker.transform.position;
            if (_isPivotCenter)
                trackerCenter += tracker.GetRendererBounds().center;
            selectionCenter += trackerCenter;

        }
        

        transform.position = selectionCenter;
        transform.localRotation = Quaternion.identity;
        
        //We dont use this
        //_focusedTrackers = selectionList;
        
        gameObject.SetActive(true);
    }

    public void ResetTrackerTarget()
    {
        _focusedTrackers = null;
        gameObject.SetActive(false);
    }
    
    private void SetAxes(bool[] axes)
    {
        _activeAxes = axes;
        print("Set axis:" + _activeAxes);
    }


    
}


