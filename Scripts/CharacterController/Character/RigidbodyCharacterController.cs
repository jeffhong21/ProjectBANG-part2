using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;


namespace CharacterController
{
    using DebugUI;

    public class RigidbodyCharacterController : RigidbodyCharacterControllerBase
    {
        private readonly string FallbackMoveStateName = "Movement";
        private readonly string FallbackAirborneStateName = "Airborne";

        public event Action<bool> OnAim = delegate {};

        #region Parameters
        [Serializable]
        public class AnimationSettings
        {
            [Tooltip("The name of the state that should be activated when the character is moving.")]
            public string moveStateName = "Movement";
            [Tooltip("The name of the state that should be activated when the character is airborne.")]
            public string airborneStateName = "Airborne";

            public AnimationSettings()
            {
                moveStateName = IsNullOrWhiteSpace(moveStateName, "Movement");
                airborneStateName = IsNullOrWhiteSpace(airborneStateName, "Airborne");
            }

            private string IsNullOrWhiteSpace(string name, string fallbackName = "Idle")
            {
                if (string.IsNullOrWhiteSpace(name)) {
                    string fallback = string.IsNullOrWhiteSpace(fallbackName) ? "Idle" : fallbackName;
                    return fallbackName;
                }
                return name; 
            }
        }

        [Serializable]
        public class ActionSettings
        {
            public CharacterAction[] actions;
            public CharacterAction activeAction;
        }

        [Serializable]
        public class ItemActionSettings
        {
            public ItemAction[] actions;
        }

        [SerializeField]
        private AnimationSettings m_animation = new AnimationSettings();
        [SerializeField]
        private ActionSettings m_charActions = new ActionSettings();
        [SerializeField]
        private ItemActionSettings m_itemActions = new ItemActionSettings();
        #endregion


        //  --  Character Actions
        [SerializeField, HideInInspector, FormerlySerializedAs("m_actions")]
        private CharacterAction[] m_actions;
        [SerializeField, HideInInspector, FormerlySerializedAs("m_activeAction")]
        private CharacterAction m_activeAction;                     //  Current active action that is not concurrent.

        private List<CharacterAction> m_activeActions;              //  Holds all the active actions.

        private Dictionary<CharacterAction, int> m_actionInfo;      //  Used for getting action priority.






        private bool m_frameUpdated;

        private Vector3 leftFootPosition, rightFootPosition;



        #region Properties


        public bool Aiming { get; set; }

        public bool CanAim { get => isGrounded; }

        public string MoveStateName {
            get{
                if (string.IsNullOrWhiteSpace(m_animation.moveStateName))
                    m_animation.moveStateName = FallbackMoveStateName;
                return m_animation.moveStateName;
            }
        }

        public string AirborneStateName{
            get {
                if (string.IsNullOrWhiteSpace(m_animation.airborneStateName))
                    m_animation.moveStateName = FallbackAirborneStateName;
                return m_animation.airborneStateName;
            }
        }


        public CharacterAction[] CharActions { get { return m_actions; } set { m_actions = value; } }

        public int ActiveActionIndex{
            get {
                if (m_activeAction != null)
                    return Array.IndexOf(m_actions, m_activeAction);
                return -1;
            }
        }
        

        #endregion







        private void Awake()
        {
            m_gameObject = gameObject;
            m_transform = transform;

            m_animatorMonitor = GetComponent<AnimatorMonitor>();

            m_physicsMaterial = new PhysicMaterial() { name = "Character Physics Material" };

            //  --------------------------
            //  Init
            //  --------------------------
            InitializeVariables();

            //  --------------------------
            //  Initialize actions;
            //  --------------------------
            m_actionInfo = new Dictionary<CharacterAction, int>();
            m_activeActions = new List<CharacterAction>();
            for (int i = 0; i < m_actions.Length; i++)
            {
                if (!m_actionInfo.ContainsKey(m_actions[i])) m_actionInfo.Add(m_actions[i], i);
                else m_actionInfo[m_actions[i]] = i;

                m_actions[i].Initialize(this, Time.deltaTime);
            }

            //  --------------------------
            //  Initialize events;
            //  --------------------------
            //EventHandler.RegisterEvent<CharacterAction, bool>(m_gameObject, EventIDs.OnCharacterActionActive, OnActionActive);
            //EventHandler.RegisterEvent<ItemAction, bool>(m_gameObject, EventIDs.OnItemActionActive, OnItemActionActive);
            //EventHandler.RegisterEvent<bool>(m_gameObject, EventIDs.OnAimActionStart, OnAimActionStart);

            //  --------------------------
            //  Time manager
            //  --------------------------

            //  --------------------------

            //  --------------------------
            //  Initialize Debugger;
            //  --------------------------
            Debugger.Initialize(this);
        }


