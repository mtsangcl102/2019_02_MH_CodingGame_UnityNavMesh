using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;
using Unity.Rendering;
using UnityEngine.AI;
using UnityEngine.Experimental.U2D;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] public Mesh TerrainMesh;
    [SerializeField] public Mesh PlayerMesh;
    [SerializeField] public Material[] GroundMaterials;
    [SerializeField] public Material PlayerMaterial;
    [SerializeField] public GameObject WallPrefab;

    public NavMeshAgent NavMeshAgent;
    public NavMeshSurface NavMeshSurface;
    private readonly GameObject[,] _walls = new GameObject[51,51];

    void Start()
    {
        _CreatePlayer();
        _CreateWalls();
    }

    private void _CreatePlayer()
    {
        EntityManager entityManager = World.Active.EntityManager;
        EntityArchetype terrainType = entityManager.CreateArchetype(
            typeof(PlayerComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
        );
        
        Entity entity = entityManager.CreateEntity(terrainType);

        var position = new float3(Random.Range(1, 50), 0, Random.Range(1, 50));
        entityManager.SetComponentData(entity, new Translation{ Value = position });
        entityManager.SetSharedComponentData(entity, new RenderMesh{ mesh = PlayerMesh, material = PlayerMaterial
        });
        
        NavMeshAgent = GameObject.FindObjectOfType<NavMeshAgent>();
        NavMeshSurface = GameObject.FindObjectOfType<NavMeshSurface>();
    }

    private void _CreateWalls()
    {
        // Create New walls
        for (int x = 0; x < 51; x++)
        {
            for (int y = 0; y < 51; y++)
            {
                _walls[x,y] = Instantiate( WallPrefab, new Vector3( x,0f, y ), Quaternion.identity, NavMeshSurface.transform );
            }
        }
    }

    public void CreateNavMeshObstacles( MazeGeneration.Maze maze )
    {
        // Create New walls
        for (int x = 0; x < 51; x++)
        {
            for (int y = 0; y < 51; y++)
            {
                _walls[x,y].gameObject.SetActive( maze.squaredCells[x,y] );
            }
        }

        
        NavMeshSurface.BuildNavMesh();
    }

    private static GameManager _instance;
    public static GameManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        return _instance;
    }
}
