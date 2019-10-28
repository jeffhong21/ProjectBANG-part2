using System;
using UnityEngine;

public static class MathUtil
{

    public static float Round(float value, int rounding = 2)
    {
        if (Mathf.Approximately(value, 0f)) return 0;
        rounding = Mathf.Clamp(rounding, 2, int.MaxValue);
        return (float)Math.Round(value, rounding);
    }

    public static float Round2(this float value, int rounding = 2)
    {
        return Round(value, rounding);
    }


    public static float Min(float value1, float value2, float value3)
    {
        float min = (value1 < value2) ? value1 : value2;
        return (min < value3) ? min : value3;
    }

    public static float Max(float value1, float value2, float value3)
    {
        float max = (value1 > value2) ? value1 : value2;
        return (max > value3) ? max : value3;
    }


    public static float Inverse(this float value) { return 1 / value; }

    public static float Squared(this float value) { return value * value; }

    public static float Squared(this int value) { return value * value; }

    public static float Cubed(this float value) { return value * value * value; }

    public static float Cubed(this int value) { return value * value * value; }


    /// <summary>
    /// Is floatA equal to zero? Takes floating point inaccuracy into account, by using Epsilon.
    /// </summary>
    /// <param name="floatA"></param>
    /// <returns></returns>
    public static bool IsEqualToZero(this float floatA)
    {
        return Mathf.Abs(floatA) < Mathf.Epsilon;
    }

    //public static float Pow(this float value, int exponent)
    //{
    //    if (exponent == 0) return 1;
    //    if (exponent == 1) return value;
    //    if (exponent == -1) return 1/value;

    //    float v = value;
    //    int e = (exponent < 0 ? -1 : 1) * exponent + 1;
    //    for (int i = 2; i < e; i++)
    //        v *= value;
    //    if (exponent < 0) v = 1 / v;

    //    return v;
    //}


    //public static int Pow(this int value, int exponent)
    //{
    //    return value.Pow(exponent);
    //}


    ///// <summary>
    ///// Clamps and updates a given value around 360 degrees.
    ///// </summary>
    ///// <param name="value">Angle to clamp. </param>
    ///// <param name="maxAngle">Max angle.  Must be less than 360.</param>
    ///// <returns>Returns the clamped value.</returns>
    public static float ClampAngle(float value, float maxAngle = 360)
    {
        maxAngle = Mathf.Abs(maxAngle);
        maxAngle = maxAngle > 360 ? 360 : maxAngle;
        float max = maxAngle;
        float min = max * -1;
        float angle = value;


        if (angle < min || angle > max) {
            int r = 0;
            r = Mathf.CeilToInt(angle % maxAngle);

            if (angle < min) angle += maxAngle * r;
            if (angle > max) angle -= maxAngle * r;

            //if (angle < -360F) angle += 360F;
            //if (angle > 360F) angle -= 360F;
        }

        return angle;
    }


    //public static int ClampAngle(this int value, float maxAngle = 360)
    //{
    //    return value.ClampAngle(maxAngle);
    //}


    /// <summary>
    /// Wraps a float between -180 and 180.
    /// </summary>
    /// <param name="toWrap">The float to wrap.</param>
    /// <returns>A value between -180 and 180.</returns>
    public static float Wrap(this float toWrap, float from = -180, float to = 180)
    {

        toWrap %= 360.0f;
        if (toWrap < -from) {
            toWrap += 360.0f;
        }
        else if (toWrap > to) {
            toWrap -= 360.0f;
        }
        return toWrap;
    }

    /// <summary>
    /// Remaps a number from one range to another.
    /// </summary>
    public static float LinearRemap(this float value, float valueRangeMin, float valueRangeMax, float newRangeMin, float newRangeMax){
        return (value - valueRangeMin) / (valueRangeMax - valueRangeMin) * (newRangeMax - newRangeMin) + newRangeMin;
    }



    public static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }

    public static float SmoothStop2(float x)
    {
        var s = 1 - x;
        return 1 - s * s;
    }

    public static float SmoothStop3(float x)
    {
        var s = 1 - x;
        return 1 - s * s * s;
    }

    public static Vector3 Multiply(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

}
