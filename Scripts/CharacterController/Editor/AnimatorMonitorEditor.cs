namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;

    [CustomEditor(typeof(AnimatorMonitor))]
    public class AnimatorMonitorEditor : Editor
    {

        private AnimatorMonitor m_AnimatorMonitor;
        private Animator m_Animator;



        private SerializedProperty State1_Prop;

        private int _layerIndex = 0;

        private void OnEnable()
        {
            if (target == null) return;
            m_AnimatorMonitor = (AnimatorMonitor)target;
            m_Animator = m_AnimatorMonitor.GetComponent<Animator>();


        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            if(Application.isPlaying)
                EditorGUILayout.HelpBox(GetAnimatorStateInfo(_layerIndex), MessageType.None);

            GUILayout.Space(8);
            if (GUILayout.Button("Register Animator States")){
                m_AnimatorMonitor.RegisterAllAnimatorStateIDs(true);
            }
            //if (GUILayout.Button("Change Parameter"))
            //{
            //    m_AnimatorMonitor.ChangeParameter("LegUpIndex1", "LegUpIndex");
            //}


            //EditorGUILayout.LabelField(string.Format("----- Play States (isPlaying: {0})-----", Application.isPlaying));
            //AnimatorStateDataInspector(serializedObject.FindProperty("State_1"));
            //AnimatorStateDataInspector(serializedObject.FindProperty("State_2"));



            serializedObject.ApplyModifiedProperties();
        }


        private void AnimatorStateDataInspector(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property);

            SerializedProperty stateName = property.FindPropertyRelative("stateName");
            int stateID = Animator.StringToHash(stateName.stringValue);

            SerializedProperty transitionDuration = property.FindPropertyRelative("transitionDuration");

            if(Application.isPlaying)
            {
                if (GUILayout.Button(" Play " + stateName.stringValue))
                {
                    if (m_Animator.HasState(_layerIndex, stateID))
                    {
                        if (transitionDuration.floatValue == 0)
                            m_Animator.Play(stateID, 0);
                        else
                            m_Animator.CrossFade(stateID, transitionDuration.floatValue, 0);
                    }
                    else
                    {
                        Debug.LogFormat("<b>{0} does not exist</b>", stateName.stringValue);
                    }
                }
            }


        }



        private string GetAnimatorStateInfo()
        {
            string info = "";


            for (int index = 0; index < m_Animator.layerCount; index++)
            {
                var animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(index);
                var nextAnimatorStateInfo = m_Animator.GetNextAnimatorStateInfo(index);
                var transitionStateInfo = m_Animator.GetAnimatorTransitionInfo(index);
                info += "<b>" + m_Animator.GetLayerName(index) + "</b>\n";
                info += "° Current State: " + m_AnimatorMonitor.GetStateName(animatorStateInfo.fullPathHash) + "\n";
                info += "° Duration: " + animatorStateInfo.length + "| NormalizedTime: " + animatorStateInfo.normalizedTime + "\n";
                info += "       --------------- \n";
                info += "       ° Next State: " + m_AnimatorMonitor.GetStateName(nextAnimatorStateInfo.fullPathHash) + "\n";
                if (m_Animator.IsInTransition(index))
                {
                    info += "       --------------- \n";
                    info += "       ° Transition Normalized Time: " + transitionStateInfo.normalizedTime + "\n";
                }

            }


            return info;
        }

        private string GetAnimatorStateInfo(int layerIndex)
        {
            string info = "";


            var animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(layerIndex);
            var nextAnimatorStateInfo = m_Animator.GetNextAnimatorStateInfo(layerIndex);
            var transitionStateInfo = m_Animator.GetAnimatorTransitionInfo(layerIndex);
            info += "<b>" + m_Animator.GetLayerName(layerIndex) + "</b>\n";
            info += "° Current State: " + m_AnimatorMonitor.GetStateName(animatorStateInfo.fullPathHash) + "\n";
            info += "° Duration: " + animatorStateInfo.length + "| NormalizedTime: " + animatorStateInfo.normalizedTime + "\n";
            info += "       --------------- \n";
            info += "       ° Next State: " + m_AnimatorMonitor.GetStateName(nextAnimatorStateInfo.fullPathHash) + "\n";
            if (m_Animator.IsInTransition(layerIndex))
            {
                info += "       --------------- \n";
                info += "       ° Transition Normalized Time: " + transitionStateInfo.normalizedTime + "\n";
            }


            return info;
        }
    }

}
