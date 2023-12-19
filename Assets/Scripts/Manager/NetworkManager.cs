using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Instance
    private static GameObject parent = null;
    private static NetworkManager _instance = null;
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
    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkManager>();

                if (_instance == null)
                {
                    _instance = new GameObject { name = "@NetworkManager" }.AddComponent<NetworkManager>();
                    _instance.transform.SetParent(Parent);
                }
            }

            return _instance;
        }
    }

    #endregion

    GameData data;
    UIManager ui;


    #region Connect
    private void Awake()
    {
        data = GameData.Instance;
        ui = UIManager.Instance;
    }

    public void Connect(string nickName)
    {
        data.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.LocalPlayer.NickName = data.NickName;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        ui.CloseUI<UIIntro>();
        ui.OpenUI<UILobby>();
        ui.CompleteLoading();
    }
    #endregion

    #region Update Lobby
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (IsClosed(room))
            {
                if (data.RoomList.ContainsKey(room.Name)) // 삭제
                { data.RoomList.Remove(room.Name); }
            }
            else
            {
                if (data.RoomList.ContainsKey(room.Name)) // 갱신
                { data.RoomList[room.Name] = room; }
                else
                { data.RoomList.Add(room.Name, room); }   // 추가
            }
        }
        UpdateLobby(data.RoomList);
    }

    bool IsClosed(RoomInfo room)
    {
        return (room.RemovedFromList || room.MaxPlayers == 0 || room.PlayerCount == 0 || !room.IsOpen);
    }

    void UpdateLobby(Dictionary<string, RoomInfo> roomList)
    {
        ui.GetUI<UILobby>().SetRoomList(roomList);
    }
    #endregion

    #region Create Room
    public void CreateRoom(string roomName, RoomOptions options) => PhotonNetwork.CreateRoom(roomName, options);

    public override void OnCreatedRoom()
    {
        ui.CloseUI<UILobby>();
        ui.CompleteLoading();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) => ui.OpenUI<UIPopUpButton>().SetMessage(message: message, title: "방 만들기 실패");
    #endregion

    #region Join Room
    public void JoinRoom(string roomName) => PhotonNetwork.JoinRoom(roomName);

    public override void OnJoinedRoom()
    {
        data.Player = ResourceManager.Instance.InstantiatePlayer();
        var room = PhotonNetwork.CurrentRoom;
        ui.OpenUI<UIRoom>().Setup(room.PlayerCount, room.MaxPlayers);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (!data.IsMaster) return;

        var room = PhotonNetwork.CurrentRoom;
        data.Player.PV.RPC(nameof(data.Player.AddMember), RpcTarget.All, newPlayer.NickName);
        data.Player.PV.RPC(nameof(data.Player.UpdateCount), RpcTarget.All, room.PlayerCount);

        if (room.PlayerCount == room.MaxPlayers) { room.IsOpen = false; }
    }

    public override void OnJoinRoomFailed(short returnCode, string message) => ui.OpenUI<UIPopUpButton>().SetMessage(message: message, title: "방 참가 실패");
    #endregion

    #region LeaveRoom
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (!data.IsMaster) return;

        var room = PhotonNetwork.CurrentRoom;
        data.Player.PV.RPC(nameof(data.Player.RemoveMember), RpcTarget.All, otherPlayer.NickName);
        data.Player.PV.RPC(nameof(data.Player.UpdateCount), RpcTarget.All, room.PlayerCount);

        if (room.IsOpen == false && room.PlayerCount != room.MaxPlayers) { room.IsOpen = true; }
    }
    #endregion
}