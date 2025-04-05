using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private float expandDuration = 1f;
    [SerializeField] private float maxRadius = 5f;
    [SerializeField] private float initialRadius = 0.5f;
    
    private CircleCollider2D waveCollider;
    private bool isExpanding = false;
    private Vector3 originalScale;
    private Tween currentTween;

    private void Start()
    {
        // 기존에 붙여놓은 콜라이더 참조
        waveCollider = GetComponent<CircleCollider2D>();
        
        // 원래 크기 저장
        originalScale = transform.localScale;
        
        // 초기 크기 설정
        transform.localScale = originalScale * (initialRadius / maxRadius);
    }

    public void OnWave(InputAction.CallbackContext context)
    {
        if (context.performed && !isExpanding)
        {
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
        
        // 크기 확장 애니메이션
        currentTween = transform.DOScale(originalScale, expandDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                // 확장 완료 후 처리
                OnExpandComplete();
            });
    }

    private void OnExpandComplete()
    {
        // Collider 비활성화
        waveCollider.enabled = false;
        
        // 원래 크기로 바로 변환
        transform.localScale = originalScale * (initialRadius / maxRadius);
        
        isExpanding = false;
    }
} 