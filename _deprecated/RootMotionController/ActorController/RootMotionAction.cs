using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController.RootMotionActions
{



    public class RootMotionAction : IRootMotionAction, IRootMotionStateAction
    {
        public bool isActive => throw new NotImplementedException();

        public int priority => throw new NotImplementedException();

        public virtual void ActionStarted()
        {
            throw new NotImplementedException();
        }

        public virtual void ActionStopped()
        {
            throw new NotImplementedException();
        }

        public virtual bool CanStartAction()
        {
            throw new NotImplementedException();
        }

        public virtual bool CanStopAction()
        {
            throw new NotImplementedException();
        }

        public void StartAction()
        {
            throw new NotImplementedException();
        }

        public void StopAction(bool force = false)
        {
            throw new NotImplementedException();
        }

        public bool TryStartAction()
        {
            throw new NotImplementedException();
        }

        public bool TryStopAction()
        {
            throw new NotImplementedException();
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateAnimator()
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateMovement()
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateRotation()
        {
            throw new NotImplementedException();
        }
    }
}
