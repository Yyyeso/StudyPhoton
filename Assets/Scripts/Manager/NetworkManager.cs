using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Text;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        ui.CloseUI<UIIntro>();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
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

    bool IsClosed(RoomInfo room) => (room.RemovedFromList || room.MaxPlayers == 0 || room.PlayerCount == 0 || !room.IsOpen);

    void UpdateLobby(Dictionary<string, RoomInfo> roomList) => ui.GetUI<UILobby>().SetRoomList(roomList);
    #endregion

    #region Create Room
    public void CreateRoom(string roomName, string roomInfo, int maxPlayers)
    {
        Hashtable roomProperties = new()
        {
            { $"{CustomKey.RoomName}", roomName },
            { $"{CustomKey.RoomInfo}", roomInfo } ,
            { $"{CustomKey.RoomMaster}", data.NickName }
        };
        string[] roomPropertiesForLobby = new string[] 
        {
            $"{CustomKey.RoomName}",
            $"{CustomKey.RoomInfo}" ,
            $"{CustomKey.RoomMaster}"
        };

        string roomKey = PhotonNetwork.LocalPlayer.UserId + "_" + DateTime.UtcNow.ToFileTime();
        RoomOptions options = new()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = maxPlayers,
            CustomRoomProperties = roomProperties,
            CustomRoomPropertiesForLobby = roomPropertiesForLobby
        };
        PhotonNetwork.CreateRoom(roomKey, options);
    }

    public override void OnCreatedRoom() => ui.CloseUI<UILobby>();

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ui.CompleteLoading();
        ui.OpenUI<UIPopUpButton>().SetMessage(message: message, title: "방 만들기 실패");
    }
    #endregion

    #region Join Room
    public void JoinRoom(string roomName) => PhotonNetwork.JoinRoom(roomName);

    public override void OnJoinedRoom()
    {
        data.Player = ResourceManager.Instance.InstantiatePlayer();
        var room = PhotonNetwork.CurrentRoom;
        ui.OpenUI<UIRoom>().Setup(room.MaxPlayers, room.PlayerCount, GetMemberList(room.Players));
        ui.CompleteLoading();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (!data.IsMaster) return;

        var room = PhotonNetwork.CurrentRoom;
        data.Player.PV.RPC(nameof(data.Player.NoticeOnPlayerEntered), RpcTarget.All, newPlayer.NickName);
        data.Player.PV.RPC(nameof(data.Player.UpdateMemberList), RpcTarget.All, room.PlayerCount, GetMemberList(room.Players));

        if (room.PlayerCount == room.MaxPlayers) { room.IsOpen = false; }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ui.CompleteLoading();
        ui.OpenUI<UIPopUpButton>().SetMessage(message: message, title: "방 참가 실패");
    }
    #endregion

    #region LeaveRoom
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (!data.IsMaster) return;

        var room = PhotonNetwork.CurrentRoom;
        data.Player.PV.RPC(nameof(data.Player.NoticeOnPlayerLeft), RpcTarget.All, otherPlayer.NickName);
        data.Player.PV.RPC(nameof(data.Player.UpdateMemberList), RpcTarget.All, room.PlayerCount, GetMemberList(room.Players));

        if (room.IsOpen == false && room.PlayerCount < room.MaxPlayers) { room.IsOpen = true; }
    }
    #endregion

    string GetMemberList(Dictionary<int, Player> players)
    {
        StringBuilder sb = new();
        foreach (var pl in players) { sb.Append(pl.Value.NickName + "\n"); }
        return sb.ToString();
    }
}