  //      protected void OnDestroy()
		//{
  //          //EventHandler.UnregisterEvent<CharacterAction, bool>(m_gameObject, EventIDs.OnCharacterActionActive, OnActionActive);
  //          //EventHandler.UnregisterEvent<ItemAction, bool>(m_gameObject, EventIDs.OnItemActionActive, OnItemActionActive);
  //          //EventHandler.UnregisterEvent<bool>(m_gameObject, EventIDs.OnAimActionStart, OnAimActionStart);
  //      }




        //private void FixedUpdate()
        //{

        //    VELOCITY_MOVE();

        //}


        private void Update()
        {

            if (m_frameUpdated) return;
            OnUpdate(m_deltaTime);
            m_frameUpdated = true;
        }



        private void LateUpdate()
        {
            DebugAttributes();


            //if (m_useRootMotionPosition)
            //    m_rigidbody.MovePosition(m_animator.rootPosition);

            //if (m_useRootMotionRotation)
            //    m_rigidbody.MoveRotation(m_animator.rootRotation);

            //m_inputVector = Vector3.zero;

            //Continue with updates.
            if (m_frameUpdated) {
                m_frameUpdated = false;
                return;
            }
        }


        private void OnAnimatorMove()
        {
            AnimatorMove();

            if(m_rigidbody.isKinematic) {
                ApplyRotation();
                ApplyMovement();
            }

        }


        private void OnAnimatorIK(int layerIndex)
        {
            leftFootPosition = GetFootPosition(AvatarIKGoal.LeftFoot);
            rightFootPosition = GetFootPosition(AvatarIKGoal.RightFoot);
        }



        public override void Move(float horizontal, float forward, Quaternion rotation)
        {
            base.Move(horizontal, forward, rotation);
        }



        #region Character Locomotion


        private void OnUpdate(float deltaTime)
        {
            m_deltaTime = deltaTime;

            m_activeActions.Clear();
            ////  Allows us to stop the active action before closing the loop off to lower priority actions.
            //bool activeActionUpdated = false;
            for (int i = 0; i < m_actions.Length; i++)
            {
                //  Move to next action if action componenet is disabled.
                if (!m_actions[i].enabled) continue;


                CharacterAction action = m_actions[i];


                //  TODO:  Implement a way to stop iterating through list after active action.
                ////  If action is not the active action, than try to start or stop the action.
                //if(action != m_activeAction && !activeActionUpdated)
                //{

                //}

                if (action.IsActive) {
                    //  If action is active and stop type is not manual, try to stop it.
                    if (action.StopType != ActionStopType.Manual) {
                        TryStopAction(action);
                    }
                }
                else {
                    if (action.StartType != ActionStartType.Manual) {
                        bool actionStarted = TryStartAction(action);
                        if (actionStarted) {
                            //if (action.IsConcurrentAction()) {
                            //    continue;
                            //}
                            m_activeAction = action;
                            //break;
                        }
                    }
                }


                //  Perform any cleanups for the actions.
                if (action == m_activeAction || action.IsActive)
                m_activeActions.Add(action);

            }

            InternalMove();
            //  Move.
            if (m_activeAction != null) {
                if (m_activeAction.Move()) Move();
            }
            else Move();
            //  Check ground.
            if (m_activeAction != null) {
                if (m_activeAction.CheckGround()) CheckGround();
            }
            else CheckGround();




            //  Check movement.
            if (m_activeAction != null) {
                if (m_activeAction.CheckMovement()) CheckMovement();
            }
            else CheckMovement();
            //  Update movement.
            if (m_activeAction != null) {
                if (m_activeAction.UpdateMovement()) UpdateMovement();
            }
            else UpdateMovement();
            //  Update rotation
            if (m_activeAction != null) {
                if (m_activeAction.UpdateRotation()) UpdateRotation();
            }
            else UpdateRotation();
            //  Set Physocs materials.
            if (m_activeAction != null) {
                if (m_activeAction.SetPhysicsMaterial()) SetPhysicsMaterial();
            }
            else SetPhysicsMaterial();
            //  Update animator.
            if (m_activeAction != null) {
                if (m_activeAction.UpdateAnimator()) UpdateAnimator();
            }
            else UpdateAnimator();


        }

