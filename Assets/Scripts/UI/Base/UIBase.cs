using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    protected T OpenUI<T>() where T : UIBase
    {
        return UIManager.Instance.OpenUI<T>();
    }

    protected T GetUI<T>() where T : UIBase
    {
        return UIManager.Instance.GetUI<T>();
    }

    protected void CloseUI()
    {
        gameObject.SetActive(false);
    }
}