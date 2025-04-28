using UnityEngine;
using System.Collections.Generic;
using Redcode.Pools;

public class ItemManager : Singleton<ItemManager>
{
    [Header("아이템 설정")]
    [SerializeField] private int totalItemCount = 20; // 전체 아이템 수
    private List<Vector2Int> floorPositions = new List<Vector2Int>();
    private List<ItemBox> spawnedItems = new List<ItemBox>();
    private PoolManager poolManager;

    private void Start()
    {
        poolManager = GetComponent<PoolManager>();
        // 게임 재시작 이벤트 구독
        GameManager.Instance.OnGameRestart += OnGameRestart;
    }

    private void OnGameRestart()
    {
        // 생성된 아이템들 삭제
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                poolManager.TakeToPool<ItemBox>(item);
            }
        }
        spawnedItems.Clear();
        floorPositions.Clear();
    }

    public void Initialize(int[,] mapData)
    {
        CollectFloorPositions(mapData);
        SpawnItems(mapData);
    }

    private void CollectFloorPositions(int[,] mapData)
    {
        floorPositions.Clear();
        int rows = mapData.GetLength(0);
        int cols = mapData.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (mapData[r, c] == (int)TileType.Floor)
                {
                    floorPositions.Add(new Vector2Int(c - cols / 2, r - rows / 2));
                }
            }
        }
    }

    private void SpawnItems(int[,] mapData)
    {
        // 전체 아이템 수가 바닥 타일 수보다 많으면 조정
        int actualItemCount = Mathf.Min(totalItemCount, floorPositions.Count);
        
        // 바닥 위치들을 랜덤하게 섞기
        ShufflePositions(floorPositions);

        // 아이템 생성
        for (int i = 0; i < actualItemCount; i++)
        {
            Vector2Int pos = floorPositions[i];
            SpawnItemAt(pos);
        }
    }

    private void SpawnItemAt(Vector2Int position)
    {
        Vector3 worldPos = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
        ItemBox itemObj = poolManager.GetFromPool<ItemBox>();
        itemObj.transform.position = worldPos;
        spawnedItems.Add(itemObj);
    }

    private void ShufflePositions(List<Vector2Int> positions)
    {
        int n = positions.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (positions[k], positions[n]) = (positions[n], positions[k]);
        }
    }
} 