using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsObject : MonoBehaviour
{
    [SerializeField, DisplayOnly]
    private int prefabID;
    [SerializeField, DisplayOnly]
    private float duration;             //  How long before the object is returned back.
    [SerializeField, DisplayOnly]
    private float currentDuration;      //  The current time since enabled.
    [SerializeField]
    private ParticleSystem[] particleSystems = new ParticleSystem[0];



	private void Start()
	{
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < particleSystems.Length; i++){
            if (particleSystems[i].main.duration > duration)
                duration = particleSystems[i].main.duration;
        }
	}



	private void OnEnable()
	{
        currentDuration = 0;

	}

	private void OnDisable()
	{
        for (int i = 0; i < particleSystems.Length; i++){
            particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        currentDuration = 0;
        playing = false;

    }




    bool playing;
	private void Update()
	{
        if(enabled)
        {
            if(playing == false)
            {

                for (int i = 0; i < particleSystems.Length; i++)
                {
                    //particleSystems[i].time = 0;
                    particleSystems[i].Play(true);
                    playing = true;
                }
            }
            if (duration > 0)
            {
                currentDuration += Time.deltaTime;
                if (currentDuration > duration)
                {
                    ObjectPool.Return(gameObject);
                }
            }
        }

	}




}
