using System;
using TMPro;
using UnityEngine;

public class ServerURLView : MonoBehaviour
{
    private TMP_InputField serverUrlInput;

    public string SuffixUploadInputVoiceApi;
    public string SuffixLoadBlendShapeApi;
    public string SuffixLoadAudioResponseApi;
    public string SuffixLoadDialogueContentApi;
    public string SuffixLoadChatHistory;

    [NonSerialized] public string ServerUrl; 

    private void Awake()
    {
        serverUrlInput = GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        serverUrlInput.onEndEdit.AddListener(OnServerUrlChanged);
    }

    private void OnDisable()
    {
        serverUrlInput.onEndEdit.RemoveListener(OnServerUrlChanged);
    }

    void OnServerUrlChanged(string url)
    {
        ServerUrl = url;
    }
}
