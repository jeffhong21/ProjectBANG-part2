namespace CharacterController.AnimatorUtil
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Animations;

    public static class GetStateMachineChildren
    {



        public static List<string> GetChildren(Animator animator, string stateMachineName)
        {
            List<string> statesList = new List<string>();
            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;

            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {
                foreach (ChildAnimatorState childState in layer.stateMachine.states) //for each state
                {
                    //Debug.Log("ChildStates: " + childState.state.name);
                    if(childState.state.name == stateMachineName) {
                        statesList.Add(childState.state.name);
                    }

                }
                foreach (ChildAnimatorStateMachine sm in layer.stateMachine.stateMachines) //for each state
                {
                    //Debug.Log("ChildStates: " + sm.stateMachine.name);
                    if (sm.stateMachine.name == stateMachineName) {
                        RegisterAnimatorStates(sm.stateMachine, sm.stateMachine.name, ref statesList);
                    }
                }
            }

            return statesList;
        }


        private static void RegisterAnimatorStates( AnimatorStateMachine stateMachine, string parentState, ref List<string> statesList )
        {
            foreach (ChildAnimatorState childState in stateMachine.states) //for each state
            {
                string stateName = childState.state.name;
                string fullPathName = parentState + "/" + stateName;

                statesList.Add(fullPathName);
            }

            foreach (ChildAnimatorStateMachine sm in stateMachine.stateMachines) //for each state
            {
                string path = parentState + "/" + sm.stateMachine.name;
                RegisterAnimatorStates(sm.stateMachine, path, ref statesList);
            }
        }


        public static void DrawStateMachineMenu(List<string> states, Action handler)
        {
            var menu = new GenericMenu();

            for (int i = 0; i < states.Count; i++) {
                var state = states[i];
                menu.AddItem(new GUIContent(state), false, () => handler() );
            }
            menu.ShowAsContext();
        }

    }
}


