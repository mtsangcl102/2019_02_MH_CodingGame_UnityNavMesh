using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cell
{
    public GameObject[] mWall ;
    public GameObject mGrid;
    public Cell[] mNeighbour;
    public int i, j;
    public bool mVisited  ;

}


public class MazeSystem : MonoBehaviour
{
    const int MAZE_SIZE = 50;
    const float CELL_SIZE = 1f;

    public NavMeshSurface mSurface;
    public GameObject mPlayer;
    private Vector3 mDestination;
    public GameObject mDestinationObject;

    [SerializeField]
    public GameObject mWallPrefab;
    public GameObject mGridPrefab;

    public Cell[,] mCells;

    // Start is called before the first frame update
    void Start()
    {
        buildWalls();
        initMaze();
        mSurface.BuildNavMesh();
        resetDestination();
        Time.timeScale = 3;
    }

    // build all four walls for ALL of the cells 
    // walls will be disabled when we init the maze
    void buildWalls()
    {
        mCells = new Cell[MAZE_SIZE, MAZE_SIZE];

        float currX = -(float)(MAZE_SIZE-1)/2, currY = -(float)(MAZE_SIZE - 1) / 2;

        for( int i=0; i<MAZE_SIZE; i++ )
        {
            currY = -(float)(MAZE_SIZE - 1) / 2;
            for( int j=0; j<MAZE_SIZE; j++)
            {
                Cell cell = new Cell();
                cell.mWall = new GameObject[4];
                cell.mNeighbour = new Cell[4];
                cell.i = i;
                cell.j = j;
                cell.mGrid = Instantiate(mGridPrefab, new Vector3(currX, 0.2f, currY), Quaternion.Euler(new Vector3(0, 0, 0)));
                cell.mGrid.SetActive(false);
                // init four walls for a cell, walls are share btw cell

                // north wall
                if (j == 0)
                    cell.mWall[0] = Instantiate(mWallPrefab, new Vector3(currX, 0.5f, currY - CELL_SIZE / 2), Quaternion.Euler(new Vector3(0, 0, 0)));
                else
                {
                    cell.mWall[0] = mCells[i, j - 1].mWall[2]; // use the south wall of prev cells 
                    cell.mNeighbour[0] = mCells[i, j - 1];  // link the two cell as well 
                    mCells[i, j - 1].mNeighbour[2] = cell;
                }

                // south wall
                cell.mWall[2] = Instantiate(mWallPrefab, new Vector3(currX, 0.5f, currY+ CELL_SIZE / 2), Quaternion.Euler(new Vector3(0, 0, 0)));


                // east wall 
                if (i == 0)
                    cell.mWall[1] = Instantiate(mWallPrefab, new Vector3(currX - CELL_SIZE / 2, 0.5f, currY), Quaternion.Euler(new Vector3(0, 90f, 0)));
                else
                {
                    cell.mWall[1] = mCells[i - 1, j].mWall[3]; // use the west wall of prev cells 
                    cell.mNeighbour[1] = mCells[i - 1, j]; // link the two cell as well 
                    mCells[i - 1, j].mNeighbour[3] = cell;
                }

                // west wall
                cell.mWall[3] = Instantiate(mWallPrefab, new Vector3(currX + CELL_SIZE / 2, 0.5f, currY), Quaternion.Euler(new Vector3(0, 90f, 0)));

                currY += 1;
                mCells[i, j] = cell;
            }

            currX += 1;
        }
    }

    // pick a random NON visited neighbour (4 sides) from a cell 
    int pickRandomNeighbour(Cell currCell)
    {
        List<int> cells = new List<int>();

        for( int i=0;i<4;i++ )
        {
            if (currCell.mNeighbour[i] != null && !currCell.mNeighbour[i].mVisited)
                cells.Add(i);
        }


        if (cells.Count == 0) return -1;
        return cells[Random.Range(0, cells.Count)];
    }

    void resetMaze()
    {
        for (int i = 0; i < MAZE_SIZE; i++)
        {
            for (int j = 0; j < MAZE_SIZE; j++)
            {
                Cell cell = mCells[i, j];

                cell.mVisited = false;
                cell.mGrid.SetActive(false);
                for ( int k=0; k<4; k++ )
                {
                    cell.mWall[k].SetActive(true);
                }
            }
        }

    }

    void initMaze()
    {
        Stack<Cell> lastCell = new Stack<Cell>();
        int totalCells = MAZE_SIZE * MAZE_SIZE;
        Cell currCell = null;
        int visitedCells = 1;

        currCell = mCells[Random.Range(0, MAZE_SIZE), Random.Range(0, MAZE_SIZE)];
        currCell.mVisited = true;

        // depth first search for all cells 
        // 1. start with 1 cell
        // 2. pick a random neighbour and remove the walls between 
        // 3. mark the current cell as visited and go to that neighbour 
        // 4. push that current cell into a stack (since some of its neighbour may not be visited)
        while( visitedCells < totalCells )
        {
            int nextCellIndex = pickRandomNeighbour(currCell);

            if( nextCellIndex != -1 ) // if we have a NON visited neighbour
            {
                // do 2,3,4
                currCell.mWall[nextCellIndex].SetActive(false);
                lastCell.Push(currCell);
                currCell = currCell.mNeighbour[nextCellIndex];
                currCell.mVisited = true;
                visitedCells++;
            }
            else
            {   // if 4 neighbour is visited, just find a cell in the stack to visit
                currCell = lastCell.Pop();
            }
        }


    }

    void resetDestination()
    {
        float randomI = Random.Range(0, MAZE_SIZE);
        float randomJ = Random.Range(0, MAZE_SIZE);
        float hSize = -(float)(MAZE_SIZE - 1) / 2;

        mDestination = new Vector3(hSize+randomI, mPlayer.transform.position.y, hSize+randomJ);
        Player player = mPlayer.GetComponent<Player>() as Player;
        player.setDestination(mDestination);
        mDestinationObject.transform.position = mDestination;
    }

    // Update is called once per frame
    void Update()
    {
        // check if player reaches destination, if so, reinit the maze and new dest
        if( Vector3.Distance( mPlayer.transform.position, mDestination) <= 0.1f )
        {
            resetMaze();
            initMaze();
            mSurface.BuildNavMesh();
            resetDestination();
        }

        float hSize = (float)(MAZE_SIZE) / 2;
        int i = (int)(mPlayer.transform.position.x + hSize) ;
        int j = (int)(mPlayer.transform.position.z + hSize) ;
        mCells[i, j].mGrid.SetActive(true);
    }
}
