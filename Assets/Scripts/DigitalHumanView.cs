using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DigitalHumanView : SerializedMonoBehaviour
{
    public Dictionary<SkinnedMeshRenderer, string> MeshRenderers;
    private DigitalHumanPresenter m_presenter;

    private Action m_onInputChatUploadSuccess; 

    void Start()
    {
        m_presenter = new DigitalHumanPresenter(this, this, FindAnyObjectByType<ServerURLView>(), GetComponent<AudioSource>());
        m_onInputChatUploadSuccess += m_presenter.OnInputChatUploadSuccess; 
    }

    public void ReceiveSignalOfUploadSuccessInputChat()
    {
        m_onInputChatUploadSuccess?.Invoke();
    }
}
