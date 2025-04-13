using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TilemapWaveEffect : MonoBehaviour
{
    [Header("타일맵 설정")]
    public Tilemap tilemap;
    public float colorChangeDuration = 0.5f;
    
    // 색상 상수 정의
    private static readonly Color NORMAL_COLOR = Color.white;
    
    private Dictionary<Vector3Int, Tween> activeTweens = new Dictionary<Vector3Int, Tween>();
    
    void Start()
    {
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wave"))
        {
            Debug.Log("충돌!");
            // 웨이브 콜라이더와 충돌한 타일의 위치 계산
            Vector3 worldPos = other.transform.position;
            Vector3Int tilePos = tilemap.WorldToCell(worldPos);
            
            // 해당 타일이 존재하는지 확인
            if (tilemap.HasTile(tilePos))
            {
                // 이전 트윈이 있다면 중지
                if (activeTweens.ContainsKey(tilePos))
                {
                    activeTweens[tilePos].Kill();
                }
                
                // 색상 변경 시작
                StartColorChange(tilePos);
            }
        }
    }
    
    void StartColorChange(Vector3Int tilePos)
    {
        // 현재 타일의 색상 가져오기
        Color currentColor = tilemap.GetColor(tilePos);
        
        // 웨이브 색상으로 변경
        Tween waveTween = DOTween.To(
            () => currentColor,
            color => tilemap.SetColor(tilePos, color),
            NORMAL_COLOR,
            colorChangeDuration
        ).SetEase(Ease.Linear);
        
        // 트윈 완료 후 정리
        waveTween.OnComplete(() => {
            activeTweens.Remove(tilePos);
        });
        
        // 활성 트윈 저장
        activeTweens[tilePos] = waveTween;
    }
    
    void OnDestroy()
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