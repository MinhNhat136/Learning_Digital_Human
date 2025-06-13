using System.Collections.Generic;
using UnityEngine;

public class PresentUpdater : MonoBehaviour
{
    public static PresentUpdater Instance { get; private set; }

    private readonly List<IPresenter> m_presenters = new();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterPresenter(IPresenter presenter)
    {
        if (!m_presenters.Contains(presenter))
        {
            m_presenters.Add(presenter);
        }
    }

    public void UnregisterPresenter(IPresenter presenter)
    {
        if (m_presenters.Contains(presenter))
        {
            m_presenters.Remove(presenter);
        }
    }

    void LateUpdate()
    {
        foreach (var presenter in m_presenters)
        {
            presenter.UpdatePresenter();
        }
    }
}
