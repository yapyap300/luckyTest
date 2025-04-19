using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float gameTime = 120f; // 2분
    
    [Header("랜덤 시드 설정")]
    public string seed;
    
    private int currentScore = 0;
    private float remainingTime;
    private bool isGameActive = false;

    public bool IsGameActive => isGameActive;

    void Start()
    {
        // 초기화만 수행
        currentScore = 0;
        remainingTime = gameTime;
        isGameActive = false;
    }
    
    // void Update()
    // {
    //     if (isGameActive)
    //     {
    //         // 시간 감소
    //         remainingTime -= Time.deltaTime;
    //         UIManager.Instance.UpdateTimeUI(remainingTime);
            
    //         // 시간 종료 체크
    //         if (remainingTime <= 0)
    //         {
    //             EndGame();
    //         }
    //     }
    // }
    
    // 랜덤 시드 생성
    private void GenerateRandomSeed()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] stringChars = new char[10];
        
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Random.Range(0, chars.Length)];
        }
        
        seed = new string(stringChars);
    }
    
    public void StartGame()
    {
        // 시드가 비어있으면 자동 생성
        if (string.IsNullOrEmpty(seed))
        {
            GenerateRandomSeed();
        }
        // 시드 적용
        Random.InitState(seed.GetHashCode());
        
        // 게임 초기화
        currentScore = 0;
        remainingTime = gameTime;
        isGameActive = true;
        
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
