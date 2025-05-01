using UnityEngine;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public float gameTime = 120f; // 2분
    public int score = 0;
    [Header("랜덤 시드 설정")]
    public string seed;
    private List<ItemBox> collectedItems = new List<ItemBox>();
    public bool IsGameActive { get; private set; }
    public event System.Action OnGameRestart;

    [SerializeField] private ItemData itemData;


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
        
        UIManager.Instance.HideUI(UIState.Start);
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

    public ItemData.ItemDrop GetRandomItemByRarity(ItemRarity rarity)
    {
        List<ItemData.ItemDrop> targetItems = rarity switch
        {
            ItemRarity.Common => itemData.commonItems,
            ItemRarity.Rare => itemData.rareItems,
            ItemRarity.Epic => itemData.epicItems,
            ItemRarity.Legendary => itemData.legendaryItems,
            _ => itemData.commonItems
        };

        return GetRandomItemByWeight(targetItems);
    }

    private ItemData.ItemDrop GetRandomItemByWeight(List<ItemData.ItemDrop> items)
    {
        float totalWeight = 0f;
        foreach (var item in items)
        {
            totalWeight += item.weight;
        }

        float randomPoint = Random.value * totalWeight;
        for (int i = 0; i < items.Count; i++)
        {
            if (randomPoint < items[i].weight)
            {
                return items[i];
            }
            randomPoint -= items[i].weight;
        }
        return items[items.Count - 1];
    }
}
