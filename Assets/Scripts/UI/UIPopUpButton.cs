using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPopUpButton : UIRefreshBase
{
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtMessage;
    [SerializeField] private GameObject empty;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnCancel;

    Action _confirmAction;
    Action _cancelAction;


    void Start()
    {
        btnConfirm.onClick.AddListener(ConfirmAction);
        btnCancel.onClick.AddListener(CancelAction);
    }

    void ConfirmAction()
    {
        CloseUI();
        _confirmAction?.Invoke();
    }

    void CancelAction()
    {
        CloseUI();
        _cancelAction?.Invoke();
    }

    void ResetPopUp()
    {
        _confirmAction = null;
        _cancelAction = null;
        btnCancel.gameObject.SetActive(false);
    }

    void SetTitle(string title)
    {
        txtTitle.gameObject.SetActive(title != "");
        empty.SetActive(title == "");
        if (title != "") { txtTitle.text = title; }
    }

    public UIPopUpButton SetMessage(string message, string title = "")
    {
        ResetPopUp();
        SetTitle(title);
        txtMessage.text = message;
        RefreshUI();
        return this;
    }

    public UIPopUpButton AddConfirmAction(Action action)
    {
        _confirmAction = action;
        return this;
    }

    public UIPopUpButton AddCancelAction(Action action)
    {
        btnCancel.gameObject.SetActive(true);
        RefreshUI();
        _cancelAction = action;
        return this;
    }
}