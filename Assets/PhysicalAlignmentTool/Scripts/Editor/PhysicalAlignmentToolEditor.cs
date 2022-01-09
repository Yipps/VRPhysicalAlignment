using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhysicalAlignmentTool))]
public class PhysicalAlignmentToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PhysicalAlignmentTool physicalAlignmentTool = (PhysicalAlignmentTool) target;
        if (GUILayout.Button("Save Alignment (Runtime)"))
        {
            physicalAlignmentTool.SaveAlignment();
        }

        if (GUILayout.Button("Load Alignment"))
        {
            physicalAlignmentTool.LoadAlignment();
        }

    }
}
