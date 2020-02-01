namespace CharacterController
{
    using UnityEngine;
    using UnityEditor.Animations;
    using System;

    public class RandomStateBehavior : StateBehavior
    {

        int additiveHash = Animator.StringToHash("RandomValue");
        [SerializeField]
        int childSubstateCount;

        protected override void OnInitialize()
        {
        }




        public override void OnStateMachineEnter( Animator animator, int stateMachinePathHash )
        {
            base.OnStateMachineEnter(animator, stateMachinePathHash);
            //Debug.LogFormat("On StateMachine <color=cyan> {0} </color> | FullHashPath: {1}", "Enter", stateMachinePathHash);

            animator.SetInteger(additiveHash, UnityEngine.Random.Range(0, childSubstateCount));
        }






    }

}