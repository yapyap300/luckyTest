using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    protected virtual void Awake()
    {
        UIManager.Instance.RegisterUI(this);
    }

    protected virtual void OnEnable()
    {
        Initialize();
    }

    protected virtual void OnDisable()
    {
        Cleanup();
    }

    protected virtual void OnDestroy()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterUI(this);
        }
    }

    // UI 초기화를 위한 추상 메서드
    protected abstract void Initialize();
    // UI 정리를 위한 추상 메서드
    protected abstract void Cleanup();

    public abstract UIState State { get; }
} 