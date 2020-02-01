using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public class LayerEditor
{
    private Dictionary<string, int> m_LayersDictionary = new Dictionary<string, int>();



    public void DrawOnGUI()
    {

    }





    private Dictionary<string, int> GetAllLayers()
    {
        Dictionary<string, int> layersDictionary = new Dictionary<string, int>();

        var arr = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (arr == null || arr.Length == 0){
            Debug.LogWarning("Unable to set up default layers, please ensure that layer mapping is properly set.");
            return layersDictionary;
        }
        SerializedObject tagManagerAsset = new SerializedObject(arr[0]);
        SerializedProperty layers = tagManagerAsset.FindProperty("layers");

        int layerSize = layers.arraySize;

        for (int index = 0; index < layerSize; index++)
        {
            SerializedProperty element = layers.GetArrayElementAtIndex(index);
            string layerName = element.stringValue;

            if(string.IsNullOrEmpty(layerName) == false)
            {
                layersDictionary.Add(layerName, index);
            }
        }

        return layersDictionary;
    }




    private void UpdateAllLayers()
    {
        for (int index = 8; index <= 31; index++)
        {
            string layerName = InternalEditorUtility.GetLayerName(index);
            if (layerName.Length > 0)
                m_LayersDictionary.Add(layerName, index);
        }
    }



}
