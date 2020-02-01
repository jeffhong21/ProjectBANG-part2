using UnityEngine;
using System.Collections;

namespace CharacterController
{
    using DebugUI;

    public partial class RigidbodyCharacterControllerBase
    {


        #region Debugging





        protected virtual void DebugAttributes()
        {
            //if (debugger.states.showDebugUI == false) return;

            DebugUI.Log(this, "inputVector", inputVector, RichTextColor.Green);
            DebugUI.Log(this, "inputDirection", inputDirection, RichTextColor.Green);
            DebugUI.Log(this, "m_velocity", m_velocity, RichTextColor.Green);
            DebugUI.Log(this, "m_inputMagnitude", m_inputMagnitude, RichTextColor.Green);
            DebugUI.Log(this, "isGrounded", isGrounded, RichTextColor.White);
            

            DebugUI.Log(this, "fallTime", fallTime, RichTextColor.Green);
            DebugUI.Log(this, "m_moveDirection", moveDirection, RichTextColor.Green);
            DebugUI.Log(this, "velocity", m_velocity, RichTextColor.White);

            //DebugUI.Log(this, "m_viewAngle", m_viewAngle, RichTextColor.White);





            //DebugUI.Log(this, "r_velocity", m_rigidbody.velocity, RichTextColor.White);
            //DebugUI.Log(this, "m_moveSpeed", m_moveSpeed, RichTextColor.Green);
            //DebugUI.Log(this, "fwd_speed", Vector3.Dot(moveDirection/m_deltaTime, m_transform.forward), RichTextColor.White);
  


        }



        protected abstract void DrawGizmos();



        private void OnDrawGizmos()
        {
            if (Debugger.debugMode && Application.isPlaying) {



                Debugger.DrawGizmos();
                //  Draw Gizmos
                DrawGizmos();
            }

        }


        #endregion



        #region Parameters for Editor

        //  For Editor.
        //  Debug parameters.
        [HideInInspector]
        public CharacterControllerDebugger Debugger { get { return debugger; } }
        protected bool DebugGroundCheck { get { return Debugger.states.showGroundCheck; } }
        protected bool DebugCollisions { get { return Debugger.states.showCollisions; } }
        protected bool DebugMotion { get { return Debugger.states.showMotion; } }
        [SerializeField, HideInInspector]
        private bool displayMovement, displayPhysics, displayCollisions, displayAnimations, displayActions;

        #endregion
    }

}
