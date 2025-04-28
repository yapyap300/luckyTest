using UnityEngine;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public float gameTime = 120f; // 2분
    
    [Header("랜덤 시드 설정")]
    public string seed;
    private List<ItemBox> collectedItems = new List<ItemBox>();
    public bool IsGameActive { get; private set; }
    public event System.Action OnGameRestart;
    void Start()
    {        
        IsGameActive = false;
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
        IsGameActive = true;
        collectedItems.Clear();
        
        // UI 초기화
        UIManager.Instance.ShowUI(UIState.Game);
        UIManager.Instance.ShowUI(UIState.Game);
        UIManager.Instance.ShowUI(UIState.Game);
    }
    
    public void EndGame()
    {
        IsGameActive = false;
        
        // 결과 화면 표시
        UIManager.Instance.ShowUI(UIState.GameOver);
    }

    public void AddItem(ItemBox item)
    {
        if (item != null)
        {
            collectedItems.Add(item);
        }
    }

    public void RestartGame()
    {
        // 게임 상태 초기화
        IsGameActive = false;
        seed = string.Empty;
        collectedItems.Clear();
        OnGameRestart?.Invoke();

        UIManager.Instance.ShowUI(UIState.Start);
    }
}
