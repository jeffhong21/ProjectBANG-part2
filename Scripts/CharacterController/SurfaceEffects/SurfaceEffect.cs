namespace CharacterController
{
    using UnityEngine;
    using System;
    using Random = UnityEngine.Random;

    [CreateAssetMenu(fileName = "Surface Effect", menuName = "-- RootMotion Controller --/Surface Effect", order = -1000)]
    public class SurfaceEffect : ScriptableObject
    {

        [Serializable]
        public class SpawnedObjects
        {
            public GameObject spawnedObject;
            [Range(0,1)]
            public float probability = 1;
            public bool randomSpin;


            public GameObject Get(Vector3 position, Quaternion rotation)
            {
                if(spawnedObject != null)
                {
                    if (Random.Range(0, 100) / 100 <= probability)
                    {
                        GameObject instance = ObjectPool.Get(spawnedObject, position, rotation);
                        //if (instance == null)
                        //    instance = UnityEngine.Object.Instantiate(spawnedObject, position, rotation);
                        if (randomSpin){
                            float eulerY = Random.Range(-180, 180);
                            Quaternion extraRotation = Quaternion.AngleAxis(eulerY, instance.transform.up);
                            instance.transform.rotation *= extraRotation;
                        }
                        return instance;
                    }
                    
                }
                return null;
            }
        }


        [Serializable]
        public class Decals
        {
            public GameObject[] prefabs;
            [Range(0.01f, 2f)]
            public float minScale = 1;
            [Range(0.01f, 2f)]
            public float maxScale = 1;

            public GameObject Get(Vector3 position, Quaternion rotation)
            {
                if (prefabs.Length > 0 || prefabs != null){
                    int index = Random.Range(0, prefabs.Length);
                    GameObject instance = ObjectPool.Get(prefabs[index], position, rotation);
                    //if (instance == null)
                    //    instance = UnityEngine.Object.Instantiate(prefabs[index], position, rotation);
                    float scale = Random.Range(minScale, maxScale);
                    instance.transform.localScale = new Vector3(scale, scale, scale);
                    return instance;
                }
                return null;
            }

        }


        [Serializable]
        public class Audio
        {
            public AudioClip[] audioClips = new AudioClip[0];
            [Range(0.01f, 1f)] public float minVolume = 1;
            [Range(0.01f, 1f)] public float maxVolume = 1;
            [Range(-3, 3)] public float minPitch = 1;
            [Range(-3, 3)] public float maxPitch = 1;
            public bool randomClipSelection = true;

            private int index = -1;

            public AudioClip Play(AudioSource audioSource)
            {
                if (audioClips.Length > 0 || audioClips != null){

                    if (randomClipSelection){
                        index = Random.Range(0, audioClips.Length);
                    } else {
                        if (index == audioClips.Length || index < 0) index = 0;
                        else index++;
                    }

                    AudioClip clip = audioClips[index];
                    audioSource.clip = clip;
                    audioSource.volume = Random.Range(minVolume, maxVolume);
                    audioSource.pitch = Random.Range(minPitch, maxPitch);

                    audioSource.Play();
                    return clip;
                }
                return null;
            }
        }
        


        public SpawnedObjects[] spawnedObjects;
        public Decals decals;
        public Audio audio;



        public void SpawnSurfaceEffect(Vector3 position, Quaternion rotation, AudioSource audioSource)
        {
            if(spawnedObjects.Length > 0){
                for (int i = 0; i < spawnedObjects.Length; i++)
                    spawnedObjects[i].Get(position, rotation);
            }

            decals.Get(position, rotation);

            if(audioSource != null) audio.Play(audioSource);
        }



            
    }
}

