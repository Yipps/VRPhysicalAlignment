using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class AlignmentObject
{
   public string objectName;
   public string objectParentName;
   public Vector3 position;
   public Quaternion rotation;


   public AlignmentObject (GameObject go)
   {
      objectName = go.name;
      objectParentName = go.transform.parent ? go.transform.parent.name : "none";
      position = go.transform.position;
      rotation = go.transform.rotation;
   }

   protected bool Equals(AlignmentObject other)
   {
      return objectName == other.objectName && objectParentName == other.objectParentName && position.Equals(other.position) && rotation.Equals(other.rotation);
   }

   public override bool Equals(object obj)
   {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((AlignmentObject) obj);
   }
   
}
