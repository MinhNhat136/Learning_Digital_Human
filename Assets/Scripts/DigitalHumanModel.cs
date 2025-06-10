using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using System.IO;

public class DigitalHumanModel
{
    public List<Dictionary<string, float>> BlendShapeFrames { get; private set; } = new();
    public AudioClip Voice { get; private set; }
    public string DialogueContent { get; private set; }
    public float EndTime { get; private set; }

    public IEnumerator UploadInputVoice(string serverURL, string filePath, Action<bool> onComplete)
    {
        byte[] audioData;

        try
        {
            audioData = File.ReadAllBytes(filePath);
        }
        catch (Exception ex)
        {
            throw new Exception("Error reading audio file: " + ex.Message);
        }

        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", audioData, "input.wav", "audio/wav");

        using UnityWebRequest request = UnityWebRequest.Post(serverURL, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload succeeded.");
            onComplete?.Invoke(true);
        }
        else
        {
            Debug.LogError("Upload failed: " + request.error);
            onComplete?.Invoke(false);
        }
    }

    public IEnumerator LoadBlendShapeDataFromApi(string apiUrl)
    {
        using var www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) 
            throw new Exception("Failed to load blendshape data: " + www.error);

        string csvText = www.downloadHandler.text;
        var (frames, endTime) = ParseCsv(csvText);
        BlendShapeFrames = frames;
        EndTime = endTime; 
        Debug.Log(EndTime);
    }

    public IEnumerator LoadAudioClipResponseDataFromApi(string apiUrl)
    {
        using var www = UnityWebRequestMultimedia.GetAudioClip(apiUrl, AudioType.WAV);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load audio: " + www.error);
            yield break;
        }

        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
        Voice = clip;
    }

    public IEnumerator LoadDialogueContent(string apiUrl)
    {
        using var www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            throw new Exception("Failed to load dialogue data: " + www.error);

        string content = www.downloadHandler.text;
        DialogueContent = content;
    }

    private (List<Dictionary<string, float>> frames, float endTime) ParseCsv(string csvText)
    {
        var lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var headers = lines[0].Split(',');
        var originalHeaders = new List<string>(headers);
        float endTime = float.Parse(lines[^1].Split(',')[1]);

        var frames = new List<Dictionary<string, float>>();

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            var frameDict = new Dictionary<string, float>();

            for (int j = 2; j < values.Length; j++)
            {
                string originalName = originalHeaders[j];
                string blendName = originalName.Replace("blendShapes.", "").Trim();
                blendName = char.ToLower(blendName[0]) + blendName.Substring(1);

                if (float.TryParse(values[j], out float val))
                    frameDict[blendName] = val * 100f;
            }

            frames.Add(frameDict);
        }

        return (frames, endTime);
    }
}
