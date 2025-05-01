using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private ItemBox interactableItem;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        MapManager.Instance.OnMapGenerated += OnMapGenerated;
    }

    private void OnMapGenerated(int[,] mapData)
    {
        CheckAndFixStartPosition(mapData);
    }

    private void CheckAndFixStartPosition(int[,] mapData)
    {
        int centerRow = mapData.GetLength(0) / 2;
        int centerCol = mapData.GetLength(1) / 2;

        // 중앙 위치가 벽인지 확인
        if (mapData[centerRow, centerCol] == (int)TileType.Wall)
        {
            // BFS로 가장 가까운 빈 공간 찾기
            var (row, col) = FindNearestEmptyCell(centerRow, centerCol, mapData);
            transform.position = new Vector3(col - centerCol + 0.5f,  row - centerRow + 0.5f, 0);
        }
    }

    private (int row, int col) FindNearestEmptyCell(int startRow, int startCol, int[,] mapData)
    {
        int rows = mapData.GetLength(0);
        int cols = mapData.GetLength(1);
        bool[,] visited = new bool[rows, cols];
        Queue<(int, int)> queue = new Queue<(int, int)>();
        
        // 시작 위치를 큐에 추가
        queue.Enqueue((startRow, startCol));
        visited[startRow, startCol] = true;

        // 상하좌우 이동을 위한 방향 벡터
        (int row, int col)[] directions = new[]
        {
            (-1, 0), // 상
            (1, 0),  // 하
            (0, -1), // 좌
            (0, 1)   // 우
        };

        while (queue.Count > 0)
        {
            var (currentRow, currentCol) = queue.Dequeue();
            
            // 현재 위치가 빈 공간이면 반환
            if (mapData[currentRow, currentCol] == (int)TileType.Floor)
            {
                return (currentRow, currentCol);
            }

            // 상하좌우 탐색
            foreach (var (dr, dc) in directions)
            {
                int nextRow = currentRow + dr;
                int nextCol = currentCol + dc;

                // 맵 범위 체크 및 방문 여부 확인
                if (nextRow >= 0 && nextRow < rows && 
                    nextCol >= 0 && nextCol < cols && 
                    !visited[nextRow, nextCol])
                {
                    queue.Enqueue((nextRow, nextCol));
                    visited[nextRow, nextCol] = true;
                }
            }
        }

        return (startRow, startCol);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            interactableItem = collision.gameObject.GetComponent<ItemBox>();
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            interactableItem = null;
        }
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
    public void OnInteract()
    {
        if (interactableItem != null)
        {
            interactableItem.Collect();
        }
    }
} 