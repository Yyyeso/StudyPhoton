using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    #region Parent Object
    private Transform _uiContents = null;
    public Transform UIContents
    {
        get
        {
            if (_uiContents == null)
            {
                GameObject uiRoot = GameObject.Find("[UIContents]") ?? new("[UIContents]");
                _uiContents = uiRoot.transform;
            }
            return _uiContents;
        }
    }
    #endregion

    private Dictionary<string, UIBase> UI = new();
    UIBase _ui;


    private T CreateUI<T>() where T : UIBase
    {
        string key = typeof(T).Name;

        T ui = ResourceManager.Instance.InstantiateUI<T>(key, UIContents);
        UI.Add(key, ui);

        return ui;
    }

    public T OpenUI<T>() where T : UIBase
    {
        SetEventSystem();

        if (IsUIExist<T>())
        {
            _ui.gameObject.SetActive(true);
            return (T)_ui;
        }
        else
        {
            return CreateUI<T>();
        }
    }

    public void CloseUI<T>()
    {
        if (IsUIExist<T>())
        {
            _ui.gameObject.SetActive(false);
        }
    }

    public T GetUI<T>() where T : UIBase
    {
        if (IsUIExist<T>())
        {
            return (T)_ui;
        }

        return default;
    }

    public void ClearUI()
    {
        if (UI == null) return;
        UI.Clear();
    }

    public bool IsUIExist<T>()
    {
        string key = typeof(T).Name;
        var value = UI.TryGetValue(key, out _ui);

        if (value && _ui == null)
        {
            UI.Remove(key);
            return false;
        }
        return value;
    }

    private void SetEventSystem()
    {
        if (GameObject.Find(nameof(EventSystem)) != null)
        { return; }

        GameObject eventSystem = new (nameof(EventSystem));
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    #region Loading
    public void OnLoading() => OpenUI<UILoading>().Setup();

    public void CompleteLoading()
    {
        if (!IsUIExist<UILoading>()) return;

        GetUI<UILoading>().Stop();
        CloseUI<UILoading>();
    }
    #endregion
}