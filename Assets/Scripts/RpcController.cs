using Photon.Pun;
using UnityEngine;

public class RpcController : MonoBehaviour
{
    public PhotonView PV { get; private set; }
    UIRoom MyRoom => UIManager.Instance.GetUI<UIRoom>();

    private void Awake() => PV = GetComponent<PhotonView>();

    [PunRPC] public void UpdateChatLog(string nickname, string message) => MyRoom.UpdateChatLog(nickname, message);
    [PunRPC] public void AddMember(string nickname) => MyRoom.AddMember(nickname);
    [PunRPC] public void RemoveMember(string nickname) => MyRoom.RemoveMember(nickname);
    [PunRPC] public void UpdateCount(int curCount) => MyRoom.UpdateCount(curCount);
}