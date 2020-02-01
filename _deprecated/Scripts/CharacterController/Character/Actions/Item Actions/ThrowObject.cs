namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class ThrowObject : CharacterAction
    {
        //public GameObject item;
        //public GameObject vfxMarkerPrefab;

        //private GameObject vfxMarker;
        //private Camera mainCamera;
        //private bool aiming;
        //private Ray ray;

        //private Vector3 throwLocation;
        //private bool objectThrown;


        //protected virtual void Start()
        //{
        //    //vfxMarker = Instantiate(vfxMarkerPrefab) as GameObject;
        //    //vfxMarker.transform.parent = m_Transform;
        //    //vfxMarker.SetActive(false);
        //    //vfxMarker.transform.hideFlags = HideFlags.HideInHierarchy;

        //    //mainCamera = CameraController.Instance.Camera;
        //    //ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        //    //EventHandler.RegisterEvent(m_GameObject, "OnActivateThrowableObject", OnActivateThrowableObject);
        //}


        //protected virtual void OnDestroy()
        //{
        //    EventHandler.UnregisterEvent(m_GameObject, "OnActivateThrowableObject", OnActivateThrowableObject);
        //}


        //protected override void ActionStarted()
        //{
        //    objectThrown = false;
        //}


        ////  Returns the state the given layer should be on.
        //public override string GetDestinationState(int layer)
        //{
        //    if (layer == 0)
        //    {
        //        m_DestinationStateName = "ThrowSingle1";  // ThrowStart
        //        //m_DestinationStateName = "ThrowStart";  // ThrowStart

        //        return m_DestinationStateName;
        //    }
        //    return "";
        //}


        //public override void UpdateAction()
        //{
        //    if (m_IsActive)
        //    {
        //        aiming = true;
        //        vfxMarker.SetActive(true);

        //        RaycastHit hit;
        //        ray.origin = mainCamera.transform.position;
        //        ray.direction = mainCamera.transform.forward;
        //        if (Physics.Raycast(ray, out hit, 50, m_Layers.SolidLayers))
        //        {
        //            vfxMarker.SetActive(true);
        //            vfxMarker.transform.position = hit.point;
        //        }
        //        else
        //        {
        //            vfxMarker.SetActive(false);

        //        }

        //        if (Input.GetMouseButtonUp(0))
        //        {
        //            vfxMarker.SetActive(false);

        //            int actionIntID = 1;
        //            if (hit.distance > 10)
        //                actionIntID = 2;
        //            m_Animator.SetInteger(HashID.ActionIntData, actionIntID);

        //            throwLocation = hit.point;
        //        }
        //    }
        //}


        //protected override void ActionStopped()
        //{
        //    aiming = false;
        //    throwLocation = Vector3.zero;
        //    vfxMarker.SetActive(false);
        //}


        //public override bool CanStopAction()
        //{
        //    if (objectThrown)
        //        return true;

        //    //int layerIndex = 0;
        //    //if (m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == 0)
        //    //{
        //    //    m_ExitingAction = true;
        //    //}
        //    //if (m_ExitingAction && m_Animator.IsInTransition(layerIndex))
        //    //{
        //    //    //Debug.LogFormat("{1} is exiting. | {0} is the next state.", m_AnimatorMonitor.GetStateName(m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash), this.GetType());
        //    //    return true;
        //    //}

        //    //if (m_Animator.GetCurrentAnimatorStateInfo(layerIndex). == 0)
        //    //{
        //    //    m_ExitingAction = true;
        //    //}

        //    return false;
        //}



        //private void OnActivateThrowableObject()
        //{
        //    //Debug.Log("OnActivateThrowableObject!");


        //    //Debug.Break();
        //    Transform rightHand = m_Animator.GetBoneTransform(HumanBodyBones.RightHand);
        //    var throwObject = Instantiate(item, rightHand);
        //    throwObject.transform.localPosition = Vector3.zero;
        //    throwObject.transform.localEulerAngles = Vector3.zero;

        //    var rb = throwObject.GetComponent<Rigidbody>();
        //    rb.isKinematic = false;
        //    throwObject.transform.parent = null;



        //    //Quaternion rotationAngle = Quaternion.AngleAxis(-20, m_Transform.right);
        //    //rb.AddForce(rotationAngle * m_Transform.forward * 100, ForceMode.Impulse);
        //    rb.AddTorque(m_Transform.forward * 45, ForceMode.Impulse);

        //    Vector3 velocity = CalculateVelocity(throwLocation, rightHand.position, 1);
        //    rb.velocity = velocity;

        //    objectThrown = true;
        //}


        //Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
        //{
        //    //  Define the distance x and y first.
        //    Vector3 distance = target - origin;
        //    Vector3 distanceXZ = distance;
        //    distanceXZ.y = 0;


        //    //  Create a float that repsents our distance
        //    float Sy = distance.y;              //  vertical distance
        //    float Sxz = distanceXZ.magnitude;   //  horizontal distance


        //    //  Calculate the initial velocity.  This is distance / time.
        //    float Vxz = Sxz / time;
        //    float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;



        //    Vector3 result = distanceXZ.normalized;
        //    result *= Vxz;
        //    result.y = Vy;

        //    return result;
        //}
    }

}

