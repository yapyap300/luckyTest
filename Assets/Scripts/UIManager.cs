using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("UI References")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timeText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text finalScoreText;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private Text seedText;
    
    [Header("시드 입력 UI")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private InputField seedInputField;
    [SerializeField] private Button startButton;
    
    [Header("시드 설정")]
    [SerializeField] private int seedLength = 10;
    [SerializeField] private string validCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private void Start()
    {
        // 초기 UI 상태 설정
        ShowGameUI(false);
        ShowGameOverUI(0);
        ShowStartPanel(true);
        
        // 시작 버튼 이벤트 설정
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        
        // 입력 필드 초기화
        if (seedInputField != null)
        {
            seedInputField.placeholder.GetComponent<Text>().text = $"시드값을 입력하세요 ({seedLength}자리)";
        }
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
    
    public void ShowStartPanel(bool show)
    {
        if (startPanel != null)
        {
            startPanel.SetActive(show);
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
    
    // 시드값 유효성 검사
    private bool IsValidSeed(string seed)
    {
        if (string.IsNullOrEmpty(seed)) return true; // 빈 값은 자동 생성되므로 유효
        
        // 길이 체크
        if (seed.Length != seedLength) return false;
        
        // 문자 유효성 체크
        foreach (char c in seed)
        {
            if (!validCharacters.Contains(c.ToString()))
            {
                return false;
            }
        }
        
        return true;
    }

    // 시작 버튼 클릭 시 호출되는 메서드
    private void OnStartButtonClicked()
    {
        string inputSeed = seedInputField.text;
        
        // 시드값 유효성 검사
        if (!IsValidSeed(inputSeed))
        {
            // 유효하지 않은 경우 에러 메시지 표시
            seedInputField.text = "";
            seedInputField.placeholder.GetComponent<Text>().text = "잘못된 시드값입니다. 다시 입력해주세요.";
            return;
        }
        
        // 시드값 설정
        if (!string.IsNullOrEmpty(inputSeed))
        {
            GameManager.Instance.seed = inputSeed;
        }
        
        // 시작 패널 숨기기
        ShowStartPanel(false);
        
        // 게임 시작
        GameManager.Instance.StartGame();
        seedText.text = GameManager.Instance.seed;
    }

    // 게임 재시작 버튼용 메서드
    public void OnRestartButtonClicked()
    {
        gameOverPanel.SetActive(false);
        ShowStartPanel(true);
        
        // 입력 필드 초기화
        if (seedInputField != null)
        {
            seedInputField.text = "";
            seedInputField.placeholder.GetComponent<Text>().text = $"시드값을 입력하세요 ({seedLength}자리)";
        }
    }
} 