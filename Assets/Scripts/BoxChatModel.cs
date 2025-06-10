using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BoxChatModel 
{
    public Action<bool> onUploadChatComplete;

    public IEnumerator UploadChatInput(string serverURL, string content)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(content);

        using UnityWebRequest request = new UnityWebRequest(serverURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "text/plain");

        yield return request.SendWebRequest();
        Debug.Log("sended");
        if (request.result == UnityWebRequest.Result.Success)
        {
            onUploadChatComplete.Invoke(true);
        }
        else
        {
            onUploadChatComplete.Invoke(false); 
        }
    }

}