        //  Float value to indicate which leg is moving forward.  Right leg is 0, Left leg is 1.  (opposite of pivotWeight)
        float m_legIndex = 0.5f;



        /// <summary>
        /// Updates the animator.
        /// </summary>
        protected override void UpdateAnimator()
        {
            base.UpdateAnimator();

            float forwardLeg = 0.5f;
            if (isMoving)
                forwardLeg = leftFootPosition.normalized.z > rightFootPosition.normalized.z ? 0 : 1;
            m_legIndex = Mathf.Lerp(m_legIndex, forwardLeg, m_deltaTime * 8);
            m_animator.SetFloat(HashID.LegFwdIndex, m_legIndex);
        }


        /// <summary>
        /// Set the collider's physics material.
        /// </summary>
        protected void SetPhysicsMaterial()
        {
            //change the physics material to very slip when not grounded or maxFriction when is

            //  Airborne.
            if (!isGrounded && Mathf.Abs(m_rigidbody.velocity.y) > 0) {
                characterCollider.material.staticFriction = 0f;
                characterCollider.material.dynamicFriction = 0f;
                characterCollider.material.frictionCombine = PhysicMaterialCombine.Minimum;
            }
            //  isGrounded and is moving.
            else if (isGrounded && isMoving) {
                characterCollider.material.staticFriction = 0.25f;
                characterCollider.material.dynamicFriction = 0f;
                characterCollider.material.frictionCombine = PhysicMaterialCombine.Multiply;
            }
            //  isGrounded but not moving.
            else if (isGrounded && !isMoving) {
                characterCollider.material.staticFriction = 1f;
                characterCollider.material.dynamicFriction = 1f;
                characterCollider.material.frictionCombine = PhysicMaterialCombine.Maximum;
            }
            else {
                characterCollider.material.staticFriction = 1f;
                characterCollider.material.dynamicFriction = 1f;
                characterCollider.material.frictionCombine = PhysicMaterialCombine.Maximum;
            }

        }



        #endregion



        /// <summary>
        /// Get the foot world position.
        /// </summary>
        public Vector3 GetFootWorldPosition()
        {
            return transform.position + m_colliderCenter + (Vector3.down * (m_colliderHeight / 2.0f + m_physics.skinWidth));
        }




        #region Character Actions


        public T GetAction<T>() where T : CharacterAction
        {
            for (int i = 0; i < m_actions.Length; i++){
                if (m_actions[i] is T){
                    return (T)m_actions[i];
                }
            }
            return null;
        }


        public bool TryStartAction(CharacterAction action)
        {
            if (action == null) return false;

            int index = Array.IndexOf(m_actions, action);
            //  If there is an active action and current action is non concurrent.
            if (m_activeAction != null && !action.IsConcurrentAction())
            {
                int activeActionIndex = Array.IndexOf(m_actions, m_activeAction);
                //Debug.LogFormat("Action index {0} | Active Action index {1}", index, activeActionIndex);
                if (index < activeActionIndex)
                {
                    if (action.CanStartAction())
                    {
                        //  Stop the current active action.
                        TryStopAction(m_activeAction);
                        //  Set the active action.
                        m_activeAction = m_actions[index];
                        //m_activeAction[index] = m_actions[index];
                        action.StartAction();
                        //action.UpdateAnimator();
                        return true;
                    }
                }
            }
            //  If there is an active action and current action is concurrent.
            else if (m_activeAction != null && action.IsConcurrentAction())
            {
                if (action.CanStartAction())
                {
                    //m_activeAction[index] = m_actions[index];
                    action.StartAction();
                    //action.UpdateAnimator();
                    return true;
                }
            }
            //  If there is no active action.
            else if (m_activeAction == null)
            {
                if (action.CanStartAction())
                {
                    m_activeAction = m_actions[index];
                    //m_activeAction[index] = m_actions[index];
                    action.StartAction();
                    //action.UpdateAnimator();
                    return true;
                }
            }
            else {

                
            }

            return false;
        }


