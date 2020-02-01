//using UnityEngine;
//using UnityEditor.Animations;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace AnimatorUtils
//{
//    public class AnimatorUtils
//    {



//        private static string parameterInfo;
//        private static int parameterCount;

//        public static void ChangeParameter(Animator animator, string parameter)
//        {

//            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;


//            foreach (AnimatorControllerLayer layer in animatorController.layers)
//            {
//                LoopThroughStateMachines(layer.stateMachine, parameter);
//            }


//            Debug.LogFormat("{0} used count: {1}\n{2}", parameter, parameterCount, parameterInfo);
//            parameterInfo = "";
//            parameterCount = 0;
//        }

//        private static void LoopThroughStateMachines(AnimatorStateMachine stateMachine, string parameter)
//        {
//            foreach (ChildAnimatorState childState in stateMachine.states) //for each state
//            {
//                AnimatorStateTransition[] transitions = childState.state.transitions;
//                foreach (AnimatorStateTransition transition in transitions)
//                {
//                    AnimatorCondition[] conditions = transition.conditions;
//                    for (int con = 0; con < conditions.Length; con++)
//                    {
//                        AnimatorCondition condition = conditions[con];
//                        if (condition.parameter == parameter)
//                        {
//                            parameterInfo += childState.state.name + " | " + condition.mode + " | " + condition.threshold + "\n";
//                            parameterCount++;

//                        }
//                    }
//                }
//            }

//            foreach (ChildAnimatorStateMachine sm in stateMachine.stateMachines) //for each state{
//                LoopThroughStateMachines(sm.stateMachine, parameter);
//        }
//    }

//}

