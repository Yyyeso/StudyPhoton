using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIRoom : UIRefreshBase
{
    int maxPlayers;
    [SerializeField] private TMP_Text txtCount;
    [SerializeField] private TMP_Text txtMember;

    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnSend;

    UIChat NewChat
    {
        get
        {
            var chat = ResourceManager.Instance.InstantiateUI<UIChat>("SubItem/UIChat", chatParent);
            chatLog.Add(chat.gameObject);
            if (chatLog.Count >= 30) chatLog.RemoveAt(0);
            return chat;
        }
    }
    List<GameObject> chatLog = new();
    [SerializeField] private Color enterColor;
    [SerializeField] private Color leaveColor;
    [SerializeField] private TMP_InputField inputChat;
    [SerializeField] private RectTransform chatParent;

    GameData data = null;


    private void Start()
    {
        btnSend.onClick.AddListener(SendChat);
        btnExit.onClick.AddListener(ExitRoom);
    }

    void SendChat()
    {
        if (inputChat.text == string.Empty)
        {
            OpenUI<UIPopUpButton>().SetMessage(message: "메시지를 입력하세요.");
            return;
        }

        string message = inputChat.text;
        inputChat.text = string.Empty;
        data.Player.PV.RPC(nameof(data.Player.UpdateChatLog), RpcTarget.All, data.NickName, message);
    }

    void ExitRoom()
    {
        UIManager.Instance.OnLoading();
        ClearChat();
        CloseUI();
        NetworkManager.Instance.LeaveRoom();
    }

    public void Setup(int maxPlayers, int playerCount, string memberList)
    {
        if (data == null) { data = GameData.Instance; }
        this.maxPlayers = maxPlayers;
        UpdateMemberList(playerCount, memberList);
    }

    public void UpdateMemberList(int curCount, string memberList)
    {
        txtCount.text = $"참가 인원 ({curCount}/{maxPlayers})";
        txtMember.text = memberList;
    }

    public void UpdateChatLog(string nickname, string message)
    {
        NewChat.SetMessage(nickname, message);
        RefreshUI();
    }

    public void NoticeOnPlayerEntered(string nickname)
    {
        NewChat.SetNotice(enterColor, $"{nickname} 님이 입장하셨습니다.");
        RefreshUI();
    }

    public void NoticeOnPlayerLeft(string nickname)
    {
        NewChat.SetNotice(leaveColor, $"{nickname} 님이 퇴장하셨습니다.");
        RefreshUI();
    }

    void ClearChat()
    {
        inputChat.text = string.Empty;
        foreach (var item in chatLog) { Destroy(item); }
        chatLog.Clear();
    }
}