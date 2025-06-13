using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxChatView : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_chatInput;
    [SerializeField] private Button m_sendButton;
    [SerializeField] private DigitalHumanView m_humanView;
    [SerializeField] private ChatHistoryView m_chatHistoryView;

    private string m_chatContent;
    private Action<string> r_onClickedSendButton;


    private void Awake()
    {
        BoxChatPresenter presenter = new(this, FindAnyObjectByType<ServerURLView>(), this);
        r_onClickedSendButton += presenter.OnClickedSendButton;

        m_humanView = FindAnyObjectByType<DigitalHumanView>(); 
        m_chatHistoryView = FindAnyObjectByType<ChatHistoryView>();
    }

    private void OnEnable()
    {
        m_chatInput.onEndEdit.AddListener(OnEndEditChatContent);
        m_sendButton.onClick.AddListener(OnClickSendButton);
    }

    private void OnDisable()
    {
        m_chatInput.onEndEdit.RemoveListener(OnEndEditChatContent);
        m_sendButton.onClick.RemoveAllListeners();
    }

    private void OnEndEditChatContent(string content)
    {
        m_chatContent = content;
    }

    private void OnClickSendButton()
    {
        if(m_chatContent == "")
        {
            Debug.LogError("no content");
            return;
        }
        r_onClickedSendButton.Invoke(m_chatContent);
        m_chatInput.text = "";
    }

    public void NotifyToBoxAndHuman()
    {
        m_humanView.ReceiveSignalOfUploadSuccessInputChat();
        m_chatHistoryView.ReceiveSignal();
    }
}
