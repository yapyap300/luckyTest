using UnityEngine;

public class CollectUI : BaseUI
{
    public override UIState State => UIState.Collect;

    public void SetPosition(Vector3 worldPosition)
    {
        transform.position = Camera.main.WorldToScreenPoint(worldPosition);
    }
} 