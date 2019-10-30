#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Editor Extension that allows for Hiding/Showing of objects in Unity
/// </summary>
public class HideObject : Editor
{
    [MenuItem("GameObject/Hide Objects/Hide Selected Objects")]
    static void HideSelection()
    {
        foreach (Object o in Selection.objects)
        {
            if (o.GetType() == typeof(GameObject) && PrefabUtility.GetPrefabParent(o) == null)
            {
                ((GameObject)o).hideFlags = ((GameObject)o).hideFlags | HideFlags.HideInHierarchy;
            }
        }
    }

    [MenuItem("GameObject/Hide Objects/Unhide All")]
    static void ShowAll()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject g in allObjects)
        {
            g.hideFlags = g.hideFlags & ~HideFlags.HideInHierarchy;
        }
    }
}
#endif