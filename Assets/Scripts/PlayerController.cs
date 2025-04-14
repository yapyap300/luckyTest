using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // 게임이 시작되지 않았다면 움직임 무시
        if (!GameManager.Instance.IsGameActive) return;

        Vector2 newPosition = (Vector2)rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
} 