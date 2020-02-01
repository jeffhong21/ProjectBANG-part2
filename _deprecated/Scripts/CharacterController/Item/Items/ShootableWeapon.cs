namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    [RequireComponent(typeof(Item))]
    public class ShootableWeapon : MonoBehaviour, IUseableItem, IReloadableItem
    {
        //
        // Fields
        //
        //[Header("--  Shootable Weapon Settings --")]
        [Tooltip(" ")]
        [SerializeField] protected Transform m_FirePoint;
        [Tooltip("Tooltip")]
        [SerializeField] protected float m_FireRate = 1;                 //  The number of shots per second
        [Tooltip("The number of rounds to fire in a single shot.")]
        [SerializeField] protected int m_FireCount = 1;

        [SerializeField] protected int m_CurrentAmmo = 30;
        [Tooltip("Tooltip")]
        [SerializeField] protected int m_MaxAmmo = 30;

        [SerializeField] protected bool m_AutoReload;
        [Range(0, 1)]
        [SerializeField] protected float m_RecoilAmount = 0.1f;              //  REMOVE
        [Range(0, 30)]
        [SerializeField] protected float m_RotationRecoilAmount = 4f;        //  REMOVE
        [Range(0,1)]
        [SerializeField] protected float m_Spread = 0.01f;               //  REMOVE

        [SerializeField] protected GameObject m_Smoke;

        [SerializeField] protected GameObject m_MuzzleFlash;

        [SerializeField] protected Transform m_SmokeLocation;

        [SerializeField] protected AudioClip[] m_FireSounds = new AudioClip[0];

        [SerializeField] protected float m_FireSoundDelay = 0.1f;

        [SerializeField] protected float m_DamageAmount = 10;

        [SerializeField] protected float m_ImpactForce = 5;

        [SerializeField] protected float m_FireRange = float.MaxValue;
        [Tooltip("A LayerMask of the layers that can be hit when fired at.")]
        [SerializeField] protected LayerMask m_ImpactLayers = -1;

        [SerializeField] protected GameObject m_Tracer;

        [SerializeField] protected GameObject m_Projectile;

        [Header("-- Decals --")]
        
        protected GameObject m_DefaultDecal;
        
        protected GameObject m_DefaultDust;


        [Header("Debug"), SerializeField]
        private bool m_DrawAimLine;

        private float m_ReloadTime = 3f;
        private float m_NextUseTime;
        //  Recoil
        private float m_RecoilAngle;
        private Vector3 m_RecoilSmoothDampVelocity;
        private float m_RecoilRotSmoothDampVelocity;
        private Vector3 m_RecoilTargetPosition;
        private Quaternion m_RecoilTargetRotation;

        //  Audio
        private WaitForSeconds m_fireSoundSecondsDelay;
        private Quaternion m_Rotation;

        //  Fire direction
        private Vector3 m_LookSourceDirection;
        private Vector3 m_TargetDirection;

        protected GameObject m_Character;

        public int CurrentAmmo{
            get { return m_CurrentAmmo; }
            set { m_CurrentAmmo = value; }
        }

        public LayerMask ImpactLayers{
            get { return m_ImpactLayers; }
            //set { m_CurrentAmmo = value; }
        }


        //
        // Methods
        //
        protected void Awake()
        {
            m_Character = gameObject;

            if (m_CurrentAmmo > m_MaxAmmo)
                m_CurrentAmmo = m_MaxAmmo;



            //if (m_Smoke != null){
            //    m_Smoke = Instantiate(m_Smoke, m_Transform);
            //    m_Smoke.transform.localPosition = m_SmokeLocation.localPosition;
            //    m_Smoke.transform.localRotation = m_SmokeLocation.localRotation;
            //    m_Smoke.SetActive(false);
            //}
            //if (m_MuzzleFlash != null){
            //    m_MuzzleFlash = Instantiate(m_MuzzleFlash, m_Transform);
            //    m_MuzzleFlash.transform.localPosition = m_FirePoint.localPosition;
            //    m_MuzzleFlash.transform.localRotation = m_FirePoint.localRotation;
            //    m_MuzzleFlash.SetActive(false);
            //}

            m_fireSoundSecondsDelay = new WaitForSeconds(m_FireSoundDelay);
        }

        //public override void Initialize(InventoryBase inventory)
        //{
        //    base.Initialize(inventory);

        //    m_Rotation = m_Transform.parent.localRotation;
        //}


		//private void LateUpdate()
		//{
  //          if(m_Transform.localPosition != Vector3.zero){
  //              m_RecoilTargetPosition = Vector3.SmoothDamp(m_Transform.localPosition, Vector3.zero, ref m_RecoilSmoothDampVelocity, 0.12f);
  //              m_Transform.localPosition = Vector3.Lerp(m_RecoilTargetPosition, Vector3.zero, Time.deltaTime * 2f);
  //          }

  //          if(m_RecoilAngle > 0 && m_RecoilAmount < 0){
  //              m_RecoilAngle = Mathf.SmoothDamp(m_RecoilAngle, 0, ref m_RecoilRotSmoothDampVelocity, 0.12f);
  //              m_RecoilTargetRotation = Quaternion.Lerp(m_Transform.localRotation, Quaternion.Euler(m_RecoilAngle, 0, 0), Time.deltaTime * 2f);
  //              m_Transform.localRotation = m_RecoilTargetRotation;
  //          }


  //          //Debug.Log(m_RecoilAngle);
		//}





		public bool UseItem()
        {
            Debug.Log("Firing " + gameObject.name);
            return true;

            if (InUse() || IsReloading()) return false;

            if (m_CurrentAmmo > 0)
            {
                Fire();
                //  Set cooldown variables.
                m_NextUseTime = Time.timeSinceLevelLoad + m_FireRate;
                return true;
            }
            return false;
        }


        public virtual bool InUse(){
            return Time.timeSinceLevelLoad < m_NextUseTime;
        }





        public virtual bool TryStartReload()
        {
            if (IsReloading() || InUse()) return false;

            m_NextUseTime = Time.timeSinceLevelLoad + m_ReloadTime;

            //Debug.LogFormat("Reloading | {0}", Time.timeSinceLevelLoad);
            return true;
        }


        public virtual bool IsReloading()
        {
            return Time.timeSinceLevelLoad < m_NextUseTime;
        }


        //public void SetLookSourceDirection(Vector3 direction){
        //    if(Vector3.Dot(m_FirePoint.forward, direction) < 0){
        //        m_TargetDirection = m_FirePoint.forward;
        //        Debug.LogFormat("Direction provided to {0} is behind it.", m_GameObject.name);
        //    } else {
        //        m_TargetDirection = direction;
        //    }
        //}

        public void SetFireAtPoint(Vector3 point){
            m_TargetDirection = point - m_FirePoint.position;
        }






        //protected override void ItemActivated()
        //{
        //    if (m_Smoke) m_Smoke.SetActive(true);
        //    if (m_MuzzleFlash) m_MuzzleFlash.SetActive(true);
        //}

        //protected override void ItemDeactivated()
        //{
        //    if (m_Smoke) m_Smoke.SetActive(false);
        //    if (m_MuzzleFlash) m_MuzzleFlash.SetActive(false);
        //}



        protected virtual void Fire()
        {

            //var targetDirection = m_controller.LookAtPoint - m_FirePoint.position;
            //var targetDirection = (m_FirePoint.position + m_controller.LookDirection) - m_FirePoint.position;
            m_TargetDirection = (m_FirePoint.position + m_TargetDirection) - m_FirePoint.position;

            if (m_Projectile){
                ProjectileFire();
            }  else {
                HitscanFire();
                if (m_Tracer){
                    var go = Instantiate(m_Tracer, m_FirePoint.position, Quaternion.LookRotation(m_FirePoint.forward));
                    var ps = go.GetComponentInChildren<ParticleSystem>();
                    ps.Play();
                    Destroy(ps.gameObject, 5);
                }
            }
            ////  Recoil
            transform.localPosition -= Vector3.forward * m_RecoilAmount;
            m_RecoilAngle += Mathf.Clamp( m_RotationRecoilAmount, 0, 30);
            transform.localEulerAngles += Vector3.left * m_RecoilAngle;

            //  Play Particle Shoot Effects.
            if(m_Smoke){
                m_Smoke.GetComponentInChildren<ParticleSystem>().Play();
            }
            if(m_MuzzleFlash){
                m_MuzzleFlash.GetComponentInChildren<ParticleSystem>().Play();
            }

            //  Play Sound Effects
            if(m_FireSounds.Length > 0)
                StartCoroutine(PlaySoundEffect());


            //  Update current ammo.
            //m_CurrentAmmo -= m_FireCount;
            //  Reload if auto reload is set.

            if (m_AutoReload && m_CurrentAmmo <= 0){
                TryStartReload();
                Debug.LogFormat("{0} is auto reloading.", gameObject.name);
            }

            m_TargetDirection = m_FirePoint.forward;
        }



        private IEnumerator PlaySoundEffect()
        {
            yield return m_fireSoundSecondsDelay;

            var audioSource = GetComponent<AudioSource>();
            var clipIndex = Random.Range(0, m_FireSounds.Length);
            audioSource.clip = m_FireSounds[clipIndex];
            audioSource.Play();

            yield return null;
        }



        protected virtual void ProjectileFire()
        {
            RaycastHit hit;
            if (Physics.Raycast(m_FirePoint.position, m_FirePoint.forward, out hit, m_FireRange))
            {
                //  Spawn Projectile from the PooManager.
                var go = Instantiate(m_Projectile, m_FirePoint.position, m_FirePoint.rotation);
                //  Get Projectile Component.
                var projectile = go.GetComponent<Projectile>();
                //  Initialize projectile.
                projectile.Initialize(m_DamageAmount, m_FirePoint.forward, hit.point,m_Character);
            }

        }




        protected virtual void HitscanFire()
        {
            RaycastHit hit;
            //Debug.DrawRay(m_FirePoint.position, targetDirection, Color.blue, 1);
            if(Physics.Raycast(m_FirePoint.position, m_TargetDirection, out hit, m_FireRange, m_ImpactLayers))
            {
                var damagableObject = hit.transform.GetComponentInParent<Health>();
                Vector3 hitDirection = hit.transform.position - m_FirePoint.position;
                Vector3 force = hitDirection.normalized * m_ImpactForce;
                var rigb = hit.transform.GetComponent<Rigidbody>();



                if (damagableObject is CharacterHealth)
                {
                    damagableObject.TakeDamage(m_DamageAmount, hit.point, force, m_Character, hit.collider.gameObject);
                }
                else if (damagableObject is Health)
                {
                    damagableObject.TakeDamage(m_DamageAmount, hit.point, force, m_Character);
                }
                else
                {
                    //ObjectPoolManager.Instance.Spawn(m_DefaultDust, collisionPoint, Quaternion.FromToRotation(m_Transform.forward, collisionPointNormal));
                    ObjectPool.Get(m_DefaultDust, hit.point, Quaternion.LookRotation(hit.normal));
                }


                if (rigb && !hit.transform.gameObject.isStatic)
                {
                    rigb.AddForceAtPosition(hitDirection.normalized * m_ImpactForce, hit.point, ForceMode.Impulse);
                }

            }
        }




        protected void OnStartAim()
        {
            
        }


        protected void OnAim(bool aim)
        {
            ////Debug.Log("Weapon Starting to aim");
            //if(aim){
            //    //var targetDirection = m_controller.LookPosition - m_Transform.position;
            //    var targetDirection = m_Character.transform.forward - m_Transform.position;
            //    //if(targetDirection == Vector3.zero)
            //        //targetDirection.Set(m_controller.transform.position.x, 1.35f, m_controller.transform.position.x + 10);
            //    Debug.DrawRay(m_Transform.position, targetDirection, Color.blue, 1);
            //    //var m_RecoilTargetRotation = Quaternion.LookRotation(targetDirection);
            //    //m_RecoilTargetRotation *= Quaternion.Euler(0, 90, 90);
            //    //m_Transform.rotation = m_RecoilTargetRotation;
            //    m_Transform.parent.localEulerAngles = new Vector3(0, 90, 90);
            //}else{
            //    m_Transform.parent.localRotation = m_Rotation;
            //}
        }



        private void SpawnHitEffects(Vector3 collisionPoint, Vector3 collisionPointNormal)
        {
            var decal = Instantiate(m_DefaultDecal, collisionPoint, Quaternion.LookRotation(collisionPointNormal));
            Destroy(decal, 5);
            var dust = Instantiate(m_DefaultDust, collisionPoint, Quaternion.LookRotation(collisionPointNormal));
            var ps = dust.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1);
        }


        private void SpawnParticles(GameObject particleObject, Vector3 collisionPoint, Vector3 collisionPointNormal)
        {
            var go = Instantiate(particleObject, collisionPoint, Quaternion.LookRotation(collisionPointNormal));
            var ps = go.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1);
        }


        private void OnDrawGizmos()
        {
            if(Application.isPlaying){
                if (m_DrawAimLine && m_FirePoint != null)
                {
                    Gizmos.color = Color.yellow;
                    //Gizmos.DrawRay(m_FirePoint.position, (m_FirePoint.forward * 50));  //  + (Vector3.up * m_FirePoint.position.y) 
                    Gizmos.DrawRay(m_FirePoint.position, m_TargetDirection * 50);  //  + (Vector3.up * m_FirePoint.position.y) 

                }
            }

        }













    }

}