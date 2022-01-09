using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomWindow : EditorWindow
{
    [MenuItem(itemName: "PhysicalAlignmentTool", menuItem = "Tool/Alignment Tool")]
    public static void Init()
    {
        GetWindow<CustomWindow>("Physical Alignment Tool", true);
    }

    [SerializeField] 
    List<GameObject> flaggedObjects = new List<GameObject>();

    private Editor _editor;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
    }

    void OnGUI()
    {
        // if (!_editor)
        // {
        //     _editor = Editor.CreateEditor(this);
        // }
        //
        // if (_editor)
        // {
        //     _editor.OnInspectorGUI();
        //     Debug.Log(listTest.Count);
        // }

        //ScriptableObject target = this;
        SerializedObject so = new SerializedObject(this);
        SerializedProperty list = so.FindProperty("flaggedObjects");
        
        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.PropertyField(list, new GUIContent("Objects flagged to be move"), true);

        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("Change occured");
        }
        so.ApplyModifiedProperties();
    }

    // [CustomEditor(typeof(CustomWindow), true)]
    // public class ListTestEditorDrawer : Editor
    // {
    //     public override void OnInspectorGUI()
    //     {
    //         var list = serializedObject.FindProperty("listTest");
    //         serializedObject.Update();
    //         EditorGUILayout.PropertyField(list, new GUIContent("Nani how does this work"));
    //         serializedObject.ApplyModifiedProperties();
    //     }
    // }
}
