using UnityEngine;
using System.Collections.Generic;

public class ItemManager : Singleton<ItemManager>
{
    [Header("아이템 설정")]
    [SerializeField] private int totalItemCount = 20; // 전체 아이템 수
    [SerializeField] private GameObject itemBoxPrefab;
    
    private int[,] map;
    private List<Vector2Int> floorPositions = new List<Vector2Int>();
    private List<ItemBox> spawnedItems = new List<ItemBox>();

    public void Initialize(int[,] mapData)
    {
        map = mapData;
        CollectFloorPositions();
        SpawnItems();
    }

    private void CollectFloorPositions()
    {
        floorPositions.Clear();
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (map[r, c] == (int)TileType.Floor)
                {
                    floorPositions.Add(new Vector2Int(r, c));
                }
            }
        }
    }

    private void SpawnItems()
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
        Vector3 worldPos = new Vector3(position.y, position.x, 0);
        GameObject itemObj = Instantiate(itemBoxPrefab, worldPos, Quaternion.identity);
        ItemBox itemBox = itemObj.GetComponent<ItemBox>();
        
        if (itemBox != null)
        {
            itemBox.Initialize(GetRandomRarity());
            spawnedItems.Add(itemBox);
        }
    }

    private ItemRarity GetRandomRarity()
    {
        float random = Random.value;
        
        if (random < 0.6f) return ItemRarity.Common;
        if (random < 0.85f) return ItemRarity.Rare;
        if (random < 0.95f) return ItemRarity.Epic;
        return ItemRarity.Legendary;
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