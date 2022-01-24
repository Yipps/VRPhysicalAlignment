using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drawing;
public class AlignmentGizmo : MonoBehaviour
{
    //[SerializeField]
    private Vector3 _activeDirection;
    private AlignmentTracker[] _focusedTrackers;
    private bool _isPivotCenter = true;
    private AlignmentGizmoHandle[] _gizmoHandles;

    private bool _isMoving;
    
    private bool _isLocked;
    private Vector3 _startPos;
    private Vector3 _deltaSum;

    private void OnEnable()
    {
        _gizmoHandles = GetComponentsInChildren<AlignmentGizmoHandle>();
        foreach (AlignmentGizmoHandle alignmentGizmoHandle in _gizmoHandles)
        {
            alignmentGizmoHandle.onHandleSelected += StartMove;
            alignmentGizmoHandle.onHandleDeselected += EndMove;
        }
        
        //gameObject.SetActive((false));
    }

    private void OnDisable()
    {
        foreach (AlignmentGizmoHandle alignmentGizmoHandle in _gizmoHandles)
        {
            alignmentGizmoHandle.onHandleSelected -= StartMove;
        }
    }

    private void Update()
    {
        DrawGUI();
    }

    public void Translate(Vector3 delta)
    {

        delta = Vector3.Project(delta, _activeDirection);

        foreach (AlignmentTracker focusedTracker in _focusedTrackers)
        {
            focusedTracker.transform.Translate(delta,Space.World);
        }
        
        if(!_isLocked)
            transform.Translate(delta,Space.World);
        _deltaSum += delta;

    }

    // private Vector3 GetAxisDirection()
    // {
    //     Vector3 directionVector = Vector3.zero;
    //     for (int i = 0; i < 3; i++)
    //     {
    //         if (_activeDirection[i])
    //         {
    //             if (i == 0)
    //             {
    //                 directionVector = transform.right;
    //                 print("(x) Forward " + directionVector);
    //             }
    //             else if (i == 1)
    //             {
    //                 directionVector = transform.up;
    //                 print("(y) Up " + directionVector);
    //             }
    //             else if (i == 2)
    //             {
    //                 directionVector = transform.forward;
    //                 print("(z) Forward " + directionVector);
    //             }
    //         }
    //     }
    //
    //     return directionVector;
    // }

    public void SetTrackerTarget(AlignmentTracker[] selectionList)
    {
        //If no valid target reset selection
        if (selectionList.Length < 1)
        {
            ResetTrackerTarget();
            return;
        }
        
        _focusedTrackers = selectionList;
        RecenterGizmo();
        
        gameObject.SetActive(true);
    }

    public void ResetTrackerTarget()
    {
        _focusedTrackers = null;
        gameObject.SetActive(false);
    }
    
    private void StartMove(Vector3 dir)
    {
        _activeDirection = dir;
        _startPos = transform.position;

        _isMoving = true;
        print("Set axis:" + _activeDirection);
    }

    private void EndMove()
    {
        _deltaSum = Vector3.zero;
        _isMoving = false;
    }

    public void RecenterGizmo()
    {
        _isLocked = false;
        Vector3 selectionCenter = Vector3.zero;
        foreach (AlignmentTracker tracker in _focusedTrackers)
        {
            Vector3 trackerCenter = tracker.transform.position;

            trackerCenter += tracker.transform.InverseTransformVector(tracker.GetRendererBounds().center);
            
            selectionCenter += trackerCenter;
        }
        selectionCenter /= _focusedTrackers.Length;
        transform.position = selectionCenter;
        transform.localRotation = Quaternion.identity;
    }

    public void PlaceGizmo(Vector3 position)
    {
        transform.position = position;
        _isLocked = true;
    }

    private void DrawGUI()
    {
        if (_isMoving)
        {
            using (Draw.WithLineWidth(3f))
            {
                Draw.Line(_startPos, _startPos + _deltaSum);

                
                Vector3 spherePos = _isLocked ? _startPos + _deltaSum : _startPos;
                Draw.WireSphere(spherePos, .025f);

                string distance = (_deltaSum.magnitude * 100f).ToString("0.00");

                Draw.Label2D(_startPos+_deltaSum/2f + (transform.up * 0.01f),distance,24f );
            }
        }
    }


    
}


