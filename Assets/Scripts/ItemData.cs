using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemData", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [System.Serializable]
    public class ItemDrop
    {
        public string itemName;
        public string description;
        public Sprite icon;
        public float weight; // 드롭 가중치
        public int baseScore; // 기본 점수
        public float scoreMultiplier; // 점수 배율 (1.0 ~ 2.0)
        
        // 최종 점수 계산
        public int GetFinalScore()
        {
            return Mathf.RoundToInt(baseScore * scoreMultiplier);
        }
    }

    public List<ItemDrop> commonItems = new List<ItemDrop>();
    public List<ItemDrop> rareItems = new List<ItemDrop>();
    public List<ItemDrop> epicItems = new List<ItemDrop>();
    public List<ItemDrop> legendaryItems = new List<ItemDrop>();
} 