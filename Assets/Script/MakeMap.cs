using UnityEngine;
using UnityEngine.Tilemaps;

public class MakeMap : MonoBehaviour
{
    [Header("타일맵 설정")]
    public Tilemap tilemap;
    public Tile wallTile;
    public Tile floorTile;

    [Header("맵 설정")]
    public int width = 50;
    public int height = 50;
    public int borderSize = 1;

    [Header("셀룰러 오토마타 설정")]
    [Range(0, 100)]
    public int fillPercent = 45;
    public int smoothCount = 5;

    [Range(0, 8)]
    public int birthLimit = 4;
    [Range(0, 8)]
    public int deathLimit = 3;

    public bool useRandomSeed = true;
    public string seed;

    private int[,] map;

    public void GenerateMap()
    {
        tilemap.ClearAllTiles();
        map = new int[width, height];

        RandomFillMap();

        for (int i = 0; i < smoothCount; i++)
        {
            SmoothMap();
        }

        DrawTiles();
    }

    private string GenerateRandomSeed()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] stringChars = new char[10];
        
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Random.Range(0, chars.Length)];
        }
        
        return new string(stringChars);
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            if (seed == null)
            {
                seed = GenerateRandomSeed();
            }
            Random.InitState(seed.GetHashCode());
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 테두리는 항상 벽으로 설정
                if (x < borderSize || x >= width - borderSize || y < borderSize || y >= height - borderSize)
                {
                    map[x, y] = 1; // 1은 벽
                }
                else
                {
                    map[x, y] = (Random.Range(0, 100) < fillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        int[,] newMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 테두리는 항상 벽으로 유지
                if (x < borderSize || x >= width - borderSize || y < borderSize || y >= height - borderSize)
                {
                    newMap[x, y] = 1;
                    continue;
                }

                int neighborWallCount = GetSurroundingWallCount(x, y);

                // 셀룰러 오토마타 규칙 적용
                if (map[x, y] == 1) // 현재 벽인 경우
                {
                    newMap[x, y] = (neighborWallCount < deathLimit) ? 0 : 1;
                }
                else // 현재 바닥인 경우
                {
                    newMap[x, y] = (neighborWallCount > birthLimit) ? 1 : 0;
                }
            }
        }

        map = newMap;
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;

        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                // 자기 자신은 건너뛰기
                if (neighborX == gridX && neighborY == gridY)
                    continue;

                // 맵 경계 확인
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    wallCount += map[neighborX, neighborY];
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

    void DrawTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x - width / 2, y - height / 2, 0);

                if (map[x, y] == 1)
                {
                    tilemap.SetTile(pos, wallTile);
                }
                else
                {
                    tilemap.SetTile(pos, floorTile);
                }
            }
        }
    }

    // 에디터에서 맵 생성 버튼을 위한 메서드
    public void GenerateMapEditor()
    {
        GenerateMap();
    }
}