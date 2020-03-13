/*
 * written by Joseph Hocking 2017
 * released under MIT license
 * text of license https://opensource.org/licenses/MIT
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;
    public GameObject wallGO;
    public GameObject wallParent;
    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;

    public int[,] data
    {
        get; private set;
    }

    public float hallWidth
    {
        get; private set;
    }
    public float hallHeight
    {
        get; private set;
    }

    public int startRow
    {
        get; private set;
    }
    public int startCol
    {
        get; private set;
    }

    public int goalRow
    {
        get; private set;
    }
    public int goalCol
    {
        get; private set;
    }

    private MazeDataGenerator dataGenerator;
    //private MazeMeshGenerator meshGenerator;

    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        //meshGenerator = new MazeMeshGenerator();

        // default to walls surrounding a single empty cell
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols, bool isFirstTime){
        DisposeOldMaze();

        data = dataGenerator.FromDimensions(sizeRows, sizeCols);
        if(isFirstTime) {
            FindStartPosition();
        }
        else {
            startRow = goalRow;
            startCol = goalCol;
        }
        FindGoalPosition();

        // store values used to generate this mesh
        //hallWidth = meshGenerator.width;
        //hallHeight = meshGenerator.height;

        DisplayMaze();

    }

    private void DisplayMaze() {
        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);
        //float halfH = height * .5f;
        for(int i = 0; i <= rMax; i++) {
            for(int j = 0; j <= cMax; j++) {
                if(data[i, j] == 1) {
                    GameObject tempGO = GameObject.Instantiate(wallGO, wallParent.transform);
                    tempGO.transform.localPosition = new Vector3(i, 0, j);
                    //// floor
                    //AddQuad(Matrix4x4.TRS(
                    //    new Vector3(j * width, 0, i * width),
                    //    Quaternion.LookRotation(Vector3.up),
                    //    new Vector3(width, width, 1)
                    //), ref newVertices, ref newUVs, ref floorTriangles);

                    //// ceiling
                    //AddQuad(Matrix4x4.TRS(
                    //    new Vector3(j * width, height, i * width),
                    //    Quaternion.LookRotation(Vector3.down),
                    //    new Vector3(width, width, 1)
                    //), ref newVertices, ref newUVs, ref floorTriangles);


                    //// walls on sides next to blocked grid cells

                    //if(i - 1 < 0 || data[i - 1, j] == 1) {
                    //    AddQuad(Matrix4x4.TRS(
                    //        new Vector3(j * width, halfH, (i - .5f) * width),
                    //        Quaternion.LookRotation(Vector3.forward),
                    //        new Vector3(width, height, 1)
                    //    ), ref newVertices, ref newUVs, ref wallTriangles);
                    //}

                    //if(j + 1 > cMax || data[i, j + 1] == 1) {
                    //    AddQuad(Matrix4x4.TRS(
                    //        new Vector3((j + .5f) * width, halfH, i * width),
                    //        Quaternion.LookRotation(Vector3.left),
                    //        new Vector3(width, height, 1)
                    //    ), ref newVertices, ref newUVs, ref wallTriangles);
                    //}

                    //if(j - 1 < 0 || data[i, j - 1] == 1) {
                    //    AddQuad(Matrix4x4.TRS(
                    //        new Vector3((j - .5f) * width, halfH, i * width),
                    //        Quaternion.LookRotation(Vector3.right),
                    //        new Vector3(width, height, 1)
                    //    ), ref newVertices, ref newUVs, ref wallTriangles);
                    //}

                    //if(i + 1 > rMax || data[i + 1, j] == 1) {
                    //    AddQuad(Matrix4x4.TRS(
                    //        new Vector3(j * width, halfH, (i + .5f) * width),
                    //        Quaternion.LookRotation(Vector3.back),
                    //        new Vector3(width, height, 1)
                    //    ), ref newVertices, ref newUVs, ref wallTriangles);
                    //}
                }
            }

            //    GameObject go = new GameObject();
            //    go.transform.position = Vector3.zero;
            //    go.name = "Procedural Maze";
            //    go.tag = "Generated";

            //    MeshFilter mf = go.AddComponent<MeshFilter>();
            //    mf.mesh = meshGenerator.FromData(data);

            //    MeshCollider mc = go.AddComponent<MeshCollider>();
            //    mc.sharedMesh = mf.mesh;

            //    MeshRenderer mr = go.AddComponent<MeshRenderer>();
            //    mr.materials = new Material[2] {mazeMat1, mazeMat2};
        }
    }

        public void DisposeOldMaze()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects) {
            Destroy(go);
        }
    }

    private void FindStartPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    startRow = i;
                    startCol = j;
                    return;
                }
            }
        }
    }

    private void FindGoalPosition()
    {
        int[,] maze = data;
        //int rMax = maze.GetUpperBound(0);
        //int cMax = maze.GetUpperBound(1);
        int randomLocationX = Random.Range(maze.GetLowerBound(0), maze.GetUpperBound(0));
        int randomLocationY = Random.Range(maze.GetLowerBound(1), maze.GetUpperBound(1));
        // loop top to bottom, right to left
        for (int i = randomLocationX; i >= 0; i--)
        {
            for (int j = randomLocationY; j >= 0; j--)
            {
                if (maze[i, j] == 0)
                {
                    goalRow = i;
                    goalCol = j;
                    return;
                }
            }
        }
    }

    // top-down debug display
    void OnGUI()
    {
        if (!showDebug)
        {
            return;
        }

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        // loop top to bottom, left to right
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += "0";
                }
                else
                {
                    msg += "1";
                }
            }
            msg += "\n";
        }

        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }
}
