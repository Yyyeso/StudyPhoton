using UnityEngine;

public class SampleScene : MonoBehaviour
{
    void Start()
    {
        if (GameData.Instance.NickName == null)
        { UIManager.Instance.OpenUI<UIIntro>(); }
    }
}