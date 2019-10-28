//namespace Apex.SurvivalShooter
//{
//    using UnityEditor;
//    using UnityEngine;

//    [InitializeOnLoad]
//    public sealed class LayerInit
//    {
//        static LayerInit()
//        {
//            EnsureLayers();
//        }

//        private static void EnsureLayers()
//        {
//            var arr = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
//            if (arr == null || arr.Length == 0)
//            {
//                Debug.LogWarning("Unable to set up default layers, please ensure that layer mapping is properly set.");
//                return;
//            }

//            var tagManagerAsset = new SerializedObject(arr[0]);

//            EnsureLayer("Floor", tagManagerAsset);
//            EnsureLayer("Cover", tagManagerAsset);
//            EnsureLayer("LightMask", tagManagerAsset);
//            EnsureLayer("Invisible", tagManagerAsset);
//            EnsureLayer("Powerup", tagManagerAsset);
//            EnsureLayer("Player", tagManagerAsset);
//            EnsureLayer("Enemies", tagManagerAsset);
//            EnsureLayer("Damageable", tagManagerAsset);


//        }

//        private static int EnsureLayer(string name, SerializedObject tagManagerAsset)
//        {
//#if UNITY_5
//            SerializedProperty layersProp = tagManagerAsset.FindProperty("layers");
//#endif
//            int firstVacant = -1;
//            SerializedProperty firstVacantProp = null;
//            for (int i = 8; i <= 31; i++)
//            {
//#if UNITY_5
//                var sp = layersProp.GetArrayElementAtIndex(i);
//#else
//                var layerPropName = "User Layer " + i;
//                var sp = tagManagerAsset.FindProperty(layerPropName);
//#endif
//                if (sp == null)
//                {
//                    continue;
//                }
//                else if (name.Equals(sp.stringValue))
//                {
//                    return 1 << i;
//                }
//                else if (firstVacant < 0 && string.IsNullOrEmpty(sp.stringValue))
//                {
//                    firstVacant = i;
//                    firstVacantProp = sp;
//                }
//            }

//            if (firstVacant < 0)
//            {
//                Debug.LogWarning(string.Format("Unable to add a {0} layer, please ensure that the Layer Mapping on the Game World is correct.", name));
//                return 0;
//            }
//            else
//            {
//                Debug.Log(string.Format("Added a {0} layer.", name));
//                firstVacantProp.stringValue = name;
//                tagManagerAsset.ApplyModifiedProperties();

//                return 1 << firstVacant;
//            }
//        }
//    }
//}
