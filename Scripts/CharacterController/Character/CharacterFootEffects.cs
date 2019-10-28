namespace CharacterController
{
    using UnityEngine;
    using System;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    public class CharacterFootEffects : MonoBehaviour
    {
        public enum FootstepMode { BodyStep, Trigger, FixedInterval }

        [Serializable]
        private class Foot
        {
            public Transform foot;
            public float stepTime { get; set; }
            public AudioSource audioSource;
        }
        

        [SerializeField]
        private SurfaceEffect surfaceEffect;
        [SerializeField, Tooltip("Minimum velocity (squared) before a footstep effect is actiuvated.")]
        private float minVelocity = 3f;
        [SerializeField]
        private FootstepMode footstepMode = FootstepMode.BodyStep;
        [SerializeField]
        private float footOffset = 0.03f;

        [SerializeField]
        private Foot leftFoot = new Foot();
        [SerializeField]
        private Foot rightFoot = new Foot();

        [SerializeField]
        private int moveDirectionFrameCount = 7;


        private Foot raisedFoot;
        private float raisedFootHeight;
        private int frameCount;


        protected LayerManager layerManager;
        protected Animator animator;
        protected Rigidbody r;
        private Transform m_transform;




        private void Awake()
        {
            m_transform = transform;
            layerManager = GetComponent<LayerManager>();
            animator = GetComponent<Animator>();
            r = GetComponent<Rigidbody>();

        }



        private void Start()
        {
            InitializeFoot(leftFoot, HumanBodyBones.LeftToes);
            InitializeFoot(rightFoot, HumanBodyBones.RightToes);

            moveDirectionFrameCount = Mathf.Clamp(moveDirectionFrameCount, 2, 10);
        }


        private void InitializeFoot(Foot foot, HumanBodyBones footBodyBone)
        {
            if(foot.foot == null)
                foot.foot = animator.GetBoneTransform(footBodyBone);

            if(foot.audioSource == null){
                if (foot.foot.GetComponent<AudioSource>() == null){
                    foot.foot.gameObject.AddComponent<AudioSource>();
                }
                foot.audioSource = foot.foot.GetComponent<AudioSource>();
                foot.audioSource.playOnAwake = false;

            }
            
        }




        private void SpawnFootstep(Foot foot)
        {

        }



        private void CheckFoot()
        {
            if(Vector3.Dot(r.velocity, m_transform.forward) > minVelocity)
            {

            }
        }



        private void FixedUpdate()
        {
            if (surfaceEffect == null) return;

            if(r.velocity.sqrMagnitude > minVelocity * minVelocity)
            {
                //  Determine which foot is raised.
                if (animator.pivotWeight > 0.5f) raisedFoot = rightFoot;
                else if (animator.pivotWeight < 0.5f) raisedFoot = leftFoot;
                else raisedFoot = null;

                if (raisedFoot != null)
                {
                    //  Get the height of the raised foot.
                    raisedFootHeight = raisedFoot.foot.position.y - transform.position.y;
                    //  Is raised foot low enough to check if it is considered down;
                    if(raisedFootHeight <= footOffset && Time.time > raisedFoot.stepTime + 0.1f)
                    {
                        //  If rasied foot is low enough to be considered down, has it moved enough frames.
                        if(frameCount >= moveDirectionFrameCount)
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(raisedFoot.foot.position, -transform.up, out hit, 0.1f + footOffset, layerManager.SolidLayers))
                            {
                                var position = hit.point + transform.up * footOffset;
                                var fwdDirection = Vector3.Cross(transform.right, hit.normal);
                                var rotation = Quaternion.LookRotation(fwdDirection, hit.normal);

                                surfaceEffect.SpawnSurfaceEffect(position, rotation, raisedFoot.audioSource);

                                //Debug.LogFormat("Foot: <b>{0}</b> | Height:  <b>{1}</b> | Velocity: {3} | FrameCount: {4} | Time:  <b>{2}</b>",
                                //    raisedFoot.foot.name, raisedFootHeight, Time.time, (float)Math.Round(r.velocity.sqrMagnitude, 2), frameCount);
                                //  Footeffect has spawned.
                                raisedFoot.stepTime = Time.time;
                                frameCount = 0;
                                raisedFoot = null;
                                return;
                            }
                        }
                        //  Foot is low enough, but hasn';t moved enough frames.
                        frameCount++;
                    }
                    //  Foot is not low enough to the ground.
                    return;
                }
            }
            //  Reset counts.
            raisedFootHeight = 0;
            frameCount = 0;


        }










    }




#if UNITY_EDITOR
    [CustomEditor(typeof(CharacterFootEffects))]
    public class CharacterFootEffectsEditor : Editor
    {
        private bool displayFeetOptions;

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            //return;

            serializedObject.Update();
            int originalIndent = EditorGUI.indentLevel;
            //EditorGUILayout.Space();


            SerializedProperty leftFoot = serializedObject.FindProperty("leftFoot");
            SerializedProperty rightFoot = serializedObject.FindProperty("rightFoot");

            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
            //  Serialized properties.
            EditorGUILayout.PropertyField(serializedObject.FindProperty("surfaceEffect"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minVelocity"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("footstepMode"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("footOffset"));

            EditorGUI.indentLevel++;
            displayFeetOptions = EditorGUILayout.Foldout(displayFeetOptions, "Foot", true);
            if (displayFeetOptions)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(leftFoot);
                if (leftFoot.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("leftFoot.foot"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("leftFoot.audioSource"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(rightFoot);
                if (rightFoot.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("rightFoot.foot"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("rightFoot.audioSource"));
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
            

            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveDirectionFrameCount"));

            EditorGUI.indentLevel--;



            EditorGUI.indentLevel = originalIndent;
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif


}
