using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIntro : UIBase
{
    [SerializeField] private TMP_InputField inputNickName;
    [SerializeField] private Button btnConnect;


    private void Start() => btnConnect.onClick.AddListener(Connect);

    void Connect()
    {
        if (inputNickName.text == "" || inputNickName.text == string.Empty)
        {
            OpenUI<UIPopUpButton>().SetMessage(message: "닉네임을 입력하세요.", title: "로비 입장 실패");
            return;
        }

        UIManager.Instance.OnLoading();
        NetworkManager.Instance.Connect(inputNickName.text);
    }
}