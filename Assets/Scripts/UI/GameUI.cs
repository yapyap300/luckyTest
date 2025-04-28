using UnityEngine;
using UnityEngine.UI;

public class GameUI : BaseUI
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timeText;

    public override UIState State => UIState.Game;

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void UpdateTime(float time)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
} 