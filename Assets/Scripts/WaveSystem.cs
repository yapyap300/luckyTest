using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private float expandDuration = 1f;
    [SerializeField] private float maxRadius = 5f;
    [SerializeField] private float initialRadius = 0.1f; // 매우 작은 초기 크기
    
    private CircleCollider2D waveCollider;
    private bool isExpanding = false;
    private Vector3 originalScale;
    private Tween currentTween;
    private Camera mainCamera;

    private void Start()
    {
        // 기존에 붙여놓은 콜라이더 참조
        waveCollider = GetComponent<CircleCollider2D>();
        
        // 원래 크기 저장
        originalScale = transform.localScale;
        
        // 초기 크기 설정 (매우 작은 점)
        transform.localScale = originalScale * initialRadius;
        
        // 메인 카메라 참조
        mainCamera = Camera.main;
    }

    public void OnWave(InputAction.CallbackContext context)
    {
        if (context.performed && !isExpanding)
        {
            // 마우스 위치를 월드 좌표로 변환
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
            
            // 웨이브 시작 위치 설정
            transform.position = new Vector3(worldPosition.x, worldPosition.y, 0f);
            
            StartExpanding();
        }
    }

    private void StartExpanding()
    {
        isExpanding = true;
        
        // 이전 트윈이 있다면 중지
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
        
        // Collider 활성화
        waveCollider.enabled = true;
        
        // 크기 확장 애니메이션 (매우 작은 점에서 목표 크기로)
        currentTween = transform.DOScale(originalScale * maxRadius, expandDuration)
            .SetEase(Ease.OutQuad) // 더 자연스러운 확장을 위해 OutQuad 사용
            .OnComplete(() => {
                // 확장 완료 후 처리
                OnExpandComplete();
            });
    }

    private void OnExpandComplete()
    {
        // Collider 비활성화
        waveCollider.enabled = false;
        
        // 원래 크기로 바로 변환 (매우 작은 점으로)
        transform.localScale = originalScale * initialRadius;
        
        isExpanding = false;
    }
} 