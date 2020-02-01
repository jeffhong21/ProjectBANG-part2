using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;

namespace DebugUI
{

    public class DebugUIView : SingletonMonoBehaviour<DebugUIView>
    {
        [SerializeField]
        private Text m_messageLogView;
        [SerializeField]
        private int m_refreshRate;
        [SerializeField]
        private float m_refreshRateinSeconds = 1f;

        private int m_elapsedFrames;
        private float m_elapsedTime;
        private GameObject m_gameObject;
        private float m_deltaTime;


        protected override void OnAwake()
        {
            base.OnAwake();

            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }


        private void Start()
        {
            m_deltaTime = Time.deltaTime;
            m_gameObject = gameObject;
            m_messageLogView.supportRichText = true;
        }





        private void LateUpdate()
        {
            if (!m_gameObject.activeSelf) return;

            if(m_elapsedTime >= m_refreshRateinSeconds) {
                m_messageLogView.text = DebugUI.WritePropertyMessages();
                m_elapsedTime = 0;
            } else {
                m_elapsedTime += m_deltaTime;
            }

            if(m_elapsedFrames >= m_refreshRate)
            {
                m_messageLogView.text = DebugUI.WritePropertyMessages();
                m_elapsedFrames = 0;
            }
            else
            {
                m_elapsedFrames++;
            }
        }






    }



}