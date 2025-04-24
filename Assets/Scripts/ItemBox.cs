using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [Header("아이템 설정")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color commonColor = Color.white;
    [SerializeField] private Color rareColor = Color.blue;
    [SerializeField] private Color epicColor = Color.magenta;
    [SerializeField] private Color legendaryColor = Color.yellow;

    public ItemRarity Rarity { get; private set; }
    public int ScoreValue => GetScoreByRarity(Rarity);

    public void Initialize(ItemRarity rarity)
    {
        Rarity = rarity;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = GetColorByRarity(Rarity);
        }
    }

    private Color GetColorByRarity(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => commonColor,
            ItemRarity.Rare => rareColor,
            ItemRarity.Epic => epicColor,
            ItemRarity.Legendary => legendaryColor,
            _ => commonColor
        };
    }

    private int GetScoreByRarity(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => 10,
            ItemRarity.Rare => 30,
            ItemRarity.Epic => 100,
            ItemRarity.Legendary => 300,
            _ => 10
        };
    }
} 