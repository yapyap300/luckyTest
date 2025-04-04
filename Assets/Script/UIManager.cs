using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private GameObject gameUI;

    private void Start()
    {
        // 초기 UI 상태 설정
        ShowGameUI(false);
        ShowGameOverUI(0);
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void UpdateTimeUI(float time)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void ShowGameUI(bool show)
    {
        if (gameUI != null)
        {
            gameUI.SetActive(show);
        }
    }

    public void ShowGameOverUI(int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {finalScore}";
            }
        }
    }

    // 게임 재시작 버튼용 메서드
    public void OnRestartButtonClicked()
    {
        gameOverPanel.SetActive(false);
        GameManager.Instance.StartGame();
    }
} 