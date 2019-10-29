using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace JH.RootMotionController.RootMotionInput
{
    using RootMotionActions;

    [CreateAssetMenu(fileName = "InputAction Data", menuName = "-- RootMotion Controller --/InputAction Data", order = -1000)]
    public class InputActionData : ScriptableObject
	{

        public PlayerInputAction MoveInput;

        public PlayerInputAction LookInput;

        public PlayerInputAction AimInput;

        public PlayerInputAction CrouchInput;

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
            public RootMotionAction rootMotionAction;

        }
    }
}
