using UnityEngine;
using System.Collections;

namespace CharacterController
{

    public interface IEntityObject
    {
        int EntityID { get; }

        void SetActive(bool active);

        void OnUpdate(float deltaTime);
    }


    public abstract class EntityObject : MonoBehaviour, IEntityObject
    {

        public void SetActive(bool value)
        {
            if (value) {
                EntityObjectManager.Instance.EnableEntity(this);
            }
            else {
                EntityObjectManager.Instance.DisableEnity(this);
            }

            this.enabled = value;
            this.gameObject.SetActive(value);
        }


        public int EntityID { get { return GetInstanceID(); } }



        private void PreAwake()
        {

        }


        

        public abstract void OnUpdate(float deltaTime);
    }

}
