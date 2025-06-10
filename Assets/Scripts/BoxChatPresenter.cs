using UnityEngine;

public class BoxChatPresenter 
{
    private readonly BoxChatView r_view;
    private readonly ServerURLView r_server;
    private readonly BoxChatModel r_model;
    private readonly MonoBehaviour r_host;

    public BoxChatPresenter(BoxChatView view, ServerURLView serverView, MonoBehaviour host)
    {
        r_view = view;
        r_server = serverView;
        r_model = new BoxChatModel();
        r_host = host;

        r_model.onUploadChatComplete += OnUploadChatComplete;
    }

    public void OnClickedSendButton(string content)
    {
        r_host.StartCoroutine(r_model.UploadChatInput(r_server.ServerUrl + r_server.SuffixUploadInputTextApi, content));
    }

    public void OnUploadChatComplete(bool success)
    {
        if (!success)
        {
            Debug.LogError("Fail Upload Chat Text");
            return;
        }
        Debug.Log("notify");
        r_view.NotifyToBoxAndHuman();
    }
}
