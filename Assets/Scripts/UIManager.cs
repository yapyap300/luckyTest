using UnityEngine;
using System.Collections.Generic;

public enum UIState
{
    None,
    Game,
    GameOver,
    Start,
    Collect
}

public class UIManager : Singleton<UIManager>
{
    private UIState currentState = UIState.None;
    private Dictionary<UIState, BaseUI> uiPanels = new Dictionary<UIState, BaseUI>();
    private Stack<UIState> uiStack = new Stack<UIState>();

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
        if (currentState != UIState.None)
        {
            uiPanels[currentState].gameObject.SetActive(false);
        }
        
        currentState = state;
        if (state != UIState.None)
        {
            uiPanels[state].gameObject.SetActive(true);
        }
    }

    public void PushUI(UIState state)
    {
        if (currentState != UIState.None)
        {
            uiPanels[currentState].gameObject.SetActive(false);
        }
        uiStack.Push(currentState);
        ShowUI(state);
    }

    public void PopUI()
    {
        if (uiStack.Count > 0)
        {
            if (currentState != UIState.None)
            {
                uiPanels[currentState].gameObject.SetActive(false);
            }
            ShowUI(uiStack.Pop());
            if(uiStack.Count == 0) currentState = UIState.None;
        }
    }
} 