using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    /// <summary>
    /// Reset all values to 0.
    /// </summary>
    public static void Zero(this Vector3 v)
    {
        v.x = 0;
        v.y = 0;
        v.z = 0;
    }


    public static Vector3 Add(this Vector3 a, Vector3 b)
    {
        a.x += b.x;
        a.y += b.y;
        a.z += b.z;
        return a;
    }


    public static Vector3 Subtract(this Vector3 a, Vector3 b)
    {
        a.x -= b.x;
        a.y -= b.y;
        a.z -= b.z;
        return a;
    }


    public static Vector3 WithY(this Vector3 v, float y)
    {
        return new Vector3(v.x, v.y + y, v.z);
    }

    public static Vector3 WithX(this Vector3 v, float x)
    {
        return new Vector3(v.x + x, v.y, v.z);
    }

    public static Vector3 WithZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, v.z + z);
    }

    public static Vector3 WithYZ(this Vector3 v, float y,float z)
    {
        return new Vector3(v.x, v.y + y, v.z + z);
    }

    public static Vector3 Multiply(this Vector3 v, float x)
    {
        return new Vector3(v.x * x, v.y * x, v.z * x);
    }

    public static Vector3 Multiply(this Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    public static Vector3 Scale(this Vector3 v, float scale)
    {
        v = v.normalized;
        return new Vector3(v.x * scale, v.y * scale, v.z * scale);
    }

    //public static Vector3 Multiply(Vector3 v1, Vector3 v2)
    //{
    //    return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    //}



    public static Vector3 Squared(this Vector3 v)
    {
        return new Vector3(v.x * v.x, v.y * v.y, v.z * v.z);
    }



    /// <summary>
    /// Finds the position closest to the given one.
    /// </summary>
    /// <param name="position">World position.</param>
    /// <param name="otherPositions">Other world positions.</param>
    /// <returns>Closest position.</returns>
    public static Vector3 GetClosest(this Vector3 position, IEnumerable<Vector3> otherPositions)
    {
        var closest = Vector3.zero;
        var shortestDistance = Mathf.Infinity;

        foreach (var otherPosition in otherPositions)
        {
            var distance = (position - otherPosition).sqrMagnitude;

            if (distance < shortestDistance)
            {
                closest = otherPosition;
                shortestDistance = distance;
            }
        }

        return closest;
    }



    /// <summary>
    /// Transforms a point to world space relative to the transform.
    /// </summary>
    /// <param name="point">Vector3 in local space.</param>
    /// <param name="transform">Transform.</param>
    /// <returns>Point in world space relative to transform.</returns>
    public static Vector3 RelativeToTransformWorld(this Vector3 point, Transform transform)
    {
        Vector3 ws = transform.TransformPoint(point);
        Vector3 dif = transform.localPosition + point - ws;
        return ws + dif;
    }




}





