using System.Collections;
using UnityEngine;
using System;
using System.IO;

public class DigitalHumanPresenter : BaseDigitalHumanPresenter
{
    private readonly DigitalHumanView r_view;
    private readonly ServerURLView r_server;
    private readonly MonoBehaviour r_CoroutineManager;
    private readonly AudioSource r_audioSource;

    private readonly DigitalHumanModel r_model;

    private AudioClip recordedClip;
    private string recordedFilePath;
    private int maxRecordTime = 10;
    private bool isRecording = false;
    private bool isBlendShapeUpdating = false; 
    private float m_elapsed;

    public DigitalHumanPresenter(MonoBehaviour coroutineManager, DigitalHumanView view, ServerURLView serverView, AudioSource audioSource)
        : base()
    {
        r_CoroutineManager = coroutineManager;
        r_view = view;
        r_server = serverView;
        r_audioSource = audioSource;

        r_model = new DigitalHumanModel();
    }

    public override void UpdatePresenter()
    {
        UpdateBlendShape();
    }

    public void OnInputChatUploadSuccess()
    {
        r_CoroutineManager.StartCoroutine(LoadAllResponseData());
    }


    public void StartInputVoiceRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            throw new Exception("No microphone detected"); 
        }

        if (isRecording)
        {
            return;
        }

        recordedClip = Microphone.Start(null, false, maxRecordTime, 44100);
        isRecording = true;
        Debug.Log("Recording started...");
    }

    public void StopAndSaveInputVoiceRecording()
    {
        if (!isRecording) return;

        Microphone.End(null);
        isRecording = false;

        recordedFilePath = Path.Combine(Application.persistentDataPath, "recorded.wav");
        SaveWavFile(recordedClip, recordedFilePath);
        Debug.Log("Audio saved to: " + recordedFilePath);

        SendInputVoiceRecording(recordedFilePath);
    }

    private void SendInputVoiceRecording(string path)
    {
        r_CoroutineManager.StartCoroutine(r_model.UploadInputVoice(r_server.ServerUrl + r_server.SuffixUploadInputVoiceApi, 
        path, 
        OnUploadedInputVoice));
    }

    private void OnUploadedInputVoice(bool success)
    {
        if (!success) 
        {
            Debug.LogError("Warning");
        }

        r_CoroutineManager.StartCoroutine(LoadAllResponseData());
    }

    private IEnumerator LoadAllResponseData()
    {
        yield return r_CoroutineManager.StartCoroutine(r_model.LoadAudioClipResponseDataFromApi(r_server.ServerUrl + r_server.SuffixLoadAudioResponseApi));
        yield return r_CoroutineManager.StartCoroutine(r_model.LoadBlendShapeDataFromApi(r_server.ServerUrl + r_server.SuffixLoadBlendShapeApi));

        r_audioSource.clip = r_model.Voice;
        isBlendShapeUpdating = true;
        r_audioSource.Play();
        m_elapsed = 0;
    }

    private void UpdateBlendShape()
    {
        if (!isBlendShapeUpdating)
        {
            return;
        }
        
        m_elapsed += Time.deltaTime;
        if (m_elapsed >= r_model.EndTime)
        {
            ResetBlendShapes();
            isBlendShapeUpdating= false;
            m_elapsed = 0;
            return;
        }


        var frameIndex = Mathf.FloorToInt(m_elapsed * 30);
        var t = (m_elapsed - (frameIndex / 30f)) * 30;
        var frame = r_model.BlendShapeFrames[frameIndex];
        var nextFrame = r_model.BlendShapeFrames[frameIndex + 1];

        foreach (var kvp in frame)
        {
            foreach(SkinnedMeshRenderer skinnedMesh in r_view.MeshRenderers.Keys)
            {
                var keyInMeshRenderer = r_view.MeshRenderers[skinnedMesh] + kvp.Key;
                int index = skinnedMesh.sharedMesh.GetBlendShapeIndex(keyInMeshRenderer);
                if (index >= 0 && nextFrame.TryGetValue(kvp.Key, out float nextVal))
                {
                    float value = Mathf.Lerp(kvp.Value, nextVal, t);
                    skinnedMesh.SetBlendShapeWeight(index, value);
                }
            }
        }
    }

    private void ResetBlendShapes()
    {
        foreach(SkinnedMeshRenderer meshRenderer in r_view.MeshRenderers.Keys)
        {
            for (int i = 0; i < meshRenderer.sharedMesh.blendShapeCount; i++)
            {
                meshRenderer.SetBlendShapeWeight(i, 0);
            }
        } 
    }

    private void SaveWavFile(AudioClip clip, string path)
    {
        string fileName = Path.GetFileName(path);
        SaveWav.Save(fileName, clip);
    }
}
