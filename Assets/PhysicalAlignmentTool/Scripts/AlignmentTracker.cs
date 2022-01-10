using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class AlignmentTracker : MonoBehaviour
{
    private bool _isMovable;
    private BoxCollider _runtimeCollider;
    private PhysicalAlignmentTool physicalAlignmentTool;
    private Bounds renderBounds;

    void Start()
    {
        physicalAlignmentTool = (PhysicalAlignmentTool)AssetDatabase.LoadAssetAtPath("Assets/PhysicalAlignmentTool/Scripts/PhysicalAlignmentTool.asset",typeof(PhysicalAlignmentTool));
        
        if (!GetComponent<Collider>())
        {
            //Make a min size for this if there are no renderers
            _runtimeCollider = gameObject.AddComponent<BoxCollider>();

            renderBounds = CalculateBounds();
            _runtimeCollider.size = renderBounds.size;
            _runtimeCollider.center = renderBounds.center;
        }
        
        //Add self to alignmenttool list, if we fail delete this component
        if (!physicalAlignmentTool.AddObject(gameObject))
        {
            Debug.LogError("Couldn't serialize tracker, deleting alignment tracker component");
            
            if (_runtimeCollider)
                Destroy(_runtimeCollider);
            Destroy(this);
        }
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
        transform.rotation = Quaternion.Euler(0f,0f,0f);
 
        Bounds bounds = new Bounds(this.transform.position, Vector3.zero);
 
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
 
        Vector3 localCenter = bounds.center - this.transform.position;
        bounds.center = localCenter;

        transform.rotation = currentRotation;

        return bounds;
    }

    public Bounds GetRendererBounds()
    {
        return renderBounds;
    }
}
