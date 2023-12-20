using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections.Generic;

public class UILobby : UIBase
{
    [SerializeField] private TMP_Text txtNickname;

    List<GameObject> objRoomList = new();
    [SerializeField] private RectTransform roomParent;

    int maxPlayers = 2;
    [SerializeField] private TMP_Text txtMaxPlayers;
    [SerializeField] private Button btnMinus;
    [SerializeField] private Button btnPlus;

    [SerializeField] private TMP_InputField inputRoomName;
    [SerializeField] private TMP_InputField inputRoomInfo;
    [SerializeField] private Button btnCreateRoom;


    void Start()
    {
        btnCreateRoom.onClick.AddListener(CreateRoom);
        btnMinus.onClick.AddListener(() => { SetMaxPlayers(--maxPlayers); });
        btnPlus.onClick.AddListener(() => { SetMaxPlayers(++maxPlayers); });
        txtNickname.text = $"어서오세요. {GameData.Instance.NickName} 님!";
        SetMaxPlayers(2);
    }

    void CreateRoom()
    {
        if (inputRoomName.text == string.Empty)
        {
            OpenUI<UIPopUpButton>().SetMessage(message: "생성할 방 제목을 입력하세요.", title: "방 만들기 실패");
            return;
        }

        UIManager.Instance.OnLoading();
        NetworkManager.Instance.CreateRoom(inputRoomName.text, inputRoomInfo.text, maxPlayers);
    }

    public void SetRoomList(Dictionary<string, RoomInfo> roomList)
    {
        ClearRoom();
        foreach (var roomData in roomList) { AddRoom().Setup(roomData.Value); }
    }

    UIRoomInfo AddRoom()
    {
        var go = ResourceManager.Instance.InstantiateUI<UIRoomInfo>("SubItem/" + nameof(UIRoomInfo), roomParent);
        objRoomList.Add(go.gameObject);
        return go;
    }

    void ClearRoom()
    {
        foreach (var item in objRoomList) { Destroy(item); }
        objRoomList.Clear();
    }

    void SetMaxPlayers(int value)
    {
        print(value);
        maxPlayers = value;
        SetButtonState(btnMinus, !(value <= 2));
        SetButtonState(btnPlus, !(value >= 10));
        txtMaxPlayers.text = maxPlayers.ToString();
    }

    void SetButtonState(Button button, bool enabled)
    {
        button.enabled = enabled;
        button.gameObject.GetComponent<Image>().color = enabled ? Color.white : Color.gray;
    }
}