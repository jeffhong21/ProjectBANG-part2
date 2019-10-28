namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Cinemachine;


    public class CinamachineCameraController : CameraController
    {
        public static CinamachineCameraController Controller { get; private set; }


        [Header("Cinemachine")]
        [SerializeField] private CinemachineBrain m_cmBrain;
        [Header("Targets")]
        [SerializeField] private Transform m_followTarget;
        [SerializeField] private Transform m_lookAtTarget;
        [Header("Zoom Options")]
        [Range(0.01f, 1f)]
        [SerializeField] private float minScale = 0.5f;
        [Range(1F, 5f)]
        [SerializeField] private float maxScale = 1;

        [Header("Free Look Settings")]
        [Tooltip("This depends on your Free Look rigs setup, use to correct Y sensitivity, about 1.5 - 2 results in good Y-X square responsiveness")]
        [SerializeField] private float yCorrection = 2f;

        private float xAxisValue;
        private float yAxisValue;
        private Dictionary<CinemachineFreeLook, CinemachineFreeLook.Orbit[]> m_freeLookOrbits;


        [SerializeField]
        private VirtualCameraState[] m_virtualCameras = { new VirtualCameraState("DEFAULT") };

        private int activeCameraIndex;
        private CinemachineVirtualCameraBase m_activeState;




        //private CinemachineCollider m_CinemachineCollider;
        //private CinemachineImpulseSource m_CinemachineImpulseSource;
        private PostProcessVolume postProcessVolume;
        private PostProcessProfile postProcessProfile;



        public int VirtualStatesCount {  get { return m_virtualCameras.Length; } }

        public VirtualCameraState[] VirtualCameras {  get { return m_virtualCameras; } }


        protected override void Awake()
        {
            base.Awake();
            _instance = this;
            Controller = this;

            m_cmBrain = GetComponentInChildren<CinemachineBrain>();
            //postProcessVolume = m_camera.GetComponent<PostProcessVolume>();
            //postProcessProfile = postProcessVolume.profile;

        }



#if UNITY_EDITOR

        private void OnValidate()
        {
            if(m_followTarget && m_lookAtTarget != null)
            {
                for (int i = 0; i < transform.childCount; i++){
                    if (transform.GetChild(i).GetComponent<ICinemachineCamera>() != null){
                        ICinemachineCamera cmCamera = transform.GetChild(i).GetComponent<ICinemachineCamera>();
                        if (cmCamera.Follow == null) cmCamera.Follow = m_followTarget;
                        if (cmCamera.LookAt == null) cmCamera.LookAt = m_lookAtTarget;

                    }
                }
            }


        }

#endif


        private void Start()
        {

            activeCameraIndex = 0;
            if (m_virtualCameras.Length > 0) {
                for (int i = 0; i < m_virtualCameras.Length; i++) {
                    m_virtualCameras[i].Initialize();
                    m_virtualCameras[i].VirtualCamera.gameObject.SetActive(false);
                }
                m_virtualCameras[0].VirtualCamera.gameObject.SetActive(true);
            }

            m_camera = m_cmBrain.OutputCamera;

            if (m_followTarget != null && m_virtualCameras[0].VirtualCamera is CinemachineFreeLook) {
                var cmFreeLook = (CinemachineFreeLook)m_virtualCameras[0].VirtualCamera;
                cmFreeLook.m_XAxis.Value = m_followTarget.eulerAngles.y;
            }
        }

        private void OnEnable()
        {
            
        }



        private void InitializeVirtualCamera(CinemachineVirtualCameraBase vCam)
        {
            if(vCam == null) { return; }

            vCam.Follow = m_followTarget;
            vCam.LookAt = m_lookAtTarget;
        }


        public override void SetMainTarget(GameObject target)
        {
            if(m_lookAtTarget == null) m_lookAtTarget = target.transform;
            if (m_followTarget == null) m_followTarget = target.transform;

            //activeVCam.LookAt = m_lookAtTarget;
            //activeVCam.Follow = m_followTarget;
        }



        // Event registered to handle the Recentering of the CM Freelook camera
        private void RecenterCamera()
        {
            //only recenter if there is no user input
            //System.NotImplementedException.
        }







        public override void UpdateRotation(float mouseX, float mouseY)
        {
            //throw new NotImplementedException();
        }


        public override void UpdateZoom(float zoomInput)
        {
            //throw new NotImplementedException();
            //Debug.Log(zoomInput);
        }


        public override bool SetCameraState(string stateName)
        {
            bool stateSet = false;
            int count = m_virtualCameras.Length;

            //  First, loop through virtual cameras and find the virtual camera.
            for (int i = 0; i < count; i++)
            {
                if(m_virtualCameras[i].StateName == stateName)
                {
                    activeCameraIndex = i;

                    m_activeState = m_virtualCameras[activeCameraIndex].VirtualCamera;
                    m_activeState.gameObject.SetActive(true);

                    //  Swap positions with top of list.
                    var temp = m_virtualCameras[0];
                    m_virtualCameras[0] = m_virtualCameras[i];
                    m_virtualCameras[i] = temp;

                    stateSet = true;
                    break;
                }

            }

            if(stateSet)
            {
                int priority = 0;
                for (int i = count - 1; i > 0; i--)
                {
                    if (i == 0) break;
                    //if (m_virtualCameras[i].VirtualCamera == m_activeState) continue;
                    m_virtualCameras[i].VirtualCamera.Priority = priority;
                    m_virtualCameras[i].VirtualCamera.gameObject.SetActive(false);
                    priority++;
                }
            }


            return stateSet;
        }



        public VirtualCameraState GetVirtualCameraState(int index)
        {
            if (index > m_virtualCameras.Length) return null;
            return m_virtualCameras[index];
        }
        




        public void PlayImpulse()
        {
            if (m_virtualCameras[activeCameraIndex].ImpulseSource != null)
                m_virtualCameras[activeCameraIndex].ImpulseSource.GenerateImpulse(Vector3.right);
            else Debug.Log("<color=yellow> No Impulse Source</color>");

            //CinemachineVirtualCameraBase vCam = GetActiveCamera() as CinemachineVirtualCameraBase;
            //StartCoroutine(ShakeVCam(GetActiveCamera(), 1, 1, 1));
        }


        public IEnumerator ShakeVCam( CinemachineVirtualCamera vCam, float amp, float freq, float duration )
        {

            CinemachineBasicMultiChannelPerlin noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            noise.m_AmplitudeGain = amp;
            noise.m_FrequencyGain = freq;
            yield return new WaitForSeconds(duration);

            noise.m_AmplitudeGain = 0;
        }




    }



}


