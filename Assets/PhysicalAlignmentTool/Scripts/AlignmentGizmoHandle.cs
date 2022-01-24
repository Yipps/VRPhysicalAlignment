using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Drawing;
using TMPro.EditorUtilities;

public class AlignmentGizmoHandle : MonoBehaviour
{
    //[SerializeField] 
    //private bool[] axesIndicator = new bool[3];
    [SerializeField] 
    private float axisLength = 10f;
    private AlignmentGizmo _gizmo;
    private bool _showAxis;
    private bool _isHovered;

    public UnityAction<Vector3> onHandleSelected;
    public UnityAction onHandleDeselected;
    
    


    private void Start()
    {
        _gizmo = GetComponentInParent<AlignmentGizmo>();
    }

    private void Update()
    {
        DrawHandleGUI();
    }

    public void SelectHandle()
    {
        onHandleSelected(transform.forward);
        _showAxis = true;
    }

    public void DeselectHandle()
    {
        onHandleDeselected();
        _showAxis = false;
    }

    private void DrawHandleGUI()
    {
        if (_showAxis||_isHovered)

        {
            Vector3 start = transform.position + (transform.forward * axisLength);
            Vector3 end = transform.position + (-transform.forward * axisLength);
            Draw.Line(start,end,Color.cyan);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        VRAligner aligner = other.GetComponent<VRAligner>();
        if (aligner)
        {
            _isHovered = true;
            print("hover");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        VRAligner aligner = other.GetComponent<VRAligner>();
        if (aligner)
        {
            _isHovered = false;
        }
    }
}
