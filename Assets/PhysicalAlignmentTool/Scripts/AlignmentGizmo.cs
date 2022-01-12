using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class AlignmentGizmo : MonoBehaviour
{
    [SerializeField]
    private bool[] _activeAxes = new bool[3];
    private AlignmentTracker _targetTracker;
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

        //print("Delta Vector:" + delta + "|| Direction Vector:" + directionVector);
        delta = Vector3.Project(delta, directionVector);
        //print(directionVector);
        
        _targetTracker.transform.Translate(delta,Space.World);
        //_targetTracker.transform.Translate(delta,Space.World);
        
    }

    public void SetTrackerTarget(AlignmentTracker target)
    {
        print("WE ARE SETTING THIS");
        Vector3 location = target.transform.position;
        if (_isPivotCenter)
            location += target.GetRendererBounds().center;
        transform.position = location;
        transform.parent = target.transform;

        transform.localRotation = Quaternion.identity;
        _targetTracker = target;
        
        gameObject.SetActive(true);
    }

    public void ResetTrackerTarget()
    {
        _targetTracker = null;
        gameObject.SetActive(false);
    }
    
    private void SetAxes(bool[] axes)
    {
        _activeAxes = axes;
        print("Set axis:" + _activeAxes);
    }


    
}
