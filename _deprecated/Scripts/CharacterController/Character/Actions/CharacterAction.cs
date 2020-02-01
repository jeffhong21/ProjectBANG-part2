using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Animations;
using System;
using System.Collections;

namespace CharacterController
{
    public enum ActionStartType { Automatic, Manual, ButtonDown, DoublePress };
    public enum ActionStopType { Automatic, Manual, ButtonUp, ButtonToggle };

    [Serializable]
    public abstract class CharacterAction : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        protected bool m_isActive;
        [SerializeField, HideInInspector]
        protected string m_stateName;
        //[SerializeField, HideInInspector]
        protected string m_destinationStateName;
        [SerializeField, HideInInspector]
        protected int m_layerIndex = 0;
        [SerializeField, HideInInspector]
        protected int m_actionID = -1;


        [SerializeField, HideInInspector]
        protected float m_TransitionDuration = 0.2f;
        [SerializeField, HideInInspector]
        protected ActionStartType m_startType = ActionStartType.Manual;
        [SerializeField, HideInInspector]
        protected ActionStopType m_stopType = ActionStopType.Automatic;
        [SerializeField]
        protected string[] m_InputNames = new string[0];
        [SerializeField, HideInInspector]
        protected float m_SpeedMultiplier = 1;
        [SerializeField, HideInInspector]
        protected AudioClip[] m_StartAudioClips = new AudioClip[0];
        [SerializeField, HideInInspector]
        protected AudioClip[] m_StopAudioClips = new AudioClip[0];
        [SerializeField, HideInInspector]
        protected GameObject m_StartEffect;
        [SerializeField, HideInInspector]
        protected GameObject m_EndEffect;
        [SerializeField, HideInInspector, FormerlySerializedAs("m_animatorMotion")]
        protected Motion m_animatorMotion;
        //[SerializeField, HideInInspector, FormerlySerializedAs("m_executeOnStart")]
        //protected bool m_executeOnStart;
        //[SerializeField, HideInInspector, FormerlySerializedAs("m_executeOnStop")]
        //protected bool m_executeOnStop;
        [SerializeField, HideInInspector, FormerlySerializedAs("m_useRootMotionPosition")]
        protected bool m_useRootMotionPosition;
        [SerializeField, HideInInspector, FormerlySerializedAs("m_useRootMotionRotation")]
        protected bool m_useRootMotionRotation;




        [SerializeField, HideInInspector]
        private KeyCode m_inputKey;
        public KeyCode inputKey { get => m_inputKey; }

        [HideInInspector]
        public bool executeOnStart;
        [HideInInspector]
        public bool executeOnStop;


        private bool m_useRootMotionPositionCached;
        private bool m_useRootMotionRotationCAched;


        private float m_StartEffectStartTime;
        private float m_EndEffectStartTime;
        private float m_EffectCooldown = 1f;



        //[SerializeField, DisplayOnly]
        //protected int m_FullPathHash;
        //  InputNames to KeyCodes
        protected KeyCode[] m_KeyCodes = new KeyCode[0];
        protected int m_InputIndex = -1;
        private KeyCode m_ButtonDownPressed = KeyCode.F12;
        private float m_ButtonDownPressedTime;

        //  Check double press variables.
        private KeyCode m_FirstButtonPressed = KeyCode.F12;
        private float m_TimeOfFirstButtoonPressed;
        //private float m_DoublePressInputTime = 0.1f;

        protected float m_ColliderHeight;
        protected Vector3 m_ColliderCenter;
        protected float m_ActionStartTime;
        private float m_DefaultActionStopTime = 0.25f;
        protected bool m_ExitingAction;
        protected float m_deltaTime;
        protected RigidbodyCharacterController m_controller;
        protected Rigidbody m_rigidbody;
        protected CapsuleCollider m_collider;
        protected Animator m_animator;
        protected AnimatorMonitor m_animatorMonitor;
        protected LayerManager m_layers;
        protected Inventory m_inventory;
        protected GameObject m_gameObject;
        protected Transform m_transform;



        [SerializeField, HideInInspector]
        protected bool m_Debug;

        //[SerializeField]
        protected bool m_ActionStopToggle;        //  Used for double clicks.


