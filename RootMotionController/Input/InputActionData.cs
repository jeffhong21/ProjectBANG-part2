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

        public PlayerInputAction MovementAction;

        public PlayerInputAction LookAction;

        public PlayerInputAction AimAction;

        public PlayerInputAction CrouchAction;

        public PlayerInputAction DrawWeaponAction;

        public PlayerInputAction HolsterWeaponAction;



        private List<PlayerInputAction> m_playerInputActions = new List<PlayerInputAction>();

        private Dictionary<InputAction, RootMotionAction> m_playerInputActionMap;


        private void Awake()
        {
            m_playerInputActionMap = new Dictionary<InputAction, RootMotionAction>();

        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }


        public void NewPlayerInputAction()
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
