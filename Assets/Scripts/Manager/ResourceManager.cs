using Photon.Pun;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public T Load<T>(string path) where T : Object => Resources.Load<T>(path);

    public Sprite LoadSprite(string path) => Load<Sprite>("Sprites/" + path);

    public GameObject Instantiate(string path, Transform parent = null) => Instantiate(Load<GameObject>(path), parent);

    public T Instantiate<T>(string path, Transform parent = null) where T : Component => Instantiate("Prefabs/" + path, parent).GetComponent<T>();

    public T InstantiateUI<T>(string path, Transform parent = null) where T : UIBase => Instantiate("UI/" + path, parent).GetComponent<T>();

    public RpcController InstantiatePlayer() => PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity).GetComponent<RpcController>();
}