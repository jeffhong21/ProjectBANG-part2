using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class AnimationImportSettingsEditor : EditorWindow
{
    private enum RootTransformType { Rotation, PositionY, PositionXZ };

    public static AnimationImportSettingsEditor ActiveInstance;

    private GUIContent m_Title = new GUIContent("Animation Import Settings");
    private GUIContent m_RootRotationHeader = new GUIContent("Root Transform Rotation");
    private GUIContent m_RootPositionYHeader = new GUIContent("Root Transform Position (Y)");
    private GUIContent m_RootPositionXZHeader = new GUIContent("Root Transform Position (XZ)");


    private ModelImporterAnimationType m_AnimationType = ModelImporterAnimationType.Human;
    private Avatar m_SourceAvatar;

    private bool m_UpdateNameOnly;
    private bool m_Loop = false;
    private bool m_LoopTime = false;
    private bool m_LoopPose = false;
    //   (Bake into Pose) Enables root motion be baked into the movement of the bones. Disable to make root motion be stored as root motion.
    private bool m_LockRootRotation = false;
    private bool m_LockRootHeightY = true;
    private bool m_LockRootPositionXZ = false;
    //   (Original or Center Mass / Body Orientation) Keeps the vertical position as it is authored in the source file.
    private bool m_KeepOriginalOrientation = true;
    private bool m_KeepOriginalPositionY = true;
    private bool m_KeepOriginalPositionXZ = true;
    //  Keeps the feet aligned with the root transform position.
    private bool m_HeightFromFeet = false;




    private bool m_DebugShowVariables;


    [MenuItem("Tools/Animation Import Settings")]
    public static void ShowWindow()
    {
        //  Initialize window
        ActiveInstance = GetWindow<AnimationImportSettingsEditor>();
        ActiveInstance.minSize = new Vector2(330f, 360f);
        ActiveInstance.maxSize = new Vector2(600f, 4000f);
        ActiveInstance.titleContent = ActiveInstance.m_Title;
        //  Show window
        ActiveInstance.Show();
    }


	private void OnGUI()
	{

        DrawImportSettings();
	}


    private void DrawImportSettings()
    {
        Rect rect = new Rect(4, 2, position.width - 4, position.height);
        GUILayout.BeginArea(rect);
        using (new GUILayout.VerticalScope())
        {

            GUILayout.Label(string.Format("Looping Animation: {0}", m_Loop));
            m_LoopTime = GUILayout.Toggle(m_LoopTime, "Loop Time");

            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup( !m_LoopTime);
            m_LoopPose = GUILayout.Toggle(m_LoopPose, "Loop Pose");
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;


            //  ROTATION
            GUILayout.Label(m_RootRotationHeader);
            m_LockRootRotation = GUILayout.Toggle(m_LockRootRotation, "Bake Into Pose");

            EditorGUI.indentLevel++;
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(new GUIContent("Based Upon (at Start)"));

                if (GUILayout.Button(m_KeepOriginalOrientation ? "Original" : "Body Orientation", EditorStyles.toolbarDropDown))
                {
                    GenericMenu toolsMenu = new GenericMenu();
                    toolsMenu.AddItem(new GUIContent("Original"), false, () => ToggleOriginalPositions(out m_KeepOriginalOrientation, true) );
                    toolsMenu.AddItem(new GUIContent("Body Orientation"), false, () => ToggleOriginalPositions(out m_KeepOriginalOrientation, false));
                    toolsMenu.ShowAsContext();
                }
            }

            if(m_DebugShowVariables){
                EditorGUI.BeginDisabledGroup(true);
                m_KeepOriginalOrientation = GUILayout.Toggle(m_KeepOriginalOrientation, "Keep Original Orientation");
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }



            //  POSITON Y

            GUILayout.Label(m_RootPositionYHeader);
            m_LockRootHeightY = GUILayout.Toggle(m_LockRootHeightY, "Bake Into Pose");

            EditorGUI.indentLevel++;
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(new GUIContent("Based Upon (at Start)"));

                if (GUILayout.Button(m_KeepOriginalPositionY ? "Original" : "Center of Mass", EditorStyles.toolbarDropDown))
                {
                    GenericMenu toolsMenu = new GenericMenu();
                    toolsMenu.AddItem(new GUIContent("Original"), false, () => ToggleOriginalPositionY(out m_KeepOriginalPositionY, true, false));
                    toolsMenu.AddItem(new GUIContent("Center Of Mass"), false, () => ToggleOriginalPositionY(out m_KeepOriginalPositionY, false, false));
                    toolsMenu.AddItem(new GUIContent("Feet"), false, () => ToggleOriginalPositionY(out m_HeightFromFeet, true, true));
                    toolsMenu.ShowAsContext();
                }
            }


            if (m_DebugShowVariables)
            {
                EditorGUI.BeginDisabledGroup(true);
                m_KeepOriginalPositionY = GUILayout.Toggle(m_KeepOriginalPositionY, "Keep Original PositionY");
                m_HeightFromFeet = GUILayout.Toggle(m_HeightFromFeet, "Height From Feet");
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }



            //  POSITON XZ

            GUILayout.Label(m_RootPositionXZHeader);
            m_LockRootPositionXZ = GUILayout.Toggle(m_LockRootPositionXZ, "Bake Into Pose");

            EditorGUI.indentLevel++;
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(new GUIContent("Based Upon"));

                if (GUILayout.Button(m_KeepOriginalPositionXZ ? "Original" : "Center of Mass", EditorStyles.toolbarDropDown))
                {
                    GenericMenu toolsMenu = new GenericMenu();
                    toolsMenu.AddItem(new GUIContent("Original"), false, () => ToggleOriginalPositions(out m_KeepOriginalPositionXZ, true));
                    toolsMenu.AddItem(new GUIContent("Center Of Mass"), false, () => ToggleOriginalPositions(out m_KeepOriginalPositionXZ, false));
                    toolsMenu.ShowAsContext();
                }
            }


            if (m_DebugShowVariables)
            {
                EditorGUI.BeginDisabledGroup(true);
                m_KeepOriginalPositionXZ = GUILayout.Toggle(m_KeepOriginalPositionXZ, "Keep Original PositionY");
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }



            GUILayout.FlexibleSpace();
            m_UpdateNameOnly = GUILayout.Toggle(m_UpdateNameOnly, "Update Name Only");
            if (GUILayout.Button("Apply")){
                ProcessAnimationClipNames();
            }
            GUILayout.Space(8);
        }

        GUILayout.EndArea();
    }



    private void ToggleOriginalPositionY(out bool keepOriginal, bool value, bool isSecondaryValue)
    {
        keepOriginal = value;
        //  If is not secondary value.
        if(!isSecondaryValue){
            m_HeightFromFeet = false;
        }
    }

    private void ToggleOriginalPositions(out bool keepOriginal, bool keepOriginalValue)
    {
        keepOriginal = keepOriginalValue;

    }




	public void ProcessAnimationClipNames()
    {
        var settings = new AnimationImportSettingsEditor();
        var selection = Selection.gameObjects;

        for (int i = 0; i < selection.Length; i++)
        {
            var asset = selection[i];
            //Debug.Log(asset.name);
            ProcessModel(asset);

        }
    }


    public void ProcessModel(GameObject root)
    {
        string path = AssetDatabase.GetAssetPath(root);
        ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;

        if (!modelImporter){
            Debug.Log("No Model Importer");
            return;
        }

        //ProcessModelRig(modelImporter);
        ProcessAnimationNames(modelImporter, root);
    }


    public void ProcessModelRig(ModelImporter modelImporter)
    {
        if (m_SourceAvatar == null){
            Debug.Log("No Avatar");
            return;
        }

        modelImporter.animationType = m_AnimationType;
        modelImporter.sourceAvatar = m_SourceAvatar;
    }



    public void ProcessAnimationNames(ModelImporter modelImporter, GameObject root)
    {
        //  Grab all model clips.
        ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;
        //  Grab the first clip.
        ModelImporterClipAnimation clipAnimation = clipAnimations[0];


        clipAnimation.name = ProcessName(root.name);
        if(m_UpdateNameOnly == false)
        {
            clipAnimation.loop = m_Loop;
            clipAnimation.loopTime = m_LoopTime;
            clipAnimation.loopPose = m_LoopPose;

            clipAnimation.lockRootRotation = m_LockRootRotation;
            clipAnimation.lockRootHeightY = m_LockRootHeightY;
            clipAnimation.lockRootPositionXZ = m_LockRootPositionXZ;

            clipAnimation.keepOriginalOrientation = m_KeepOriginalOrientation;
            clipAnimation.keepOriginalPositionY = m_KeepOriginalPositionY;
            clipAnimation.keepOriginalPositionXZ = m_KeepOriginalPositionXZ;

            if( m_HeightFromFeet){
                clipAnimation.keepOriginalPositionY = false;
                clipAnimation.heightFromFeet = m_HeightFromFeet;
            } 
        }

        //Debug.LogFormat("\"{0}\" {1}", clipAnimation.name, clipAnimation.m_Loop ? "does m_Loop" : "does NOT m_Loop");
        modelImporter.clipAnimations = clipAnimations;
        // Save
        modelImporter.SaveAndReimport();
    }



    private string ProcessName(string rootName)
    {

        char[] delimiter = "@".ToCharArray();
        string[] parts = rootName.Split(delimiter, 2);
        string newName = parts.Length > 1 ? parts[1] : rootName;

        //int index = rootName.IndexOf("@", System.StringComparison.CurrentCulture);
        //newName = index > 0 ? rootName.Substring(index + 1) : rootName;

        return newName;
    }



    //private ModelImporterClipAnimation GetModelImporterClip(ModelImporter mi)
    //{
    //    ModelImporterClipAnimation clip = null;
    //    if (mi.clipAnimations.Length == 0)
    //    {
    //        //if the animation was never manually changed and saved, we get here. Check defaultClipAnimations
    //        if (mi.defaultClipAnimations.Length > 0)
    //        {
    //            clip = mi.defaultClipAnimations[0];
    //        }
    //        else
    //        {
    //            Debug.LogError("GetModelImporterClip can't find clip information");
    //        }
    //    }
    //    else
    //    {
    //        clip = mi.clipAnimations[0];
    //    }
    //    return clip;
    //}
}