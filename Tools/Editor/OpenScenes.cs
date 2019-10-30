using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

[InitializeOnLoad]
public class OpenScenes
{
    private readonly static string[] SceneSearchFolders = { "Assets/Scenes" };

    //private static Dictionary<string, SceneAsset> allSceneAssets = new Dictionary<string, SceneAsset>();
    //private static Dictionary<string, Scene> allScenes = new Dictionary<string, Scene>();



    //static OpenScenes()
    //{
    //    RegisterAllScenes();


    //    EditorSceneManager.newSceneCreated -= RegisterAllScenes;
    //    EditorSceneManager.newSceneCreated += RegisterAllScenes;
    //}





    [MenuItem("Scenes/Prefab Edit Environment", false, -1010)]
    public static void OpenPrefabEditEnvironmentScene()
    {
        string path = "Assets/Scenes/_Prefab_Edit_Environment/PEE.unity";
        OpenScene(path);
    }

    [MenuItem("Scenes/UI Setup", false, -1009)]
    public static void OpenUISetupScene()
    {
        string path = "Assets/Scenes/UISetupScene.unity";
        OpenScene(path);
    }

    [MenuItem("Scenes/Prototype Environment Scene", false, 101)]
    public static void OpenPrototypeEnvironmentScene()
    {
        string path = "Assets/Scenes/Prototype_Environment/Prototype_Environment.unity";
        OpenScene(path);
    }






    [MenuItem("Scenes/Western Frontier Town Scene", false, 201)]
    public static void WesternDemoScene()
    {
        string path = "Assets/Scenes/Western Frontier Town/Western Frontier Town.unity";
        OpenScene(path);
    }


    [MenuItem("Scenes/Western Frontier Town Dusk Scene", false, 201)]
    public static void WesternDemoScene_Dusk()
    {
        string path = "Assets/Scenes/Western Frontier Town/Western Frontier Town Dusk.unity";
        OpenScene(path);
    }

    [MenuItem("Scenes/Western Frontier Town Scene-Organized", false, 210)]
    public static void WesternDemoSceneBase()
    {
        string path = "Assets/Scenes/Western Frontier Town/Western Demo Scene-Organized.unity";
        OpenScene(path);
    }



    private static void OpenScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
        EditorSceneManager.OpenScene(path);
    }
}
