using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

using Cinemachine;
using CharacterController;
using ThirdPersonCamera;

namespace CharacterController.CharacterInput
{

    [DisallowMultipleComponent]
    public partial class PlayerInput : MonoBehaviour
    {
        private const string HorizontalInputName = "Horizontal";
        private const string VerticalInputName = "Vertical";
        private const string MouseXInputName = "Mouse X";
        private const string MouseYInputName = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";


        public event Action recenterCamera;

        private bool m_axisRaw;
        [SerializeField, Range(0.2f, 5f)]
        private float verticalLookSensitivity = 3;
        [SerializeField, Range(0.2f, 5f)]
        private float horizontalLookSensitivity = 3;
        [SerializeField, Range(0.001f, 0.3f)]
        private float m_moveAxisDeadZone = 0.1f;
        [SerializeField, Range(0.2f, 3f)]
        private float m_recenterTime = 1;
        [SerializeField]
        bool m_cursorLocked = true;

        [SerializeField]
        private Transform m_camera;
        [SerializeField]
        private Transform m_vcamTarget;
        [SerializeField]
        private Transform m_lookTarget;
        [SerializeField]
        private bool m_hideInHeirarchy = true;
        [SerializeField, DisplayOnly]
        private float m_zoomInput;
        [Header("Debug Options")]
        public bool debugMode;



        public Transform vcamTarget {
            get {
                if (m_vcamTarget == null) m_vcamTarget = transform;
                return m_vcamTarget;
            }
            set {
                if(value == null) value = transform;
                m_vcamTarget = value;
            }
        }





        private Ray m_lookRay;
        private float m_lookHeight = 1.4f;
        private Vector3 m_cameraForward = new Vector3(1, 0, 1);
        private Vector3 m_lookDirection, m_inputVector;
        private Quaternion m_lookRotation;
        private float m_moveInputStopElapseTime;
        private float m_lookInputStopElapseTime;
        
        Vector3 m_lookTargetVel;  //  Used for lookTargets position SmoothDamp



        private ThirdPersonCameraController m_camController;
        private RigidbodyCharacterController m_controller;
        private Transform m_transform;
        private Inventory m_inventory;


        public bool hasLookInput { get { return lookInput != Vector2.zero; } }

        public bool hasMovementInput { get { return moveInput != Vector2.zero; } }

        //public Vector2 moveInput { get; private set; }

        //public Vector2 lookInput { get; private set; }

        //public float zoomInput { get { return m_zoomInput; } private set { m_zoomInput = value; } }

        public Vector2 moveInput
        {
            get {
                var inputX = m_axisRaw ? Input.GetAxisRaw(HorizontalInputName) : Input.GetAxis(HorizontalInputName);
                var inputZ = m_axisRaw ? Input.GetAxisRaw(VerticalInputName) : Input.GetAxis(VerticalInputName);
                return new Vector2(Mathf.Abs(inputX) < m_moveAxisDeadZone ? 0 : inputX, Mathf.Abs(inputZ) < m_moveAxisDeadZone ? 0 : inputZ);
            }
        }

        public Vector2 lookInput
        {
            get { return new Vector2(Input.GetAxis(MouseXInputName) * horizontalLookSensitivity, Input.GetAxis(MouseYInputName) * verticalLookSensitivity); }
        }

        public float zoomInput { get { return Input.GetAxis(MouseScrollInput); } }


        // --- TEMP ---
        private CameraController m_cameraController;
        // ------------


        private void Awake()
        {
            m_controller = GetComponent<RigidbodyCharacterController>();
            m_transform = transform;
            m_inventory = GetComponent<Inventory>();
            //m_finalIKController = GetComponent<FinalIKController>();
            if (m_camera == null) m_camera = Camera.main.transform;
            if (vcamTarget == null) vcamTarget = m_transform;


        }


        private void OnEnable()
        {
            if (m_cameraController == null) m_cameraController = CameraController.Instance;
            //CinemachineCore.GetInputAxis += GetInputAxisOverride;
            HandleCursorLock();
        }


        private void OnDisable()
        {
            //CinemachineCore.GetInputAxis -= GetInputAxisOverride;
        }


        private void Start()
        {
            ////  Create the look target transform.
            //CreateLookTarget("Look Target [Player Input]");
            //FinalIKController finalIK = GetComponent<FinalIKController>();
            //if (finalIK != null) {
            //    finalIK.lookTarget = m_lookTarget;
            //}

            var m_finalIKController = GetComponent<FinalIKController>();
            //  Create the look target transform.
            CreateLookTarget("Look Target [Player Input]");
            if (m_finalIKController != null && m_lookTarget != null) {
                Debug.Log("<color=blue>" + GetType().Name + "| m_finalIKController <b>[" + m_finalIKController.lookTarget + "]</b></color>");
                Debug.Log("<color=blue>" + GetType().Name + "| m_lookTarget <b>[" + m_lookTarget + "]</b></color>");
                m_finalIKController.lookTarget = m_lookTarget;
            }

            m_lookRay = new Ray(transform.position + Vector3.up * m_lookHeight, transform.forward);
        }


        private void CreateLookTarget(string lookTargetName = "LookTarget")
        {
            m_lookTarget = new GameObject(lookTargetName).transform;
            m_lookTarget.parent = transform;
        }



        private void Update()
        {
            //SetInputs(false);
            HandleLookTarget();

            HandleCharacter();

            HandleInputs();
        }


