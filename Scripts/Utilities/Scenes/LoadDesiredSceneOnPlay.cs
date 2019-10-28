//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEditor;
//using UnityEditor.SceneManagement;

//using System.Collections;


///// <summary>
///// Load scene on pressing play.  Probably best to create a scriptable object to store the playModeStartScene.
///// </summary>
//[InitializeOnLoad]
//public static class LoadDesiredSceneOnPlay
//{
//    public static bool debugMessages;

//    public static string originalScene;

//    //public static Scene

//    static LoadDesiredSceneOnPlay()
//    {
//        EditorApplication.playModeStateChanged += StateChange;
//    }

//    static void StateChange(PlayModeStateChange state)
//    {
//        if (EditorApplication.isPlaying)
//        {
//            EditorApplication.playModeStateChanged -= StateChange;
//            if (!EditorApplication.isPlayingOrWillChangePlaymode)
//            {
//                //We're in playmode, just about to change playmode to Editor
//                Debug.Log("Loading original level");
//                //EditorSceneManager.OpenScene(originalScene);
//            }
//            else
//            {
//                //We're in playmode, right after having pressed Play
//                //originalScene = EditorApplication.currentScene;
//                //originalScene = SceneManager.GetActiveScene().name;

//                Debug.Log("Loading custom level");

//            }
//        }
//    }


//    public static void SetPlayModeStartScene(string scenePath)
//    {
//        SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
//        if (myWantedStartScene != null)
//            //EditorSceneManager.playModeStartScene = myWantedStartScene;
//            Debug.Log("Playmode start Scene is " + scenePath);
//        else
//            Debug.Log("Could not find Scene " + scenePath);
//    }







//}