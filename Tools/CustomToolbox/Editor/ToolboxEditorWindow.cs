using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;


namespace JH_Toolbox
{
    public class ToolboxEditorWindow : EditorWindow
    {
        private GUIContent m_title = new GUIContent("Toolbox");
        private Vector2 m_minSize = new Vector2(150, 300);




        [MenuItem("Tools/Toolbox/Toolbox Window", false, -1000)]
        public static void ShowWindow()
        {
            var window = GetWindow<ToolboxEditorWindow>();
            window.minSize = window.m_minSize;
            window.titleContent = window.m_title;



            window.Show();
        }



        protected void OnGUI()
        {

            OnDrawEditLayersTool();

        }


        private void OnDrawEditLayersTool()
        {
            GUILayout.BeginVertical("box");

            for (int index = 8; index <= 31; index++) {
                string layerName = InternalEditorUtility.GetLayerName(index);
                if (layerName.Length > 0)
                    EditorGUILayout.LabelField(index + " : " + layerName);
            }


            GUILayout.EndVertical();
        }



        //public bool Foldout(bool display, string title, int fontSize = 12)
        //{
        //    var style = new GUIStyle("ShurikenModuleTitle");
        //    style.font = new GUIStyle(EditorStyles.label).font;
        //    style.fontSize = fontSize;
        //    //style.fontStyle = FontStyle.Bold;
        //    style.border = new RectOffset(15, 7, 4, 4);
        //    style.fixedHeight = 22;
        //    style.contentOffset = new Vector2 (20f, -2f);

        //    var rect = GUILayoutUtility.GetRect(16f, 22f, style);
        //    GUI.Box(rect, title, style);

        //    var e = Event.current;

        //    var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        //    if (e.type == EventType.Repaint)
        //    {
        //        EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        //    }

        //    if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        //    {
        //        display = !display;
        //        e.Use();
        //    }

        //    return display;
        //}
    }

}
