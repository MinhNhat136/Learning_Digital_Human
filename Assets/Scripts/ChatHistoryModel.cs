using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text;

public class ChatHistoryModel
{
    public List<ChatEntry> ChatEntries { get; private set; } = new();

    public void AddIntroduction()
    {
        ChatEntries.Add(new ChatEntry("", "", "Xin chào! Tôi là Nova – một nữ trợ lý kỹ thuật số vô cùng  thân thiện, được tạo ra bởi MinhNhat. Tôi được thiết kế để hỗ trợ bạn trong các vấn đề liên quan đến luật đất đai. Nếu bạn có bất kỳ thắc mắc nào về pháp lý, hãy thoải mái đặt câu hỏi. Tôi luôn sẵn sàng giúp đỡ!"));
    }

    public IEnumerator LoadChatHistory(string apiUrl, Action<bool> onComplete)
    {
        AddIntroduction();

        using var request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load chat history: " + request.error);
            onComplete?.Invoke(false);
            yield break;
        }

        string jsonlText = request.downloadHandler.text;
        string[] lines = jsonlText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < lines.Length; i++)
        {
            sb.Append(lines[i]);
            if (i < lines.Length - 1)
                sb.Append(",");
        }

        Debug.Log(sb.ToString());

        string jsonArray = sb.ToString();

        List<ChatEntry> chatData;
        try
        {
            chatData = JsonUtilityWrapper.FromJsonList<ChatEntry>(jsonArray);
        }
        catch (Exception e)
        {
            Debug.LogError("JSON parse error: " + e.Message);
            onComplete?.Invoke(false);
            yield break;
        }

        if (chatData != null && chatData.Count > 0)
        {
            ChatEntries.AddRange(chatData);
            Debug.Log("Chat entries loaded: " + chatData.Count);
            onComplete?.Invoke(true);
        }
        else
        {
            onComplete?.Invoke(false);
        }
    }


    public IEnumerator ResetChatHistory(string apiUrl, System.Action<bool> onComplete)
    {
        UnityWebRequest request = UnityWebRequest.Delete(apiUrl);
        yield return request.SendWebRequest();

        onComplete?.Invoke(request.result == UnityWebRequest.Result.Success);
    }
}

[System.Serializable]
public struct ChatEntry
{
    public string timestamp;
    public string user_asr;
    public string ai_response;

    public ChatEntry(string time, string user_text, string ai_text)
    {
        timestamp = time;
        user_asr = user_text;
        ai_response = ai_text;
    }
}