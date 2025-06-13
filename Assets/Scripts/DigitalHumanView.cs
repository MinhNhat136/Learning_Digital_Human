using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DigitalHumanView : SerializedMonoBehaviour
{
    public Dictionary<SkinnedMeshRenderer, string> MeshRenderers;
    private DigitalHumanPresenter m_presenter;

    private AudioToolBarView m_toolBarView; 
    private Action m_onInputChatUploadSuccess;
    private ChatHistoryView m_chatHistoryView;

    private bool m_isRecording = false;

    private void Awake()
    {
        m_toolBarView = FindAnyObjectByType<AudioToolBarView>();
        m_chatHistoryView = FindAnyObjectByType<ChatHistoryView>();
    }

    private void Start()
    {
        m_presenter = new DigitalHumanPresenter(this, this, FindAnyObjectByType<ServerURLView>(), GetComponentInParent<AudioSource>());
        m_onInputChatUploadSuccess += m_presenter.OnInputChatUploadSuccess; 
    }

    private void OnEnable()
    {
        m_toolBarView.onClickRecordButton += () => m_isRecording = !m_isRecording; 
    }

    public void ReceiveSignalOfUploadSuccessInputChat()
    {
        m_onInputChatUploadSuccess.Invoke();
    }

    private void Update()
    {
        if (m_isRecording)
        {
            m_presenter.StartInputVoiceRecording();
        }
        else
        {
            m_presenter.StopAndSaveInputVoiceRecording();
        }
    }

    public void NotifyHistory()
    {
        m_chatHistoryView.ReceiveSignal();
    }
}
