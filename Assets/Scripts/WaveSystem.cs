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
    [SerializeField] private float fadeDuration;

    // 색상 상수 정의
    private static readonly Color NORMAL_COLOR = new(0,0,0,1);
    private static readonly Color WAVE_COLOR = new(0,0,0,0);

    private bool isExpanding = false;
    private Vector3 originalScale;
    private Tween currentTween;
    private Camera mainCamera;
    private float currentRadius = 0f;
    private float previousRadius = 0f;
    [SerializeField] private Tilemap tilemap;
    private Dictionary<Vector3Int, Tween> activeTweens = new Dictionary<Vector3Int, Tween>();
    private CircleCollider2D waveCollider;

    private void Start()
    {
        originalScale = transform.localScale;
        transform.localScale = originalScale * initialRadius;
        mainCamera = Camera.main;
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

        currentTween = transform.DOScale(originalScale * maxRadius, waveDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                OnExpandComplete();
            });

        // 타일 확인 코루틴 시작
        StartCoroutine(CheckTilesCoroutine());
    }

    private IEnumerator CheckTilesCoroutine()
    {
        HandleTileInteraction(tilemap.WorldToCell(transform.position));
        
        float checkInterval = waveDuration * 0.1f;

        while (isExpanding)
        {
            currentRadius = transform.localScale.x * 0.5f;
            CheckNewTiles();
            previousRadius = currentRadius;
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void CheckNewTiles()
    {
        Vector3Int centerCell = tilemap.WorldToCell(transform.position);
        int currentRadiusInt = Mathf.CeilToInt(currentRadius);
        int previousRadiusInt = Mathf.CeilToInt(previousRadius);

        for (int c = -currentRadiusInt; c <= currentRadiusInt; c++)
        {
            for (int r = -currentRadiusInt; r <= currentRadiusInt; r++)
            {
                float distance = Mathf.Sqrt(c * c + r * r);
                
                // 이전 반경 내의 타일은 스킵
                if (distance <= previousRadiusInt)
                    continue;

                // 현재 반경 밖의 타일은 스킵
                if (distance > currentRadiusInt)
                    continue;

                Vector3Int checkPos = centerCell + new Vector3Int(c, r, 0);
                if (tilemap.HasTile(checkPos))
                {
                    HandleTileInteraction(checkPos);
                }
            }
        }
    }

    private void HandleTileInteraction(Vector3Int tilePosition)
    {
        if (activeTweens.ContainsKey(tilePosition))
        {
            activeTweens[tilePosition].Kill();
            activeTweens.Remove(tilePosition);
        }
        Tween restoreTween = DOTween.To(
            () => WAVE_COLOR,
            color => tilemap.SetColor(tilePosition, color),
            NORMAL_COLOR,
            waveDuration * fadeDuration
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
}