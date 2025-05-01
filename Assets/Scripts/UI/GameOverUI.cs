using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : BaseUI
{
    [SerializeField] private Text finalScoreText;

    public override UIState State => UIState.GameOver;

    protected override void Cleanup()
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {GameManager.Instance.score}";
        }
    }

    protected override void Initialize()
    {
        throw new System.NotImplementedException();
    }
} 