        private void LateUpdate()
        {
            HandleCamera();

            HandleLateInputs();
        }





        private void HandleCharacter()
        {
            m_cameraForward.Set(1, 0, 1);
            //  Look direction.
            m_lookDirection = Vector3.Scale(m_camera.forward, m_cameraForward).normalized;
            m_lookDirection.y = 0;
            //  Look rotation
            m_lookRotation = Quaternion.FromToRotation(m_transform.forward, m_lookDirection);
            //m_lookRotation = CalculateTargetRotation(m_lookDirection);
            //  Input vector.
            m_inputVector.Set(moveInput.x, 0, moveInput.y);


            //Quaternion targetRotation = Quaternion.FromToRotation(m_transform.forward, lookDirection);
            m_controller.Move(moveInput.x, moveInput.y, m_lookRotation);
        }




        private void HandleLookTarget(float lookDistance = 10)
        {
            if (m_lookTarget == null) return;

            //  Set the look target's position and rotation.
            m_lookRay.origin = transform.position + Vector3.up * m_lookHeight;
            m_lookRay.direction = m_lookDirection;
            m_lookTarget.position = Vector3.SmoothDamp(m_lookTarget.position, m_lookRay.GetPoint(lookDistance), ref m_lookTargetVel, 0.12f);
        }



        private void HandleInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                m_cursorLocked = !m_cursorLocked;
                HandleCursorLock();
            }


            //  -- Temp Item Handler
            HandleItemActions();
        }






        private void HandleCamera()
        {

            //if(hasMovementInput || moveInput != Vector2.zero)
            //{
            //    recenterCamera();
            //}

            TEMP_HandleCamera();
        }


        private void TEMP_HandleCamera()
        {
            //  -----------
            //  Camera Input
            if (m_cameraController == null) return;
            m_cameraController.UpdateRotation(lookInput.x, lookInput.y);
            m_cameraController.UpdateZoom(zoomInput);

            for (int number = 0; number < CinamachineCameraController.Controller.VirtualStatesCount; number++) {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(number.ToString())) {
                    string stateName = CinamachineCameraController.Controller.GetVirtualCameraState(number).StateName;
                    CinamachineCameraController.Controller.SetCameraState(stateName);
                }
            }
        }


        private void HandleLateInputs()
        {
            if (Input.GetKeyDown(KeyCode.P)) {
                UnityEditor.Selection.activeGameObject = gameObject;
            }

            if (Input.GetKeyDown(KeyCode.Return)) {
                Debug.Break();
            }
        }


        //private float GetInputAxisOverride(string axis)
        //{

        //    if (axis == "Vertical") {
        //        return Input.GetAxis("Mouse X");
        //    }

        //    if (axis == "Horizontal") {
        //        return Input.GetAxis("Mouse Y");
        //    }

        //    return 0;
        //}





        private Quaternion CalculateTargetRotation(Vector3 localDirection)
        {
            Vector3 flatForward = m_camera.transform.forward;
            flatForward.y = 0f;
            flatForward.Normalize();

            Quaternion cameraToInputOffset = Quaternion.FromToRotation(Vector3.forward, localDirection);
            cameraToInputOffset.eulerAngles = new Vector3(0f, cameraToInputOffset.eulerAngles.y, 0f);

            return Quaternion.LookRotation(cameraToInputOffset * flatForward);
        }


        // Handles the cursor lock state
        void HandleCursorLock()
        {
            Cursor.lockState = m_cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }


        void SetInputs(bool axisRaw = false)
        {
            var inputX = axisRaw ? Input.GetAxisRaw(HorizontalInputName) : Input.GetAxis(HorizontalInputName);
            if (Mathf.Abs(inputX) < m_moveAxisDeadZone)
                inputX = 0;
            var inputZ = axisRaw ? Input.GetAxisRaw(VerticalInputName) : Input.GetAxis(VerticalInputName);
            if (Mathf.Abs(inputZ) < m_moveAxisDeadZone)
                inputZ = 0;

            moveInput.Set(inputX, inputZ);


            var mouseX = axisRaw ? Input.GetAxisRaw(MouseXInputName) : Input.GetAxis(MouseXInputName);
            if (Mathf.Abs(mouseX) < m_moveAxisDeadZone)
                mouseX = 0;
            mouseX *= horizontalLookSensitivity;
            var mouseY = axisRaw ? Input.GetAxisRaw(MouseYInputName) : Input.GetAxis(MouseYInputName);
            if (Mathf.Abs(mouseY) < m_moveAxisDeadZone)
                mouseY = 0;
            mouseY *= verticalLookSensitivity;

            lookInput.Set(mouseX, mouseY);
            //zoomInput.va = Input.GetAxis(MouseScrollInput);
        }








        void OnDrawGizmos() => DrawGizmos(debugMode);

        //void OnDrawGizmosSelected() => DrawGizmos(debugMode);

        void DrawGizmos(bool debug)
        {
            if (debug && m_lookTarget != null) {
                float debugSize = 0.25f;
                Gizmos.color = new Color(1, 1, 0, debug ? 0.5f : 0.75f);
                Gizmos.matrix = m_lookTarget.localToWorldMatrix;
                Gizmos.DrawWireSphere(Vector3.zero, debugSize);

                if (debug && m_lookTarget != null)
                    UnityEditor.Handles.ArrowHandleCap(0, m_lookTarget.position, m_lookTarget.rotation, debugSize, EventType.Repaint);
            }

        }
    }
}


