using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityTemplateProjects;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color unselectedColor;
    [SerializeField]
    private Color parentSelectedColor;
    [SerializeField] 
    private Color hoveredColor;
    
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
            UpdateColor();
        }
    }
    
    private bool _IsHovered;
    public bool IsHovered
    {
        get => _IsHovered;
        set
        {
            _IsHovered = value;
            UpdateColor();
        }
    }

    private bool _isParentSelected;
    public bool IsParentSelected
    {
        get => _isParentSelected;
        set
        {
            _isParentSelected = value;
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        //Color priority
        //Hover > Selected > Parent Selected > unselected
        //
        Color color = unselectedColor;

        if (IsHovered)
        {
            color = hoveredColor;
        }
        else if (IsSelected)
        {
            color = selectedColor;
        }
        else if (IsParentSelected)
        {
            color = parentSelectedColor;
        }

        GetComponent<SpriteRenderer>().color = color;

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
