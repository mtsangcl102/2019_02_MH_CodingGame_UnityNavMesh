using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MazeGen : MonoBehaviour
{
    [Serializable]
    struct MazeCell
    {
        public int x, y;
        public bool isVisited;
        public bool hasWallN, hasWallE, hasWallS, hasWallW;
    }

    [SerializeField] private int _mazeSize;
    [SerializeField] private GameObject _wallPrefab, _endEffectPrefab;
    [SerializeField] private Transform _goal;
    [SerializeField] private Agent _agent;
    [SerializeField] private float _wallWidth, _wallHeight;    
    [SerializeField] private Material _wallMaterial;
    [SerializeField] private float _HueShiftSpeed;


    EntityManager _manager;
    private Entity _entityPrefab;
    
    private MazeCell[,] _mazeCellList;
    List<GameObject> wallList = new List<GameObject>();

    private bool end;
    private float _timer;

    public void GenMaze(int size)
    {
        _mazeCellList = new MazeCell[size, size];

        for (int x = 0; x < _mazeCellList.GetLength(0); x++)
        {
            for (int y = 0; y < _mazeCellList.GetLength(1); y++)
            {
                Vector3 pos = new Vector3(_wallWidth / 2f + _wallWidth * x, 0f, _wallWidth / 2f + _wallWidth * x);
                
                _mazeCellList[x, y] = new MazeCell()
                {
                    x = x,
                    y = y,
                    hasWallN = true,
                    hasWallE = true,
                    hasWallS = true,
                    hasWallW = true
                };
            }
        }

        void VisitCell(int x, int y, int lastX, int lastY)
        {
            if(x < 0 || x >= size || y < 0 || y >= size)
                return;
            
            if(_mazeCellList[x, y].isVisited)
                return;
            
            
            List<int> dirList = new List<int>(){1, 2, 3, 4};

            if (lastX >= 0 && lastX < size && lastY >= 0 && lastY < size)
            {
                if (x > lastX)
                    if (_mazeCellList[x, y].hasWallW)
                    {
                        _mazeCellList[x, y].isVisited = true;
                        _mazeCellList[x, y].hasWallW = false;
                        _mazeCellList[lastX, lastY].hasWallE = false;
                    }

                if (x < lastX)
                    if (_mazeCellList[x, y].hasWallE)
                    {
                        _mazeCellList[x, y].isVisited = true;
                        _mazeCellList[x, y].hasWallE = false;
                        _mazeCellList[lastX, lastY].hasWallW = false;
                    }

                if (y > lastY)
                    if (_mazeCellList[x, y].hasWallS)
                    {
                        _mazeCellList[x, y].isVisited = true;
                        _mazeCellList[x, y].hasWallS = false;
                        _mazeCellList[lastX, lastY].hasWallN = false;
                    }

                if (y < lastY)
                    if (_mazeCellList[x, y].hasWallN)
                    {
                        _mazeCellList[x, y].isVisited = true;
                        _mazeCellList[x, y].hasWallN = false;
                        _mazeCellList[lastX, lastY].hasWallS = false;
                    }
            }


            for (int i = 0; i < 4; i++)
            {
                int dir = dirList[UnityEngine.Random.Range(0, dirList.Count)];
                dirList.Remove(dir);

                switch (dir)
                {
                    case 1:
                        VisitCell(x, y + 1, x, y);
                        break;
                    case 2:
                        VisitCell(x + 1, y, x, y);
                        break;
                    case 3:
                        VisitCell(x, y - 1, x, y);
                        break;
                    case 4:
                        VisitCell(x - 1, y, x, y);
                        break;
                }
            }
            if(dirList.Count > 0)
                Debug.Log($"count {dirList.Count} {x}, {y}");
        };

        int tempX = UnityEngine.Random.Range(0, size),
            tempY = UnityEngine.Random.Range(0, size);
        VisitCell(tempX, tempY, tempX, tempY);
        
        for (int x = 0; x < _mazeCellList.GetLength(0); x++)
        {
            for (int y = 0; y < _mazeCellList.GetLength(1); y++)
            {
                Vector3 pos = new Vector3(_wallWidth / 2f + _wallWidth * x, 0f, _wallWidth / 2f + _wallWidth * y);

                if (_mazeCellList[x, y].hasWallN)
                {
                    Transform t = Instantiate(_wallPrefab).GetComponent<Transform>();
                    t.position = new Vector3(pos.x, _wallHeight / 2f, pos.z + _wallWidth / 2f);
                    wallList.Add(t.gameObject);
                }
                if (_mazeCellList[x, y].hasWallE)
                {
                    Transform t = Instantiate(_wallPrefab).GetComponent<Transform>();
                    t.position = new Vector3(pos.x + _wallWidth / 2f, _wallHeight / 2f, pos.z);
                    t.Rotate(0f, 90f, 0f);
                    wallList.Add(t.gameObject);
                }
                if (y == 0 && _mazeCellList[x, y].hasWallS)
                {
                    Transform t = Instantiate(_wallPrefab).GetComponent<Transform>();
                    t.position = new Vector3(pos.x, _wallHeight / 2f, pos.z - _wallWidth / 2f);
                    wallList.Add(t.gameObject);
                }
                if (x == 0 && _mazeCellList[x, y].hasWallW)
                {
                    Transform t = Instantiate(_wallPrefab).GetComponent<Transform>();
                    t.position = new Vector3(pos.x - _wallWidth / 2f, _wallHeight / 2f, pos.z);
                    t.Rotate(0f, 90f, 0f);
                    wallList.Add(t.gameObject);
                }
            }
        }
    }

    
    void SetAgent(int x, int y)
    {
        _agent.SetPos(new Vector3(_wallWidth / 2f + _wallWidth * x, 2f, _wallWidth / 2f + _wallWidth * y));
    }
    
    void SetGoal(int x, int y)
    {
        _goal.position = new Vector3(_wallWidth / 2f + _wallWidth * x, 1.5f, _wallWidth / 2f + _wallWidth * y);
        _agent.SetTarget(_goal);
    }



    void StartMazeFlow()
    {
        foreach (var wall in wallList)
        {
            Destroy(wall);
        }
        wallList = new List<GameObject>();
        
        GenMaze(_mazeSize);
        
        int goalX = UnityEngine.Random.Range(0, _mazeSize),
            goalY = UnityEngine.Random.Range(0, _mazeSize);
        SetGoal(goalX, goalY);
        
        
        int startX = UnityEngine.Random.Range(0, _mazeSize),
            startY = UnityEngine.Random.Range(0, _mazeSize);
        SetAgent(startX, startY);
        
//        FindTargetInMaze(startX, startY, goalX, goalY);
        
        _agent.StartAgent(); 

        end = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _manager = World.Active.EntityManager;
        _entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_wallPrefab, World.Active);

        StartMazeFlow();
    }

    private void Update()
    {
        Color c = Color.HSVToRGB((Time.time * _HueShiftSpeed) % 1f, 1f, 1f);

        _wallMaterial.color = c;
        _wallMaterial.SetColor("_EmissionColor", c * 3f);

        if (Vector3.Distance(_agent.transform.position, _goal.position) < 1f && !end)
        {
            _agent.StopAgent();
            
            GameObject go = Instantiate(_endEffectPrefab);
            go.transform.position = _goal.position;
            Destroy(go, 3f);
            
            Invoke("StartMazeFlow", 5f);
            end = true;
        }
    }
}
