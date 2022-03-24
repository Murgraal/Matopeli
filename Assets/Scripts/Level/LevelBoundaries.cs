using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LevelBoundaries
{
    public readonly int MaxX;
    public readonly int MinX;
    public readonly int MaxY;
    public readonly int MinY;
    
    public readonly List<Vector2> gridPositions = new List<Vector2>();
    public readonly Vector2[,] grid;
    
    public LevelBoundaries(params Vector2[] wallPositions)
    {
        MaxX = 0;
        MinX = 0;
        MaxY = 0;
        MinY = 0;
        
        foreach (var wallPos in wallPositions)
        {
            if (wallPos.x > MaxX - 1) MaxX = (int) wallPos.x - 1;
            if (wallPos.x < MinX + 1) MinX = (int) wallPos.x + 1;
            if (wallPos.y > MaxY - 1) MaxY = (int) wallPos.y - 1;
            if (wallPos.y < MinY + 1) MinY = (int) wallPos.y + 1;
        }

        var xSize = Mathf.Abs(MaxX - MinX);
        var ySize = Mathf.Abs(MaxY - MinY);
        
        grid = new Vector2[xSize,ySize];
        
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                var xOffset = (xSize / 2);
                var yOffset = (ySize / 2);
                
                grid[x, y] = new Vector2(x - xOffset, y - yOffset);
                
                gridPositions.Add(grid[x,y]);
            }
        }
    }
}
