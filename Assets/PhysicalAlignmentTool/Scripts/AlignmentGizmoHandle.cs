using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AlignmentGizmoHandle : MonoBehaviour
{
    [SerializeField] 
    private bool[] axesIndicator = new bool[3];
    private AlignmentGizmo _gizmo;

    public UnityAction<bool[]> OnHandleSelected;
    // Start is called before the first frame update

    private void Start()
    {
        _gizmo = GetComponentInParent<AlignmentGizmo>();
    }

    public void SelectHandle()
    {
        OnHandleSelected(axesIndicator);
    }
    
}
