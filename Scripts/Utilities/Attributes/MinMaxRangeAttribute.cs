using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class MinMaxRangeAttribute : PropertyAttribute
{

    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }
    public int Round { get; private set; }

    public MinMaxRangeAttribute(float minValue, float maxValue)
    {
        this.MinValue = minValue;
        this.MaxValue = maxValue;
        this.Round = 2;
    }

    public MinMaxRangeAttribute(float minValue, float maxValue, int roundTo)
    {
        this.MinValue = minValue;
        this.MaxValue = maxValue;
        this.Round = roundTo;
    }

}
