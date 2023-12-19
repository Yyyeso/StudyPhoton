using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UILobby : UIBase
{
    [SerializeField] private TMP_Text txtNickname;
    [SerializeField] private RectTransform roomParent;
    [SerializeField] private TMP_InputField inputRoomName;
    [SerializeField] private Button btnCreateRoom;
    List<GameObject> objRoomList = new();

    void Start()
    {
        btnCreateRoom.onClick.AddListener(CreateRoom);
        txtNickname.text = $"어서오세요 {GameData.Instance.NickName}님!";
    }

    void CreateRoom()
    {
        if (inputRoomName.text == "" || inputRoomName.text == string.Empty)
        {
            PopUpMessage(message: "생성할 방 제목을 입력하세요.", title: "방 만들기 실패");
            return;
        }

        UIManager.Instance.OnLoading();

        string roomName = PhotonNetwork.LocalPlayer.UserId + "_" + DateTime.UtcNow.ToFileTime();
        string[] propertiesLobby = new string[] { $"{CustomKey.RoomName}", $"{CustomKey.RoomMaster}" };
        Hashtable roomProperties = new()
        {
            { $"{CustomKey.RoomName}", inputRoomName.text },
            { $"{CustomKey.RoomMaster}", GameData.Instance.NickName }
        };
        RoomOptions options = new()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = GameData.Instance.MaxPlayers,
            CustomRoomProperties = roomProperties,
            CustomRoomPropertiesForLobby = propertiesLobby
        };

        NetworkManager.Instance.CreateRoom(roomName, options);
    }

    public void SetRoomList(Dictionary<string, RoomInfo> roomList)
    {
        ClearRoom();
        foreach (var roomData in roomList)
        {
            AddRoom().Setup(roomData.Value);
        }
    }

    UIRoomInfo AddRoom()
    {
        var go = ResourceManager.Instance.InstantiateUI<UIRoomInfo>("SubItem/" + nameof(UIRoomInfo), roomParent);
        objRoomList.Add(go.gameObject);

        return go;
    }

    void ClearRoom()
    {
        foreach (var item in objRoomList)
        {
            Destroy(item);
        }
        objRoomList.Clear();
    }

    UIPopUpButton PopUpMessage(string message, string title = "") => OpenUI<UIPopUpButton>().SetMessage(message, title);
}