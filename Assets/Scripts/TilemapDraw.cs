using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDraw : MonoBehaviour
{
    [Header("타일맵 설정")]
    public Tilemap tilemap;
    public TileBase ruleTile;
    
    [Header("타일 타입 설정")]
    public TileType tileType = TileType.Wall;
    
    [Header("오프셋 설정")]
    public Vector2Int offset = Vector2Int.zero;
    
    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        
        // 맵 생성 이벤트 구독
        MapManager.Instance.OnMapGenerated += RenderTiles;
        
        // 게임 재시작 이벤트 구독
        GameManager.Instance.OnGameRestart += OnGameRestart;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (MapManager.Instance != null)
        {
            MapManager.Instance.OnMapGenerated -= RenderTiles;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameRestart -= OnGameRestart;
        }
    }

    private void OnGameRestart()
    {
        if (tilemap != null)
        {
            tilemap.ClearAllTiles();
        }
    }
    
    // 맵 데이터를 기반으로 타일 배치
    public virtual void RenderTiles(int[,] mapData)
    {        
        
        int rows = mapData.GetLength(0);    // 행 (세로)
        int columns = mapData.GetLength(1);  // 열 (가로)
        
        // 맵 데이터를 순회하며 타일 배치
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                // 현재 위치의 타일 타입이 이 타일맵의 타일 타입과 일치하는 경우에만 배치
                if (mapData[r, c] == (int)tileType)
                {
                    // 월드 좌표 계산 (중앙 기준)
                    Vector3Int pos = new Vector3Int(
                        c - columns / 2 + offset.x, 
                        r - rows / 2 + offset.y,
                        0
                    );
                    
                    // 룰 타일 배치                    
                    tilemap.SetTile(pos, ruleTile);
                }
            }
        }
    }
} 