        //
        // Properties
        //
        public bool IsActive { get { return m_isActive; } set { m_isActive = value; } }

        public virtual int ActionID { get { return m_actionID; } set { m_actionID = Mathf.Clamp(value, -1, int.MaxValue); } }

        public virtual string StateName { get { return m_stateName; } set { m_stateName = value; } }

        public float SpeedMultiplier { get { return m_SpeedMultiplier; } set { m_SpeedMultiplier = value; } }

        public string[] InputNames { get { return m_InputNames; } }

        public ActionStartType StartType{
            get { return m_startType; }
            set { m_startType = value; }
        }
    

        public ActionStopType StopType{
            get { return m_stopType; }
            set { m_stopType = value; }
        }


        public float ActionStartTime { get { return m_ActionStartTime; } }


        //
        // Methods
        //
        protected virtual void Awake()
        {
            //m_controller = GetComponent<RigidbodyCharacterController>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_animator = GetComponent<Animator>();
            m_animatorMonitor = GetComponent<AnimatorMonitor>();
            m_layers = GetComponent<LayerManager>();
            m_inventory = GetComponent<Inventory>();
            m_gameObject = gameObject;
            m_transform = transform;
            m_collider = GetComponent<CapsuleCollider>();
            //m_deltaTime = Time.deltaTime;

            //EventHandler.RegisterEvent<CharacterAction, bool>(m_gameObject, "OnCharacterActionActive", OnActionActive);

            //m_collider = GetComponent<CapsuleCollider>();

            //Initialize();
        }

        public void Initialize(RigidbodyCharacterController characterController, float deltaTime)
        {
            m_controller = characterController;

            m_deltaTime = deltaTime;

            //  Translate input name to keycode.
            if (m_startType != ActionStartType.Automatic || m_startType != ActionStartType.Manual ||
               m_stopType != ActionStopType.Automatic || m_stopType != ActionStopType.Manual)
            {
                m_KeyCodes = new KeyCode[m_InputNames.Length];
                for (int i = 0; i < m_InputNames.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(m_InputNames[i]))
                        continue;

                    KeyCode? inputKey = (KeyCode)Enum.Parse(typeof(KeyCode), m_InputNames[i]);
                    if(inputKey == null) {
                        if (m_InputNames[i].Length == 1)
                            m_InputNames[i].ToUpper();
                    }

                    if(inputKey != null)
                        m_KeyCodes[i] = (KeyCode)Enum.Parse(typeof(KeyCode), m_InputNames[i]);
                }
            }

