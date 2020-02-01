namespace CharacterController
{

    using UnityEngine;
    using System;

    [Serializable]
    public class AnimationEventTrigger
    {
        private string eventName = "OnAnimator";
        [Tooltip("Should item wait for the animator event.")]
        public bool waitForAnimationEvent = true;
        [Tooltip("The amount time to wait before being used."), Min(0)]
        public float duration = 0f;


        public string EventName {  get { return eventName; } }


        public AnimationEventTrigger(string eventName, bool waitForAnimationEvent = true, float duration = 0)
        {
            this.eventName = eventName;
            this.waitForAnimationEvent = waitForAnimationEvent;
            this.duration = waitForAnimationEvent ? 0 : duration;
        }
    }

}
