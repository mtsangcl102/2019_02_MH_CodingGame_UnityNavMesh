using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [FormerlySerializedAs("mesh")] [SerializeField] private UnityEngine.Mesh terrainMesh;
    [SerializeField] private UnityEngine.Mesh playerMesh;
    [SerializeField] private UnityEngine.Material[] playerMat;
    [SerializeField] private UnityEngine.Material[] terrainMat;
    [SerializeField] private Transform treasure;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject ground;
    [SerializeField] private NavAgent agent;
    [SerializeField] private NavMeshSurface surface;
    [SerializeField] private Transform mazeParent;

    private int[,] maze; // 1 = visited, 2 = startpos, 0 = wall

    public const int SIZE = 15;
    private const int N = 1;
    private const int E = 1 << 2;
    private const int S = 1 << 3;
    private const int W = 1 << 4;
    private int[] directions = {N, E, S, W};

    public static int endX, endY;

    private GameObject[,,] mazeWalls;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        GenerateMaze(Random.Range(0, SIZE), Random.Range(0, SIZE));
    }

    public void GenerateMaze(int x, int y)
    {
        foreach (Transform t in mazeParent)
        {
            Destroy(t.gameObject);
        }

        maze = new int[SIZE,SIZE]; // true = road; false = wall
        Carve(x, y, maze);

        while (true)
        {
            endX = Random.Range(0, SIZE);
            endY = Random.Range(0, SIZE);

            if (endX != x && endY != y)
            {
                treasure.transform.position = new Vector3(endX*2, 1, endY*2);
                break;
            }
        }

        mazeWalls = new GameObject[SIZE*2,SIZE*2,2];
        for (int i = -1; i < SIZE * 2; i++)
        {
            for (int j = -1; j < SIZE * 2; j++)
            {
                if (i == -1 || j == -1)
                {
                    Instantiate(wall, new Vector3(i, 1, j), Quaternion.identity, mazeParent);
                    Instantiate(wall, new Vector3(i, 2, j), Quaternion.identity, mazeParent);
                }
                else
                {
                    mazeWalls[i,j,0] = Instantiate(wall, new Vector3(i, 1, j), Quaternion.identity, mazeParent);
                    mazeWalls[i,j,1] = Instantiate(wall, new Vector3(i, 2, j), Quaternion.identity, mazeParent);
                }
            }
        }

        for (int mapX = 0; mapX < SIZE; mapX++)
        {
            for (int mapY = 0; mapY < SIZE; mapY++)
            {
                int actualX = mapX * 2;
                int actualY = mapY * 2;

                Destroy(mazeWalls[actualX, actualY,0]);
                Destroy(mazeWalls[actualX, actualY,1]);
                foreach (int d in directions)
                {
                    int coordX = 0, coordY = 0;
                    switch (d)
                    {
                        case N:
                            coordX = actualX;
                            coordY = actualY + 1;
                            break;
                        case E:
                            coordX = actualX + 1;
                            coordY = actualY;
                            break;
                        case S:
                            coordX = actualX;
                            coordY = actualY - 1;
                            break;
                        case W:
                            coordX = actualX - 1;
                            coordY = actualY;
                            break;
                    }

                    if ((maze[mapX, mapY] & d) > 0)
                    {
                        Destroy(mazeWalls[coordX, coordY,0]);
                        Destroy(mazeWalls[coordX, coordY,1]);

                    }
                }
            }
        }

        surface.BuildNavMesh();
        agent.RefreshSetup(treasure, x, y);

    }

    public void Carve(int x, int y, int[,] mazeMap)
    {
        // 0 = north, 1 = east, 2 = south, 3 = west
        int[] direction = directions.OrderBy(v => Random.value).ToArray();

        foreach (int d in direction)
        {
            int nX = x;
            int nY = y;
            switch (d)
            {
                case N: // north
                    nY++;
                    break;
                case E: // east
                    nX++;
                    break;
                case S: // south
                    nY--;
                    break;
                case W: // west
                    nX--;
                    break;
            }

            if (nX >= 0 && nX < SIZE && nY >= 0 && nY < SIZE)
            {
                if (mazeMap[nX, nY] == 0) // haven't be revisited
                {
                    mazeMap[x,y] |= d;
                    mazeMap[nX, nY] |= OppositeDirection(d);
                    Carve(nX, nY, mazeMap);
                }
            }
        }
    }

    private int OppositeDirection(int x)
    {
        switch (x)
        {
            case N:
                return S;
            case E:
                return W;
            case S:
                return N;
            case W:
                return E;
        }

        return 0;
    }

}