        public void TryStopAllActions()
        {
            for (int i = 0; i < m_actions.Length; i++)
            {
                if (m_actions[i].IsActive)
                {
                    TryStopAction(m_actions[i]);
                }
            }
        }


        public void TryStopAction(CharacterAction action)
        {
            if (action == null) return;

            if (action.CanStopAction())
            {
                int index = Array.IndexOf(m_actions, action);
                if (m_activeAction == action)
                    m_activeAction = null;


                action.StopAction();
                ActionStopped();
            }
        }


        public void TryStopAction(CharacterAction action, bool force)
        {
            if (action == null) return;
            if (force)
            {
                //int index = Array.IndexOf(m_actions, action);
                if (m_activeAction == action)
                    m_activeAction = null;
                action.StopAction();
                ActionStopped();
                return;
            }

            TryStopAction(action);
        }

        #endregion



        #region Event Actions


        private void OnActionActive(CharacterAction action, bool activated)
        {
            int index = Array.IndexOf(m_actions, action);
            if (action == m_actions[index])
            {
                if (m_actions[index].enabled)
                {
                    if(activated)
                    {
                        //Debug.LogFormat(" {0} is starting.", action.GetType().Name);

                    }
                    else
                    {

                    }
                }
            }

        }


        private void OnItemActionActive( ItemAction action, bool activated )
        {

        }


        private void OnAimActionStart( bool aim )
        {
            Aiming = aim;
            movementType = Aiming ? MovementTypes.Combat : MovementTypes.Adventure;
        }


        private void OnImpactHit()
        {

        }

        private void ActionStopped()
        {

        }

        #endregion



        public int GetActionPriority(CharacterAction action)
        {
            if (!m_actionInfo.TryGetValue(action, out int index)) index = -1;
            return index;
        }


        public void SetMovementType( MovementTypes movement )
        {
            movementType = movement;
        }






        private Vector3 GetFootPosition(AvatarIKGoal foot, bool worldPos = false)
        {
            if (foot == AvatarIKGoal.LeftFoot || foot == AvatarIKGoal.RightFoot) {
                Vector3 footPos = m_animator.GetIKPosition(foot);
                Quaternion footRot = m_animator.GetIKRotation(foot);
                float botFootHeight = foot == AvatarIKGoal.LeftFoot ? m_animator.leftFeetBottomHeight : m_animator.rightFeetBottomHeight;
                Vector3 footHeight = new Vector3(0, -botFootHeight, 0);


                footPos += footRot * footHeight;

                return !worldPos ? footPos : m_transform.InverseTransformPoint(footPos);
            }

            return Vector3.zero;
        }

        private Vector3 GetFootPosition(HumanBodyBones foot, bool worldPos = false)
        {
            int side = 0;
            if (foot == HumanBodyBones.LeftFoot || foot == HumanBodyBones.LeftToes) side = -1;
            else if (foot == HumanBodyBones.RightFoot || foot == HumanBodyBones.RightToes) side = 1;

            if (side == -1 || side == 1)
            {
                foot = side == -1 ? HumanBodyBones.LeftToes : HumanBodyBones.RightToes;
                Vector3 footPos = m_animator.GetBoneTransform(foot).localPosition;
                Quaternion footRot = m_animator.GetBoneTransform(foot).localRotation;
                //var rot = Quaternion.
                float botFootHeight = side == -1 ? m_animator.leftFeetBottomHeight : m_animator.rightFeetBottomHeight;
                Vector3 footHeight = new Vector3(0,side * -botFootHeight, 0);

                if (side == -1) footPos = Quaternion.Euler(0, 0, 180) * footPos;
                footPos = footRot * footPos + footHeight;
                //footPos += footRot * footHeight;

                

                return worldPos ? m_animator.GetBoneTransform(foot).TransformPoint(footPos) : footPos;
            }

            return Vector3.zero;
        }




        //------


