using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController.UI
{
    public class UserHudController : SingletonMonoBehaviour<UserHudController>
    {

        public GameObject crosshairsHud;
        public GameObject debugHud;


        protected override void OnAwake()
        {
            base.OnAwake();


        }
    }
}
