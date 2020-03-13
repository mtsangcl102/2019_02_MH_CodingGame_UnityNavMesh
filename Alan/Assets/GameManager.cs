using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private Mesh _terrianMesh;
    [SerializeField] private Material _material;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject wallGroup;
    [SerializeField] private GameObject treasureChest;
    
    private CellState[,] _cells;
    private int _width;
    private int _height;
    private List<GameObject> _wallList = new List<GameObject>();
    private GameObject _treasureChest;
    private bool destinationSet = false;

    public bool UpdateDest = false;
    
    // Start is called before the first frame update
    void Start() {
        NavMesh.pathfindingIterationsPerFrame = int.MaxValue-1;
        _treasureChest = Object.Instantiate( treasureChest ) as GameObject;
        SetMazeAndDestination();
    }

    void Update() {
        if (_agent.pathStatus == NavMeshPathStatus.PathComplete 
            && _agent.remainingDistance == 0 && !destinationSet
            && Mathf.Abs(_agent.transform.position.x - _treasureChest.transform.position.x) * Mathf.Abs(_agent.transform.position.z - _treasureChest.transform.position.z) < 1 )  {
            SetMazeAndDestination();
        }
    }

    void SetMazeAndDestination() {
        Maze(15,15);
        Display();
        _agent.SetDestination( _treasureChest.transform.position );
    }

    public void Maze(int width, int height) {
        _width = width;
        _height = height;
        _cells = new CellState[width, height];
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            _cells[x, y] = CellState.Initial;
        VisitCell(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
    }

    public CellState this[int x, int y] {
        get { return _cells[x, y]; }
        set { _cells[x, y] = value; }
    }

    public IEnumerable<RemoveWallAction> GetNeighbours(Vector2 p) {
        if (p.x > 0) yield return new RemoveWallAction {Neighbour = new Vector2(p.x - 1, p.y), Wall = CellState.Left};
        if (p.y > 0) yield return new RemoveWallAction {Neighbour = new Vector2(p.x, p.y - 1), Wall = CellState.Top};
        if (p.x < _width - 1) yield return new RemoveWallAction {Neighbour = new Vector2(p.x + 1, p.y), Wall = CellState.Right};
        if (p.y < _height - 1) yield return new RemoveWallAction {Neighbour = new Vector2(p.x, p.y + 1), Wall = CellState.Bottom};
    }

    public void VisitCell(int x, int y) {
        this[x, y] |= CellState.Visited;
        foreach (var p in GetNeighbours(new Vector2(x, y)).Shuffle(new Random())
            .Where(z => !(this[(int) z.Neighbour.x, (int) z.Neighbour.y].HasFlag(CellState.Visited)))) {
            this[x, y] -= p.Wall;
            this[(int) p.Neighbour.x, (int) p.Neighbour.y] -= p.Wall.OppositeWall();
            VisitCell((int) p.Neighbour.x, (int) p.Neighbour.y);
        }
    }

    public void Display() {
        int wallCounter = 1;
        bool setChest = false;
        var firstLine = string.Empty;
        
        for (var y = 0; y < _height; y++) {
            var sbTop = new StringBuilder();
            var sbMid = new StringBuilder();
            
            for (var x = 0; x < _width; x++) {
                sbTop.Append(this[x, y].HasFlag(CellState.Top) ? "+-" : "+ ");
                sbMid.Append(this[x, y].HasFlag(CellState.Left) ? "| " : "  ");
            }

            sbTop.Append( "+");
            sbMid.Append( "|");
            
            if (firstLine == string.Empty)
                firstLine = sbTop.ToString();

            for (int i = 0; i < sbTop.Length; i++) {
                if (sbTop.ToString()[i] != ' ') {
                    InitWall(wallCounter++, new Vector3(i, 0, 2 * y));
                }
            }
            
            for (int i = 0; i < sbMid.Length; i++) {
                if (sbMid.ToString()[i] != ' ') {
                    InitWall(wallCounter++, new Vector3(i, 0, 2 * y + 1));
                }else if (Mathf.Abs(_agent.transform.position.x - i) * Mathf.Abs(_agent.transform.position.z - y) > (_width/2 * _height/2) &&
                          Random.Range(0, 100) > 95 && !setChest) {
                    setChest = true;
                    //set treasure chest
                    _treasureChest.transform.position = new Vector3(i, 0, 2 * y + 1);
                }
            }
                
        }
        
        for (int i = 0; i < firstLine.Length; i++) {
            if (firstLine.ToString()[i] != ' ') {
                InitWall(wallCounter++, new Vector3(i, 0, 2 * _height));
            }
        }

        CleanWall(wallCounter);
    }

    private void InitWall( int counter, Vector3 position ) {
        if (counter <= _wallList.Count) {
            _wallList[counter - 1].transform.position = position;
        }
        else {
            GameObject tempWall = Object.Instantiate( wall ) as GameObject;
            tempWall.transform.SetParent(wallGroup.transform);
            tempWall.transform.position = position;
            _wallList.Add(tempWall);
        }
    }

    private void CleanWall(int counter) {
        List<GameObject> toRemove = new List<GameObject>();
        for (int i = counter; i < _wallList.Count; i++) {
            toRemove.Add( _wallList[i] );
        }

        foreach ( GameObject go in toRemove ) {
            _wallList.Remove(go);
            Object.Destroy(go);
        }
    }
}

public static class Extensions {
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng) {
        var e = source.ToArray();
        for (var i = e.Length - 1; i >= 0; i--) {
            var swapIndex = UnityEngine.Random.Range(0, i + 1);
            yield return e[swapIndex];
            e[swapIndex] = e[i];
        }
    }

    public static CellState OppositeWall(this CellState orig) {
        return (CellState) (((int) orig >> 2) | ((int) orig << 2)) & CellState.Initial;
    }

    public static bool HasFlag(this CellState cs, CellState flag) {
        return ((int) cs & (int) flag) != 0;
    }
}

public enum CellState {
    Top = 1,
    Right = 2,
    Bottom = 4,
    Left = 8,
    Visited = 128,
    Initial = Top | Right | Bottom | Left,
}

public struct RemoveWallAction {
    public Vector2 Neighbour;
    public CellState Wall;
}

