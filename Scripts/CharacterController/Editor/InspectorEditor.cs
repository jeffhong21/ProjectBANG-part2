using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CharacterController.ccEditor
{

    public class InspectorEditor : Editor
    {
        private SerializedProperty script;
        protected IEnumerable<FieldInfo> fields;
        protected HashSet<FieldInfo> groupedFields;
        protected Dictionary<string, List<FieldInfo>> cachedGroupFields;
        protected Dictionary<string, SerializedProperty> serializedPropertiesByFieldName;

        protected Dictionary<string, bool> groupExpanded = new Dictionary<string, bool>();


        protected virtual void OnEnable()
        {
            if (serializedObject == null) return;

            this.script = this.serializedObject.FindProperty("m_Script");

            //  Cache serialized fields.
            this.fields = PropertyUtility.GetAllFields(this.target, f => this.serializedObject.FindProperty(f.Name) != null);


            //  Check if there are any "specific" attributes.


            //  Cache all group attributes.
            this.groupedFields = new HashSet<FieldInfo>(this.fields.Where(f => f.GetCustomAttributes(typeof(GroupAttribute), true).Length > 0));
            // Cache grouped fields by group name
            this.cachedGroupFields = new Dictionary<string, List<FieldInfo>>();
            foreach (FieldInfo groupedField in this.groupedFields)
            {
                //  Get attribute group name.
                string groupName = (groupedField.GetCustomAttributes(typeof(GroupAttribute), true)[0] as GroupAttribute).Name;
                GroupAttribute attr = groupedField.GetCustomAttributes(typeof(GroupAttribute), true)[0] as GroupAttribute;

                if (this.cachedGroupFields.ContainsKey(groupName)){
                    this.cachedGroupFields[groupName].Add(groupedField);
                }
                else {
                    this.cachedGroupFields[groupName] = new List<FieldInfo>(){ groupedField };
                }
                //  Cache if expanded.
                if (!this.groupExpanded.ContainsKey(groupName)){
                    this.groupExpanded[groupName] = attr.Expanded;
                }

            }




            // Cache serialized properties by field name
            this.serializedPropertiesByFieldName = new Dictionary<string, SerializedProperty>();
            foreach (var field in this.fields)
            {
                this.serializedPropertiesByFieldName[field.Name] = this.serializedObject.FindProperty(field.Name);
            }
        }


        protected virtual void OnDisable()
        {
            
        }





        public override void OnInspectorGUI()
        {
            if (serializedObject == null) return;
            serializedObject.Update();

            if (this.script != null)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(this.script);
                GUI.enabled = true;
            }


            DrawGroups();



            this.serializedObject.ApplyModifiedProperties();
        }







        private void DrawGroups()
        {
            EditorGUILayout.BeginVertical();

            HashSet<string> drawnGroups = new HashSet<string>();
            foreach (var group in groupedFields)
            {
                if(this.groupedFields.Contains(group))
                {
                    //  Get group name.
                    string groupName = (group.GetCustomAttributes(typeof(GroupAttribute), true)[0] as GroupAttribute).Name;
                    if(!drawnGroups.Contains(groupName))
                    {
                        drawnGroups.Add(groupName);
                        
                        //   Draw foldout.
                        groupExpanded[groupName] = EditorGUILayout.Foldout(groupExpanded[groupName], groupName);
                        if (groupExpanded[groupName])
                        {
                            EditorGUI.indentLevel = 1;
                            foreach (var field in cachedGroupFields[groupName])
                            {
                                if(ShouldDrawField(field))
                                    InspectorUtility.PropertyField(this.serializedObject.FindProperty(field.Name), true);
                            }
                            EditorGUILayout.Space();
                            EditorGUI.indentLevel = 0;
                        }
                    }

                }



            }
            

            EditorGUILayout.EndVertical();
        }


        private bool ShouldDrawField(FieldInfo field)
        {
            if (serializedPropertiesByFieldName.ContainsKey(field.Name))
            {
                // Check if the field has HideInInspectorAttribute
                HideInInspector[] hideInInspectorAttributes = (HideInInspector[])field.GetCustomAttributes(typeof(HideInInspector), true);
                if (hideInInspectorAttributes.Length > 0)
                {
                    return false;
                }
            }

            return true;
        }



    }


}