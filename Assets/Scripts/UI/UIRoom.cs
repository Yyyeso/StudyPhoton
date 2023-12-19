using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRoom : UIRefreshBase
{
    [SerializeField] private TMP_Text txtCount;
    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnSend;
    [SerializeField] private TMP_InputField inputChat;
    [SerializeField] private RectTransform chatParent;
    List<GameObject> chatLog = new();

    int capacity = 10;
    GameData data;

    private void Start()
    {
        btnSend.onClick.AddListener(SendChat);
        btnExit.onClick.AddListener(ExitRoom);
    }

    public UIRoom Setup(int curCount, int capacity)
    {
        data = GameData.Instance;
        this.capacity = capacity;
        UpdateCount(curCount);

        return this;
    }
    void SendChat()
    {
        if (inputChat.text == string.Empty)
        {
            OpenUI<UIPopUpButton>().SetMessage("메시지를 입력하세요.");
            return;
        }

        string message = inputChat.text;
        inputChat.text = string.Empty;

        data.Player.PV.RPC(nameof(data.Player.UpdateChatLog), RpcTarget.All, data.NickName, message);
    }
    void ExitRoom()
    {
        foreach (var item in chatLog)
        { Destroy(item); }
        chatLog.Clear();

        inputChat.text = string.Empty;
        CloseUI();
        NetworkManager.Instance.LeaveRoom();
    }

    public void UpdateChatLog(string nickname, string message)
    {
        NewChat.SetMessage(nickname, message);
        RefreshUI();
    }

    public void AddMember(string nickname)
    {
        NewChat.SetNotice(Color.green, $"{nickname}님이 입장하셨습니다.");
        RefreshUI();
    }

    public void RemoveMember(string nickname)
    {
        NewChat.SetNotice(Color.magenta, $"{nickname}님이 퇴장하셨습니다.");
        RefreshUI();
    }

    public void UpdateCount(int curCount)
    {
        txtCount.text = $"참가 인원 ({curCount}/{capacity})";
    }

    UIChat NewChat
    {
        get 
        { 
            var chat = ResourceManager.Instance.InstantiateUI<UIChat>("SubItem/UIChat", chatParent);
            chatLog.Add(chat.gameObject);
            return chat;
        }
    }
}
