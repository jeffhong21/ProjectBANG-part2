﻿
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController.RootMotionActions
{
    public interface IRootMotionAction
    {
        bool isActive { get; }
        int priority {get; }

        bool TryStartAction();

        bool TryStopAction();

        bool CanStartAction();

        bool CanStopAction();

        void StartAction();

        void StopAction(bool force = false);



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