            //if(m_collider == null) m_collider = m_controller.Collider;
            //m_ColliderHeight = m_collider.height;
            //m_ColliderCenter = m_collider.center;
        }





        protected virtual void OnValidate()
        {
            m_actionID = ActionID >= 0 ? ActionID : -1;
            if (string.IsNullOrEmpty(m_stateName)) m_stateName = GetType().Name;
        }


        protected void MoveToTarget(Vector3 targetPosition, Quaternion targetRotation, float minMoveSpeed, Action onComplete)
        {
            StartCoroutine(MoveToTarget(targetPosition, targetRotation, minMoveSpeed));
            if (onComplete != null)
                onComplete();
        }

        private IEnumerator MoveToTarget(Vector3 targetPosition, Quaternion targetRotation, float minMoveSpeed)
        {
            var startTime = Time.time;

            var direction = targetPosition - m_transform.position;
            var distanceRemainingSqr = direction.sqrMagnitude;
            while (distanceRemainingSqr >= 0.1f || startTime > startTime + 5)
            {
                var velocityDelta = targetPosition - m_transform.position;
                m_transform.position += velocityDelta.normalized * minMoveSpeed * m_deltaTime;

                distanceRemainingSqr = velocityDelta.sqrMagnitude;
                startTime += m_deltaTime;
                yield return null;
            }
        }


        private bool CheckDoubleTap(KeyCode key)
        {
            if (Input.GetKeyDown(key) && m_FirstButtonPressed == key)
            {
                m_FirstButtonPressed = KeyCode.F12;
                if (Time.time - m_TimeOfFirstButtoonPressed < 0.25f)
                {
                    return true;
                }
            }
            if (Input.GetKeyDown(key) && m_FirstButtonPressed != key)
            {
                m_FirstButtonPressed = key;
                m_TimeOfFirstButtoonPressed = Time.time;
                return false;
            }

            return false;
        }




        //  Checks if action can be started.
        public virtual bool CanStartAction()
        {
            if (this.enabled == false) return false;

            if (m_isActive == false)
            {
                switch (m_startType)
                {
                    case ActionStartType.Automatic:
                        if (m_isActive == false)
                            return true;
                        break;
                    case ActionStartType.Manual:
                        if (m_isActive == false)
                            return true;
                        break;
                    case ActionStartType.ButtonDown:

                        for (int i = 0; i < m_KeyCodes.Length; i++)
                        {
                            if (Input.GetKeyDown(m_KeyCodes[i]))
                            {
                                if (m_stopType == ActionStopType.ButtonToggle)
                                    m_ActionStopToggle = true;
                                m_InputIndex = i;
                                return true;
                            }
                        }
                        break;
                    case ActionStartType.DoublePress:

                        for (int i = 0; i < m_KeyCodes.Length; i++)
                        {
                            if (Input.GetKeyDown(m_KeyCodes[i]) && m_FirstButtonPressed == m_KeyCodes[i])
                            {
                                m_FirstButtonPressed = KeyCode.F12;
                                if (Time.time - m_TimeOfFirstButtoonPressed < 0.25f )
                                {
                                    return true;
                                }
                            }
                            if (Input.GetKeyDown(m_KeyCodes[i]) && m_FirstButtonPressed != m_KeyCodes[i])
                            {
                                m_FirstButtonPressed = m_KeyCodes[i];
                                m_TimeOfFirstButtoonPressed = Time.time;
                                return false;
                            }

                        }
                        break;
                }
            }

            return false;
        }



        public virtual bool CanStartAction(CharacterAction action)
        {
            if (action == null) return false;

            int index = Array.IndexOf(m_controller.CharActions, action);
            for (int i = 0; i < index; i++)
            {
                if (m_controller.CharActions[i].IsActive && m_controller.CharActions[i].IsConcurrentAction() == false)
                {
                    return false;
                }
            }

            return true;
        }



        public virtual bool CanStopAction()
        {
            if (enabled == false || m_isActive == false) return false;


            switch (m_stopType)
            {
                case ActionStopType.Automatic:

                    string fullPathName = m_animator.GetLayerName(m_layerIndex) + "." + m_destinationStateName + ".";
                    if (m_animator.GetCurrentAnimatorStateInfo(m_layerIndex).fullPathHash == Animator.StringToHash(fullPathName))
                    {
                        if (m_animator.GetNextAnimatorStateInfo(m_layerIndex).fullPathHash == 0)
                        {
                            m_ExitingAction = true;
                        }
                        if (m_ExitingAction)
                        {
                            if (m_animator.GetNextAnimatorStateInfo(m_layerIndex).fullPathHash != 0)
                            {
                                return true;
                            }
                        }
                        if (m_animator.GetCurrentAnimatorStateInfo(m_layerIndex).normalizedTime >= 1f)
                        {
                            //Debug.LogFormat("{0} has stopped by comparing nameHASH", m_MatchTargetState.stateName);
                            return true;
                        }
                        return false;
                    }

                    if (m_animator.GetCurrentAnimatorStateInfo(m_layerIndex).IsName(m_destinationStateName))
                    {
                        Debug.LogFormat("Stopping Action State {0}", m_destinationStateName);
                        return false;
                    }
                    //Debug.LogFormat("Trying to stopping Action State {0}", m_stateName);
                    m_isActive = false;
                    return true;
                case ActionStopType.Manual:

                    if (m_isActive)
                    {
                        m_isActive = false;
                        return true;
                    }
                    return false;

                case ActionStopType.ButtonUp:

                    if (Input.GetKeyUp(m_KeyCodes[m_InputIndex]))
                    {
                        m_InputIndex = -1;
                        m_isActive = false;
                        return true;
                    }
                    break;
                case ActionStopType.ButtonToggle:

                    if (m_ActionStopToggle)
                    {
                        if (Input.GetKeyDown(m_KeyCodes[m_InputIndex]))
                        {
                            m_InputIndex = -1;
                            m_ActionStopToggle = false;
                            m_isActive = false;
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }



        public void StartAction()
        {
            //  Initialize stuff
            m_useRootMotionPositionCached = m_controller.useRootMotionPosition;
            m_useRootMotionRotationCAched = m_controller.useRootMotionRotation;
            m_controller.useRootMotionPosition = m_useRootMotionPosition;
            m_controller.useRootMotionRotation = m_useRootMotionRotation;

            m_ActionStartTime = Time.time;
            m_isActive = true;
            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnCharacterActionActive, this, m_isActive);


            if (this is ItemAction) {
                if (m_actionID != -1) m_animatorMonitor.SetItemID(m_actionID);
                m_animatorMonitor.SetItemStateChange();
            }
            else if (this is CharacterAction) {
                if (m_actionID != -1) m_animatorMonitor.SetActionID(m_actionID);
                m_animatorMonitor.ActionChanged();
            }


            //  Initialize action start.
            ActionStarted();


            //  Loop through all layers and play destination states.
            for (int layer = 0; layer < m_animator.layerCount; layer++)
            {
                string destinationState = GetDestinationState(layer);
                if (destinationState != string.Empty)
                {
                    bool stateChanged = m_animatorMonitor.ChangeAnimatorState(this, destinationState, m_TransitionDuration, layer);

                    if (stateChanged)
                    {

                    }
                }
            }

            if (executeOnStart) {
                if (Time.time > m_StartEffectStartTime + m_EffectCooldown)
                    PlayEffect(m_StartEffect, ref m_StartEffectStartTime);

            }

        }


        public void StopAction()
        {
            ActionStopped();

            m_controller.useRootMotionPosition = m_useRootMotionPositionCached;
            m_controller.useRootMotionRotation = m_useRootMotionRotationCAched;

            m_isActive = false;
            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnCharacterActionActive, this, m_isActive);

            if (executeOnStop) {
                if (Time.time > m_EndEffectStartTime + m_EffectCooldown)
                    PlayEffect(m_EndEffect, ref m_EndEffectStartTime);
            }


            if (this is ItemAction) {
                if (m_actionID != -1) m_animatorMonitor.SetItemID(0);
                m_animatorMonitor.SetItemStateIndex(0);
                m_animatorMonitor.ResetItemStateTrigger();
            }
            else if(this is CharacterAction) {
                if (m_actionID != -1) m_animatorMonitor.SetActionID(0);
                m_animatorMonitor.SetActionIntData(0);
                m_animatorMonitor.SetActionFloatData(0f);
                m_animatorMonitor.ResetActionTrigger();
            }
            else {
                m_animatorMonitor.ResetActionTrigger();
                m_animatorMonitor.ResetItemStateTrigger();
            }


            m_ExitingAction = false;
            m_ActionStartTime = -1;
        }


        private GameObject PlayEffect(GameObject prefab)
        {
            if (prefab == null) return null;

            GameObject effect = null;
            if (ObjectPool.Instance != null){
                effect = ObjectPool.Get(prefab, m_transform.position, m_transform.rotation);
            }
            else{
                effect = Instantiate(prefab, m_transform.position, m_transform.rotation);
            }
            Debug.LogFormat("{0} has just spawned {1}", GetType(), effect.name);
            return effect;
        }

        private GameObject PlayEffect(GameObject prefab, ref float startTime)
        {
            if (prefab == null) return null;

            startTime = Time.time;
            GameObject effect = null;
            if (ObjectPool.Instance != null)
            {
                effect = ObjectPool.Get(prefab, m_transform.position, m_transform.rotation);
            }
            else
            {
                effect = Instantiate(prefab, m_transform.position, m_transform.rotation);
            }
            //Debug.LogFormat("{0} has just spawned {1}", GetType(), effect.name);
            return effect;
        }




        //  The action has started.  Best as an initializer.
        protected virtual void ActionStarted()
        {

        }


        //  The action has stopped.  Best for cleaning up after action is stopped.
        protected virtual void ActionStopped()
        {

        }


        //  When an action is about to be stopped, notify which action is starting.
        public virtual void ActionWillStart(CharacterAction nextAction)
        {

        }

        //  Should the action override the item's high priority state?
        public virtual bool OverrideItemState(int layer)
        {
            return false;
        }

        // Executed on every action to allow the action to update.
        // The action may need to update if it needs to do something when inactive or show a GUI icon when the ability can be started.
        public virtual void UpdateAction()
        {

        }

        //  Moves the character according to the input
        public virtual bool Move()
        {

            return true;
        }


        //  Ensure the current movement direction is valid.
        public virtual bool CheckMovement()
        {
            return true;
        }


        public virtual bool SetPhysicsMaterial()
        {
            return true;
        }

        //  Should the RigidbodyCharacterController continue execution of its UpdateMovement method?
        public virtual bool UpdateMovement()
        {
            return true;
        }

        //  Should the RigidbodyCharacterController continue execution of its UpdateRotation method?
        public virtual bool UpdateRotation()
        {
            return true;
        }


        // Updates the animator.  If true is returned, controller can continue with its animation.  
        // If false is returned, controller stops the current animation
        public virtual bool UpdateAnimator()
        {
            return true;
        }

        //  Can this ability run at the same time as another ability?
        public virtual bool IsConcurrentAction()
        {
            return false;
        }

        public virtual bool CanUseIK(int layer)
        {
            return true;
        }

        //  Called when another actioin is attempting to start and the current actioin is active.
        public virtual bool ShouldBlockActionStart(CharacterAction startingAction)
        {
            return false;
        }


        public virtual bool CheckGround()
        {
            return true;
        }


        public virtual bool HasAnimatorControl(int layer)
        {
            return true;
        }



        public virtual float GetColliderHeightAdjustment()
        {
            return m_collider.height - m_ColliderHeight;
        }


        //  Returns the state the given layer should be on.
        public virtual string GetDestinationState(int layer)
        {
            if (string.IsNullOrWhiteSpace(m_stateName))
                return "";
            var layerName = m_animator.GetLayerName(layer);
            var destinationState = layerName + "." + m_stateName;

            if(m_animator.HasState(layer, Animator.StringToHash(destinationState)))
                return destinationState;

            return "";
        }


        public virtual float GetTransitionDuration()
        {
            int layerIndex = 0;
            float transitionDuration = m_animator.GetAnimatorTransitionInfo(layerIndex).duration;
            return transitionDuration;
        }


        public virtual float GetNormalizedTime()
        {
            int layerIndex = 0;
            //float normalizedTime = m_animator.GetCurrentAnimatorStateInfo(m_animatorMonitor.BaseLayerIndex).normalizedTime % 1;
            return m_animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime % 1; ;
        }


        //protected void ScaleCapsule(float scaleFactor)
        //{
        //    if (m_collider.height != m_ColliderHeight * scaleFactor)
        //    {
        //        m_collider.height = Mathf.MoveTowards(m_collider.height, m_ColliderHeight * scaleFactor, Time.deltaTime * 4);
        //        m_collider.center = Vector3.MoveTowards(m_collider.center, m_ColliderCenter * scaleFactor, Time.deltaTime * 2);
        //    }
        //}





        //private void OnAnimatorMove()
        //{

        //    if (m_isActive && m_controller.UseRootMotion && m_ApplyBuiltinRootMotion)
        //        m_animator.ApplyBuiltinRootMotion();
        //}










        #region Debug




        //Color debugTextColor = new Color(0, 0.6f, 1f, 1);
        private GUIStyle textStyle = new GUIStyle();
        private GUIStyle style = new GUIStyle();

        private Vector2 size;

        private Rect location = new Rect();
        protected GUIContent content = new GUIContent();
        protected string debugMsg;

        private void OnGUI()
        {
            if (Application.isPlaying && m_isActive)
            {
                GUI.color = Color.black;
                textStyle.fontStyle = FontStyle.Bold;
                size = new GUIStyle(GUI.skin.label).CalcSize(content);
                location.Set(Screen.width - size.x - 10, 15, size.x, size.y * 2);
                GUILayout.BeginArea(location, GUI.skin.box);

                //GUILayout.Label(string.Format("Transition Duration: {0}", GetTransitionDuration()));
                DrawOnGUI();

                GUILayout.Label(content);

                GUILayout.EndArea();
            }
        }


        protected virtual void DrawOnGUI()
        {
            //content.text = string.Format("-- {0} --", GetType());
        }


        #endregion

    }
}
