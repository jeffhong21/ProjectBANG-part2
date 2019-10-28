using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

[Serializable]
public class VirtualCameraState
{
    [SerializeField]
    private string m_stateName;
    [SerializeField]
    private CinemachineVirtualCameraBase m_virtualCamera;
    private CinemachineImpulseSource impulseSource;



    //
    //  Properties
    //
    public string StateName{
        get { return m_stateName; }
        set{
            if(string.IsNullOrEmpty(value)) m_stateName = m_virtualCamera.name;
            else m_stateName = value;
        }
    }

    public CinemachineVirtualCameraBase VirtualCamera { get { return m_virtualCamera; } set { m_virtualCamera = value; } }

    public CinemachineImpulseSource ImpulseSource { get { return impulseSource; } }

    //
    //  Constructors.
    //
    public VirtualCameraState(string m_stateName)
    {
        this.m_stateName = m_stateName;
    }

    public VirtualCameraState(string m_stateName, CinemachineVirtualCameraBase m_virtualCamera)
    {
        this.m_stateName = m_stateName;
        this.m_virtualCamera = m_virtualCamera;
    }


    public void Initialize()
    {
        impulseSource = m_virtualCamera.GetComponent<CinemachineImpulseSource>();
    }





    public void SetActive(bool active)
    {
        m_virtualCamera.gameObject.SetActive(active);
    }
}


