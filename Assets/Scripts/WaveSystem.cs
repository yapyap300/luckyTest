using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private float maxRadius;
    [SerializeField] private float initialRadius;
    [SerializeField] private float waveDuration;
    [SerializeField] private float waveEffectDuration;
    
    // 색상 상수 정의
    private static readonly Color NORMAL_COLOR = Color.black;
    private static readonly Color WAVE_COLOR = Color.white;
    
    private bool isExpanding = false;
    private Vector3 originalScale;
    private Tween currentTween;
    private Camera mainCamera;
    private float currentRadius = 0f;
    private float previousRadius = 0f;
    private Tilemap[] tilemaps;
    private Dictionary<Vector3Int, Tween> activeTweens = new Dictionary<Vector3Int, Tween>();
    private CircleCollider2D waveCollider;

    private void Start()
    {
        originalScale = transform.localScale;
        transform.localScale = originalScale * initialRadius;
        mainCamera = Camera.main;
        tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        waveCollider = GetComponent<CircleCollider2D>();
    }

    public void OnWave(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.IsGameActive) return;
        
        if (context.performed && !isExpanding)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
            transform.position = new Vector3(worldPosition.x, worldPosition.y, 0f);
            
            StartExpanding();
        }
    }

    private void StartExpanding()
    {
        isExpanding = true;
        currentRadius = initialRadius;
        previousRadius = initialRadius;
        
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
        
        // Collider 활성화
        waveCollider.enabled = true;
        
        currentTween = transform.DOScale(maxRadius, waveDuration)
            .SetEase(Ease.Linear)
            .OnUpdate(() => {
                currentRadius = transform.localScale.x * 0.5f;
                CheckNewTiles();
                previousRadius = currentRadius;
            })
            .OnComplete(() => {
                OnExpandComplete();
            });
    }

    private void CheckNewTiles()
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            Vector3Int centerCell = tilemap.WorldToCell(transform.position);
            int currentRadiusInt = Mathf.CeilToInt(currentRadius);
            int previousRadiusInt = Mathf.CeilToInt(previousRadius);
            
            // 원 반경 내의 타일만 검사
            for (int c = -currentRadiusInt; c <= currentRadiusInt; c++)
            {
                for (int r = -currentRadiusInt; r <= currentRadiusInt; r++)
                {
                    // 이전 반경 내의 타일은 스킵
                    if (c * c + r * r <= previousRadiusInt * previousRadiusInt)
                        continue;
                        
                    // 현재 반경 밖의 타일은 스킵
                    if (c * c + r * r > currentRadiusInt * currentRadiusInt)
                        continue;
                        
                    Vector3Int checkPos = centerCell + new Vector3Int(c, r, 0);
                    if (tilemap.HasTile(checkPos))
                    {
                        HandleTileInteraction(tilemap, checkPos);
                    }
                }
            }
        }
    }

    private void HandleTileInteraction(Tilemap tilemap, Vector3Int tilePosition)
    {
        // 이전 트윈이 있다면 재시작
        if (activeTweens.ContainsKey(tilePosition))
        {
            activeTweens[tilePosition].Restart();
            return;
        }
        
        // 즉시 하얀색으로 변경
        tilemap.SetColor(tilePosition, WAVE_COLOR);
        
        // 천천히 검은색으로 복귀
        Tween restoreTween = DOTween.To(
            () => WAVE_COLOR,
            color =>tilemap.SetColor(tilePosition, color),
            NORMAL_COLOR,
            waveEffectDuration
        ).SetEase(Ease.InQuad).OnComplete(() => activeTweens.Remove(tilePosition));
        
        activeTweens[tilePosition] = restoreTween;
    }

    private void OnExpandComplete()
    {
        // Collider 비활성화
        waveCollider.enabled = false;
        
        transform.localScale = originalScale * initialRadius;
        isExpanding = false;
    }
    
    private void OnDestroy()
    {
        // 모든 활성 트윈 정리
        foreach (var tween in activeTweens.Values)
        {
            if (tween != null && tween.IsActive())
            {
                tween.Kill();
            }
        }
        activeTweens.Clear();
    }
} 