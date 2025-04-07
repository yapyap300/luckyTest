using UnityEngine;
using UnityEngine.Tilemaps;

public class MakeMap : MonoBehaviour
{
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

    // 맵 데이터 (0: 벽, 1: 바닥)
    private int[,] map;

    // 맵 데이터를 외부에서 접근할 수 있도록 프로퍼티 추가
    public int[,] MapData => map;

    // 맵 생성 이벤트
    public delegate void MapGeneratedEvent(int[,] mapData);
    public event MapGeneratedEvent OnMapGenerated;

    void Start()
    {
        GenerateMap();
    }
    public void GenerateMap()
    {
        map = new int[width, height];

        RandomFillMap();

        for (int i = 0; i < smoothCount; i++)
        {
            SmoothMap();
        }

        // 맵 생성 완료 이벤트 발생
        OnMapGenerated?.Invoke(map);
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
            // if (seed == null)
            // {
            //     seed = GenerateRandomSeed();
            // }
            seed = GenerateRandomSeed();
            Random.InitState(seed.GetHashCode());
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 테두리는 항상 벽으로 설정
                if (x < borderSize || x >= width - borderSize || y < borderSize || y >= height - borderSize)
                {
                    map[x, y] = 0; // 0은 벽
                }
                else
                {
                    map[x, y] = (Random.Range(0, 100) < fillPercent) ? 0 : 1;
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
                    newMap[x, y] = 0;
                    continue;
                }

                int neighborWallCount = GetSurroundingWallCount(x, y);

                // 셀룰러 오토마타 규칙 적용
                if (map[x, y] == 0) // 현재 벽인 경우
                {
                    newMap[x, y] = (neighborWallCount < deathLimit) ? 1 : 0;
                }
                else // 현재 바닥인 경우
                {
                    newMap[x, y] = (neighborWallCount > birthLimit) ? 0 : 1;
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
                    // 벽(0)인 경우에만 카운트
                    if (map[neighborX, neighborY] == 0)
                        wallCount++;
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