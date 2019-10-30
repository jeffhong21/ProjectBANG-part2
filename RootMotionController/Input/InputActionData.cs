using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace JH.RootMotionController.RootMotionInput
{
    using RootMotionActions;

    public enum InputActionType { Pressed, PressAndRelease, Hold, Toggle }


    [CreateAssetMenu(fileName = "InputAction Data", menuName = "-- RootMotion Controller --/InputAction Data", order = -1000)]
    public class InputActionData : ScriptableObject
	{

        public InputActionMap gameplayActions;

        public PlayerInputAction moveAction;
        public PlayerInputAction lookAction;
        public PlayerInputAction aimAction;
        public PlayerInputAction crouchAction;
        public PlayerInputAction NextWeaponInput;
        public PlayerInputAction PreviousWeaponInput;



        private List<PlayerInputAction> m_playerInputActions;
        



        private void Awake()
        {
            m_playerInputActions = new List<PlayerInputAction>()
            {
                MoveInput,
                LookInput,
                AimInput,
                CrouchInput,
                NextWeaponInput,
                PreviousWeaponInput
            };
        }

        private void OnEnable()
        {
            for (int i = 0; i < m_playerInputActions.Count; i++)
            {
                var playerInputAction = m_playerInputActions[i];
                playerInputAction.inputAction.started += ctx =>
                {

                };
                playerInputAction.inputAction.performed += ctx =>
                {

                };
                playerInputAction.inputAction.canceled += ctx =>
                {

                };
                playerInputAction.inputAction.Enable();
            }

        }

        private void OnDisable()
        {
            for (int i = 0; i < m_playerInputActions.Count; i++) {
                var playerInputAction = m_playerInputActions[i];
                playerInputAction.inputAction.Disable();
            }
        }


        public void AddPlayerInputAction()
        {

        }



        [Serializable]
        public class PlayerInputAction
        {

            public InputAction inputAction;
            [SerializeField]
            private RootMotionAction m_rootMotionAction;
            [SerializeField]
            private InputActionType m_inputActionType = InputActionStartType.PressAndRelease;



            public int id { get{ return inputAction != null ? inputAction.id : -1; }}
            public IRootMotionAction rootMotionAction {
                get{ return m_rootMotionAction = null ? null : (IRootMotionAction)m_rootMotionAction;}
            }
            public InputActionType inputActionType {get => m_inputActionType;}



            public void Enable()
            {
                switch(inputActionType)
                {
                    case InputActionType.Pressed:

                        break;
                    case InputActionType.PressAndRelease:
                        inputAction.performed += ctx => { rootMotionAction.TryStartAction(); };
                        break;
                    case InputActionType.Hold:

                        break;
                    case InputActionType.Toggle:

                        break;
                }

                inputAction.Enable();
            }


            public void Disable()
            {
                inputAction.started += ctx =>
                {

                };
                inputAction.performed += ctx =>
                {

                };
                inputAction.canceled += ctx =>
                {

                };
            }



            public PlayerInputAction(InputAction playerInputAction)
            {
                inputAction = playerInputAction;
                rootMotionAction = null;
            }

            public PlayerInputAction(RootMotionAction playerRootMotionAction)
            {
                inputAction = new InputAction();
                rootMotionAction = playerRootMotionAction;
            }

            public PlayerInputAction(InputAction playerInputAction, RootMotionAction playerRootMotionAction)
            {
                inputAction = playerInputAction;
                rootMotionAction = playerRootMotionAction;
            }
        }
    }
}
