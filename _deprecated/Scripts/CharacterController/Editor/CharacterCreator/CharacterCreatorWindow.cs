using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using CharacterController.CharacterInput;

namespace CharacterController.CharacterCreator
{


    public class CharacterCreatorWindow : EditorWindow
    {
        private string m_name;
        private GameObject m_character;
        private Animator m_animator;
        private RuntimeAnimatorController m_runtimeAnimator;

        private bool addPlayerInput, addHealth = true, addIk = true, addRagdoll = true, addInventory = true;

        private Editor m_humanpreview;
        private Vector2 m_minSize = new Vector2(500, 630);
        private GUIContent m_title = new GUIContent("Character Creator");
        private GUIContent m_header = new GUIContent("Character");
        private GUIStyle m_previewBgColor = new GUIStyle();

        private bool m_isHuman, m_isValidAvatar, m_charExist;


        private float m_mass = 100;
        private float m_maxHealth = 100;
        private LayerMask m_enemyLayer;
        private LayerMask m_invisibleLayer;
        private LayerMask m_solidLayer;

        private GameObject _CharacterObject;


        [MenuItem("Character Controller/Create Character", false, -1)]
        public static void ShowWindow()
        {
            var window = GetWindow<CharacterCreatorWindow>();
            window.minSize = window.m_minSize;
            window.titleContent = window.m_title;


            window.m_character = (GameObject)Selection.activeObject;
            if (window.m_character != null && window.m_character.GetComponent<RigidbodyCharacterController>() == null)
                window.m_humanpreview = Editor.CreateEditor(window.m_character);
            
            window.Show();
        }



		private void OnGUI()
		{
            
            using(new GUILayout.VerticalScope("box"))
            {
                GUILayout.Label(m_header);
                m_name = EditorGUILayout.TextField(new GUIContent("Name"), m_name, GUILayout.ExpandWidth(true));
                m_character = EditorGUILayout.ObjectField("FBX Model", m_character, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;

                if (GUI.changed && m_character != null && m_character.GetComponent<RigidbodyCharacterController>() == null)
                    m_humanpreview = Editor.CreateEditor(m_character);
                if(m_character != null && m_character.GetComponent<RigidbodyCharacterController>() != null){
                    EditorGUILayout.HelpBox("This gameObject already contains the component RigidbodyCharacterController", MessageType.Warning);
                    if (GUILayout.Button("Reset Character"))
                        ResetCharacter();
                        
                }    

                m_runtimeAnimator = EditorGUILayout.ObjectField("Animator Controller: ", m_runtimeAnimator, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;


                EditorGUILayout.Space();
                addPlayerInput = GUILayout.Toggle(addPlayerInput, "Player Input");
                addHealth = GUILayout.Toggle(addHealth, "Health");
                addIk = GUILayout.Toggle(addIk, "Character IK");
                addRagdoll = GUILayout.Toggle(addRagdoll, "Ragdoll");
                addInventory = GUILayout.Toggle(addInventory, "Inventory");


                if (m_character) 
                    m_animator = m_character.GetComponent<Animator>();
                m_charExist = m_animator != null;
                m_isHuman = m_charExist ? m_animator.isHuman : false;
                m_isValidAvatar = m_charExist ? m_animator.avatar.isValid : false;

                if (CanCreate())
                {
                    DrawHumanoidPreview();
                    using (new GUILayout.HorizontalScope())
                    {
                        //GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Create"))
                            Create();
                        //if (m_runtimeAnimator != null)
                        //{
                        //    if (GUILayout.Button("Create"))
                        //        Create(m_name);
                        //}
                        //GUILayout.FlexibleSpace();
                    }
                }
                else{
                    string helpboxMsg = "Something is missing.\n";
                    if (m_animator == false)
                        helpboxMsg += "• Animator is missing. \n";
                    if (m_isHuman == false)
                        helpboxMsg += "• Rig is not set to human. \n";
                    if (m_isValidAvatar == false)
                        helpboxMsg += "• Animator does not have a valid Avatar.";
                    
                    EditorGUILayout.HelpBox(helpboxMsg, MessageType.Error);
                }
            }


		}



        private bool CanCreate()
        {
            return m_isValidAvatar && m_isHuman && m_character != null && m_character.GetComponent<RigidbodyCharacterController>() == null;
        }


        private void DrawHumanoidPreview()
        {
            GUILayout.FlexibleSpace();

            if (m_humanpreview != null)
            {
                m_humanpreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 400), "window");
            }
        }

