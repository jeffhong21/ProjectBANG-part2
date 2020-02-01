using UnityEngine;
using System;

using Unity.Mathematics;

[Serializable]
public struct Trajectory
{
    public Trajectory(float timeHorizon, float sampleRate, AffineTransform transform)
    {
        halfTrajectoryLength = Mathf.FloorToInt(timeHorizon * sampleRate);
        int trajectoryLength = halfTrajectoryLength * 2;

        rootTransforms = new AffineTransform[trajectoryLength];

        for (int i = 0; i < trajectoryLength; i++)
        {
            rootTransforms[i] = transform;
        }

        this.timeHorizon = timeHorizon;
        this.sampleRate = sampleRate;
    }


    public int Length { get { return rootTransforms.Length; } }

    public ref AffineTransform this[int index]
    {
        get { return ref rootTransforms[index]; }
    }



    public Vector3 Next(int index = 1)
    {
        index = Mathf.Clamp(index, 0, halfTrajectoryLength);
        return this[halfTrajectoryLength + index].translation;
    }


    public float timeHorizon;
    public float sampleRate;
    [SerializeField]
    private AffineTransform[] rootTransforms;

    private int halfTrajectoryLength;
}



