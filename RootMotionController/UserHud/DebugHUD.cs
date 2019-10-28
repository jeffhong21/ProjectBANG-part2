using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JH.RootMotionController.RootMotionUI
{
    using RootMotionDebug;



    public class DebugHUD : SingletonMonoBehaviour<DebugHUD>
    {
        public static Color defaultColor = Color.white;

        public RootMotionController target;


        [SerializeField]
        private Text m_text;
        [SerializeField]
        private float updateInterval = 0.25f;
        private float lastUpdateTime = 0f;

        private int m_capacity = 2000;
        private StringBuilder m_debuglog = new StringBuilder(2000);

        private bool m_isActive;






        private void Start()
        {
            m_text = GetComponent<Text>();
            if (m_text == null) m_text = GetComponentInChildren<Text>();



            m_text.supportRichText = true;
            m_text.resizeTextForBestFit = true;
            m_text.resizeTextMinSize = 10;
            m_text.resizeTextMaxSize = 14;
        }






        void LateUpdate()
        {
            if (!m_isActive) return;

#if UNITY_EDITOR
            if (target == null && UnityEditor.Selection.activeGameObject.GetComponent<RootMotionController>()) {
                target = UnityEditor.Selection.activeGameObject.GetComponent<RootMotionController>();
            }
#endif
            lastUpdateTime += Time.deltaTime;

            /** Don't refresh at 60FPS; wasteful! */
            if (lastUpdateTime > updateInterval) {
                lastUpdateTime = 0;

                if (target == null) {
                    m_text.text = "[Unattached]";
                }
                else {
                    UpdateDebugLog();
                }
            }
        }


        public void SetActive(bool isActive)
        {
            //  Update log when being activated.
            if (!m_isActive && isActive) {
                UpdateDebugLog();
            }

            transform.gameObject.SetActive(isActive);
            m_isActive = isActive;


        }


        private void UpdateDebugLog()
        {
            if (m_debuglog == null) m_debuglog = new StringBuilder(m_capacity);
            m_debuglog.Clear();

            GetFields(target);
            GetProperties(target);

            //m_text.fontSize = AdjustFontSize();
            m_text.text = m_debuglog.ToString();

        }





        private void GetProperties(RootMotionController rmc)
        {
            PropertyInfo[] properties = rmc.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++) {
                PropertyInfo propertyInfo = properties[i];
                ObsoleteAttribute obsoleteAttr = propertyInfo.GetCustomAttribute<ObsoleteAttribute>(true);
                if(obsoleteAttr == null) {
                    DebugDisplayAttribute debugDisplayAttr = propertyInfo.GetCustomAttribute<DebugDisplayAttribute>(true);
                    if(debugDisplayAttr != null) {
                        GetColorValues(debugDisplayAttr, out string nameColor, out string valueColor);
                        m_debuglog.AppendFormat("<b><color={1}>[{0}]:</color></b> <color={3}>{2}</color>\n", propertyInfo.Name, nameColor, propertyInfo.GetValue(rmc, null), valueColor);
                    }

                }
            }
        }

        private void GetFields(RootMotionController rmc)
        {
            FieldInfo[] fields = rmc.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++) {
                FieldInfo fieldInfo = fields[i];
                DebugDisplayAttribute debugDisplayAttr = fieldInfo.GetCustomAttribute<DebugDisplayAttribute>(false);
                if (debugDisplayAttr != null) {
                    GetColorValues(debugDisplayAttr, out string nameColor, out string valueColor);
                    m_debuglog.AppendFormat("<b><color={1}>[{0}]:</color></b> <color={3}>{2}</color>\n", fieldInfo.Name, nameColor, fieldInfo.GetValue(rmc), valueColor);
                }

                ////ObsoleteAttribute obsoleteAttr = fieldInfo.GetCustomAttribute<ObsoleteAttribute>(true);
                //if (obsoleteAttr == null) {

                //}
            }
        }



        private void GetColorValues(DebugDisplayAttribute debugDisplayAttr, out string nameColor, out string valueColor)
        {
            nameColor = "#" + ColorUtility.ToHtmlStringRGBA(debugDisplayAttr.color);
            valueColor = "#";
            if (defaultColor == Color.white)
                valueColor += ColorUtility.ToHtmlStringRGBA(debugDisplayAttr.color.Darker());
            else if (defaultColor == Color.black)
                valueColor += ColorUtility.ToHtmlStringRGBA(debugDisplayAttr.color.Brighter());
            else
                valueColor += ColorUtility.ToHtmlStringRGBA(debugDisplayAttr.color);
        }

    }
}
