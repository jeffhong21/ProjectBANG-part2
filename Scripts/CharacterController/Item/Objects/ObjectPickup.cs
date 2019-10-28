namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;

    public abstract class ObjectPickup : MonoBehaviour
    {
        [Tooltip("The amount of time to enable the trigger.")]
        [SerializeField] protected float triggerEnableDelay = 1f;
        [Tooltip("Should the item be pickedup when the character enters the trigger?")]
        [SerializeField] protected bool pickupOnTriggerEnter;
        [Tooltip("AudioClips that can be played when the object is pickedup.")]
        [SerializeField] protected AudioClip[] audioClips;


        [SerializeField] protected Collider pickupTrigger;

        [SerializeField] protected Collider pickupCollider;

        protected GameObject character;
        protected float triggerEnterTime;



        protected abstract void OnTriggerEnter(Collider other);


        protected abstract void OnTriggerExit(Collider other);

    }
}