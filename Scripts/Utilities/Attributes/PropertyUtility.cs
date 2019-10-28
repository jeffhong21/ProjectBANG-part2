using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;


public static class PropertyUtility
{
    public static T GetAttribute<T>(SerializedProperty property) where T : Attribute
    {
        T[] attributes = GetAttributes<T>(property);
        return attributes.Length > 0 ? attributes[0] : null;
    }

    public static T[] GetAttributes<T>(SerializedProperty property) where T : Attribute
    {
        FieldInfo fieldInfo = GetField(GetTargetObject(property), property.name);

        return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
    }

    public static UnityEngine.Object GetTargetObject(SerializedProperty property)
    {
        return property.serializedObject.targetObject;
    }


    public static FieldInfo GetField(object target, string fieldName)
    {
        return GetAllFields(target, f => f.Name.Equals(fieldName, StringComparison.InvariantCulture)).FirstOrDefault();
    }

    public static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
    {
        List<Type> types = new List<Type>()
            {
                target.GetType()
            };

        while (types.Last().BaseType != null)
        {
            types.Add(types.Last().BaseType);
        }

        for (int i = types.Count - 1; i >= 0; i--)
        {
            IEnumerable<FieldInfo> fieldInfos = types[i]
                .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(predicate);

            foreach (var fieldInfo in fieldInfos)
            {
                yield return fieldInfo;
            }
        }
    }
}