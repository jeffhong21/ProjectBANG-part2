using UnityEngine;
using System.Collections.Generic;
using System;

namespace ActorSkins
{
    public class ActorSkinComponent : MonoBehaviour
    {

        [SerializeField]
        public Transform[] actors;
        [SerializeField]
        private bool hideInHierarchy;

        [HideInInspector]
        public int current;

        public event Action onTransformChildrenChanged;


        public void ChangeActor(bool next = true)
        {
            if (next) current++;
            else current--;

            if (current >= actors.Length) current = 0;
            if (current < 0) current = actors.Length - 1;

            foreach (var actor in actors) {
                if (actor) actor.gameObject.SetActive(false);
                if (hideInHierarchy) actor.hideFlags = HideFlags.HideInHierarchy;
            }

            if (actors[current]) {
                actors[current].gameObject.SetActive(true);
                if (hideInHierarchy) actors[current].hideFlags = HideFlags.None;
            }
        }


        public Transform GetCurrentActiveActor()
        {
            return actors[current];
        }

        public void ChangeActor(int index)
        {
            current = index - 1;
            ChangeActor();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            
        }

        private void OnTransformChildrenChanged()
        {
            onTransformChildrenChanged();
        }
#endif

    }

}
