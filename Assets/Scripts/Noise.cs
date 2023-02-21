using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public static float Get2DPerlin(Vector2Int position, Vector2Int chunkSize, float scale, Vector2 offset)
    {
        return Mathf.Clamp(Mathf.PerlinNoise((float)position.x / chunkSize.x * scale + offset.x, (float)position.y / chunkSize.y * scale + offset.y),0,1);
    }
}
