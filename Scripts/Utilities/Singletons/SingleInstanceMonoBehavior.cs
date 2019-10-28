using UnityEngine;
using System.Collections;

public abstract class SingleInstanceMonoBehavior<T> : MonoBehaviour where T : SingleInstanceMonoBehavior<T>
{
    //[SerializeField]
    //private readonly bool _persistent;

    public static T Instance { get; private set; }



    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarningFormat("Another instance of {0} has already been registered for this scene, destroying this one", this);
            Destroy(this.gameObject);
            return;
        }
        Instance = (T)this;

        //if(_persistent)
        DontDestroyOnLoad(gameObject);

        OnAwake();
    }


    protected virtual void OnAwake()
    {

    }
}
