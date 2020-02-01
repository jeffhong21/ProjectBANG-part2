using System;
using UnityEngine;
using Object = UnityEngine.Object;
//using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SceneInfo
{
    [SerializeField] private Object sceneAsset;
    [SerializeField] private string sceneName = "";
    [SerializeField] private string scenePath = "";
    [SerializeField] private string fullScenePath = "";
    [SerializeField] private int buildIndex;


    public string SceneName
    {
        get { return sceneName; }
    }

    public string ScenePath{
        get { return scenePath; }
    }

    public string FullScenePath{
        get { return fullScenePath; }
    }

    public UnityEngine.SceneManagement.Scene Scene{
        get {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(fullScenePath);
            return scene;
        }
    }


    public int BuildIndex{
        get{
            buildIndex = UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(fullScenePath);
            return buildIndex;
        }
    }


    // makes it work with the existing Unity methods (LoadLevel/LoadScene)
    public static implicit operator string(SceneInfo sceneField)
    {
        return sceneField.SceneName;
    }
}



#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneInfo))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);
        var sceneAsset = property.FindPropertyRelative("sceneAsset");
        var sceneName = property.FindPropertyRelative("sceneName");
        var scenePath = property.FindPropertyRelative("scenePath");
        var fullScenePath = property.FindPropertyRelative("fullScenePath");
        var buildIndex = property.FindPropertyRelative("buildIndex");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        if (sceneAsset != null)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                sceneAsset.objectReferenceValue = value;
                if (sceneAsset.objectReferenceValue != null)
                {
                    var fullPath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                    var assetsIndex = fullPath.IndexOf("Assets", StringComparison.Ordinal) + 7;
                    var extensionIndex = fullPath.LastIndexOf(".unity", StringComparison.Ordinal);
                    var path = fullPath.Substring(assetsIndex, extensionIndex - assetsIndex);

                    sceneName.stringValue = path;
                    scenePath.stringValue = path;
                    fullScenePath.stringValue = fullPath;
                    buildIndex.intValue = UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(fullPath);
                    //scenePath.stringValue = "Assets/" + path + ".unity";


                }
            }
        }
        EditorGUI.EndProperty();
    }
}
#endif