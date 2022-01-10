using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentGizmo : MonoBehaviour
{
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
        
        gameObject.SetActive((false));
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
        for (int i = 0; i < 3; i++)
        {
            if (!_activeAxes[i])
                delta[i] = 0;
        }
        
        _targetTracker.transform.Translate(delta);
        
    }

    public void SetTrackerTarget(AlignmentTracker target)
    {
        
        Vector3 location = target.transform.position;
        if (_isPivotCenter)
            location += target.GetRendererBounds().center;
        transform.position = location;
        transform.parent = target.transform;
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
    }

    public void RaycastSelection()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, 10f);

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            AlignmentTracker hitTracker = hitObject.GetComponent<AlignmentTracker>();

            if (hitTracker)
            {
                SetTrackerTarget(hitTracker);
                break;
            }
            else
            {
                ResetTrackerTarget();
            }
        }
    }
    
}
