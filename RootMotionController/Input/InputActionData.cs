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


        public bool CROUCHING;

        public bool AIMING;



        //public InputActionMap GameplayMoveActionMap = new InputActionMap("GameplayMoveActionMap");

        //public PlayerInputAction MoveAction;
        //public PlayerInputAction LookAction;
        //public PlayerInputAction AimAction;
        //public PlayerInputAction CrouchAction;
        //public PlayerInputAction DrawHolsterSideArm;
        //public PlayerInputAction DrawHolsterLongArm;



        private List<PlayerInputAction> m_playerInputActions;
        



        private void Awake()
        {
            //f(GameplayMoveActionMap == null) GameplayMoveActionMap = new InputActionMap("GameplayMoveActionMap");
            

            //m_playerInputActions = new List<PlayerInputAction>()
            //{
            //    MoveAction, LookAction, AimAction, CrouchAction,
            //    DrawHolsterSideArm, DrawHolsterLongArm
            //};

            //for (int i = 0; i < m_playerInputActions.Count; i++) {
            //    var playerInputAction = m_playerInputActions[i];
            //    if(playerInputAction.inputAction != null)
            //        GameplayMoveActionMap.AddAction(playerInputAction.inputAction.name);
            //}
        }

        public void Enable()
        {
            //for (int i = 0; i < m_playerInputActions.Count; i++)
            //{
            //    var playerInputAction = m_playerInputActions[i];
            //    if (playerInputAction.inputAction != null)
            //        playerInputAction.inputAction.Enable();
            //}

        }

        public void Disable()
        {
            //for (int i = 0; i < m_playerInputActions.Count; i++) {
            //    var playerInputAction = m_playerInputActions[i];
            //    if (playerInputAction.inputAction != null)
            //        playerInputAction.inputAction.Disable();
            //}
        }






        [Serializable]
        public class PlayerInputAction
        {
            [SerializeField]
            private string m_inputActionName;
            [SerializeField]
            private RootMotionAction m_rootMotionAction;
            [SerializeField]
            private InputActionType m_inputActionType = InputActionType.PressAndRelease;


            public string inputActionName { get; set; }

            public IRootMotionAction rootMotionAction{
                get { return m_rootMotionAction == null ? null : m_rootMotionAction; }
            }

            public InputActionType inputActionType {
                get => m_inputActionType;
            }






            public InputAction CreateInputAction(string inputActionName)
            {
                InputAction inputAction = new InputAction(inputActionName);


                return inputAction;
            }



            //public void Enable()
            //{
            //    switch(inputActionType)
            //    {
            //        case InputActionType.Pressed:

            //            break;
            //        case InputActionType.PressAndRelease:
            //            inputAction.performed += ctx => { rootMotionAction.TryStartAction(); };
            //            break;
            //        case InputActionType.Hold:

            //            break;
            //        case InputActionType.Toggle:

            //            break;
            //    }

            //    inputAction.Enable();
            //}


            //public void Disable()
            //{
            //    inputAction.Disable();
            //}



            //public PlayerInputAction(InputAction playerInputAction)
            //{
            //    inputAction = playerInputAction;
            //    m_rootMotionAction = null;
            //}

            //public PlayerInputAction(RootMotionAction playerRootMotionAction)
            //{
            //    inputAction = new InputAction();
            //    m_rootMotionAction = playerRootMotionAction;
            //}

            //public PlayerInputAction(InputAction playerInputAction, RootMotionAction playerRootMotionAction)
            //{
            //    inputAction = playerInputAction;
            //    m_rootMotionAction = playerRootMotionAction;
            //}
        }


        public enum InputActionType { Pressed, PressAndRelease, Hold, Toggle }
    }
}
