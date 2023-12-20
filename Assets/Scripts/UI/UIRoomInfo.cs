using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class UIRoomInfo : UIBase
{
    [SerializeField] private TMP_Text txtRoomName;
    [SerializeField] private TMP_Text txtRoomInfo;
    [SerializeField] private TMP_Text txtRoomMaster;
    [SerializeField] private Button btnEnter;

    string key;


    void Start() => btnEnter.onClick.AddListener(EnterRoom);

    public void Setup(RoomInfo room)
    {
        var roomName = (string)room.CustomProperties[$"{CustomKey.RoomName}"];
        var roomInfo = (string)room.CustomProperties[$"{CustomKey.RoomInfo}"];
        var roomMaster = (string)room.CustomProperties[$"{CustomKey.RoomMaster}"];
        
        key = room.Name;
        txtRoomName.text = roomName;
        txtRoomInfo.text = $"({room.PlayerCount}/{room.MaxPlayers})\n{roomInfo}";
        txtRoomMaster.text = $"방장: {roomMaster}";
    }

    void EnterRoom()
    {
        UIManager.Instance.OnLoading();
        NetworkManager.Instance.JoinRoom(key);
    }
}