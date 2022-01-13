﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class VRAligner : MonoBehaviour
{

    [SerializeField]
    private AlignmentGizmo _gizmo;

    private List<AlignmentTracker> _selection = new List<AlignmentTracker>();
    
    //Undo/Redo data
    private Vector3[] _initSelectionLocations;
    
    //Ontrigger enter updated
    private AlignmentGizmoHandle _hoveredHandle;
    
    //Pass aligner pos delta to gizmo
    private bool _isTranslating;
    private Vector3 _alignerLastPos;

    private void Update()
    {
        if (_isTranslating && _gizmo.gameObject.activeSelf)
        {
            Vector3 delta = transform.position - _alignerLastPos;

            _gizmo.Translate(delta);
            
            _alignerLastPos = transform.position;
            
        }
    }

    public void GrabHoveredGizmo()
    {
        if (_hoveredHandle)
        {
            //We dont need to deselect because every selection overrides the active axis handle
            //print("SelectHandle: " + HoveredHandle.gameObject.name);
            _hoveredHandle.SelectHandle();
            _isTranslating = true;
            _alignerLastPos = transform.position;

            _initSelectionLocations = GetSelectionPositions();
        }
    }

    public void ReleaseGizmo()
    {
        _isTranslating = false;
        
        //Undo/Redo Command
        Vector3[] selectionCurrentPosition = GetSelectionPositions();
        TranslateTrackersCommand translateCommand = new TranslateTrackersCommand(_selection.ToArray(), _initSelectionLocations, selectionCurrentPosition);
        CommandManager.DoCommand(translateCommand);
    }

    private void OnTriggerEnter(Collider other)
    {
        _hoveredHandle = other.GetComponent<AlignmentGizmoHandle>();
        //print("Hovered Handle" + HoveredHandle.gameObject.name);
    }
    
    private AlignmentTracker RaycastSelection()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, 10f);
        Debug.DrawRay(transform.position,transform.forward,Color.red,10f);

        AlignmentTracker hitTracker = null;
        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            hitTracker = hitObject.GetComponent<AlignmentTracker>();
        }
        
        return hitTracker;
    }

    public void SingleSelect()
    {
        _selection.Clear();
        
        AlignmentTracker hit = RaycastSelection();
        if (hit)
        {
            _selection.Add(hit);
            _gizmo.SetTrackerTarget(_selection.ToArray());
        }
    }

    public void ToggleSelect()
    {
        AlignmentTracker hit = RaycastSelection();

        if (hit)
        {
            if (_selection.Contains(hit))
            {
                _selection.Remove(hit);
                _gizmo.SetTrackerTarget(_selection.ToArray());
            }
            else
            {
                _selection.Add(hit);
                _gizmo.SetTrackerTarget(_selection.ToArray());
            }
        }
    }

    private Vector3[] GetSelectionPositions()
    {
        Vector3[] locations = new Vector3[_selection.Count];

        for (int i = 0; i < _selection.Count; i++)
        {
            locations[i] = _selection[i].transform.position;
        }

        return locations;
    }
}

public class TranslateTrackersCommand : IAlignmentCommand
{
    private AlignmentTracker[] _trackers;
    private Vector3[] _prevLocation;
    private Vector3[] _newLocation;

    public TranslateTrackersCommand(AlignmentTracker[] trackers, Vector3[] prevLocation, Vector3[] newLocation)
    {
        _trackers = trackers;
        _prevLocation = prevLocation;
        _newLocation = newLocation;
    }

    public void Execute()
    {
        for (int i = 0; i < _newLocation.Length; i++)
        {
            _trackers[i].transform.position = _newLocation[i];
        }
    }

    public void Undo()
    {
        for (int i = 0; i < _newLocation.Length; i++)
        {
            _trackers[i].transform.position = _prevLocation[i];
        }
    }
}