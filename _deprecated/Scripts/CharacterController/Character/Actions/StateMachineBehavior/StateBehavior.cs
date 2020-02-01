namespace CharacterController
{
    using UnityEngine;
    using System;

    public enum AnimationEventType
    {
        Default, Bool, Int, Float, String
    }

    public abstract class StateBehavior : StateMachineBehaviour
    {
        [Serializable]
        public class AnimationEventItem
        {
            public string animationEvent;
            public AnimationEventType eventType = AnimationEventType.Default;
            public bool boolValue;
            public float floatValue;
            public int intValue;
            public string stringValue;

            [Range(0, 0.99f)]
            public float time;
            public bool sent;

            [Tooltip("Is animation event enabled.")]
            public bool enabled = true;

            public AnimationEventItem()
            {
                animationEvent = string.Empty;
                enabled = true;
            }
        }


        public AnimationEventItem[] onTimedEvents = new AnimationEventItem[0];

        protected AnimatorMonitor m_animatorMonitor;
        protected Animator m_animator;
        private bool m_isInitialized;





        public void Initialize(AnimatorMonitor animatorMonitor, Animator animator)
        {
            m_animatorMonitor = animatorMonitor;
            m_animator = animator;
            OnInitialize();
        }



        //protected virtual void OnDestroy()
        //{
        //    //isInitialized = false;
        //    //animatorMonitor = null;
        //    Debug.Log("State destroyed");
        //}


        protected abstract void OnInitialize();


        //public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        //{
        //    base.OnStateEnter(animator, stateInfo, layerIndex);
        //    //Debug.LogFormat("On State <color=magenta> {0} </color> | Length: {1} | NormalizedTime: {2}", "Enter", stateInfo.length, stateInfo.normalizedTime);


        //}


        //public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        //{
        //    base.OnStateUpdate(animator, stateInfo, layerIndex);


        //}


        //public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        //{
        //    base.OnStateExit(animator, stateInfo, layerIndex);

        //    //Debug.LogFormat("On State <color=red> {0} </color> | Length: {1} | NormalizedTime: {2}", "Exit", stateInfo.length, stateInfo.normalizedTime);

        //}


        //public override void OnStateMachineEnter( Animator animator, int stateMachinePathHash )
        //{
        //    base.OnStateMachineEnter(animator, stateMachinePathHash);
        //    //Debug.LogFormat("On StateMachine <color=cyan> {0} </color> | FullHashPath: {1}", "Enter", stateMachinePathHash);

        //}

        //public override void OnStateMachineExit( Animator animator, int stateMachinePathHash )
        //{
        //    base.OnStateMachineExit(animator, stateMachinePathHash);
        //    //Debug.LogFormat("On StateMachine <color=blue> {0} </color> | FullHashPath: {1}", "Exit", stateMachinePathHash);

        //}
    }

}