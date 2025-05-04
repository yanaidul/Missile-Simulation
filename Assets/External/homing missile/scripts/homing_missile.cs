using MissileSimulation.Missile;
using MissileSimulation.Replay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HomingMissile
{
    public class homing_missile : MonoBehaviour
    {
        [SerializeField] private GameEventNoParam _onPlaneDestroy;

        public int speed = 857;
        public int downspeed = 30;
        public int damage = 35;
        public bool fully_active = false;
        public int timebeforeactivition = 20;
        public int timebeforebursting = 40;
        public int timebeforedestruction = 350;
        public int timealive;
        public GameObject initialTarget;
        public GameObject target;
        public GameObject shooter;
        public Rigidbody projectilerb;
        public bool isactive = false;
        public Vector3 sleepposition;
        public GameObject targetpointer;
        public float turnSpeed = 0.035f;
        public AudioSource launch_sound;
        public AudioSource thrust_sound;
        public GameObject smoke_obj;
        public ParticleSystem smoke;
        public GameObject smoke_position;
        public GameObject destroy_effect;

        private homing_missile_pointer _homingMissilePointer;
        private bool _stopMissileRotation = false;
        private void Start()
        {
            _stopMissileRotation = false;
            projectilerb = this.GetComponent<Rigidbody>();
            _homingMissilePointer = targetpointer.GetComponent<homing_missile_pointer>();
        }
        public void call_destroy_effects()
        {
            Instantiate(destroy_effect, transform.position, transform.rotation);
        }
        public void setmissile()
        {
            timealive = 0;
            //transform.rotation = shooter.transform.rotation;
            //transform.Rotate(0, 0, 0);
            transform.position = shooter.transform.position;
        }
        public void DestroyMe()
        {
            _onPlaneDestroy.Raise();
            isactive = false;
            fully_active = false;
            timealive = 0;
            smoke.transform.SetParent(null);
            smoke.Pause();
            smoke.transform.position =sleepposition;
            smoke.Stop();
            projectilerb.linearVelocity = Vector3.zero;
            if (!SfxManager.GetInstance().IsSfxOff) thrust_sound.Pause();
            if (SfxManager.GetInstance().IsSfxOff)
            {
                destroy_effect.GetComponent<AudioSource>().enabled = false;
            }
            call_destroy_effects();
            transform.position = sleepposition;
            Destroy(smoke.gameObject,3);
            //Destroy(this.gameObject);
            gameObject.transform.GetChild(2).gameObject.SetActive(false);

            if (!MissileManager.GetInstance().IsMissile1CanBeUsed && !MissileManager.GetInstance().IsMissile2CanBeUsed)
            {
                GameManager.GetInstance().SetGameOver(false);
            }
        }
        public void usemissile()
        {
            SfxManager.GetInstance().PlayContinousBeepSFX();
            ReplayManager.GetInstance().StartRecording();
            Timer.GetInstance().OnStopTimer();
            if (!SfxManager.GetInstance().IsSfxOff) launch_sound.Play();
            isactive = true;
            setmissile();

        }
        private void OnTriggerEnter(Collider other)
        {
            if (isactive)
            {
                if (other.gameObject.CompareTag("Plane"))
                {
                    if (AimManager.GetInstance().IsLockedOnFront)
                    {
                        StartCoroutine(DelayStartReplay(1));
                        other.gameObject.SetActive(false);
                        //GameManager.GetInstance().SetGameOver(true);

                        if (other.gameObject == shooter)
                        {
                            if (fully_active)
                            {
                                //damege the shooter;
                                DestroyMe();
                            }
                        }
                        else
                        {
                            //damage the enemy;
                            DestroyMe();
                        }
                    }
                    else
                    {
                        _stopMissileRotation = true;
                        ReplayManager.GetInstance().StopRecording();
                        if (!MissileManager.GetInstance().IsMissile1CanBeUsed && !MissileManager.GetInstance().IsMissile2CanBeUsed)
                        {
                            GameManager.GetInstance().SetGameOver(false);
                        }
                    }
                }
            }
        }
        void FixedUpdate()
        {
            if (isactive)
            {
                if (!target.activeInHierarchy)
                {
                    DestroyMe();
                }
                if (timealive == timebeforeactivition)
                {
                    smoke = (Instantiate(smoke_obj, smoke_position.transform.position, smoke_position.transform.rotation)).GetComponent<ParticleSystem>();
                    smoke.Play();
                    smoke.transform.SetParent(this.transform);
                    fully_active = true;
                    if(!SfxManager.GetInstance().IsSfxOff) thrust_sound.Play();
                }
                timealive++;
                if (timealive < timebeforebursting)
                {
                    projectilerb.linearVelocity = transform.forward * downspeed;
                }
                if (timealive == timebeforedestruction)
                {
                    DestroyMe();
                }
                if (timealive >= timebeforebursting && timealive < timebeforedestruction)
                {
                    if(!_stopMissileRotation) transform.rotation = Quaternion.Slerp(transform.rotation, targetpointer.transform.rotation, turnSpeed);
                    projectilerb.linearVelocity = transform.forward * speed;
                    _homingMissilePointer.target = target;
                }
            }
        }

        IEnumerator DelayStartReplay(float duration)
        {
            yield return new WaitForSeconds(duration);
            ReplayManager.GetInstance().StopRecording();
            GameManager.GetInstance().StartReplay();
            ReplayManager.GetInstance().StartPlayback();
            gameObject.SetActive(false);
        }
    }
}