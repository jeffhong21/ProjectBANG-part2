
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController.RootMotionActions
{
    public interface IRootMotionAction
    {

        bool CanStartAction();

        bool CanStopAction();

        void StartAction();

        void StopAction(bool force = false);

        void ActionStarted();

        void ActionStopped();

        void Update();

        void UpdateRotation();

        void UpdateMovement();

        void UpdateAnimator();
    }


    public interface IRootMotionStateAction
    {

    }

    public interface IRootMotionItemAction
    {

    }
}
