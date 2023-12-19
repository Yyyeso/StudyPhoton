using Photon.Pun;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public Sprite LoadSprite(string path)
    {
        return Load<Sprite>("Sprites/" + path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        var go = Load<GameObject>(path);
        return Instantiate(go, parent);
    }

    public T Instantiate<T>(string path, Transform parent = null) where T : Component
    {
        return Instantiate("Prefabs/" + path, parent).GetComponent<T>();
    }

    public T InstantiateUI<T>(string path, Transform parent = null) where T : UIBase
    {
        return Instantiate("UI/" + path, parent).GetComponent<T>();
    }

    public RpcController InstantiatePlayer()
    {
        var player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        player.name = $"Player({GameData.Instance.NickName})";

        return player.GetComponent<RpcController>();
    }
}