using Photon.Pun;
using UnityEngine;

public class RpcController : MonoBehaviour
{
    public PhotonView PV { get; private set; }
    UIRoom MyRoom => UIManager.Instance.GetUI<UIRoom>();


    private void Awake() => PV = GetComponent<PhotonView>();

    [PunRPC] public void UpdateChatLog(string nickname, string message) => MyRoom.UpdateChatLog(nickname, message);

    [PunRPC] public void NoticeOnPlayerEntered(string nickname) => MyRoom.NoticeOnPlayerEntered(nickname);

    [PunRPC] public void NoticeOnPlayerLeft(string nickname) => MyRoom.NoticeOnPlayerLeft(nickname);

    [PunRPC] public void UpdateMemberList(int curCount, string memberList) => MyRoom.UpdateMemberList(curCount, memberList);
}