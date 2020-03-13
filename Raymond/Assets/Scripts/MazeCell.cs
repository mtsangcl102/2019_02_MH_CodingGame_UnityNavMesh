// Copyright (c) Mad Head Limited All Rights Reserved

using System.Collections.Generic;

public class MazeCell
{
    public int X;
    public int Y;
    public bool IsVisited = false ;
    
    public List<MazeCell> Neighbors = new List<MazeCell>();

    public MazeCell( int x , int y )
    {
        X = x;
        Y = y;
    }
}