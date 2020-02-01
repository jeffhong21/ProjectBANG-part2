//   https://forum.unity.com/threads/free-look-camera-and-mouse-responsiveness.642886/

namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;
    using System;
    using System.Collections;
    using Cinemachine;

    
    public class CMFreeLook : MonoBehaviour
    {

        [Tooltip("The minimum scale for the orbits")]
        [Range(0.01f, 1f)]
        public float minScale = 0.5f;
        [Tooltip("The maximum scale for the orbits")]
        [Range(1F, 5f)]
        public float maxScale = 1;
        [SerializeField]
        private CinemachineFreeLook m_cmFreeLook;
        private CinemachineFreeLook.Orbit[] m_originalOrbits;

        [Tooltip("The zoom axis.  Value is 0..1.  How much to scale the orbits")]
        [AxisStateProperty]
        public AxisState zAxis = new AxisState(0, 1, false, true, 50f, 0.1f, 0.1f, "Mouse ScrollWheel", false);


        [Tooltip("This depends on your Free Look rigs setup, use to correct Y sensitivity,"
            + " about 1.5 - 2 results in good Y-X square responsiveness")]
        public float yCorrection = 2f;

        private float xAxisValue;
        private float yAxisValue;



        public void Initialize(CinemachineFreeLook freeLook)
        {
            m_cmFreeLook = freeLook;
            if (m_cmFreeLook != null)
            {
                m_originalOrbits = new CinemachineFreeLook.Orbit[m_cmFreeLook.m_Orbits.Length];
                for (int i = 0; i < m_cmFreeLook.m_Orbits.Length; i++)
                {
                    m_originalOrbits[i].m_Height = m_cmFreeLook.m_Orbits[i].m_Height;
                    m_originalOrbits[i].m_Radius = m_cmFreeLook.m_Orbits[i].m_Radius;
                }
#if UNITY_EDITOR
                SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
                SaveDuringPlay.SaveDuringPlay.OnHotSave += RestoreOriginalOrbits;
#endif
            }

        }

#if UNITY_EDITOR
        private void OnDestroy()
        {
            SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
        }

        private void RestoreOriginalOrbits()
        {
            if (m_originalOrbits != null)
            {
                for (int i = 0; i < m_originalOrbits.Length; i++)
                {
                    m_cmFreeLook.m_Orbits[i].m_Height = m_originalOrbits[i].m_Height;
                    m_cmFreeLook.m_Orbits[i].m_Radius = m_originalOrbits[i].m_Radius;
                }
            }
        }
#endif



        public void UpdateInput(float mouseX, float mouseY)
        {
            // Correction for Y
            mouseY /= 360f;
            mouseY *= yCorrection;

            xAxisValue += mouseX;
            yAxisValue = Mathf.Clamp01(yAxisValue - mouseY);

            m_cmFreeLook.m_XAxis.Value = xAxisValue;
            m_cmFreeLook.m_YAxis.Value = yAxisValue;
        }


        /// <summary>
        /// Allows us to zoom with FreeLook camera.
        /// </summary>
        /// <param name="minScale"></param>
        /// <param name="maxScale"></param>
        public void UpdateOrbit(float minScale = 0.5f, float maxScale = 1)
        {
            minScale = Mathf.Clamp(minScale, 0.01f, 1f);
            maxScale = Mathf.Clamp(maxScale, 1f, 5f);

            if (m_originalOrbits != null)
            {
                zAxis.Update(Time.deltaTime);
                float scale = Mathf.Lerp(minScale, maxScale, zAxis.Value);
                for (int i = 0; i < m_originalOrbits.Length; i++)
                {
                    m_cmFreeLook.m_Orbits[i].m_Height = m_originalOrbits[i].m_Height * scale;
                    m_cmFreeLook.m_Orbits[i].m_Radius = m_originalOrbits[i].m_Radius * scale;
                }
            }
        }




        public void FreeLook(bool freeLook)
        {
            if (freeLook)
            {
                m_cmFreeLook.m_XAxis.m_InputAxisName = "Mouse X";

            }

            if (freeLook == false)
            {
                m_cmFreeLook.m_XAxis.m_InputAxisValue = 0;
            }
        }



        //private void OnValidate()
        //{
        //    if (followTarget && lookAtTarget != null)
        //    {
        //        for (int i = 0; i < transform.childCount; i++)
        //        {
        //            if (transform.GetChild(i).GetComponent<ICinemachineCamera>() != null)
        //            {
        //                ICinemachineCamera cmCamera = transform.GetChild(i).GetComponent<ICinemachineCamera>();
        //                if (cmCamera.Follow == null) cmCamera.Follow = followTarget;
        //                if (cmCamera.LookAt == null) cmCamera.LookAt = lookAtTarget;

        //            }
        //        }
        //    }

        //}


        //private float GetInputAxis(string axisName)
        //{
        //    return !_freeLookActive ? 0 : Input.GetAxis(axisName == "Mouse Y" ? "Mouse Y" : "Mouse X");
        //}








    }



}


