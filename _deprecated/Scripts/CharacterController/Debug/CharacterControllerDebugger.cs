namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using DebugUI;

    [Serializable]
    public class CharacterControllerDebugger
    {
        DebugUIView debugUIView;
        RigidbodyCharacterControllerBase character;
        Transform transform;

        [Serializable]
        public class DebugModeStates
        {
            public bool showDebugUI;
            public bool showGroundCheck;
            public bool showCollisions;
            public bool showMotion;
        }

        [Serializable]
        public class DebugColors
        {
            public Color moveDirectionColor = Color.blue;
            public Color velocityColor = Color.green;
            public Color magenta = new Color(0.75f, 0, 0.75f, 0.9f);

            public Color animatorColor = new Color(0, 0.7f, 1f, 1);
            public Color yellow1 = new Color(1, 0.92f, 0.16f, 1);
        }




        public bool debugMode;

        public DebugModeStates states;
        public DebugColors colors;
        [Range(0.01f, 1f)]
        public float heightOffset = 0.05f;


        [SerializeField]
        private float arrowTipSize = 0.1f;
        [SerializeField]
        private float arrowWidth = 0.2f;

        private Queue<Delegate> drawActions;





        protected Vector3 VectorOffset { get { return new Vector3(0, heightOffset, 0); } }



        public CharacterControllerDebugger(){ }

        public CharacterControllerDebugger(CharacterControllerDebugger debugger)
        {
            Initialize(debugger.character);
        }


        public void Initialize(RigidbodyCharacterControllerBase character)
        {
            this.character = character;
            this.transform = character.transform;

            if (DebugUIView.Instance == null)
                debugUIView = Resources.Load<DebugUIView>("DebugUIView");
            debugUIView = UnityEngine.Object.Instantiate(debugUIView);
            //Debug.Log(DebugUIView.Instance);

            debugUIView.gameObject.SetActive(states.showDebugUI);
        }






        public void DrawRayFromOrigin(Vector3 direction, float heightOffset)
        {
            var start = transform.position + Vector3.up * heightOffset;
            var end = transform.TransformDirection(direction) + start;
            DrawRay(start, end, Color.blue);
        }



        private void DrawRay(Vector3 start, Vector3 end, Color color, bool drawEndPosition = true)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(start, end);
        }


        public Vector3 TransformOriginCast(float yOffset, Vector3 posOffset = default)
        {
            return transform.position + Vector3.up * yOffset + posOffset;
        }


        public void DrawGizmos()
        {
            if (!debugMode) return;







            //Gizmos.color = Color.white;
            //Gizmos.DrawRay(transform.position + Vector3.up * 1.5f, m_transform.InverseTransformDirection(LookRotation * m_transform.forward));
            //GizmosUtils.DrawText(GUI.skin, "LookDirection", transform.position + Vector3.up * 1.5f + LookRotation * transform.forward, Color.green);

            //if(m_rigidbody.velocity != Vector3.zero) {
            //    Gizmos.color = Color.green;
            //    GizmosUtils.DrawArrow(RaycastOrigin, m_rigidbody.velocity);
            //    GizmosUtils.DrawText(GUI.skin, "Velocity", RaycastOrigin + transform.forward, Color.green);
            //}
            //Gizmos.color = Color.cyan;
            //Gizmos.DrawWireSphere(m_transform.position, ColliderRadius);
            //Gizmos.color = Color.yellow;
            //GizmosUtils.DrawWireCircle(m_animator.pivotPosition, 0.1f);
            //GizmosUtils.DrawMarker(m_animator.rootPosition, 0.1f, Color.magenta);

            //if (DebugCollisions) GizmosUtils.DrawWireCapsule(RaycastOrigin + charCollider.center, charCollider.radius + m_skinWidth, charCollider.height - m_skinWidth);


            //GizmosUtils.DrawText(GUI.skin, MathUtil.Round(m_targetAngle).ToString(), transform.position + Vector3.up * 1.8f, Color.black);


            //if (states.showMotion)
            //{
            //    Gizmos.color = Color.white;
            //    Gizmos.DrawRay(transform.position + Vector3.up, transform.InverseTransformDirection(character.LookRotation * transform.TransformDirection(transform.forward)));




            //    Vector3 origin = transform.position + Vector3.up * heightOffset;
            //    //  Move direction vector.
            //    Gizmos.color = Color.blue;
            //    GizmosUtils.DrawArrow(TransformOriginCast(heightOffset), transform.forward, 1f, arrowTipSize, arrowWidth);
            //    //  Draw rotation vector.
            //    Gizmos.color = Color.magenta;
            //    GizmosUtils.DrawArrow(TransformOriginCast(heightOffset + 0.1f), Quaternion.Inverse(transform.rotation) * transform.TransformDirection(character.moveDirection), 1f, arrowTipSize, arrowWidth);


            //    //Gizmos.color = Color.blue;
            //    ////  Velocity vector.
            //    //GizmosUtils.DrawArrow(origin, m_velocity, m_velocity.magnitude, arrowTipSize, arrowWidth);
            //}




        }


        public void DrawSceneGUI()
        {

        }
    }
}


