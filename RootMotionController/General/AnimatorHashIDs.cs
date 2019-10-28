using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController
{
    public class Hash
    {
        public static readonly int ForwardSpeed = Animator.StringToHash("Vertical");
        public static readonly int LateralSpeed = Animator.StringToHash("Horizontal");
        public static readonly int VerticalSpeed = Animator.StringToHash("Horizontal");

        public static readonly int RotationSpeed = Animator.StringToHash("Horizontal");

        public static readonly int IsMoving = Animator.StringToHash("IsMoving");
    }
}
