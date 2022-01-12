using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentToolManager : MonoBehaviour
{
    [SerializeField]
    private PhysicalAlignmentTool _physicalAlignmentTool;
    
    public GameObject[] trackedObjects;

    private void Start()
    {
        foreach (GameObject obj in trackedObjects)
        {
            obj.AddComponent<AlignmentTracker>();
        }
    }

    public void Save()
    {
        _physicalAlignmentTool.SaveAlignment();
    }

    public void Load()
    {
        
    }

    private void OnDisable()
    {
        Save();
    }
}
