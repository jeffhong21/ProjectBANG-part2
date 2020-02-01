namespace CharacterController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class UseableItem : MonoBehaviour, IUseableItem
    {

        [Tooltip("The amount of time that must elapse before the item can be used again."), Min(0)]
        [SerializeField] protected float useCooldown = 0.1f;

        [Tooltip("Should the character rotation to face the target?")]
        [SerializeField] protected bool faceTarget;

        [Tooltip("The amount of extra time it takes for the action to stop use.  Useful for preventing jumps between states."), Min(0)]
        [SerializeField] protected float stopUseDelay;

        [SerializeField] protected AnimationEventTrigger useEvent = new AnimationEventTrigger("OnAnimatorUseEvent");
        [SerializeField] protected AnimationEventTrigger useCompleteEvent = new AnimationEventTrigger("OnAnimatorUseCompleteEvent");

        private WaitForSeconds useItemStartDelay;
        private WaitForSeconds useItemCompleteDuration;
        protected float useItemStartTime;
        protected float useItemDuration;
        private bool itemBeingUsed;
        protected Item item;
        protected Transform mTransform;
        protected GameObject mGameObject;



        protected virtual void Awake()
        {
            item = gameObject.GetComponent<Item>();

            mTransform = transform;
            mGameObject = gameObject;

            useItemStartDelay = new WaitForSeconds(useEvent.duration);
            useItemCompleteDuration = new WaitForSeconds(useCompleteEvent.duration);
        }


        protected virtual void OnDestroy()
        {

        }


        public bool InUse()
        {
            if (useItemStartTime < 0) return false;

            itemBeingUsed = (Time.time - useItemStartTime < useCooldown) ? true : false;
            return itemBeingUsed;
        }


        public bool UseItem()
        {
            if (InUse()) return false;

            if (useEvent.waitForAnimationEvent) {
                useItemStartTime = Time.time;
                itemBeingUsed = true;
            } else {

                StartCoroutine(InternalUseItem(useEvent.duration));

            }

            if (useCompleteEvent.waitForAnimationEvent) {
                useItemStartTime = Time.time;
                itemBeingUsed = true;
            } else {
                StartCoroutine(InternalUseItemComplete(useCompleteEvent.duration));

            }


            return true;
        }


        public void UseItemComplete()
        {
            useItemStartTime = -1;
            itemBeingUsed = false;
        }



        private IEnumerator InternalUseItem( float startDelay )
        {
            yield return new WaitForSeconds(startDelay);
            useItemStartTime = Time.time;
            itemBeingUsed = true;
        }

        private IEnumerator InternalUseItemComplete( float complete )
        {
            yield return new WaitForSeconds(complete);
            UseItemComplete();


        }

    }
}
