using System;
using System.Collections.Generic;
using Drawing;
using UnityEngine;


public class VRAligner : MonoBehaviour
{

    [SerializeField]
    private AlignmentGizmo _gizmo;

    private static List<AlignmentTracker> _selection = new List<AlignmentTracker>();
    
    //Undo/Redo data
    private Vector3[] _initSelectionLocations;
    
    //Ontrigger enter updated
    private AlignmentGizmoHandle _hoveredHandle;
    private AlignmentGizmoHandle _grippedHandle;

    private AlignmentTracker _hoveredTracker;
    
    //Pass aligner pos delta to gizmo
    private bool _isTranslating;
    private Vector3 _alignerLastPos;
    private float _moveSpeed = 1f;

    private void Update()
    {
        if (_isTranslating && _gizmo.gameObject.activeSelf)
        {
            Vector3 delta = transform.position - _alignerLastPos;

            _gizmo.Translate(delta * _moveSpeed);
            
            _alignerLastPos = transform.position;
        }
        
        //Raycast UI
        RaycastUI();
        DrawGUI();
        print(_selection.Count);
    }

    private void RaycastUI()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, 10f);

        foreach (RaycastHit hit in hits)
        {
            VRButton button = hit.collider.gameObject.GetComponent<VRButton>();
            if (button)
            {
                button.Select();
                print("Select");
            }
                
        }

        Debug.DrawRay(transform.position, transform.forward, Color.red);
        //if (Physics.Raycast(transform.position, transform.forward, out hit, 10f, ))
    }

    public void RaycastUIPress()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 20f))
        {
            VRButton button = hit.collider.gameObject.GetComponent<VRButton>();

            if (button)
                button.Press();
        }

        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }
    public void GrabHoveredGizmo()
    {
        if (_hoveredHandle)
        {
            //We dont need to deselect because every selection overrides the active axis handle
            //print("SelectHandle: " + HoveredHandle.gameObject.name);

            _grippedHandle = _hoveredHandle;
            _hoveredHandle.SelectHandle();
            _isTranslating = true;
            _alignerLastPos = transform.position;

            _initSelectionLocations = GetSelectionPositions();
        }
    }

    public void ReleaseGizmo()
    {
        _isTranslating = false;
        if (_grippedHandle)
        {
            _grippedHandle.DeselectHandle();
            _grippedHandle = null;
        }
        
        
        //Undo/Redo Command
        Vector3[] selectionCurrentPosition = GetSelectionPositions();
        TranslateTrackersCommand translateCommand = new TranslateTrackersCommand(_selection.ToArray(), _initSelectionLocations, selectionCurrentPosition);
        CommandManager.DoCommand(translateCommand);
    }

    private void OnTriggerEnter(Collider other)
    {
        _hoveredHandle = other.GetComponent<AlignmentGizmoHandle>();

        AlignmentTracker otherTracker = other.GetComponent<AlignmentTracker>();
        
        if (_hoveredTracker != otherTracker)
        {
            //print("FoundTrigger");
            if (_hoveredTracker)
                _hoveredTracker.IsHovered = false;
            
            _hoveredTracker = otherTracker;
        
            if (_hoveredTracker)
            {
                _hoveredTracker.IsHovered = true;
                print("Color true");
            }
        }

        //print("Hovered Handle" + _hoveredHandle.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        AlignmentTracker otherTracker = other.GetComponent<AlignmentTracker>();

        if (_hoveredTracker && _hoveredTracker == otherTracker)
        {
            _hoveredTracker.IsHovered = false;
            _hoveredTracker = null;
        }
            
    }

    private AlignmentTracker RaycastSelection()
    {
        RaycastHit[] hits;

        hits = Physics.RaycastAll(transform.position, transform.forward, 10f);
        AlignmentTracker hitTracker = null;
        
        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            hitTracker = hitObject.GetComponent<AlignmentTracker>();
            
            if (hitTracker == null) 
                continue;
            
            if (AlignmentTracker.IsLockedView && hitTracker.IsLocked)
            {
                if (hitTracker.IsLocked)
                    return hitTracker;
            }
            else if(!hitTracker.IsLocked)
            {
                //Found a tracker
                return hitTracker;
            }
        }

        return hitTracker;
    }

    public void ToggleLockView()
    {
        AlignmentTracker.ToggleLockView();
    }
    
    private AlignmentTracker RaycastSelection(bool findNew)
    {
        RaycastHit[] hits;
        
        bool hasFoundTracker = false;
        
        hits = Physics.RaycastAll(transform.position, transform.forward, 10f);
        Debug.DrawRay(transform.position,transform.forward,Color.red,10f);

        // string debug = "Hits: " + hits.Length;
        // foreach (RaycastHit hit in hits)
        // {
        //     debug += hit.collider.gameObject.name;
        // }
        //
        // print(debug);

        AlignmentTracker hitTracker = null;
        foreach (RaycastHit hit in hits)
        {
            //Check for tracker, continue if not found
            GameObject hitObject = hit.collider.gameObject;
            hitTracker = hitObject.GetComponent<AlignmentTracker>();
            if (hitTracker == null) continue;
            
            if (findNew)
            {
                //Check if tracker is new
                if (!_selection.Contains(hitTracker))
                {
                    //If we find a new tracker immediately return it
                    if (AlignmentTracker.IsLockedView && hitTracker.IsLocked)
                    {
                        if (hitTracker.IsLocked)
                            return hitTracker;
                    }
                    else if(!hitTracker.IsLocked)
                    {
                        //Found a tracker
                        return hitTracker;
                    }
                }
                //If tracker isn't new, set it to null and keep looking
                hitTracker = null;
            }
            else
            {
                //Not looking for a unique tracker, return immediately 
                
                if (AlignmentTracker.IsLockedView && hitTracker.IsLocked)
                {
                    if (hitTracker.IsLocked)
                        return hitTracker;
                }
                else if(!hitTracker.IsLocked)
                {
                    //Found a tracker
                    return hitTracker;
                }
                
            }

        }
        return hitTracker;
    }
    
    private AlignmentTracker RaycastSelection(AlignmentTracker[] currentSelection)
    {
        RaycastHit[] hits;
        
        bool hasFoundTracker = false;
        
        hits = Physics.RaycastAll(transform.position, transform.forward, 10f);

        AlignmentTracker hitTracker = null;
        
        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            hitTracker = hitObject.GetComponent<AlignmentTracker>();
            if (hitTracker!= null)
            {
                hasFoundTracker = true;
                return hitTracker;
            }
        }

        return hitTracker;
    }

    public void SingleSelect()
    {
        
        Debug.Log("SingleSelect");
        AlignmentTracker hit = RaycastSelection(true);
        ClearSelection();
        if (hit)
        {
            _selection.Add(hit);
            _gizmo.SetTrackerTarget(_selection.ToArray());
            print("Hilighting target");
            hit.IsSelected = true;
        }
        else
        {
            
        }
    }

    public void ToggleLock()
    {
         foreach (AlignmentTracker alignmentTracker in _selection)
         {
             alignmentTracker.IsLocked = !alignmentTracker.IsLocked;
             print("Wasd" + alignmentTracker.IsLocked);
         }
        print("Selection Size: " + _selection.Count);
        ClearSelection();
    }

    public void ToggleSelect()
    {
        AlignmentTracker hit = RaycastSelection();

        if (hit)
        {
            if (_selection.Contains(hit))
            {
                hit.IsSelected = false;
                _selection.Remove(hit);
                _gizmo.SetTrackerTarget(_selection.ToArray());
            }
            else
            {
                hit.IsSelected = true;
                _selection.Add(hit);
                _gizmo.SetTrackerTarget(_selection.ToArray());
            }
        }
    }

    private void ClearSelection()
    {
        foreach (AlignmentTracker alignmentTracker in _selection)
        {
            alignmentTracker.IsSelected = false;
        }
        _selection.Clear();
        _gizmo.SetTrackerTarget(_selection.ToArray());
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

    public void UndoMovement()
    {
        print("Undo");
        CommandManager.Undo();
    }

    public void RedoMovement()
    {
        print("Redo");
        CommandManager.Redo();
    }

    public void TakeGizmo()
    {
        _gizmo.PlaceGizmo(transform.position);
    }

    public void IncrementMoveSpeed()
    {
        _moveSpeed += 0.1f;
        _moveSpeed = Mathf.Clamp(_moveSpeed, 0.1f, 3f);
    }

    public void DecrementMoveSpeed()
    {
        _moveSpeed -= 0.1f;
        _moveSpeed = Mathf.Clamp(_moveSpeed, 0.1f, 3f);
    }
    
    private void DrawGUI()
    {
        Draw.Label2D(transform.position + new Vector3(0,.025f,0),_moveSpeed.ToString(),48f);
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