using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class UIRoomInfo : UIBase
{
    [SerializeField] private TMP_Text txtRoomName;
    [SerializeField] private TMP_Text txtRoomMaster;
    [SerializeField] private Button btnEnter;
    string key;

    void Start()
    {
        btnEnter.onClick.AddListener(() =>
        {
            NetworkManager.Instance.JoinRoom(key);
        });
    }

    public void Setup(RoomInfo room)
    {
        var roomName = (string)room.CustomProperties[$"{CustomKey.RoomName}"];
        var roomMaster = (string)room.CustomProperties[$"{CustomKey.RoomMaster}"];

        key = room.Name;
        txtRoomName.text = roomName;
        txtRoomMaster.text = roomMaster;
    }
}
