using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class CollisionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity player1, ref PlayerComponent playerComponent1, ref Translation translation1) =>
        {
            var position1 = translation1.Value;
            var playerComponent = playerComponent1;
            // collide with player

            var treasureCount = 0;
            
            Entities.ForEach((Entity treasure, ref TreasureComponent treasureComponent, ref Translation translation2) =>
            {
                treasureCount++;
                
                if ( Mathf.Abs(position1.x - translation2.Value.x) < 0.1f && Mathf.Abs(position1.z - translation2.Value.z ) < 1 )
                {
                    PostUpdateCommands.DestroyEntity(treasure);
                }
            });

            if (treasureCount == 0)
            {
                CreateMaze(position1);
            }
        });
    }

    private void CreateMaze(Vector3 playerPosition)
    {
        // Clear old Walls
        Entities.ForEach((Entity wall, ref TerrainComponent terrainComponent) =>
        {
            PostUpdateCommands.DestroyEntity(wall);
        });

        bool isPassed = false;
        MazeGeneration.Maze mazeData = null;
        while (!isPassed)
        {
            // Create Maze
            mazeData = new MazeGeneration.Maze(25, 25);
            mazeData.WallToSquare();

            int playerPositionX = Mathf.RoundToInt(playerPosition.x);
            int playerPositionY = Mathf.RoundToInt(playerPosition.z);

            isPassed = !mazeData.squaredCells[playerPositionX, playerPositionY];
            
            playerPosition = new Vector3(playerPositionX, playerPosition.y, playerPositionY);
        }

        // Create Entity
        EntityManager entityManager = World.Active.EntityManager;
        EntityArchetype obsticleType = entityManager.CreateArchetype(
            typeof(TerrainComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
        );

        List<Vector2> moveablePosition = new List<Vector2>();
        for (int x = 0; x < 51; x++)
        {
            for (int y = 0; y < 51; y++)
            {
                Enum.TerrainType type = Enum.TerrainType.None;

                if (!mazeData.squaredCells[x,y])
                {
                    if(x%2 == 1 && y%2 == 1)
                        moveablePosition.Add( new Vector2(x, y) );
                    continue;
                }

                Entity entity = entityManager.CreateEntity( obsticleType);
                entityManager.SetComponentData(entity, new Translation{ Value = new float3( x, 0, y)});
                Material material = GameManager.GetInstance().GroundMaterials[1];
                
                entityManager.SetSharedComponentData(entity, new RenderMesh{ mesh = GameManager.GetInstance().TerrainMesh, material = material
                });
            }
        }
        
        // Create new treasure
        Entity treasure = entityManager.CreateEntity( entityManager.CreateArchetype(
            typeof(TreasureComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
        ) );

        var treasurePosition = moveablePosition[Random.Range(0, moveablePosition.Count)];
        var treasurePositionV3 = new float3(treasurePosition.x,0,treasurePosition.y);
        
        entityManager.SetComponentData(treasure, new Translation{ Value = treasurePositionV3 });
        Material material2 = GameManager.GetInstance().GroundMaterials[2];
        entityManager.SetSharedComponentData(treasure,
            new RenderMesh {mesh = GameManager.GetInstance().PlayerMesh, material = material2});
        
        GameManager.GetInstance().CreateNavMeshObstacles( mazeData );
        GameManager.GetInstance().NavMeshAgent.transform.position = playerPosition;
        GameManager.GetInstance().NavMeshAgent.SetDestination(treasurePositionV3);
    }
}
