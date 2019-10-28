using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace JH.RootMotionController.RootMotionInput
{
    using RootMotionUI;


    public class RootMotionInput : MonoBehaviour //, StandardPlayerControls.IStandardControlsActions
    {
        private const string k_mouseXInputName = "Mouse X";
        private const string k_mouseYInputName = "Mouse Y";
        private const string k_mouseScrollInput = "Mouse ScrollWheel";
        private readonly string k_lookTargetName = "Look Target [Player Input]";

        public event Action recenterCamera;


        [Header("Input Settings")]
        [Range(0.2f, 2f), Tooltip(" ")]
        [SerializeField] private float verticalLookSensitivity;
        [Range(0.2f, 2f), Tooltip(" ")]
        [SerializeField] private float horizontalLookSensitivity;
        [Tooltip("Input Action Map asset for mouse/keyboard and game pad inputs")]
        [SerializeField] private StandardPlayerControls m_standardControls;
        [Tooltip(" ")]
        [SerializeField] private bool m_cursorLocked = true;

        [Header("Camera Target")]
        [Tooltip(" ")]
        [SerializeField] private Transform m_camera;
        [Tooltip(" ")]
        [SerializeField] private Transform m_vcamTarget;
        [Tooltip(" ")]
        [SerializeField] private Transform m_lookTarget;




        [Header("Input Displays")]
        [SerializeField, DisplayOnly]
        private Vector2 m_moveInput;
        [SerializeField, DisplayOnly]
        private Vector2 m_lookInput;
        private Vector3 m_lookDirection;
        private Quaternion m_lookRotation;
        private Vector3 m_lookVelocity;  //  Used for lookTargets position SmoothDamp



        private Ray m_lookRay;
        private RootMotionController m_controller;
        private Transform m_transform;

        [Header("Debug Options")]
        [SerializeField] private bool debugMode;




        public Transform vcamTarget
        {
            get {
                if (m_vcamTarget == null) m_vcamTarget = transform;
                return m_vcamTarget;
            }
            set {
                if (value == null) value = transform;
                m_vcamTarget = value;
            }
        }

        public bool hasLookInput { get { return lookInput != Vector2.zero; } }

        public bool hasMovementInput { get { return moveInput != Vector2.zero; } }

        public Vector2 lookInput { get { return m_lookInput; } private set { m_lookInput = value; } }

        public Vector2 moveInput { get { return m_moveInput; } private set { m_moveInput = value; } }






        private void Awake()
        {
            m_controller = GetComponent<RootMotionController>();
            m_transform = transform;
            m_standardControls = new StandardPlayerControls();

        }


        private void OnEnable()
        {
            CinemachineCore.GetInputAxis += GetInputAxisOverride;
            if (m_standardControls != null) {
                m_standardControls.standardControls.move.performed += OnMoveInput;
                m_standardControls.standardControls.mouseLook.performed += OnMouseLookInput;

                m_standardControls.standardControls.aim.started += OnAim;
                m_standardControls.standardControls.aim.performed += OnAim;

                m_standardControls.standardControls.crouch.started += OnCrouch;
                m_standardControls.standardControls.crouch.performed += OnCrouch;



                m_standardControls.standardControls.move.canceled += OnMoveInputCanceled;
                m_standardControls.standardControls.crouch.canceled += OnCrouch;

                m_standardControls.Enable();
            }
            HandleCursorLock();
        }


        private void OnDisable()
        {
            CinemachineCore.GetInputAxis -= GetInputAxisOverride;

            if (m_standardControls != null) {
                m_standardControls.standardControls.move.performed -= OnMoveInput;
                m_standardControls.standardControls.mouseLook.performed -= OnMouseLookInput;

                m_standardControls.standardControls.aim.started -= OnAim;
                m_standardControls.standardControls.aim.performed -= OnAim;

                m_standardControls.standardControls.crouch.started -= OnCrouch;
                m_standardControls.standardControls.crouch.performed -= OnCrouch;

                m_standardControls.standardControls.move.canceled -= OnMoveInputCanceled;
                m_standardControls.standardControls.crouch.canceled -= OnCrouch;
                
                m_standardControls.Disable();
            }
        }


        private void Start()
        {
            if (m_camera == null) m_camera = Camera.main.transform;

            if (m_lookTarget == null) {
                m_lookTarget = new GameObject(k_lookTargetName).transform;
                m_lookTarget.hideFlags = HideFlags.HideInHierarchy;
                m_lookTarget.parent = transform;
            }

            m_lookRay = new Ray(m_transform.position.WithY(m_controller.actorCollider.height * 0.75f), m_transform.forward);

        }



        private void Update()
        {


            UpdateRootMotionController();

            UpdateLookTarget();


#if UNITY_EDITOR

            if(DebugHUD.Instance)
                DebugHUD.Instance.SetActive(m_controller.debugMode.showDisplaySettings);

            if (debugMode) {
                m_lookTarget.hideFlags = HideFlags.None;
                Debug.DrawRay(m_lookRay.origin, m_lookRay.direction, Color.cyan);
            }
            else {
                m_lookTarget.hideFlags = HideFlags.HideInHierarchy;
            }
#endif
        }

        private void LateUpdate()
        {
            UpdateCameraController();



#if UNITY_EDITOR

            HandleHelperInputs();
#endif
        }






        private void UpdateRootMotionController()
        {

            var cameraFwd = new Vector3(1, 0, 1);
            m_lookDirection = Vector3.Scale(m_camera.forward, cameraFwd);
            m_lookDirection.y = 0;
            m_lookDirection.Normalize();

            m_lookRotation = Quaternion.FromToRotation(m_transform.forward, m_lookDirection);

            m_controller.Move(moveInput.x, moveInput.y, m_lookRotation);

            m_controller.m_isRunning = Input.GetKey(KeyCode.LeftShift);
        }


        private void UpdateLookTarget(float maxDistance = 50)
        {
            if (m_lookTarget == null) return;

            var lookHeight = m_controller.actorCollider.height * 0.75f;
            //  Set the look target's position and rotation.
            m_lookRay.origin = m_transform.position + Vector3.up * lookHeight;
            m_lookRay.direction = m_lookDirection;
            m_lookTarget.position = Vector3.SmoothDamp(m_lookTarget.position, m_lookRay.GetPoint(maxDistance), ref m_lookVelocity, 0.12f);



        }


        private void UpdateCameraController()
        {
            //if (hasMovementInput || moveInput != Vector2.zero) {
            //    recenterCamera();
            //}
        }


        private void HandleHelperInputs()
        {
            if (Input.GetKeyDown(KeyCode.P)) {
                UnityEditor.Selection.activeGameObject = gameObject;
            }

            if (Input.GetKeyDown(KeyCode.Return)) {
                Debug.Break();
            }
        }


        // Handles the cursor lock state
        private void HandleCursorLock()
        {
            Cursor.lockState = m_cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }


        private int lookInputProcessedFrame;
        private bool hasProcessedLookInput;
        private float GetInputAxisOverride(string axis)
        {
            var currentFrame = Time.frameCount;
            if ((lookInputProcessedFrame < currentFrame) && hasProcessedLookInput) {
                lookInput = Vector2.zero;
            }
            lookInputProcessedFrame = currentFrame;
            hasProcessedLookInput = true;

            if (axis == k_mouseYInputName) {
                return lookInput.y * verticalLookSensitivity;
            }

            if (axis == k_mouseXInputName) {
                return lookInput.x * horizontalLookSensitivity;
            }

            return 0;
            //return Input.GetAxis(axis);
        }


        // Provides the input vector for the mouse look control
        void OnMouseLookInput(InputAction.CallbackContext context)
        {
            var newInput = context.ReadValue<Vector2>();
            // If the mouse look input was already processed, then clear the value before accumulating again
            if (hasProcessedLookInput) {
                lookInput = Vector2.zero;
                hasProcessedLookInput = false;
            }

            lookInput += newInput;
        }


        // Provides the input vector for the move control
        void OnMoveInput(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        // Resets the move input vector to zero once input has stopped
        void OnMoveInputCanceled(InputAction.CallbackContext context)
        {
            moveInput = Vector2.zero;
        }


        void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started)
                Debug.Log("Crouching start: " + context.startTime);

            if (context.performed)
                Debug.Log("Crouching performed: " + context.duration);
        }

        void OnAim(InputAction.CallbackContext context)
        {


            if (context.performed)
                Debug.Log("Aiming: " + context.ReadValueAsObject());
        }
    }
}
