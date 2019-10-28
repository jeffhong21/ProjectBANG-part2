namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;





    public abstract class CameraController : MonoBehaviour
    {
        protected static CameraController _instance;

        public static CameraController Instance{
            get { return _instance; }
        }


        protected Camera m_camera;

        public Camera Camera{
            get { return m_camera; }
            private set{ m_camera = value; }
        }



        public Vector2 RotationSensitivity { get; set; } = new Vector2(3, 3);

        //
        //  Methods
        //
        protected virtual void Awake()
        {
            _instance = this;

            m_camera = GetComponent<Camera>();
            if(m_camera == null)
                m_camera = GetComponentInChildren<Camera>();


        }



        public abstract void SetMainTarget(GameObject target);


        public abstract void UpdateRotation(float mouseX, float mouseY);


        public abstract void UpdateZoom(float zoomInput);


        public virtual bool SetCameraState(string stateName)
        {
            return false;
        }



        //protected float ClampAngle(float clampAngle, float min, float max)
        //{
        //    do
        //    {
        //        if (clampAngle < -360)
        //            clampAngle += 360;
        //        if (clampAngle > 360)
        //            clampAngle -= 360;
        //    } while (clampAngle < -360 || clampAngle > 360);

        //    return Mathf.Clamp(clampAngle, min, max);
        //}










    }
}


