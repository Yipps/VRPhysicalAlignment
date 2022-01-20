using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class AlignmentTracker : MonoBehaviour
{
    private bool _isMovable;
    private BoxCollider _runtimeCollider;
    private PhysicalAlignmentTool _physicalAlignmentTool;
    private Bounds _renderBounds;

    private GameObject _indicatorPrefab;
    private Indicator _indicator;

    private bool _isSelected;
    private bool _isHovered;
    private bool _isParentSelected;

    public bool IsParentSelected
    {
        get => _isParentSelected;
        set
        {
            _isParentSelected = value;
            _indicator.IsParentSelected = value;
        }
    }

    public Vector3 scale;
    
    public bool IsHovered
    {
        get => _isHovered;
        set
        {
            _isHovered = value;
            _indicator.IsHovered = value;
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            _indicator.IsSelected = value;
            
            AlignmentTracker[] children =GetComponentsInChildren<AlignmentTracker>();
            foreach (AlignmentTracker tracker in children)
            {
                if (tracker != this)
                {
                    tracker.IsParentSelected = value;
                }
            }
        }
    }
    

    void Start()
    {
        //Use resource load for builsd
        //_physicalAlignmentTool = (PhysicalAlignmentTool)AssetDatabase.LoadAssetAtPath("Assets/PhysicalAlignmentTool/Scripts/PhysicalAlignmentTool.asset",typeof(PhysicalAlignmentTool));
        _physicalAlignmentTool = Resources.Load("AlignmentTool/PhysicalAlignmentTool") as PhysicalAlignmentTool;
        
        if (!GetComponent<Collider>())
        {
            //Make a min size for this if there are no renderers
            _runtimeCollider = gameObject.AddComponent<BoxCollider>();
            _runtimeCollider.isTrigger = true;

            _renderBounds = CalculateBounds();
            _runtimeCollider.size = _renderBounds.size;
            _runtimeCollider.center = _renderBounds.center;
        }
        
        //Add self to alignmenttool list, if we fail delete this component
        if (!_physicalAlignmentTool.AddObject(gameObject))
        {
            Debug.LogError("Couldn't serialize tracker, deleting alignment tracker component");
            
            if (_runtimeCollider)
                Destroy(_runtimeCollider);
            Destroy(this);
        }

        _indicatorPrefab = Resources.Load("AlignmentTool/Indicator") as GameObject;
        GameObject indicatorGO = Instantiate(_indicatorPrefab, transform);
        _indicator = indicatorGO.GetComponent<Indicator>();
        _indicator.transform.localPosition = _renderBounds.center;

    }

    private void Update()
    {
        //Gizmos.Cube(transform.position + _renderBounds.center, transform.rotation,_renderBounds.size, Color.black);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, .6f, 0f);
        Gizmos.DrawWireCube(transform.position + transform.InverseTransformVector(_renderBounds.center), _renderBounds.size);
    }


    // private void CalculateBounds()
    // {
    //     //Renderer[] renderers = GetComponentsInChildren<Renderer>();
    //     Bounds bigbounds = new Bounds();
    //     
    //     Mesh[] meshes = GetComponentsInChildren<Mesh>();
    //     foreach (Mesh mesh in meshes)
    //     {
    //         bigbounds.Encapsulate(mesh.bounds);
    //     }
    //     
    //     
    //     //
    //     // foreach (Renderer renderer in renderers)
    //     // {
    //     //     bigbounds.Encapsulate(renderer.bounds);
    //     // }
    //
    //
    //     Vector3 size = bigbounds.size;
    //     size.x /= 2;
    //     size.z /= 2;
    //
    //     Vector3 center = bigbounds.center;
    //     center.x = 0;
    //     center.z = 0;
    //
    //     _runtimeCollider.size = size;
    //     _runtimeCollider.center = center;
    // }

    private Bounds CalculateBounds()
    {
        Quaternion currentRotation = this.transform.rotation;
        Vector3 currentScale = this.transform.localScale;
        
        transform.rotation = Quaternion.Euler(0f,0f,0f);
        Vector3 inverse = transform.lossyScale;
        inverse.x = 1 / inverse.x;
        inverse.y = 1 / inverse.y;
        inverse.z = 1 / inverse.z;

        scale = transform.lossyScale;


        transform.localScale = Vector3.Scale(transform.localScale,inverse);

        Bounds bounds = new Bounds(this.transform.position, Vector3.zero);
 
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
 
        Vector3 localCenter = bounds.center - this.transform.position;
        Vector3 localScale = (transform.localScale * -1);
        bounds.center = localCenter;
        bounds.size.Scale(localScale);

        transform.rotation = currentRotation;
        transform.localScale = currentScale;

        return bounds;
    }

    public Bounds GetRendererBounds()
    {
        return _renderBounds;
    }
}
