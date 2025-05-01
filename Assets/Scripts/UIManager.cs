using UnityEngine;
using System.Collections.Generic;
using System;

public enum UIState
{
    Game,
    GameOver,
    Start,
    Collect
}

public class UIManager : Singleton<UIManager>
{
    private Dictionary<UIState, BaseUI> uiPanels = new Dictionary<UIState, BaseUI>();

    private void Start()
    {
        ShowUI(UIState.Start);
    }

    public void RegisterUI(BaseUI ui)
    {
        if (ui != null)
        {
            uiPanels[ui.State] = ui;
            ui.gameObject.SetActive(false);
        }
    }

    public void UnregisterUI(BaseUI ui)
    {
        if (ui != null)
        {
            uiPanels.Remove(ui.State);
        }
    }

    public void ShowUI(UIState state)
    {
        if (uiPanels.ContainsKey(state))
        {
            uiPanels[state].gameObject.SetActive(true);
        }
    }
    public void ShowUI(UIState state, Vector3 worldPosition)
    {
        if (uiPanels.ContainsKey(state))
        {
            uiPanels[state].transform.position = Camera.main.WorldToScreenPoint(worldPosition);
            uiPanels[state].gameObject.SetActive(true);
        }
    }
    public void HideUI(UIState state)
    {
        if (uiPanels.ContainsKey(state))
        {
            uiPanels[state].gameObject.SetActive(false);
        }
    }

}