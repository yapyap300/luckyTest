using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : BaseUI
{
    [SerializeField] private Text finalScoreText;

    public override UIState State => UIState.GameOver;

    public void SetFinalScore(int score)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {score}";
        }
    }
} 