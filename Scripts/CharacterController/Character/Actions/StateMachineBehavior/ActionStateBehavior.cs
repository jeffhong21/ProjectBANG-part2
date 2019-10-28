namespace CharacterController
{
    using UnityEngine;
    using System;

    public class ActionStateBehavior : StateBehavior
    {
        public bool enableMatchTarget;
        public AnimatorMatchTarget matchTarget;








        protected override void OnInitialize()
        {
            matchTarget = new AnimatorMatchTarget(m_animator);
        }




        public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            ResetEvents(onTimedEvents);
            //m_animatorMonitor.SetActiveStateBehavior(this);
        }


        public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            ExecuteEvents(onTimedEvents, stateInfo);

            //if (matchTarget.matchTarget) matchTarget.MatchTarget();
            //if (!m_animator.isMatchingTarget && )
            //{

            //}
        }


        public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            //if (animatorMonitor.HasMatchTarget) {
            //    animatorMonitor.ResetMatchTarget();
            //}

            //if (matchTarget.matchTarget && m_animator.isMatchingTarget){
            //    matchTarget.Reset(true, false);
            //}
            //m_animatorMonitor.SetActiveStateBehavior(null);
        }



        public bool MatchTarget(Vector3 matchPosition, Quaternion matchRotation)
        {
            //matchTarget.matchTarget = true;
            return matchTarget.SetMatchTarget(matchPosition, matchRotation);
        }



        private void ResetEvents(AnimationEventItem[] animationEvents)
        {
            for (int i = 0; i < animationEvents.Length; i++) {
                animationEvents[i].sent = false;
            }
        }

        private void ExecuteEvents(AnimationEventItem[] animationEvents, AnimatorStateInfo stateInfo)
        {
            if (animationEvents != null) {
                for (int i = 0; i < animationEvents.Length; i++) {
                    var animEvent = animationEvents[i];
                    if (animEvent.enabled && animEvent.animationEvent != string.Empty) {
                        float stateTime = stateInfo.normalizedTime % 1;
                        if (!animEvent.sent && (stateTime >= animEvent.time)) {
                            animEvent.sent = true;
                            //Debug.LogFormat("<b>[{0}]</b> <color=green>{1}</color>", animEvent.animationEvent, stateTime);
                            m_animatorMonitor.ExecuteEvent(animEvent.animationEvent);
                        }
                    }
                }
            }
        }



    }

}