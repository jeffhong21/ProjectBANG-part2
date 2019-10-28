using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;

using Cinemachine;
using CharacterController.CharacterInput;

using JH.RootMotionController.RootMotionInput;

namespace ThirdPersonCamera
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField]
        private RootMotionInput rootMotionInput;
        private PlayerInput m_playerInput;
        [SerializeField]
        private CinemachineStateDrivenCamera m_stateDrivenCam;


        private CinemachineFreeLook LiveFreeLook
        {
            get { return m_stateDrivenCam == null ? null : m_stateDrivenCam.LiveChild as CinemachineFreeLook; }
        }



        private void Awake()
        {
            SetupMainCamera();
            FindPlayerInput(true);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            FindPlayerInput(false);
            SetupMainCamera();
        }

        private void OnValidate()
        {
            FindPlayerInput(false);
            SetupMainCamera();
        }
#endif


        private void OnEnable()
        {
            //if (m_playerInput != null) {
            //    m_playerInput.recenterCamera -= RecenterCamera;
            //    m_playerInput.recenterCamera += RecenterCamera;
            //}
            if (rootMotionInput != null) {
                rootMotionInput.recenterCamera -= RecenterCamera;
                rootMotionInput.recenterCamera += RecenterCamera;
            }
        }


        private void OnDisable()
        {
            //if (m_playerInput != null) {
            //    m_playerInput.recenterCamera -= RecenterCamera;
            //}
            if (rootMotionInput != null) {
                rootMotionInput.recenterCamera -= RecenterCamera;
            }
        }


        private void Update()
        {
            //if ((m_playerInput == null) || !m_playerInput.hasMovementInput || (m_playerInput.lookInput == Vector2.zero)) {
            //    DisableRecentering(LiveFreeLook);
            //}
            if ((rootMotionInput == null) || !rootMotionInput.hasMovementInput || (rootMotionInput.lookInput == Vector2.zero)) {
                DisableRecentering(LiveFreeLook);
            }
        }


        private void RecenterCamera()
        {

            //if ((m_playerInput == null) || m_playerInput.hasMovementInput) {
            //    EnableRecentering(LiveFreeLook);
            //}
            if ((rootMotionInput == null) || rootMotionInput.hasMovementInput) {
                EnableRecentering(LiveFreeLook);
            }
        }

        private void EnableRecentering(CinemachineFreeLook freeLook)
        {
            if (freeLook != null) {
                freeLook.m_RecenterToTargetHeading.m_enabled = true;
                freeLook.m_YAxisRecentering.m_enabled = true;
            }
        }

        private void DisableRecentering(CinemachineFreeLook freeLook)
        {
            if (freeLook != null) {
                freeLook.m_RecenterToTargetHeading.m_enabled = false;
                freeLook.m_YAxisRecentering.m_enabled = false;
            }
        }

        private void OnCameraChange()
        {

        }

        private void SetupMainCamera()
        {
            var mainCamera = Camera.main;
            if (mainCamera != null && mainCamera.GetComponent<CinemachineBrain>() == null) {
                mainCamera.gameObject.AddComponent<CinemachineBrain>();
            }
        }


        private void FindPlayerInput(bool autoDisable)
        {
            //            if (m_playerInput == null) {
            //                PlayerInput[] playerInputObjects = FindObjectsOfType<PlayerInput>();
            //                int length = playerInputObjects.Length;
            //                bool found = true;
            //                if (length != 1) {
            //                    string errorMessage = "No ThirdPersonBrain in scene! Disabling Camera Controller";
            //                    if (length > 1) {
            //                        errorMessage = "Too many ThirdPersonBrains in scene! Disabling Camera Controller";
            //                    }
            //                    else // none found
            //                    {
            //                        found = false;
            //                    }

            //                    if (autoDisable) {
            //                        Debug.LogError(errorMessage);
            //#if UNITY_EDITOR
            //                        EditorUtility.DisplayDialog("Error detecting ThirdPersonBrain", errorMessage, "Ok");
            //#endif
            //                        gameObject.SetActive(false);
            //                    }
            //                }

            //                if (found) {
            //                    m_playerInput = playerInputObjects[0];
            //                }
            //                else {
            //                    return;
            //                }
            //            }

            if (rootMotionInput == null) {
                RootMotionInput[] playerInputObjects = FindObjectsOfType<RootMotionInput>();
                int length = playerInputObjects.Length;
                bool found = true;
                if (length != 1) {
                    string errorMessage = "No ThirdPersonBrain in scene! Disabling Camera Controller";
                    if (length > 1) {
                        errorMessage = "Too many ThirdPersonBrains in scene! Disabling Camera Controller";
                    }
                    else // none found
                    {
                        found = false;
                    }

                    if (autoDisable) {
                        Debug.LogError(errorMessage);
#if UNITY_EDITOR
                        EditorUtility.DisplayDialog("Error detecting ThirdPersonBrain", errorMessage, "Ok");
#endif
                        gameObject.SetActive(false);
                    }
                }

                if (found) {
                    rootMotionInput = playerInputObjects[0];
                }
                else {
                    return;
                }
            }

            m_stateDrivenCam = GetComponent<CinemachineStateDrivenCamera>();
            if (m_stateDrivenCam != null) {
                m_stateDrivenCam.m_LookAt = rootMotionInput.vcamTarget;
                m_stateDrivenCam.m_Follow = rootMotionInput.vcamTarget;
                m_stateDrivenCam.m_AnimatedTarget = rootMotionInput.GetComponent<Animator>();
            }

        }


    }
}


