﻿using UnityEngine;
//using Primitives;
//using CollisionLib;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

public class DebugDraw
{
    public static void Circle(Vector3 center, Vector3 normal, float radius, Color color)
    {
        Vector3 v1;
        Vector3 v2;
        CalculatePlaneVectorsFromNormal(normal, out v1, out v2);
        CircleInternal(center, v1, v2, radius, color);
    }

    public static void Sphere(Vector3 center, float radius)
    {
        Sphere(center, radius, Color.white);
    }

    public static void Sphere(Vector3 center, float radius, Color color, float duration = 0)
    {
        CircleInternal(center, Vector3.right, Vector3.up, radius, color, duration);
        CircleInternal(center, Vector3.forward, Vector3.up, radius, color, duration);
        CircleInternal(center, Vector3.right, Vector3.forward, radius, color, duration);
    }

    //public static void Prim(sphere sphere, Color color, float duration = 0)
    //{
    //    Sphere(sphere.center, sphere.radius, color, duration);
    //}

    //public static void Prim(ray ray, Color color, float duration = 0)
    //{
    //    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1000, color, duration);
    //    Sphere(ray.origin, 0.03f, color, duration);
    //}



    public static void Capsule(Vector3 center, Vector3 dir, float radius, float height, Color color, float duration = 0)
    {
        var cylinderHeight = height - radius * 2;
        var v = Vector3.Angle(dir, Vector3.up) > 0.001 ? Vector3.Cross(dir, Vector3.up) : Vector3.Cross(dir, Vector3.left);

        CircleInternal(center + dir * cylinderHeight * 0.5f, dir, v, radius, color, duration);
        CircleInternal(center - dir * cylinderHeight * 0.5f, dir, v, radius, color, duration);

        Cylinder(center, dir, radius, cylinderHeight * 0.5f, color, duration);
    }





    //public static void Prim(capsule capsule, Color color, float duration = 0)
    //{
    //    var v = capsule.p2 - capsule.p1;
    //    Capsule(capsule.p1 + v * 0.5f, math.normalize(v), capsule.radius, math.length(v) + 2 * capsule.radius, color, duration);
    //}

    public static void Cylinder(Vector3 center, Vector3 normal, float radius, float halfHeight, Color color, float duration = 0)
    {
        Vector3 v1;
        Vector3 v2;
        CalculatePlaneVectorsFromNormal(normal, out v1, out v2);

        var offset = normal * halfHeight;
        CircleInternal(center - offset, v1, v2, radius, color, duration);
        CircleInternal(center + offset, v1, v2, radius, color, duration);

        const int segments = 20;
        float arc = Mathf.PI * 2.0f / segments;
        for (var i = 0; i < segments; i++)
        {
            Vector3 p = center + v1 * Mathf.Cos(arc * i) * radius + v2 * Mathf.Sin(arc * i) * radius;
            Debug.DrawLine(p - offset, p + offset, color, duration);
        }
    }

    //public static void Prim(box box, Color color, float duration = 0)
    //{
    //    var size = box.size;
    //    var axisX = mul(box.rotation, float3(1, 0, 0)) * size.x;
    //    var axisY = mul(box.rotation, float3(0, 1, 0)) * size.y;
    //    var axisZ = mul(box.rotation, float3(0, 0, 1)) * size.z;

    //    var A = box.center + (axisX + axisY + axisZ) * 0.5f;
    //    var B = A - axisY;
    //    var C = B - axisZ;
    //    var D = C + axisY;

    //    var E = A - axisX;
    //    var F = B - axisX;
    //    var G = C - axisX;
    //    var H = D - axisX;

    //    Debug.DrawLine(A, B, color, duration);
    //    Debug.DrawLine(B, C, color, duration);
    //    Debug.DrawLine(C, D, color, duration);
    //    Debug.DrawLine(D, A, color, duration);

    //    Debug.DrawLine(E, F, color, duration);
    //    Debug.DrawLine(F, G, color, duration);
    //    Debug.DrawLine(G, H, color, duration);
    //    Debug.DrawLine(H, E, color, duration);

    //    Debug.DrawLine(A, E, color, duration);
    //    Debug.DrawLine(B, F, color, duration);
    //    Debug.DrawLine(C, G, color, duration);
    //    Debug.DrawLine(H, D, color, duration);
    //}

    static void CircleInternal(Vector3 center, Vector3 v1, Vector3 v2, float radius, Color color, float duration = 0)
    {
        const int segments = 20;
        float arc = Mathf.PI * 2.0f / segments;
        Vector3 p1 = center + v1 * radius;
        for (var i = 1; i <= segments; i++)
        {
            Vector3 p2 = center + v1 * Mathf.Cos(arc * i) * radius + v2 * Mathf.Sin(arc * i) * radius;
            Debug.DrawLine(p1, p2, color, duration);
            p1 = p2;
        }
    }

