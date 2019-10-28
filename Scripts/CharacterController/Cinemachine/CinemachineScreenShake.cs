namespace CharacterController
{
    using UnityEngine;
    using System.Collections;
    using Cinemachine;


    /// <summary>
    /// In editor click on VirtualCamera.  Select Noise: "Basic Multi Channel Perlin" and pick a profile
    /// </summary>
    public class CinemachineScreenShake : MonoBehaviour
    {

        public IEnumerator ShakeVCam(CinemachineVirtualCamera vCam, float amp, float freq, float duration)
        {
            CinemachineBasicMultiChannelPerlin noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            noise.m_AmplitudeGain = amp;
            noise.m_FrequencyGain = freq;
            yield return new WaitForSeconds(duration);

            noise.m_AmplitudeGain = 0;
        }


    }

}
