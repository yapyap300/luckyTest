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
    public bool IsCollectible { get; private set; }
    private bool isPlayerColliding = false;

    public void Initialize(ItemRarity rarity)
    {
        Rarity = rarity;
        IsCollectible = false;
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
            isPlayerColliding = true;
            UIManager.Instance.ShowUI(UIState.Collect, transform.position);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerColliding = false;
            UIManager.Instance.HideUI(UIState.Collect);
        }
    }

    private IEnumerator CollectibleTimer()
    {
        IsCollectible = true;
        yield return new WaitForSeconds(collectibleDuration);
        IsCollectible = false;
        UIManager.Instance.HideUI(UIState.Collect);
    }

    public void Collect()
    {
        if (IsCollectible && isPlayerColliding)
        {
            GameManager.Instance.AddItem(this);
            gameObject.SetActive(false);
            UIManager.Instance.HideUI(UIState.Collect);
        }
    }
} 
