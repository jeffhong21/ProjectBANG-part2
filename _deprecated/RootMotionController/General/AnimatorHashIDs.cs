using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController
{
    public static class Hash
    {
        public static readonly int ForwardSpeed = Animator.StringToHash("ForwardInput");
        public static readonly int LateralSpeed = Animator.StringToHash("HorizontalInput");
        public static readonly int TurnSpeed = Animator.StringToHash("TurnSpeed");
        public static readonly int ViewAngle = Animator.StringToHash("ViewAngle");



        public static readonly int Moving = Animator.StringToHash("Moving");
    }
}
