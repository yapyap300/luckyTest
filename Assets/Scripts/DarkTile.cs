using UnityEngine;

public class DarkTile : TilemapDraw
{
    override public void RenderTiles(int[,] mapData)
    {

        int rows = mapData.GetLength(0);
        int columns = mapData.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3Int pos = new Vector3Int(
                    c - columns / 2 + offset.x,
                    r - rows / 2 + offset.y,
                    0
                );
                
                tilemap.SetTile(pos, ruleTile);
            }
        }
    }
}
