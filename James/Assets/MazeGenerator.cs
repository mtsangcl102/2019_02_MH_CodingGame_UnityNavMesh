//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Drawing;
// 
//namespace MazeGeneration
//{
//    public static class Extensions
//    {
//        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
//        {
//            var e = source.ToArray();
//            for (var i = e.Length - 1; i >= 0; i--)
//            {
//                var swapIndex = rng.Next(i + 1);
//                yield return e[swapIndex];
//                e[swapIndex] = e[i];
//            }
//        }
// 
//        public static CellState OppositeWall(this CellState orig)
//        {
//            return (CellState)(((int) orig >> 2) | ((int) orig << 2)) & CellState.Initial;
//        }
// 
//        public static bool HasFlag(this CellState cs,CellState flag)
//        {
//            return ((int)cs & (int)flag) != 0;
//        }
//    }
// 
//    [Flags]
//    public enum CellState
//    {
//        Top = 1,
//        Right = 2,
//        Bottom = 4,
//        Left = 8,
//        Visited = 128,
//        Initial = Top | Right | Bottom | Left,
//    }
// 
//    public struct RemoveWallAction
//    {
//        public Point Neighbour;
//        public CellState Wall;
//    }
// 
//    
//    public class MazeGenerator
//    {
//        private int _width;
//        private Block[] _blocks;
//        
//        public Generate( int width )
//        {
//            _width = width;
//            _blocks = new Block[ width * width ];
//            _VisitCell( );
//        }
//        
//        private void _VisitCell( int index )
//        {
//            _blocks[ index ].IsVisited = true;
//            int x = index % -width;
//            int y = index / width;
//            
//            int[] randomArray = { 0, 1, 2 ,3 };
//            for( int i = 0; i < randomArray.Length; i++ )
//            {
//                int randomNumber = Random.Range( 0, randomArray.Length );
//                int a = randomArray[ i ];
//                randomArray[ i ] = randomArray[ randomNumber ];
//                randomArray[ randomNumber ] = randomArray[ i ];
//            }
//            
//            for( int i = 0; i < 4; i++ )
//            {
//                if( i == randomArray[ 0 ] && x > 0 )
//                {
//                    _blocks[ index ].HasLeftWall = false;
//                    _blocks[ (y) * _width + x - 1 ].HasRightWall = false;
//                    _VisitCell( (y) * _width + x - 1 );
//                }
//                
//                if( i == randomArray[ 1 ] && y > 0 )
//                {
//                    _blocks[ index ].HasTopWall = false;
//                    _blocks[ (y - 1) * _width + x ].HasBottomWall = false;
//                    _VisitCell( (y - 1) * _width + x );
//                }
//                
//                if( i == randomArray[ 2 ] && x < _width - 1 )
//                {
//                    _blocks[ index ].HasRightWall = false;
//                    _blocks[ (y) * _width + x + 1 ].HasLeftWall = false;
//                    _VisitCell( (y) * _width + x + 1 );
//                }
//                
//                if( i == randomArray[ 3 ] && y < _width - 1 )
//                {
//                    _blocks[ index ].HasBottomWall = false;
//                    _blocks[ (y + 1) * _width + x ].HasTopWall = false;
//                    _VisitCell( (y + 1) * _width + x );
//                }
//            }
//        }
//        
//        class Block
//        {
//            public bool HasTopWall = true;
//            public bool HasBottomWall = true;
//            public bool HasLeftWall = true;
//            public bool HasRightWall = true;
//            public bool IsVisited = false;
//        }
//    }
//    
//    public class Maze
//    {
//
//        
//        private readonly CellState[,] _cells;
//        private readonly int _width;
//        private readonly int _height;
//        private readonly Random _rng;
// 
//        public Maze(int width, int height)
//        {
//            _width = width;
//            _height = height;
//            _cells = new CellState[width, height];
//            for(var x=0; x<width; x++)
//                for(var y=0; y<height; y++)
//                    _cells[x, y] = CellState.Initial;
//            _rng = new Random();
//            VisitCell(_rng.Next(width), _rng.Next(height));
//        }
// 
//        public CellState this[int x, int y]
//        {
//            get { return _cells[x,y]; }
//            set { _cells[x,y] = value; }
//        }
// 
//        public IEnumerable<RemoveWallAction> GetNeighbours(Point p)
//        {
//            if (p.X > 0) yield return new RemoveWallAction {Neighbour = new Point(p.X - 1, p.Y), Wall = CellState.Left};
//            if (p.Y > 0) yield return new RemoveWallAction {Neighbour = new Point(p.X, p.Y - 1), Wall = CellState.Top};
//            if (p.X < _width-1) yield return new RemoveWallAction {Neighbour = new Point(p.X + 1, p.Y), Wall = CellState.Right};
//            if (p.Y < _height-1) yield return new RemoveWallAction {Neighbour = new Point(p.X, p.Y + 1), Wall = CellState.Bottom};
//        }
// 
//        public void VisitCell(int x, int y)
//        {
//            this[x,y] |= CellState.Visited;
//            foreach (var p in GetNeighbours(new Point(x, y)).Shuffle(_rng).Where(z => !(this[z.Neighbour.X, z.Neighbour.Y].HasFlag(CellState.Visited))))
//            {
//                this[x, y] -= p.Wall;
//                this[p.Neighbour.X, p.Neighbour.Y] -= p.Wall.OppositeWall();
//                VisitCell(p.Neighbour.X, p.Neighbour.Y);
//            }
//        }
// 
////        public void Display()
////        {
////            var firstLine = string.Empty;
////            for (var y = 0; y < _height; y++)
////            {
////                var sbTop = new StringBuilder();
////                var sbMid = new StringBuilder();
////                for (var x = 0; x < _width; x++)
////                {
////                    sbTop.Append(this[x, y].HasFlag(CellState.Top) ? "+--" : "+  ");
////                    sbMid.Append(this[x, y].HasFlag(CellState.Left) ? "|  " : "   ");
////                }
////                if (firstLine == string.Empty)
////                    firstLine = sbTop.ToString();
////                Debug.WriteLine(sbTop + "+");
////                Debug.WriteLine(sbMid + "|");
////                Debug.WriteLine(sbMid + "|");
////            }
////            Debug.WriteLine(firstLine);
////        }
//    }
// 
////    class Program
////    {
////        static void Main(string[] args)
////        {
////            var maze = new Maze(20, 20);
////            maze.Display();
////        }
////    }
//}