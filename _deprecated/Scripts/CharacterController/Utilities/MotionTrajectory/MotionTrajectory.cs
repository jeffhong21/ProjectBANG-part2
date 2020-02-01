using UnityEngine;
using System;

[Serializable]
public class MotionTrajectory
{
    public float timeHorizon;
    public float sampleRate;

    private MotionPoint[] motionPath;
    private int resolution;




    public MotionTrajectory (int resolution)
    {
        this.resolution = resolution;
    }


    public MotionPoint[] CalculateTrajectory(Vector3 startPosition, Vector3 motionDirection, Vector3 endPosition)
    {
        if (motionPath == null) motionPath = new MotionPoint[resolution];
        Vector3 p0 = startPosition;
        Vector3 p1 = startPosition + motionDirection;
        Vector3 p2 = endPosition;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            motionPath[i].position = GetPoint(p0, p1, p2, t);
        }

        return motionPath;
    }


    public Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    public Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }


    public Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            2f * (1f - t) * (p1 - p0) +
            2f * t * (p2 - p1);
    }


    public Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }


    //public Vector3 GetVelocity(float t)
    //{
    //    return transform.TransformPoint(GetFirstDerivative(motionPoints[0], motionPoints[1], motionPoints[2], t)) -
    //        transform.position;
    //}



    public void DrawMotionPath(float pointSize = 0.01f)
    {
        if (motionPath == null || motionPath.Length == 0) return;
        Vector3 lineStart = motionPath[0].position;
        Gizmos.color = Color.white;
        for (int i = 0; i < resolution; i++)
        {
            Vector3 lineEnd = motionPath[i].position;
            Gizmos.DrawLine(lineStart, lineEnd);
            Gizmos.DrawSphere(lineEnd, pointSize);
            lineStart = lineEnd;
        }
    }




    [Serializable]
    public struct MotionPoint
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 direction;

        public MotionPoint(Vector3 position)
        {
            this.position = position;
            this.velocity = Vector3.zero;
            this.direction = Vector3.zero;
        }
    }
}





