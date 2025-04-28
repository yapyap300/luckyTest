using Redcode.Pools;
using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour, IPoolObject
{
    [Header("아이템 설정")]
    [SerializeField] private Collider2D itemCollider;
    [SerializeField] private float collectibleDuration = 3f; // 획득 가능 상태 지속 시간

    private Coroutine collectibleCoroutine;

    public ItemRarity Rarity { get; private set; }
    public int ScoreValue => GetScoreByRarity(Rarity);
    public bool IsCollectible { get; private set; }

    public void Initialize(ItemRarity rarity)
    {
        Rarity = rarity;
        IsCollectible = false;
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

    public void OnCreatedInPool()
    {
        if (itemCollider == null)
        {
            itemCollider = GetComponent<Collider2D>();
        }
    }

    public void OnGettingFromPool()
    {
        gameObject.SetActive(true);
        Initialize(GetRandomRarity());
    }

    private ItemRarity GetRandomRarity()
    {
        float random = Random.value;
        
        if (random < 0.6f) return ItemRarity.Common;
        if (random < 0.85f) return ItemRarity.Rare;
        if (random < 0.95f) return ItemRarity.Epic;
        return ItemRarity.Legendary;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wave"))
        {
            if (collectibleCoroutine != null)
            {
                StopCoroutine(collectibleCoroutine);
            }
            collectibleCoroutine = StartCoroutine(CollectibleTimer());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && IsCollectible)
        {
            UIManager.Instance.ShowUI(UIState.Collect);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            UIManager.Instance.PopUI();
        }
    }

    private IEnumerator CollectibleTimer()
    {
        IsCollectible = true;
        yield return new WaitForSeconds(collectibleDuration);
        IsCollectible = false;
        UIManager.Instance.PopUI();
    }

    public void Collect()
    {
        if (IsCollectible)
        {
            GameManager.Instance.AddItem(this);
            gameObject.SetActive(false);
            UIManager.Instance.PopUI();
        }
    }
} 