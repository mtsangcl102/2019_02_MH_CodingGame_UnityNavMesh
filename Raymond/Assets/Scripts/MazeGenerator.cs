using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public static class MazeGenerator
{
    private static System.Random rng = new System.Random();  

    public static void Shuffle<T>(this IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }

    public static MazeCell[] GenerateMaze( int size )
    {
        Debug.Log( $"Start to generate maze: {size} x {size}" );

        // Init a grid board
        var mazeCells = new MazeCell[size * size];

        for ( var y = 0 ; y < size ; ++y )
        for ( var x = 0 ; x < size ; ++x )
        {
            var mazeCell = new MazeCell( x , y );
            mazeCells[y * size + x] = mazeCell;
        }

        var offsets = new[] { new[] { -1 , 0 } , new[] { 0 , -1 } , new[] { 1 , 0 } , new[] { 0 , 1 } };

        // Init edges
        for ( var j = 0 ; j < size ; ++j )
        for ( var i = 0 ; i < size ; ++i )
        {
            var currentCell = mazeCells[j * size + i];

            for ( var o = 0 ; o < 4 ; ++o )
            {
                var offset = offsets[o];

                var neighborX = i + offset[0];
                var neighborY = j + offset[1];

                if ( neighborX < 0 || neighborX >= size || neighborY < 0 || neighborY >= size )
                    continue;

                var neighborCell = mazeCells[neighborY * size + neighborX];
                currentCell.Neighbors.Add( neighborCell );
            }
        }

        // Start to generate the maze using DFS
        var cellStack = new Stack<MazeCell>();

        // Randomly pick one cell
        var cell = mazeCells[Random.Range( 0 , size * size )];
        cell.IsVisited = true;
        cellStack.Push( cell );

        while ( cellStack.Count > 0 )
        {
            cell = cellStack.Peek();

            // shuffle the neighbor list
            cell.Neighbors.Shuffle();
            
            for ( var i = cell.Neighbors.Count - 1 ; i >= 0 ; --i )
            {
                var cellNeighbor = cell.Neighbors[i];

                if ( cellNeighbor.IsVisited ) continue;

                // remove the edge
                cell.Neighbors.Remove( cellNeighbor );
                cellNeighbor.Neighbors.Remove( cell );

                cellNeighbor.IsVisited = true;
                cellStack.Push( cellNeighbor );
                break;
            }

            // no more neighbor is added
            if ( cell == cellStack.Peek() )
            {
                cellStack.Pop();
            }
        }

        Debug.Log( $"Generate Maze finished!" );

        return mazeCells;
    }

    public static GameObject CreateMazeGameObject( MazeCell[] mazeCells , int mazeSize ,  GameObject unitCube , NavMeshSurface floor )
    {
        var root = new GameObject();

        GameObject cube = null;
        
        // var offsets = new[] { new[] { 1 , 0 } , new[] { 0 , 1 } };

        // floor
        var f = GameObject.Instantiate( floor , root.transform );
        f.transform.localScale = new Vector3( mazeSize * 2 + 1 , 1 , mazeSize * 2 + 1 );
        f.transform.position = new Vector3( mazeSize -1 , -0.5f , mazeSize-1 ) ;
        
        // top edge
        cube = GameObject.Instantiate( unitCube , root.transform );
        cube.transform.position = new Vector3( -1 , 0.5f , -1 );
            
        for ( var x = 0 ; x < mazeSize ; ++x )
        {
            cube = GameObject.Instantiate( unitCube , root.transform );
            cube.transform.position = new Vector3( x * 2 + 0 , 0.5f , -1 );
                
            cube = GameObject.Instantiate( unitCube , root.transform );
            cube.transform.position = new Vector3( x * 2 + 1 , 0.5f , -1 );
        }

        for ( var y = 0 ; y < mazeSize ; ++y )
        {
            // left edge
            cube = GameObject.Instantiate( unitCube , root.transform );
            cube.transform.position = new Vector3( -1 , 0.5f , y * 2 + 0 );
                
            cube = GameObject.Instantiate( unitCube , root.transform );
            cube.transform.position = new Vector3( -1 , 0.5f , y * 2 + 1 );

            for ( var x = 0 ; x < mazeSize ; ++x )
            {
                var currentCell = mazeCells[y * mazeSize + x];
                
                // connected with right cell ?
                var rightCell = x == mazeSize - 1 ? null : mazeCells[y * mazeSize + x + 1];

                if ( currentCell.Neighbors.Contains( rightCell ) || rightCell == null )
                {
                    cube = GameObject.Instantiate( unitCube , root.transform );
                    cube.transform.position = new Vector3( x * 2 + 1 , 0.5f , y * 2 );
                }
                
                // connected with bottom cell ?
                var bottomCell = y == mazeSize - 1 ? null : mazeCells[(y + 1) * mazeSize + x];
                
                if ( currentCell.Neighbors.Contains( bottomCell ) || bottomCell == null )
                {
                    cube = GameObject.Instantiate( unitCube , root.transform );
                    cube.transform.position = new Vector3( x * 2 , 0.5f , y * 2 + 1 );
                }
                
                // corner
                cube = GameObject.Instantiate( unitCube , root.transform );
                cube.transform.position = new Vector3( x * 2 + 1 , 0.5f , y * 2 + 1 );
            }
        }

        var surface = f.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
        
        return root;
    }
}