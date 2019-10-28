namespace CharacterController
{
    using UnityEngine;


    public static class HashID
    {
        public static readonly int InputMagnitude = Animator.StringToHash("InputMagnitude");
        public static readonly int StartAngle = Animator.StringToHash("StartAngle");
        public static readonly int LegFwdIndex = Animator.StringToHash("LegFwdIndex");
        //public static readonly int isGrounded = Animator.StringToHash("isGrounded");


        public static readonly int HorizontalInput = Animator.StringToHash("HorizontalInput");
        public static readonly int ForwardInput = Animator.StringToHash("ForwardInput");
        public static readonly int Yaw = Animator.StringToHash("Yaw");
        public static readonly int Pitch = Animator.StringToHash("Pitch");
        public static readonly int ActionID = Animator.StringToHash("ActionID");
        public static readonly int ActionIntData = Animator.StringToHash("ActionIntData");
        public static readonly int ActionFloatData = Animator.StringToHash("ActionFloatData");
        public static readonly int MovementSetID = Animator.StringToHash("MovementSetID");

        public static readonly int ItemID = Animator.StringToHash("ItemID");
        public static readonly int ItemStateIndex = Animator.StringToHash("ItemStateIndex");
        public static readonly int ItemStateIndexChange = Animator.StringToHash("ItemStateIndexChange");
        public static readonly int ItemSubstateIndex = Animator.StringToHash("ItemSubstateIndex");

        public static readonly int Moving = Animator.StringToHash("Moving");
        public static readonly int Aiming = Animator.StringToHash("Aiming");
        public static readonly int Crouching = Animator.StringToHash("Crouching");
        public static readonly int Grounded = Animator.StringToHash("Grounded");


        public static readonly int Height = Animator.StringToHash("Height");
        public static readonly int Speed = Animator.StringToHash("Speed");

        public static readonly int ColliderHeight = Animator.StringToHash("ColliderHeight");

        public static readonly int ActionChange = Animator.StringToHash("ActionChange");


    }
}