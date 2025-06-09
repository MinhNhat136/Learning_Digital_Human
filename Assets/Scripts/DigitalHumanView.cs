using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DigitalHumanView : SerializedMonoBehaviour
{
    public Dictionary<SkinnedMeshRenderer, string> MeshRenderers;
    private DigitalHumanPresenter m_presenter;

    void Start()
    {
        m_presenter = new DigitalHumanPresenter(this, this, FindAnyObjectByType<ServerURLView>(), GetComponent<AudioSource>());
    }
}
