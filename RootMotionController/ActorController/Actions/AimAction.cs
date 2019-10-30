using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController.RootMotionActions
{
    public class AimAction : IRootMotionAction, IRootMotionItemAction
    {
        public bool isActive => throw new NotImplementedException();

        public int priority => throw new NotImplementedException();

        public GameObject item { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CanStartAction()
        {
            throw new NotImplementedException();
        }

        public bool CanStopAction()
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
    }
}