        protected override void DebugAttributes()
        {
            base.DebugAttributes();

            var lf = m_transform.position - GetFootPosition(HumanBodyBones.LeftFoot, true);
            lf = m_animator.GetBoneTransform(HumanBodyBones.LeftFoot).localPosition;
            DebugUI.Log(this, m_animator.GetBoneTransform(HumanBodyBones.LeftFoot).name, lf, RichTextColor.Orange);
            var rf = m_transform.position - GetFootPosition(HumanBodyBones.RightFoot, true);
            rf = m_animator.GetBoneTransform(HumanBodyBones.RightFoot).localPosition;
            DebugUI.Log(this, m_animator.GetBoneTransform(HumanBodyBones.RightFoot).name, rf, RichTextColor.Orange);

        }



        protected override void DrawGizmos()
        {
            //if(Application.isPlaying) {
            //    GizmosUtils.DrawWireSphere(GetFootWorldPosition(), 0.5f);
            //}

        }




    }
}


//protected bool canCheckGround;
//protected bool canCheckMovement;
//protected bool canSetPhysicsMaterial;
//protected bool canUpdateRotation;
//protected bool canUpdateMovement;
//protected bool canUpdateAnimator;
//protected bool canMove;


//private void ActionUpdate()
//{
//    timeScale = Time.timeScale;
//    if (Math.Abs(timeScale) < float.Epsilon) return;
//    m_deltaTime = deltaTime;


//    canMove = true;

//    ////  Start Stop Actions.
//    for (int i = 0; i < m_actions.Length; i++)
//    {
//        if (m_actions[i].enabled == false)
//            continue;
//        CharacterAction charAction = m_actions[i];
//        //  If action was started, move onto next action.
//        StopStartAction(charAction);
//        //if (StopStartAction(charAction)) continue;

//        if (charAction.IsActive)
//        {
//            //  Move charatcer based on input values.
//            if (canMove) canMove = charAction.Move();

//            //// Update the Animator.
//            //if (m_UpdateAnimator) m_UpdateAnimator = charAction.UpdateAnimator();
//        }
//        //  Call Action Update.
//        charAction.UpdateAction();
//    }

//    //  Moves the character according to the input.
//    InternalMove();
//    if (canMove) Move();
//}

//private void ActionFixedUpdate()
//{
//    timeScale = Time.timeScale;
//    if (Math.Abs(timeScale) < float.Epsilon) return;
//    m_deltaTime = deltaTime;


//    //canMove = true;

//    canCheckGround = true;
//    canCheckMovement = true;
//    canSetPhysicsMaterial = true;
//    canUpdateRotation = true;
//    canUpdateMovement = true;
//    canUpdateAnimator = true;


//    for (int i = 0; i < m_actions.Length; i++)
//    {
//        if (m_actions[i].enabled == false)
//        {
//            //  Call Action Update.
//            m_actions[i].UpdateAction();
//            continue;
//        }
//        CharacterAction charAction = m_actions[i];
//        if (charAction.IsActive)
//        {
//            ////  Move charatcer based on input values.
//            //if (canMove) canMove = charAction.Move();

//            //  Perform checks to determine if the character is on the ground.
//            if (canCheckGround) canCheckGround = charAction.CheckGround();
//            //  Ensure the current movement direction is valid.
//            if (canCheckMovement) canCheckMovement = charAction.CheckMovement();
//            //  Apply any movement.
//            if (canUpdateMovement) canUpdateMovement = charAction.UpdateMovement();
//            //  Update the rotation forces.
//            if (canUpdateRotation) canUpdateRotation = charAction.UpdateRotation();

//            // Update the Animator.
//            if (canUpdateAnimator) canUpdateAnimator = charAction.UpdateAnimator();

//        }
//        //  Call Action Update.
//        charAction.UpdateAction();
//    }  //  end of for loop


//    ////  Moves the character according to the input.
//    //if (canMove) Move();


//    //  Perform checks to determine if the character is on the ground.
//    if (canCheckGround) CheckGround();
//    //  Ensure the current movement direction is valid.
//    if (canCheckMovement) CheckMovement();
//    //  Set the physic material based on the grounded and stepping state
//    if (canSetPhysicsMaterial) SetPhysicsMaterial();

