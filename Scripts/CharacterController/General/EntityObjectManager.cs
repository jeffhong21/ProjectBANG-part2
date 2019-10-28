using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using DebugUI;
namespace CharacterController
{
    public class EntityObjectManager : SingletonMonoBehaviour<EntityObjectManager>
    {

        private Dictionary<int, IEntityObject> m_allEntityObjects;
        private List<IEntityObject> m_activeEntities;
        private Queue<IEntityObject> m_updateQueue;

        [SerializeField]
        private float m_deltaTime;
        [SerializeField]
        private float m_lastFrameTime;

        //  Has the frame been updated.
        private bool m_frameUpdated;
        //  The amount of fixedUpdates between frames.
        private int m_fixedUpdateCount;



        protected override void OnAwake()
        {
            m_lastFrameTime = 0f;
            m_allEntityObjects = new Dictionary<int, IEntityObject>();
            m_activeEntities = new List<IEntityObject>();
            m_updateQueue = new Queue<IEntityObject>();
        }


        public void EnableEntity(IEntityObject entity)
        {
            if(m_allEntityObjects.TryGetValue(entity.EntityID, out IEntityObject entityObject)) {

            }

            //if (!m_allEntityObjects.ContainsKey(entity)) {
            //    m_allEntityObjects.Add(entity, entity.EntityID);
            //}
        }

        public void DisableEnity(IEntityObject entity)
        {

        }



        #region Update Loop

        private void FixedUpdate()
        {
            m_deltaTime = Time.time - m_lastFrameTime;
            //  Update entity;
            OnEntityUpdate(m_deltaTime);
            m_frameUpdated = true;

            m_fixedUpdateCount++;

            //DebugUI.DebugUI.Log(this, Time.deltaTime, "deltaTime_FixedUpdate", RichTextColor.Cyan);
        }


        private void Update()
        {
            m_deltaTime = Time.time - m_lastFrameTime;
            m_lastFrameTime = Time.time;

            //DebugUI.DebugUI.Log(this, Time.deltaTime, "deltaTime", RichTextColor.Cyan);

            if (m_frameUpdated) return;
            //  Update entity;
            OnEntityUpdate(m_deltaTime);
            m_frameUpdated = true;
        }


        private void LateUpdate()
        {
            m_frameUpdated = false;
            m_fixedUpdateCount = 0;
        }

        #endregion



        private void OnEntityUpdate(float deltaTime)
        {
            //DebugUI.DebugUI.Log(this, m_deltaTime, "dcustom deltaTime", RichTextColor.Cyan);
        }




    }
}
