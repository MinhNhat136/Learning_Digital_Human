using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatHistoryView : MonoBehaviour
{
    public Transform chatContentParent;
    public GameObject chatFramePrefab;

    public Action onLoadHistory;


    private void Awake()
    {
        ChatHistoryPresenter chatHistoryPresenter = new(this, FindAnyObjectByType<ServerURLView>(), this);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Delete))
        {
            onLoadHistory.Invoke();
        }
    }

    public void DisplayChatHistory(List<ChatEntry> entries)
    {
        ClearHistoryUI();
        if (entries.Count <= 0)
            return;

        foreach (var entry in entries)
        {
            if (entry.user_asr != "") 
            { 
                GameObject frameUser = Instantiate(chatFramePrefab, chatContentParent);
                var uiUser = frameUser.GetComponent<ChatFrame>();
                uiUser.Setup("User", entry.user_asr);
                frameUser.SetActive(true);
            }

            GameObject frameDH = Instantiate(chatFramePrefab, chatContentParent);
            var uiDH = frameDH.GetComponent<ChatFrame>();
            uiDH.Setup("Nova", entry.ai_response);
            frameDH.SetActive(true);
        }
    }

    public void ClearHistoryUI()
    {
        foreach (Transform child in chatContentParent)
            Destroy(child.gameObject);
    }
}
