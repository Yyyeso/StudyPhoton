using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    #region Parent Object
    private static GameObject parent = null;
    private static GameObject disposable = null;
    public static Transform Parent
    {
        get
        {
            if (parent == null)
            {
                parent = GameObject.Find("[Singleton]");
                if (parent == null)
                {
                    parent = new GameObject("[Singleton]");
                    DontDestroyOnLoad(parent);
                }
            }
            return parent.transform;
        }
    }
    public static Transform Disposable
    {
        get
        {
            if (disposable == null)
            {
                disposable = GameObject.Find("[Disposable]");
                if (disposable == null)
                {
                    disposable = new GameObject("[Disposable]");
                }
            }
            return disposable.transform;
        }
    }
    #endregion

    private static T _instance = null;
    private static bool shuttingDown = false;

    public static T Instance
    {
        get
        {
            if (shuttingDown)
            {
                return default;
            }

            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    _instance = new GameObject { name = $"@{typeof(T)}" }.AddComponent<T>();

                    if(_instance.GetComponent<IDestroy>() == null)
                        _instance.transform.SetParent(Parent);
                    else
                        _instance.transform.SetParent(Disposable);
                }
            }

            return _instance;
        }
    }


    public virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        print($"Init: {typeof(T)}");
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }
}