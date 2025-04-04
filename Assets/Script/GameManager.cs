using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float gameTime = 120f; // 2분
    
    private MakeMap mapGenerater;
    private int currentScore = 0;
    private float remainingTime;
    private bool isGameActive = false;

    protected override void Awake()
    {
        base.Awake();
        mapGenerater = GetComponent<MakeMap>();
    }
    void Start()
    {
        StartGame();
    }
    
    void Update()
    {
        if (isGameActive)
        {
            // 시간 감소
            remainingTime -= Time.deltaTime;
            UIManager.Instance.UpdateTimeUI(remainingTime);
            
            // 시간 종료 체크
            if (remainingTime <= 0)
            {
                EndGame();
            }
        }
    }
    
    public void StartGame()
    {
        // 게임 초기화
        currentScore = 0;
        remainingTime = gameTime;
        isGameActive = true;
        
        // 맵 생성
        mapGenerater.GenerateMap();
        
        // UI 초기화
        UIManager.Instance.UpdateScoreUI(currentScore);
        UIManager.Instance.UpdateTimeUI(remainingTime);
        UIManager.Instance.ShowGameUI(true);
    }
    
    public void EndGame()
    {
        isGameActive = false;
        
        // 결과 화면 표시
        UIManager.Instance.ShowGameOverUI(currentScore);
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UIManager.Instance.UpdateScoreUI(currentScore);
    }
}