//    //  Apply any movement.
//    if (canUpdateMovement) UpdateMovement();
//    //  Update the rotation forces.
//    if (canUpdateRotation) UpdateRotation();

//    // Update the Animator.
//    if (canUpdateAnimator) UpdateAnimator();


//    ApplyMovement();
//    ApplyRotation();
//}





//public bool TryStartAction(CharacterAction action)
//{
//    if (action == null) return false;

//    int actionPriority = Array.IndexOf(m_actions, action);
//    //  If there is an active action and current action is non concurrent.
//    if (m_activeAction != null && action.IsConcurrentAction() == false)
//    {
//        int activeActionPriority = Array.IndexOf(m_actions, m_activeAction);
//        //Debug.LogFormat("Action index {0} | Active Action index {1}", index, activeActionIndex);
//        if (actionPriority < activeActionPriority)
//        {
//            if (action.CanStartAction())
//            {
//                //  Stop the current active action.
//                TryStopAction(m_activeAction, true);
//                //  Set the active action.
//                m_activeAction = action;
//                action.StartAction();
//                return true;
//            }
//        }
//    }
//    //  If there is an active action and current action is concurrent.
//    else if (m_activeAction != null && action.IsConcurrentAction())
//    {
//        if (action.CanStartAction())
//        {
//            //m_activeAction[index] = m_actions[index];
//            action.StartAction();
//            //action.UpdateAnimator();
//            return true;
//        }
//    }
//    //  If there is no active action.
//    else
//    {
//        if (action.CanStartAction())
//        {
//            m_activeAction = m_actions[actionPriority];
//            //m_activeAction[index] = m_actions[index];
//            action.StartAction();
//            //action.UpdateAnimator();
//            return true;
//        }
//    }


//    return false;
//}


//public void TryStopAction(CharacterAction action, bool force)
//{
//    if (force)
//    {
//        if (action == null || !action.IsActive || !action.enabled) return;

//        if (action.enabled && action.IsActive)
//        {
//            action.StopAction();
//            ActionStopped();
//        }

//    }
//    else
//    {
//        TryStopAction(action);
//    }






//}


//public void TryStopAction(CharacterAction action)
//{
//    if (action == null || !action.IsActive || !action.enabled) return;

//    if (action.enabled && action.IsActive)
//    {
//        if (action.CanStopAction())
//        {
//            action.StopAction();
//            if (m_activeAction == action)
//                m_activeAction = null;
//            ActionStopped();
//            return;
//        }
//    }
//}






///// <summary>
///// Starts and Stops the Action internally.
///// </summary>
///// <param name="charAction"></param>
///// <returns>Returns true if action was started.</returns>
//protected void StopStartAction(CharacterAction charAction)
//{
//    //  First, check if current Action can Start or Stop.

//    //  Current Action is Active.
//    if (charAction.enabled && charAction.IsActive)
//    {
//        //  Check if can stop Action is StopType is NOT Manual.
//        if (charAction.StopType != ActionStopType.Manual)
//        {
//            if (charAction.CanStopAction())
//            {
//                //  Start the Action and update the animator.
//                charAction.StopAction();
//                //  Reset Active Action.
//                if (m_activeAction = charAction)
//                    m_activeAction = null;
//                //  Move on to the next Action.
//                return;
//            }
//        }
//    }
//    //  Current Action is NOT Active.
//    else
//    {
//        //  Check if can start Action is StartType is NOT Manual.
//        if (charAction.enabled && charAction.StartType != ActionStartType.Manual)
//        {
//            if (m_activeAction == null)
//            {
//                if (charAction.CanStartAction())
//                {
//                    //  Start the Action and update the animator.
//                    charAction.StartAction();
//                    //charAction.UpdateAnimator();
//                    //  Set active Action if not concurrent.
//                    if (charAction.IsConcurrentAction() == false)
//                        m_activeAction = charAction;
//                    //  Move onto the next Action.
//                    return;
//                }
//            }
//            else if (charAction.IsConcurrentAction())
//            {
//                if (charAction.CanStartAction())
//                {
//                    //  Start the Action and update the animator.
//                    charAction.StartAction();
//                    //charAction.UpdateAnimator();
//                    //  Move onto the next Action.
//                    return;
//                }
//            }
//            else
//            {

//            }
//        }
//    }

//    return;
//}