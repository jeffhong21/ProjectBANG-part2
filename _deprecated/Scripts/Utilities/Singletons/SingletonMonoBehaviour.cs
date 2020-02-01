using UnityEngine;

/// <summary>
/// Base class for components that must behave as singletons - being unique in the scene and having a static Instance property for easy access.
/// </summary>
/// <typeparam name="T">The type of the deriving component.</typeparam>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{

    //[SerializeField]
    //private readonly bool _persistent;

    public static T Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this){
            Debug.LogWarningFormat("Another instance of {0} has already been registered for this scene, destroying this one", this);
            Destroy(this.gameObject);
            return;
        }
        Instance = (T)this;

        //if(_persistent)
            //DontDestroyOnLoad(gameObject);

        OnAwake();
    }


    protected virtual void OnAwake()
    {

    }
}