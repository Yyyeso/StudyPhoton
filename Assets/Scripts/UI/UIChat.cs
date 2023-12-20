using TMPro;
using UnityEngine;

public class UIChat : UIBase
{
    public void SetMessage(string nickname, string message)
    {
        string color = (nickname == GameData.Instance.NickName) ? "<color=#FFCE53>" : "<color=#5DF4FF>";
        GetComponent<TMP_Text>().text = $"{color}{nickname}</color>: {message}";
    }

    public void SetNotice(Color color, string message)
    {
        var chat = GetComponent<TMP_Text>();
        chat.color = color;
        chat.text = message;
    }
}