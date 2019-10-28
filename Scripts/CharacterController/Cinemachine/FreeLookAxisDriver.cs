using System;
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;

[Serializable]
public struct CinemachineInputAxisDriver
{
    [Tooltip("Multiply the input by this amount prior to processing.  Controls the input power.")]
    public float multiplier;

    [Tooltip("The amount of time in seconds it takes to accelerate to a higher speed")]
    public float accelTime;

    [Tooltip("The amount of time in seconds it takes to decelerate to a lower speed")]
    public float decelTime;

    [Tooltip("The name of this axis as specified in Unity Input manager. "
        + "Setting to an empty string will disable the automatic updating of this axis")]
    public string name;

    [NoSaveDuringPlay]
    [Tooltip("The value of the input axis.  A value of 0 means no input.  You can drive "
        + "this directly from a custom input system, or you can set the Axis Name and "
        + "have the value driven by the internal Input Manager")]
    public float inputValue;

    /// Internal state
    private float mCurrentSpeed;
    const float Epsilon =  UnityVectorExtensions.Epsilon;

    /// Call from OnValidate: Make sure the fields are sensible
    public void Validate()
    {
        accelTime = Mathf.Max(0, accelTime);
        decelTime = Mathf.Max(0, decelTime);
    }

    public bool Update(float deltaTime, ref AxisState axis)
    {
        if (!string.IsNullOrEmpty(name))
        {
            try { inputValue = CinemachineCore.GetInputAxis(name); }
            catch (ArgumentException) {}
            //catch (ArgumentException e) { Debug.LogError(e.ToString()); }
        }

        float input = inputValue * multiplier;
        if (deltaTime < Epsilon)
            mCurrentSpeed = 0;
        else
        {
            float speed = input / deltaTime;
            float dampTime = Mathf.Abs(speed) < Mathf.Abs(mCurrentSpeed) ? decelTime : accelTime;
            speed = mCurrentSpeed + Damper.Damp(speed - mCurrentSpeed, dampTime, deltaTime);
            mCurrentSpeed = speed;

            // Decelerate to the end points of the range if not wrapping
            float range = axis.m_MaxValue - axis.m_MinValue;
            if (!axis.m_Wrap && decelTime > Epsilon && range > Epsilon)
            {
                float v0 = ClampValue(ref axis, axis.Value);
                float v = ClampValue(ref axis, v0 + speed * deltaTime);
                float d = (speed > 0) ? axis.m_MaxValue - v : v - axis.m_MinValue;
                if (d < (0.1f * range) && Mathf.Abs(speed) > Epsilon)
                    speed = Damper.Damp(v - v0, decelTime, deltaTime) / deltaTime;
            }
            input = speed * deltaTime;
        }

        axis.Value = ClampValue(ref axis, axis.Value + input);
        return Mathf.Abs(inputValue) > Epsilon;
    }

    float ClampValue(ref AxisState axis, float v)
    {
        float r = axis.m_MaxValue - axis.m_MinValue;
        if (axis.m_Wrap && r > Epsilon)
        {
            v = (v - axis.m_MinValue) % r;
            v += axis.m_MinValue + ((v < 0) ? r : 0);
        }
        return Mathf.Clamp(v, axis.m_MinValue, axis.m_MaxValue);
    }
}


[RequireComponent(typeof(CinemachineFreeLook)), DisallowMultipleComponent]
public class FreeLookAxisDriver : MonoBehaviour
{
    public CinemachineInputAxisDriver xAxis;
    public CinemachineInputAxisDriver yAxis;

    private CinemachineFreeLook freeLook;

    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        freeLook.m_XAxis.m_MaxSpeed = freeLook.m_XAxis.m_AccelTime = freeLook.m_XAxis.m_DecelTime = 0;
        freeLook.m_XAxis.m_InputAxisName = string.Empty;
        freeLook.m_YAxis.m_MaxSpeed = freeLook.m_YAxis.m_AccelTime = freeLook.m_YAxis.m_DecelTime = 0;
        freeLook.m_YAxis.m_InputAxisName = string.Empty;
    }

    private void OnValidate()
    {
        xAxis.Validate();
        yAxis.Validate();
    }

    private void Reset()
    {
        xAxis = new CinemachineInputAxisDriver
        {
            multiplier = -10f,
            accelTime = 0.1f,
            decelTime = 0.1f,
            name = "Mouse X",
        };
        yAxis = new CinemachineInputAxisDriver
        {
            multiplier = 0.1f,
            accelTime = 0.1f,
            decelTime = 0.1f,
            name = "Mouse Y",
        };
    }

    private void Update()
    {
        bool changed = xAxis.Update(Time.deltaTime, ref freeLook.m_XAxis);
        if (yAxis.Update(Time.deltaTime, ref freeLook.m_YAxis))
            changed = true;
        if (changed)
        {
            freeLook.m_RecenterToTargetHeading.CancelRecentering();
            freeLook.m_YAxisRecentering.CancelRecentering();
        }
    }
}
