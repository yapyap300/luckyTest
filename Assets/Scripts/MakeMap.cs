using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

// 타일 타입을 정의하는 enum
public enum TileType
{
    Wall = 0,    // 벽
    Floor = 1,   // 바닥
    Item = 2     // 아이템 배치 위치
}

public class MakeMap : MonoBehaviour
{
    [Header("맵 설정")]
    [SerializeField] private int rows = 50;  // 행 (세로)
    [SerializeField] private int columns = 50;  // 열 (가로)
    [SerializeField] private int borderSize = 1;  // 가장자리 보장 크기
    [SerializeField] private int smoothCount = 5;
    [SerializeField] private int wallThreshold = 45;
    [SerializeField] private int deathLimit = 4;
    [SerializeField] private int birthLimit = 4;
    
    private int[,] map;
    
    public event System.Action<int[,]> OnMapGenerated;
    
    public void GenerateMap()
    {
        map = new int[rows, columns];
        RandomFillMap();
        
        for (int i = 0; i < smoothCount; i++)
        {
            SmoothMap();
        }

        // 아이템 매니저 초기화
        ItemManager.Instance.Initialize(map);
        
        // 맵 생성 완료 이벤트 발생
        OnMapGenerated?.Invoke(map);
    }
    
    private void RandomFillMap()
    {        
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                // 가장자리는 벽으로 설정
                if (r < borderSize || r >= rows - borderSize || 
                    c < borderSize || c >= columns - borderSize)
                {
                    map[r, c] = (int)TileType.Wall;
                }
                else
                {
                    map[r, c] = (Random.Range(0, 100) < wallThreshold) ? 
                        (int)TileType.Wall : (int)TileType.Floor;
                }
            }
        }
    }
    
    private void SmoothMap()
    {
        int[,] newMap = new int[rows, columns];
        
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                // 가장자리는 항상 벽으로 유지
                if (r < borderSize || r >= rows - borderSize || 
                    c < borderSize || c >= columns - borderSize)
                {
                    newMap[r, c] = (int)TileType.Wall;
                    continue;
                }

                int neighborWallCount = GetNeighborWallCount(r, c);
                
                // 셀룰러 오토마타 규칙 적용
                if (map[r, c] == (int)TileType.Wall) // 현재 벽인 경우
                {
                    newMap[r, c] = (neighborWallCount < deathLimit) ? (int)TileType.Floor : (int)TileType.Wall;
                }
                else // 현재 바닥인 경우
                {
                    newMap[r, c] = (neighborWallCount > birthLimit) ? (int)TileType.Wall : (int)TileType.Floor;
                }
            }
        }
        
        map = newMap;
    }
    
    private int GetNeighborWallCount(int row, int col)
    {
        int wallCount = 0;
        
        for (int r = -1; r <= 1; r++)
        {
            for (int c = -1; c <= 1; c++)
            {
                if (r == 0 && c == 0) continue;
                
                int checkRow = row + r;
                int checkCol = col + c;
                
                if (checkRow >= 0 && checkRow < rows && 
                    checkCol >= 0 && checkCol < columns)
                {
                    wallCount += (map[checkRow, checkCol] == (int)TileType.Wall) ? 1 : 0;
                }
                else
                {
                    // 맵 바깥은 벽으로 간주
                    wallCount++;
                }
            }
        }
        
        return wallCount;
    }

    // 에디터에서 맵 생성 버튼을 위한 메서드
    public void GenerateMapEditor()
    {
        GenerateMap();
    }
}