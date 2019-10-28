//namespace CharacterController
//{
//    using UnityEngine;
//    using UnityEditor;


//    [CustomEditor(typeof(ThirdPersonCameraController))]
//    public class CameraControllerEditor : Editor
//    {

//        private ThirdPersonCameraController m_controller;
//        private SerializedObject m_CameraState;

//        private bool m_CameraStateToggle;



//        private void OnEnable()
//        {
//            if (target == null) return;
//            m_controller = (ThirdPersonCameraController)target;

//            m_CameraStateToggle = serializedObject.FindProperty("m_CameraStateToggle").boolValue;
//        }



//        public override void OnInspectorGUI()
//        {
//            DrawDefaultInspector();
//            serializedObject.Update();


//            GUILayout.Space(12);
//            m_CameraStateToggle = EditorGUILayout.ToggleLeft(new GUIContent("Show Camera States Options"), m_CameraStateToggle);
//            if(m_CameraStateToggle)
//            {
//                if(m_controller.ActiveState != null){
//                    if(m_CameraState == null){
//                        m_CameraState = new SerializedObject(m_controller.ActiveState);
//                    }
//                    SerializedProperty propertyIterator = m_CameraState.GetIterator();
//                    while (propertyIterator.NextVisible(true))
//                    {
//                        EditorGUILayout.PropertyField(propertyIterator);
//                    }

//                    m_CameraState.ApplyModifiedProperties();
//                    GUILayout.Space(12);
//                }

//            }



//            serializedObject.ApplyModifiedProperties();
//        }






//    }

//}

