using UnityEngine;

public interface IPresenter
{
    void UpdatePresenter();
}

public abstract class BaseDigitalHumanPresenter : IPresenter
{
    protected BaseDigitalHumanPresenter()
    {
        if (PresentUpdater.Instance != null)
        {
            PresentUpdater.Instance.RegisterPresenter(this);
        }
        else
        {
            Debug.LogWarning("PresentUpdater.Instance is null. Presenter not registered.");
        }
    }

    public abstract void UpdatePresenter();
}