using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Vector2 chuckCoordinates;

    public Chunk (Vector2 newChunkCoordinates)
    {
        chuckCoordinates = newChunkCoordinates;
    }
    public void SetCoordinates (Vector2 newChunkCoordinates)
    {
        chuckCoordinates = newChunkCoordinates;
        transform.name = chuckCoordinates.ToString();
    }
    public Vector2 GetCoordinates()
    {
        return chuckCoordinates;
    }

    public void DestroyChunk()
    {
        Destroy(gameObject);
    }
}
