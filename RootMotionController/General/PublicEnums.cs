using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController
{

    public enum LocomotionState
    {
        Standing = 0,
        Walking = 1,
        Running = 2,
        Sprinting = 3
    }


    public enum MovementSet
    {
        Default = 0,
        Strafe = 1,
        Revolver = 2,
        Rifle = 3,
        Injured = 11,
        Drunk = 100

    }

}
