namespace CharacterController
{
    using UnityEngine;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;


    public class MovementType
    {
        private static HashSet<MovementType> g_MovementTypes;


        public MovementType()
        {
            var movementTypes = Assembly.GetAssembly(typeof(MovementType)).GetTypes()
                          .Where(t => t.IsClass && t.IsSubclassOf(typeof(MovementType))).ToArray();

            //for (int i = 0; i < movementTypes.Length; i++) {
            //    var mt = movementTypes[i].DeclaringType;
            //}
        }




        public float GetMoveDirectionInDegrees(float characterHorizontalMovement, float characterFwdMovement)
        {
            throw new System.NotImplementedException();
        }



        public Quaternion GetTargetRotation()
        {
            throw new System.NotImplementedException();
        }




        public Vector2 GetInputDirection(Vector2 inputVector)
        {
            throw new System.NotImplementedException();
        }


        public float GetRotationAngle( float charHorizontalMovement, float charFwdMovement, float cameraHorizontalMovement, float cameraVerticalMovement)
        {

            throw new System.NotImplementedException();
        }
    }

}

