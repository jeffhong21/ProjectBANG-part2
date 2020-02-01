using UnityEngine;
using UnityEditor;
using System.Collections;

public static class MovementUtilities
{
    //[SerializeField] LineRenderer lineRenderer;
    //[SerializeField] Transform point0, point1, point2;
    //[SerializeField] int numberPoints = 25;


    //private void Start()
    //{
        
    //}

    //private void Update()
    //{
    //    DrawQuadraticCurve(numberPoints, point0, point1, point2);
    //}







    #region Quadratic Bezier Curve

    public static void DrawQuadraticCurve(int numPoints, Transform point0, Transform point1, Transform point2 )
    {
        Vector3[] positions = new Vector3[numPoints];
        for (int i = 1; i < numPoints + 1; i++) {
            float t = i / (float)numPoints;
            positions[i - 1] = CalculateQuadraticBezierPoint(point0.position, point1.position, point2.position, t);
        }
        //lineRenderer.positionCount = numPoints;
        //lineRenderer.SetPositions(positions);
    }

    private static Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        //  Formula:  P0 = vector parameter points.
        //  (1-t)2 P0 + 2*(1-t)tP1 + t2 * P2
        //    u             u          tt
        //    uu * P0 + 2 * u * t * P1 + tt * P2
        
        float u = 1 - t;
        //float tt = t * t;
        //float uu = u * u;
        //Vector3 p = uu * p0;
        //p += 2 * u * t * p1;
        //p += tt * p2;

        
        Vector3 point = (u.Squared() * p0) + (2 * u * t * p1) + (t.Squared() * p2);
        return point;
    }

    #endregion

    /// <summary>
    /// Calculate the jump speed.
    /// </summary>
    /// <param name="jumpHeight">Height of the jump.</param>
    /// <param name="gravity">Downward force applied</param>
    /// <returns></returns>
    public static float CalculateJumpSpeed(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }




    public static Vector3 CalculateVelocity(Vector3 origin, Vector3 target,float time)
    {
        //  Define the distance x and y first.
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        //  Create a float that repsents our distance
        float verticalDistance = distance.y;              //  vertical distance
        float horizontalDistance = distanceXZ.magnitude;   //  horizontal distance

        //  Calculate the initial velocity.  This is distance / time.
        float velocityXZ = horizontalDistance / time;
        float velocityY = (verticalDistance / time) + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= velocityXZ;
        result.y = velocityY;

        return result;
    }


    /// <summary>
    /// Predict the falling position of the rigid body by physics.
    /// </summary>
    public static Vector3 GetFallPosition(float time, float power, float mass, float gravity = -9.8f)
    {
        float halfGravity = gravity * 0.5f;
        float z = time * power / mass;
        float y = halfGravity * (time * time);
        return new Vector3(0, y, z);
    }




    public static Vector3 ComputeTorque(Rigidbody rb, Quaternion desiredRotation )
    {
        //q will rotate from our current rotation to desired rotation
        Quaternion q = desiredRotation * Quaternion.Inverse(rb.transform.rotation);
        //convert to angle axis representation so we can do math with angular velocity
        Vector3 axis;
        float axisMagnitude;
        q.ToAngleAxis(out axisMagnitude, out axis);
        axis.Normalize();
        //w is the angular velocity we need to achieve
        Vector3 targetAngularVelocity = axis * axisMagnitude * Mathf.Deg2Rad / Time.fixedDeltaTime;
        targetAngularVelocity -= rb.angularVelocity;
        //to multiply with inertia tensor local then rotationTensor coords
        Vector3 wl = rb.transform.InverseTransformDirection(targetAngularVelocity);
        Vector3 Tl;
        Vector3 wll = wl;
        wll = rb.inertiaTensorRotation * wll;
        wll.Scale(rb.inertiaTensor);
        Tl = Quaternion.Inverse(rb.inertiaTensorRotation) * wll;
        Vector3 T = rb.transform.TransformDirection(Tl);
        return T;
    }




    //protected Vector3 CalculateJumpVelocity()
    //{
    //    //speed = Mathf.Sqrt(-2.0f * m_controller.Gravity.y  * jumpHeight);
    //    speed = .8f;
    //    acceleration = -(2 * jumpHeight) /(jumpVelocity * jumpVelocity);
    //    verticalVelocity = Mathf.Abs(acceleration) * speed;

    //    velocity = m_controller.Velocity;
    //    velocity.y += verticalVelocity;

    //    //Debug.Log(velocity);
    //    return velocity;
    //}



    //public static Vector3[] CalculateArcPoints(int resolution)
    //{
    //    Vector3[] arcArray = new Vector3[resolution];



    //    return arcArray;
    //}



    //public class DrawLineRenderer
    //{
    //    float velocity;
    //    float angle = 45f;
    //    int resolution = 5;

    //    float gravity;
    //    float radianAngle;
    //    Vector3[] arcPoints = new Vector3[0];

    //    protected Vector3[] CalculateArcArray()
    //    {
    //        resolution = 5;
    //        Vector3[] arcArray = new Vector3[resolution];
    //        angle = 45f;
    //        velocity = m_DistanceToWall - m_CapsuleCollider.radius * 2;
    //        gravity = Mathf.Abs(Physics.gravity.y);
    //        radianAngle = Mathf.Deg2Rad * angle;

    //        //Debug.Log(gravity);



    //        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / gravity;

    //        for (int index = 0; index < resolution; index++) {
    //            float t = (float)index / (float)resolution;
    //            arcArray[index] = CalculateArcPoint(t, maxDistance);
    //        }

    //        return arcArray;
    //    }


    //    //  Calculate platformHeight and distance of each vertex.
    //    Vector3 CalculateArcPoint( float t, float maxDistance )
    //    {
    //        float x = t * maxDistance;
    //        float y = x * Mathf.Tan(radianAngle) - ((gravity * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
    //        return new Vector3(x, y);
    //    }
    //}






    //private static RaycastHit[] s_Hits = new RaycastHit[64];

    //public static bool RaycastNonAllocSingle(Ray ray, out RaycastHit hit, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers, Transform ignoreRoot = null, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    //{
    //    int closest = -1;
    //    int hitCount = Physics.RaycastNonAlloc(ray, s_Hits, maxDistance, layerMask, queryTriggerInteraction);
    //    if (hitCount > 0)
    //    {
    //        // Get the closest (not ignored)int closest = -1;
    //        for (int i = 0; i < hitCount; ++i)
    //        {
    //            // Check if closer
    //            if (closest == -1 || s_Hits[i].distance < s_Hits[closest].distance)
    //            {
    //                if (ignoreRoot != null)
    //                {
    //                    // Check if transform or parents match ignore root
    //                    Transform t = s_Hits[i].transform;
    //                    bool ignore = false;
    //                    while (t != null)
    //                    {
    //                        if (t == ignoreRoot)
    //                        {
    //                            ignore = true;
    //                            break;
    //                        }
    //                        t = t.parent;
    //                    }

    //                    // Not ignored. This is closest
    //                    if (!ignore)
    //                        closest = i;
    //                }
    //                else
    //                    closest = i;
    //            }
    //        }
    //        // Check if all ignored
    //        if (closest == -1)
    //        {
    //            hit = new RaycastHit();
    //            return false;
    //        }

    //        // Return the relevant hit
    //        hit = s_Hits[closest];
    //        return true;
    //    }
    //    else
    //    {
    //        hit = new RaycastHit();
    //        return false;
    //    }
    //}


}
