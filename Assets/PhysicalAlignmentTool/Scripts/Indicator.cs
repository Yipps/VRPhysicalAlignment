using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    private Color selected;
    [SerializeField]
    private Color unselected;
    [SerializeField]
    private Color focused;
    [SerializeField] 
    private Color hovered;
    
    private Camera _cam;
    
    [SerializeField]
    private GameObject target;

    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            GetComponent<SpriteRenderer>().color = value ? selected : unselected;
        }
    }

    private bool _IsHovered;

    public bool IsHovered
    {
        get => _IsHovered;
        set
        {
            Color current = _isSelected ? selected : unselected;
            GetComponent<SpriteRenderer>().color = value ? hovered : current;
        }
    }

    private void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        if (_cam)
            transform.LookAt(_cam.transform, Vector3.up);
    }
}
