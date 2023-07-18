using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer), typeof(MeshCollider))]

public class Generation : MonoBehaviour
{
    [Header("Perlin Settings")]
    [SerializeField] private float Multiplier = 1f;
    [SerializeField] private float HorizontalMultiplier = 1f;
    [SerializeField] private float Power = 1;
    [SerializeField] private float waterLevel = 0.3f;

    [Header("Mesh Settings")]
    [SerializeField] private int xSize;
    [SerializeField] private int ySize;

    [Header("Trees")]
    public GameObject[] treePrefab;
    [SerializeField] private int treeGenerationAttempsPerChunk;
    [SerializeField] private float maxTreesHeihgt;
    [SerializeField] private float minTreesHeihgt;

    [Header("Bushes")]
    public GameObject[] bushPrefab;
    [SerializeField] private int bushGenerationAttempsPerChunk;
    [SerializeField] private float maxBushHeihgt;
    [SerializeField] private float minBushHeihgt;

    private Vector3[] _vertices;
    private Mesh _mesh;
    private GameObject thisGameObject;

    private void Start()
    {
        GenerateMesh();
        GenerateHeights(true);
        GenerateTrees();
        GenerateBushes();
    }


    public void GenerateMesh()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;       
        _mesh.name = "Grid";

        GenerateVertices(_mesh, xSize, ySize);
        GenerateTris(_mesh, xSize, ySize);
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
    }
    private void GenerateVertices(Mesh mesh, int xVert, int yVert)
    {
        xVert += 1;
        yVert += 1;
        _vertices = new Vector3[(xVert) * (yVert)];
        Vector2[] uvs = new Vector2[_vertices.Length];

        for (int x = 0, i = 0; x < xVert; x++)
        {
            for (int y = 0; y < yVert; y++)
            {
                _vertices[i] = new Vector3(x, 0, y);
                uvs[i] = new Vector2((float)x / xVert, (float)y / yVert);
                i++;
            }
        }

        mesh.vertices = _vertices;
        mesh.uv = uvs;
    }

    private void GenerateTris(Mesh mesh, int xt, int yt)
    {
        int[] triangles = new int[xt * yt * 6];

        for (int y = 0, ti = 0, vi = 0; y < yt; y++, vi++)
        {
            for (int x = 0; x < xt; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = triangles[ti + 4] = vi + xt + 1;
                triangles[ti + 2] = triangles[ti + 3] = vi + 1;
                triangles[ti + 5] = vi + xt + 2;
            }
        }

        mesh.triangles = triangles.Reverse().ToArray();
    }

    private void GenerateHeights(bool instant)
    {
        Vector2 seed = ChunkLoader.instance.generationSeed;
        float seedX = seed.x;
        float seedY = seed.y;

        Vector2 position = new Vector2(transform.position.x, transform.position.z);

        Vector3[] newVertices = new Vector3[_mesh.vertices.Length];

        for (int i = 0; i < _mesh.vertices.Length; i++)
        {
            newVertices[i] = _mesh.vertices[i];
            newVertices[i].y = Multiplier * Mathf.Pow(Mathf.PerlinNoise(((position.x + newVertices[i].x) * HorizontalMultiplier + seedX) / xSize, ((position.y + newVertices[i].z) * HorizontalMultiplier + seedY) / ySize),Power) - Multiplier * waterLevel;
        }

        _mesh.vertices = newVertices;
        GetComponent<MeshCollider>().sharedMesh = _mesh;      
    }

    private void GenerateTrees()
    {
        for (int attemp = 0; attemp < treeGenerationAttempsPerChunk; attemp++)
        {
            Vector3 startPoint = new Vector3(transform.position.x + Random.Range(0, xSize), 50f, transform.position.z + Random.Range(0, ySize));
            RaycastHit hit;
            if (Physics.Raycast(startPoint, Vector3.down, out hit))
            {
                Vector3 hitPoint = hit.point;
                if (hitPoint.y < maxTreesHeihgt && hitPoint.y > minTreesHeihgt)
                {
                    GameObject newTree = Instantiate(treePrefab[Random.Range(0, treePrefab.Length)], hitPoint, Quaternion.identity);
                    newTree.transform.parent = transform;
                }

            }
        }
    }

    private void GenerateBushes()
    {
        for (int attemp = 0; attemp < bushGenerationAttempsPerChunk; attemp++)
        {
            Vector3 startPoint = new Vector3(transform.position.x + Random.Range(0, xSize), 50f, transform.position.z + Random.Range(0, ySize));
            RaycastHit hit;
            if (Physics.Raycast(startPoint, Vector3.down, out hit))
            {
                Vector3 hitPoint = hit.point;
                if (hitPoint.y < maxBushHeihgt && hitPoint.y > minBushHeihgt)
                {
                    GameObject newBush = Instantiate(bushPrefab[Random.Range(0, bushPrefab.Length)], hitPoint, Quaternion.identity);
                    newBush.transform.parent = transform;
                }
            }
        }
    }

    /*
    IEnumerator PerlinGenerate()
    {
        float seedX = Random.Range(0f, 10000f);
        float seedY = Random.Range(0f, 10000f);

        Vector3[] newVertices = new Vector3[_mesh.vertices.Length];
        for (int i = 0; i < _mesh.vertices.Length; i++)
        {
            newVertices[i] = _mesh.vertices[i];
        }

        for (int i = 0; i < _mesh.vertices.Length; i++)
        {
            newVertices[i].y = Multiplier * Mathf.Pow(Mathf.PerlinNoise((newVertices[i].x * HorizontalMultiplier + seedX) / xSize, (newVertices[i].z * HorizontalMultiplier + seedY) / ySize), Power) - Multiplier * waterLevel;
            _mesh.vertices = newVertices;
            yield return new WaitForEndOfFrame();
        }
        GetComponent<MeshCollider>().sharedMesh = _mesh;
    }
    */
    /*
        private void OnDrawGizmos()
        {
            if (_vertices == null) return;

            Gizmos.color = Color.red;
            for (int i = 0; i < _vertices.Length; i++) Gizmos.DrawSphere(_vertices[i], 0.1f);
        }
        */
}
