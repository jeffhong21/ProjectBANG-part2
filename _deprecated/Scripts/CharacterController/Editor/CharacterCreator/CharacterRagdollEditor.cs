namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;



    [CustomEditor(typeof(CharacterRagdoll))]
    public class CharacterRagdollEditor : Editor
    {

        private CharacterRagdoll m_CharacterRagdoll;

        public override void OnInspectorGUI()
        {
            m_CharacterRagdoll = (CharacterRagdoll)target;
            DrawDefaultInspector();

            //GUILayout.Space(8);
            //if(GUILayout.Button("Update Character Joint Settings")){
            //    m_CharacterRagdoll.UpdateCharacterJoints();
            //}
        }


    }

}

