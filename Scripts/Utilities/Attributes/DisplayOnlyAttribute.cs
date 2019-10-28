using System;
using UnityEngine;


/// <summary>
/// Convenience <see cref="PropertyAttribute"/> for marking fields as "Read Only".
/// Used for exposing values and references in the Unity inspector, which cannot be changed or modified outside of code.
/// </summary>
/// <seealso cref="UnityEngine.PropertyAttribute" />
[AttributeUsage(AttributeTargets.Field)]
public sealed class DisplayOnlyAttribute : PropertyAttribute
{

}