    static void CalculatePlaneVectorsFromNormal(Vector3 normal, out Vector3 v1, out Vector3 v2)
    {
        if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.99)
        {
            v1 = Vector3.Cross(Vector3.up, normal).normalized;
            v2 = Vector3.Cross(normal, v1);
        }
        else
        {
            v1 = Vector3.Cross(Vector3.left, normal).normalized;
            v2 = Vector3.Cross(normal, v1);
        }
    }

    public static void Arrow(Vector3 pos, float angle, Color color, float length = 1f, float tipSize = 0.25f, float width = 0.5f)
    {
        var angleRot = Quaternion.AngleAxis(angle, Vector3.up);
        var dir = angleRot * Vector3.forward;
        Arrow(pos, dir, color, length, tipSize, width);
    }

    public static void Arrow(Vector3 pos, Vector2 direction, Color color, float length = 1f, float tipSize = 0.25f, float width = 0.5f)
    {
        var dir = new Vector3(direction.x, 0f, direction.y);
        Arrow(pos, dir, color, length, tipSize, width);
    }

    public static void Arrow(Vector3 pos, Vector3 direction, Color color, float length = 1f, float tipSize = 0.25f, float width = 0.5f)
    {
        direction.Normalize();

        var sideLen = length - length * tipSize;
        var widthOffset = Vector3.Cross(direction, Vector3.up) * width;

        var baseLeft = pos + widthOffset * 0.3f;
        var baseRight = pos - widthOffset * 0.3f;
        var tip = pos + direction * length;
        var upCornerInRight = pos - widthOffset * 0.3f + direction * sideLen;
        var upCornerInLeft = pos + widthOffset * 0.3f + direction * sideLen;
        var upCornerOutRight = pos - widthOffset * 0.5f + direction * sideLen;
        var upCornerOutLeft = pos + widthOffset * 0.5f + direction * sideLen;

        Debug.DrawLine(baseLeft, baseRight, color);
        Debug.DrawLine(baseRight, upCornerInRight, color);
        Debug.DrawLine(upCornerInRight, upCornerOutRight, color);
        Debug.DrawLine(upCornerOutRight, tip, color);
        Debug.DrawLine(tip, upCornerOutLeft, color);
        Debug.DrawLine(upCornerOutLeft, upCornerInLeft, color);
        Debug.DrawLine(upCornerInLeft, baseLeft, color);
    }









    public static void DrawMarker(Vector3 position, float size, Color color, float duration = 0, bool depthTest = true)
    {
        Vector3 line1PosA = position + Vector3.up * size * 0.5f;
        Vector3 line1PosB = position - Vector3.up * size * 0.5f;

        Vector3 line2PosA = position + Vector3.right * size * 0.5f;
        Vector3 line2PosB = position - Vector3.right * size * 0.5f;

        Vector3 line3PosA = position + Vector3.forward * size * 0.5f;
        Vector3 line3PosB = position - Vector3.forward * size * 0.5f;

        Debug.DrawLine(line1PosA, line1PosB, color, duration, depthTest);
        Debug.DrawLine(line2PosA, line2PosB, color, duration, depthTest);
        Debug.DrawLine(line3PosA, line3PosB, color, duration, depthTest);
    }

    // Courtesy of robertbu
    public static void DrawPlane(Vector3 position, Vector3 normal, float size, Color color, float duration, bool depthTest = true)
    {
        Vector3 v3;

        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;

        Vector3 corner0 = position + v3 * size;
        Vector3 corner2 = position - v3 * size;

        Quaternion q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        Vector3 corner1 = position + v3 * size;
        Vector3 corner3 = position - v3 * size;

        Debug.DrawLine(corner0, corner2, color, duration, depthTest);
        Debug.DrawLine(corner1, corner3, color, duration, depthTest);
        Debug.DrawLine(corner0, corner1, color, duration, depthTest);
        Debug.DrawLine(corner1, corner2, color, duration, depthTest);
        Debug.DrawLine(corner2, corner3, color, duration, depthTest);
        Debug.DrawLine(corner3, corner0, color, duration, depthTest);
        Debug.DrawRay(position, normal * size, color, duration, depthTest);
    }





    //public static Color RandomColor()
    //{
    //    return new Color(Random.value, Random.value, Random.value);
    //}









    public static void DrawCapsule(Vector3 start, Vector3 end, float radius, Color color, float duration = .001f, bool depthTest = true)
    {
        Vector3 up = (end - start).normalized * radius;
        if (up == Vector3.zero) up = Vector3.up; //This can happen when the capsule is actually a sphere, so the start and end are right on eachother
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * radius;

        //Radial circles
        DrawCircle(start, up, color, radius, duration, depthTest);
        DrawCircle(end, -up, color, radius, duration, depthTest);

        //Side lines
        Debug.DrawLine(start + right, end + right, color, duration, depthTest);
        Debug.DrawLine(start - right, end - right, color, duration, depthTest);

        Debug.DrawLine(start + forward, end + forward, color, duration, depthTest);
        Debug.DrawLine(start - forward, end - forward, color, duration, depthTest);

        for (int i = 1; i < 26; i++)
        {

            //Start endcap
            Debug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);

            //End endcap
            Debug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
            Debug.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
        }
    }



    public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f, float duration = .001f, bool depthTest = true)
    {
        Vector3 _up = up.normalized * radius;
        Vector3 _forward = Vector3.Slerp(_up, -_up, 0.5f);
        Vector3 _right = Vector3.Cross(_up, _forward).normalized * radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix[0] = _right.x;
        matrix[1] = _right.y;
        matrix[2] = _right.z;

        matrix[4] = _up.x;
        matrix[5] = _up.y;
        matrix[6] = _up.z;

        matrix[8] = _forward.x;
        matrix[9] = _forward.y;
        matrix[10] = _forward.z;

        Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
        Vector3 _nextPoint = Vector3.zero;

        color = (color == default(Color)) ? Color.white : color;

        for (var i = 0; i < 91; i++)
        {
            _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
            _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
            _nextPoint.y = 0;

            _nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

            Debug.DrawLine(_lastPoint, _nextPoint, color, duration, depthTest);
            _lastPoint = _nextPoint;
        }
    }

}
