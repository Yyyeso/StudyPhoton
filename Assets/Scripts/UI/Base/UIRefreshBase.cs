using UnityEngine;
using System.Collections;

public class UIRefreshBase : UIBase
{
    [SerializeField] private RectTransform[] _rects;


    public void RefreshUI(float time = 0.01f)
    {
        if (_rects.Length < 0 || !gameObject.activeInHierarchy)
        { return; }

        StartCoroutine(RefreshAction(time));
    }

    IEnumerator RefreshAction(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        ForceRebuild();
    }

    protected void ForceRebuild()
    {
        foreach (RectTransform rect in _rects)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}