        private void ResetCharacter()
        {
            var controller = m_character.GetComponent<RigidbodyCharacterController>();
            if (controller != null)
                DestroyImmediate(controller);

            var components = m_character.GetComponents<Component>();
            for (int i = 1; i < components.Length; i++)
            {
                if (components[i] is Transform || components[i] is Animator) continue;

                DestroyImmediate(components[i]);
            }

            m_humanpreview = Editor.CreateEditor(m_character);
            //var controller = _CharacterObject.GetComponent<RigidbodyCharacterController>();
            //var rigidbody = _CharacterObject.GetComponent<Rigidbody>();
            //var collider = _CharacterObject.GetComponent<CapsuleCollider>();
            //var layermanager = _CharacterObject.GetComponent<LayerManager>();


            //if (rigidbody != null) DestroyImmediate(rigidbody);
            //if (collider != null) DestroyImmediate(collider);
            //if (layermanager != null) DestroyImmediate(layermanager);
        }



        private void Create()
        {
            if (string.IsNullOrEmpty(m_name))
                m_name = m_character.name;

            _CharacterObject = Instantiate(m_character, Vector3.zero, Quaternion.identity) as GameObject;
            if (_CharacterObject == false) return;

            _CharacterObject.name = m_name;        
            _CharacterObject.AddComponent<RigidbodyCharacterController>();
            AddActions();


            AddCapsuleCollider();
            AddRigidbody();
            AddLayerManager();
            AddAnimatorMonitor();
            if (addPlayerInput) AddPlayerInput();
            if (addHealth) AddChararacterHealth();
            if (addIk) AddCharacterIK();
            if (addRagdoll) AddCharacterRagdoll();
            if (addInventory) AddInventory();


            if (m_runtimeAnimator)
                _CharacterObject.GetComponent<Animator>().runtimeAnimatorController = m_runtimeAnimator;



            this.Close();
        }


        private void AddActions()
        {
            List<CharacterAction> CharActions = new List<CharacterAction>();

            var Fall = _CharacterObject.AddComponent<Fall>();
            CharActions.Add(Fall);
            //var TakeDamage = _CharacterObject.AddComponent<TakeDamage>();
            //CharActions.Add(TakeDamage);
//            var Idle = _CharacterObject.AddComponent<Idle>();
    //        CharActions.Add(Idle);

            _CharacterObject.GetComponent<RigidbodyCharacterController>().CharActions = CharActions.ToArray();
        }



        private void AddCapsuleCollider()
        {

            var collider = _CharacterObject.GetComponent<CapsuleCollider>();
            if (collider == null) collider = _CharacterObject.AddComponent<CapsuleCollider>();

            // capsule collider 
            collider.height = ColliderHeight(_CharacterObject.GetComponent<Animator>());
            collider.center = new Vector3(0, (float)System.Math.Round(collider.height * 0.5f, 2), 0);
            collider.radius = (float)System.Math.Round(collider.height * 0.2f, 2);

            ComponentUtility.MoveComponentDown(collider);
        }


        private void AddRigidbody()
        {
            var rigidbody = _CharacterObject.GetComponent<Rigidbody>();
            if (rigidbody == null) rigidbody = _CharacterObject.AddComponent<Rigidbody>();

            //rigidbody.mass = _CharacterObject.M
            rigidbody.useGravity = true;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.mass = 100;

            ComponentUtility.MoveComponentDown(rigidbody);
        }


        private void AddLayerManager()
        {
            var layermanager = _CharacterObject.GetComponent<LayerManager>();
            if(layermanager == null) layermanager = _CharacterObject.AddComponent<LayerManager>();

            layermanager.EnemyLayer = 1;
            layermanager.InvisibleLayer = 1;
            layermanager.SolidLayers = 1;

            ComponentUtility.MoveComponentDown(layermanager);
        }

        private void AddAnimatorMonitor()
        {
            var animatorMonitor = _CharacterObject.AddComponent<AnimatorMonitor>();

        }




        private void AddPlayerInput()
        {
            var playerInput = _CharacterObject.AddComponent<PlayerInput>();


        }

        private void AddChararacterHealth()
        {
            var health = _CharacterObject.AddComponent<CharacterHealth>();
            health.MaxHealth = 100;

        }

        private void AddCharacterIK()
        {
            var characterIK = _CharacterObject.AddComponent<CharacterIK>();


        }

        private void AddCharacterRagdoll()
        {
            var ragdoll = _CharacterObject.AddComponent<CharacterRagdoll>();


        }

        private void AddInventory()
        {
            var inventory = _CharacterObject.AddComponent<Inventory>();


        }






        /// <summary>
        /// Capsule Collider height based on the Character height
        /// </summary>
        /// <param name="animator">animator humanoid</param>
        /// <returns></returns>
        float ColliderHeight(Animator animator)
        {
            var foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            return (float)System.Math.Round(Vector3.Distance(foot.position, hips.position) * 2f, 2);
        }  



	}

}

