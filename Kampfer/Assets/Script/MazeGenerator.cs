using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour {
    public int width, height;
    public Material brick;
    private int[,] Maze;
    private List<Vector3> pathMazes = new List<Vector3>();
    private Stack<Vector2> _tiletoTry = new Stack<Vector2>();
    private List<Vector2> offsets = new List<Vector2> { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    private System.Random rnd = new System.Random();

    private List<GameObject> blocks = new List<GameObject>();
//    private int _width, _height;
    private Vector2 _currentTile;
    public Vector2 CurrentTile
    {
        get { return _currentTile; }
        private set
        {
            if (value.x < 1 || value.x >= this.width - 1 || value.y < 1 || value.y >= this.height - 1)
            {
                throw new ArgumentException("CurrentTile must be within the one tile border all around the maze");
            }
            if (value.x % 2 == 1 || value.y % 2 == 1)
            { _currentTile = value; }
            else
            {
                throw new ArgumentException("The current square must not be both on an even X-axis and an even Y-axis, to ensure we can get walls around all tunnels");
            }
        }
    }
 
    private static MazeGenerator instance;
    public static MazeGenerator Instance
    {
        get
        {
            return instance;
        }
    }
 
    void Awake()
    {
        instance = this;
    }
 
    void Start()
    {
        GenerateMaze();
    }

    
    public NavMeshSurface Surface;
    public GameObject targetGO;
    public GameObject planeGO;
    private float mazeSize = -1f;
    private float lastSize = -1f;
    private void FixedUpdate()
    {
        if (lastSize != mazeSize)
        {
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = mazeSize / 2f;
            Camera.main.transform.position = new Vector3(mazeSize / 2f, 10f, mazeSize / 2f);
            lastSize = mazeSize;
        }
        Camera.main.transform.localEulerAngles = transform.localEulerAngles;
    }

    public void GenerateMaze()
    {
        var targetNavMeshAgent = targetGO.GetComponent<NavMeshAgent>();
        targetNavMeshAgent.enabled = false;
        
        foreach (var block in blocks)
        {
            Destroy(block.gameObject);
        }
        blocks = new List<GameObject>();
        mazeSize = Mathf.Max(width, height);
        Maze = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Maze[x, y] = 1;
            }
        }
        CurrentTile = Vector2.one;
        _tiletoTry.Push(CurrentTile);
        Maze = CreateMaze();
        GameObject ptype = null;

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        for (int i = 0; i <= Maze.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= Maze.GetUpperBound(1); j++)
            {
                if (Maze[i, j] == 1)
                {
                    ptype = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    ptype.transform.position = new Vector3(i * ptype.transform.localScale.x, j * ptype.transform.localScale.y, 0);
                    if (brick != null)
                    {
                        ptype.GetComponent<Renderer>().material = brick;
                    }
                    ptype.transform.parent = transform;
                    var navMeshModifier = ptype.AddComponent<NavMeshModifier>();
                    navMeshModifier.overrideArea = true;
                    navMeshModifier.area = 1;
                    blocks.Add(ptype);
                }
                else if (Maze[i, j] == 0)
                {
                    pathMazes.Add(new Vector3(i, j, 0));
                }
 
            }
        }
        
        transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        transform.localPosition = new Vector3(0f, 0f, 0.5f);
        
        planeGO.transform.localPosition = new Vector3(mazeSize / 2f - 0.5f, -0.5f, mazeSize / 2f);
        planeGO.transform.localScale = new Vector3(mazeSize / 10f, 1f, mazeSize / 10f);
        
        var random = Random.insideUnitCircle * mazeSize;
        targetGO.transform.position = new Vector3(Mathf.Abs(random.x), 0f, Mathf.Abs(random.y));
        targetNavMeshAgent.enabled = true;
//        Debug.LogError(random);
        Surface.BuildNavMesh();
    }
 
    public int[,] CreateMaze()
        {
            //local variable to store neighbors to the current square
            //as we work our way through the maze
            List<Vector2> neighbors;
            //as long as there are still tiles to try
            while (_tiletoTry.Count > 0)
            {
                //excavate the square we are on
                Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 0;
 
                //get all valid neighbors for the new tile
                neighbors = GetValidNeighbors(CurrentTile);
 
                //if there are any interesting looking neighbors
                if (neighbors.Count > 0)
                {
                    //remember this tile, by putting it on the stack
                    _tiletoTry.Push(CurrentTile);
                    //move on to a random of the neighboring tiles
                    CurrentTile = neighbors[rnd.Next(neighbors.Count)];
                }
                else
                {
                    //if there were no neighbors to try, we are at a dead-end
                    //toss this tile out
                    //(thereby returning to a previous tile in the list to check).
                    CurrentTile = _tiletoTry.Pop();
                }
            }
 
            return Maze;
        }
        /// <summary>
        /// Get all the prospective neighboring tiles
        /// </summary>
        /// <param name="centerTile">The tile to test</param>
        /// <returns>All and any valid neighbors</returns>
        private List<Vector2> GetValidNeighbors(Vector2 centerTile)
        {
 
            List<Vector2> validNeighbors = new List<Vector2>();
 
            //Check all four directions around the tile
            foreach (var offset in offsets)
            {
                //find the neighbor's position
                Vector2 toCheck = new Vector2(centerTile.x + offset.x, centerTile.y + offset.y);
 
                //make sure the tile is not on both an even X-axis and an even Y-axis
                //to ensure we can get walls around all tunnels
                if (toCheck.x % 2 == 1 || toCheck.y % 2 == 1)
                {
                    //if the potential neighbor is unexcavated (==1)
                    //and still has three walls intact (new territory)
                    if (Maze[(int)toCheck.x, (int)toCheck.y] == 1 && HasThreeWallsIntact(toCheck))
                    {
                        //add the neighbor
                        validNeighbors.Add(toCheck);
                    }
                }
            }
 
            return validNeighbors;
        }
 
 
        /// <summary>
        /// Counts the number of intact walls around a tile
        /// </summary>
        /// <param name="Vector2ToCheck">The coordinates of the tile to check</param>
        /// <returns>Whether there are three intact walls (the tile has not been dug into earlier.</returns>
        private bool HasThreeWallsIntact(Vector2 Vector2ToCheck)
        {
            int intactWallCounter = 0;
 
            //Check all four directions around the tile
            foreach (var offset in offsets)
            {
                //find the neighbor's position
                Vector2 neighborToCheck = new Vector2(Vector2ToCheck.x + offset.x, Vector2ToCheck.y + offset.y);
 
                //make sure it is inside the maze, and it hasn't been dug out yet
                if (IsInside(neighborToCheck) && Maze[(int)neighborToCheck.x, (int)neighborToCheck.y] == 1)
                {
                    intactWallCounter++;
                }
            }
 
            //tell whether three walls are intact
            return intactWallCounter == 3;
 
        }
 
        private bool IsInside(Vector2 p)
        {
            return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
        }
}