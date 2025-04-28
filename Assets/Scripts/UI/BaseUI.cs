using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    protected virtual void Awake()
    {
        UIManager.Instance.RegisterUI(this);
    }

    protected virtual void OnDestroy()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterUI(this);
        }
    }

    public abstract UIState State { get; }
} 