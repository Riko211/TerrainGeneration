using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public Transform playerPos;
    public GameObject chunkPrefab;
    public Vector2 generationSeed;

    [SerializeField] private int chunkSizeX = 16;
    [SerializeField] private int chunkSizeY = 16;

    [SerializeField] private int loadingRange = 1;
    [SerializeField] private float updateTime = 1f;
    [SerializeField] private int loadingChumksPerSecond = 10;

    [Header("Coordinates")]
    [SerializeField] private Vector2 playerPosition;
    [SerializeField] private Vector2Int chunkCoordinates;


    private bool firstLoad = true;
    private Vector2 lastPos;
    [SerializeField] private List<Chunk> Chunks = new List<Chunk>();
    [SerializeField] private List<Vector2Int> ChunksToInstantiate = new List<Vector2Int>();


    public static ChunkLoader instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        generationSeed = new Vector2 (Random.Range(0, 1000), Random.Range(0,1000));
        FirstLoad();
        StartCoroutine (ChunkUpdater());
        StartCoroutine (ChunkInstantiator());
    }

    IEnumerator ChunkUpdater()
    {
        while (true)
        {
            playerPosition.x = playerPos.position.x;
            playerPosition.y = playerPos.position.z;
            chunkCoordinates = ConvertCoordinates(playerPos.position.x, playerPos.position.z);

            if (lastPos != chunkCoordinates)
            {

                for (int chunkCount = 0; chunkCount < Chunks.Count; chunkCount++)
                {                                      
                    if (Mathf.Abs(Chunks[chunkCount].GetCoordinates().x - chunkCoordinates.x) > loadingRange || Mathf.Abs(Chunks[chunkCount].GetCoordinates().y - chunkCoordinates.y) > loadingRange)
                    {
                        Chunk chunkToDelete = Chunks[chunkCount];
                        Chunks.RemoveAt(chunkCount);
                        chunkToDelete.DestroyChunk();
                        chunkCount--;
                    }
                }

                for (int xi = chunkCoordinates.x - loadingRange; xi <= chunkCoordinates.x + loadingRange; xi++)
                {
                    for (int yi = chunkCoordinates.y - loadingRange; yi <= chunkCoordinates.y + loadingRange; yi++)
                    {
                        bool isSame = false;
                        for (int chunkCount = 0; chunkCount < Chunks.Count; chunkCount++)
                        {
                            if (Chunks[chunkCount].GetCoordinates() == new Vector2(xi, yi))
                            {
                                isSame = true;
                                break;
                            }
                        }
                        if (!isSame)
                        {
                            for (int chunkCount = 0; chunkCount < ChunksToInstantiate.Count; chunkCount++)
                            {
                                if (ChunksToInstantiate[chunkCount] == new Vector2(xi, yi))
                                {
                                    isSame = true;
                                    break;
                                }
                            }
                        }
                        if (!isSame)
                        {
                            ChunksToInstantiate.Add(new Vector2Int(xi, yi));
                        }
                    }
                }

                lastPos = chunkCoordinates;
            }
            yield return new WaitForSeconds(updateTime);
        }       
    }

    IEnumerator ChunkInstantiator()
    {
        while (true)
        {
            if (ChunksToInstantiate.Count != 0)
            {
                InstantiateChunk(ChunksToInstantiate[0].x, ChunksToInstantiate[0].y);
                ChunksToInstantiate.RemoveAt(0);
            }
            yield return new WaitForSeconds(1 / loadingChumksPerSecond);
        }
    }

    private void FirstLoad()
    {
        for (int xi = chunkCoordinates.x - loadingRange; xi <= chunkCoordinates.x + loadingRange; xi++)
        {
            for (int yi = chunkCoordinates.y - loadingRange; yi <= chunkCoordinates.y + loadingRange; yi++)
            {
                InstantiateChunk(xi, yi);
            }
        }
    }

    private void InstantiateChunk(int xi, int yi)
    {
        Vector3 chunkPos = new Vector3(xi * chunkSizeX, 0, yi * chunkSizeY);
        GameObject newChunk = Instantiate(chunkPrefab, chunkPos, Quaternion.identity);
        Chunk chunk = newChunk.GetComponent<Chunk>();
        chunk.SetCoordinates(new Vector2(xi, yi));
        Chunks.Add(chunk);
    }

    private Vector2Int ConvertCoordinates(float x, float y)
    {
        Vector2Int chunkCoords = new Vector2Int (0,0);

        if (x < 0) chunkCoords.x = (int)x / chunkSizeX - 1;
        else chunkCoords.x = (int)x / chunkSizeX;
        if (y < 0) chunkCoords.y = (int)y / chunkSizeY - 1;
        else chunkCoords.y = (int)y / chunkSizeY;

        return chunkCoords;
    }
}
