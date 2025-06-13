using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatHistoryPresenter
{
    private readonly ChatHistoryModel r_model;
    private readonly ChatHistoryView r_view;
    private readonly ServerURLView r_server;
    private readonly MonoBehaviour r_host;

    public ChatHistoryPresenter(ChatHistoryView view, ServerURLView serverView, MonoBehaviour host)
    {
        r_model = new();
        r_view = view;
        r_server = serverView;
        r_host = host;

        view.onLoadHistory += LoadHistory;

    }

    public void LoadHistory()
    {
        r_host.StartCoroutine(r_model.LoadChatHistory(r_server.ServerUrl + r_server.SuffixLoadChatHistory, OnHistoryLoaded));
    }

    public void DeleteHistory(string apiUrl)
    {
        r_host.StartCoroutine(r_model.ResetChatHistory(apiUrl, success =>
        {
            r_model.AddIntroduction();
            if (success)
                r_view.ClearHistoryUI();
                r_view.DisplayChatHistory(r_model.ChatEntries);
        }));
    }

    private void OnHistoryLoaded(bool success)
    {
        if (!success)
        {
            Debug.LogError(" Error load chat");
        }
        
        r_view.DisplayChatHistory(r_model.ChatEntries);
    }
}
