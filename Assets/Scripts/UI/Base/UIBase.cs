using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    protected T OpenUI<T>() where T : UIBase => UIManager.Instance.OpenUI<T>();

    protected T GetUI<T>() where T : UIBase => UIManager.Instance.GetUI<T>();

    protected void CloseUI() => gameObject.SetActive(false);
}