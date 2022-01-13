using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "PhysicalAlignmentData")]
public class PhysicalAlignmentTool : ScriptableObject
{
    public string saveName = "AlignmentSave";
    
    //Cant serialize runtime sets
    private readonly List<GameObject> flaggedObjects = new List<GameObject>();
    
    //Used for debugging 
    [SerializeField]
    private AlignmentObject[] _alignmentObjects;
    
    
    [SerializeField]
    private string jsonDebug;
    //public AlignmentObject[] loaded;

    public void SaveAlignment()
    {
        _alignmentObjects = CreateDataArray();
        String json = JsonHelper.ToJson(_alignmentObjects);
        jsonDebug = json;
        
        WriteJsonToDisk(json);
    }

    private AlignmentObject[] CreateDataArray()
    {
        List<AlignmentObject> alignmentObjects = new List<AlignmentObject>();
        
        foreach (GameObject flaggedObject in flaggedObjects)
        {
            AlignmentObject newObject = new AlignmentObject(flaggedObject);
            alignmentObjects.Add(newObject);
        }
        return alignmentObjects.ToArray();
    }

    private void WriteJsonToDisk(String json)
    {
        string path = Application.persistentDataPath + "/" + saveName + ".json";
        Debug.Log(path);
        System.IO.File.WriteAllText(path,json);
    }
    
    public bool AddObject(GameObject gameObject)
    {
        if (flaggedObjects.Contains(gameObject))
        {
            return false;
        }
        else
        {
            flaggedObjects.Add(gameObject);
            return true;
        }
    }

    private void OnDisable()
    {
        flaggedObjects.Clear();
    }

    public void LoadAlignment()
    {
        string path = Application.persistentDataPath + "/" + saveName + ".json";
        
        if (File.Exists(path))
        {
            //Get json string
            String data = File.ReadAllText(path);
            //Deserialize json
            AlignmentObject[] alignmentData = JsonHelper.FromJson<AlignmentObject>(data);

            //iterate through all data objects
            foreach (AlignmentObject alignmentObject in alignmentData)
            {
                string name = alignmentObject.objectName;
                //Add parent name for hierarchy search
                if (alignmentObject.objectParentName != "none")
                    name = alignmentObject.objectParentName + "/" + alignmentObject.objectName;
                GameObject obj = GameObject.Find(name);
                
                //Set transforms
                obj.transform.position = alignmentObject.position;
                obj.transform.rotation = alignmentObject.rotation;
                
            }
        }
        else
        {
            Debug.LogError("Json not found at " + path);
        }
    }
    

}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
