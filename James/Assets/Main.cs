using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Main : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private GameObject _mazeGameObject;
    private Vector3 _endPosition;
    
    private void Start()
    {
        _CreateMaze();
    }
    
    private void _CreateMaze()
    {
        _mazeGameObject = new GameObject();
        
        List<GameObject> _gameObjects = new List<GameObject>();
        
        GameObject wallPrefab = (GameObject)Resources.Load( "Wall" );
        GameObject floorPrefab = (GameObject)Resources.Load( "Floor" );
        GameObject startPrefab = (GameObject)Resources.Load( "Start" );
        GameObject targetPrefab = (GameObject)Resources.Load( "Target" );
        
        int size = 51;
        
        bool[] isWalls = new MazeGenerator().Generate( size );
        
        for( int i = 0; i < isWalls.Length; i++ )
        {
            int x = i % size;
            int z = i / size;
            
            if( isWalls[ i ] )
            {
                _gameObjects.Add( GameObject.Instantiate( wallPrefab, new Vector3( x, 1, z ), Quaternion.identity ) );
            }
            _gameObjects.Add( GameObject.Instantiate( floorPrefab, new Vector3( x, 0, z ), Quaternion.identity ) );
        }
        
        int startIndex = Random.Range( 0, size * size );
        while( isWalls[ startIndex ] ){
            startIndex = Random.Range( 0, size * size );
        }
        
        
        int endIndex = Random.Range( 0, size * size );
        while( endIndex == startIndex || isWalls[ endIndex ]){
            endIndex = Random.Range( 0, size * size );
        }
        
        startPrefab = GameObject.Instantiate( startPrefab, new Vector3( startIndex % size, 1, startIndex / size ), Quaternion.identity );
        targetPrefab = GameObject.Instantiate( targetPrefab, new Vector3( endIndex % size, 1, endIndex / size ), Quaternion.identity );
        
        NavMeshHandler.Build( _gameObjects );
        
        _navMeshAgent = startPrefab.GetComponent<NavMeshAgent>();
        _navMeshAgent.SetDestination( targetPrefab.transform.position );
        _endPosition = targetPrefab.transform.position;
        
        startPrefab.transform.parent = _mazeGameObject.transform;
        targetPrefab.transform.parent = _mazeGameObject.transform;
        for( int i = 0; i < _gameObjects.Count; i++ ){
            _gameObjects[ i ].transform.parent = _mazeGameObject.transform;
        }
    }

    private void Update()
    {
        if( _IsSimilar( _endPosition, _navMeshAgent.transform.position ) )
        {
            GameObject.DestroyImmediate( _mazeGameObject  );
            _CreateMaze();
        }
    }
    
    private bool _IsSimilar( Vector3 a, Vector3 b )
    {
        if( (a-b).sqrMagnitude <= 0.15f * 0.15f ){
            return true;
        }
        return false;
    }
    
    public class NavMeshHandler
    {
        private static NavMeshDataInstance navMeshDataInstance;
        
        public static void Build( List<GameObject> gameObjects )
        {
             List<NavMeshBuildSource> buildSources = new List<NavMeshBuildSource>();
             
            for( int i = 0; i < gameObjects.Count; i++ )
            {
                NavMeshBuildSource navMeshSource = new NavMeshBuildSource();
                navMeshSource.shape = NavMeshBuildSourceShape.Mesh;
                navMeshSource.sourceObject = gameObjects[ i ].GetComponent<MeshFilter>().sharedMesh;
                navMeshSource.transform = gameObjects[ i ].transform.localToWorldMatrix;
                navMeshSource.area = 0;
                buildSources.Add( navMeshSource );
            }
             
            NavMeshBuildSettings navSetting = new NavMeshBuildSettings();
            navSetting.agentRadius = 0.4f;
        
             NavMeshData navData = UnityEngine.AI.NavMeshBuilder.BuildNavMeshData( 
                navSetting,
                buildSources,
                new Bounds(Vector3.zero, new Vector3(10000, 10000, 10000)),
                Vector3.zero,
                Quaternion.identity
                );
             navMeshDataInstance.Remove();
             navMeshDataInstance = NavMesh.AddNavMeshData(navData);
        }
    }
    
    public class MazeGenerator
    {
        private int _blockWidth;
        private Block[] _blocks;
        
        public bool[] Generate( int mazeWidth )
        {
            _blockWidth = mazeWidth / 2;
            _blocks = new Block[ _blockWidth * _blockWidth ];
            for( int i = 0; i < _blocks.Length; i++ ){
                _blocks[ i ] = new Block();
            }
            _VisitCell( Random.Range( 0, _blocks.Length ) );
            
            bool[] returnValue = new bool[ mazeWidth * mazeWidth ];
            
            for( int i = 0; i < returnValue.Length; i++ )
            {
                returnValue[ i ] = true;
            }
            
            for( int x = 0; x < _blockWidth; x++ )
            {
                for( int y = 0; y < _blockWidth; y++ )
                {
                    int mazeX = x * 2 + 1;
                    int mazeY = y * 2 + 1;
                    returnValue[ (mazeY) * mazeWidth + mazeX ] = false;
                    
                    if( ! _blocks[ (y) * _blockWidth + x ].HasTopWall ){
                        returnValue[ (mazeY - 1) * mazeWidth + mazeX ] = false;
                    }
                    if( ! _blocks[ (y) * _blockWidth + x ].HasLeftWall ){
                        returnValue[ (mazeY) * mazeWidth + mazeX - 1 ] = false;
                    }
                }
            }
            return returnValue;
        }

        private void _VisitCell( int index )
        {
            _blocks[ index ].IsVisited = true;
            int x = index % -_blockWidth;
            int y = index / _blockWidth;
            
            int[] randomArray = { 0, 1, 2 ,3 };
            for( int i = 0; i < randomArray.Length; i++ )
            {
                int randomNumber = Random.Range( 0, randomArray.Length );
                int a = randomArray[ i ];
                randomArray[ i ] = randomArray[ randomNumber ];
                randomArray[ randomNumber ] = a;
            }
            
            for( int i = 0; i < 4; i++ )
            {
                if( i == randomArray[ 0 ] && x > 0 && ! _blocks[ (y) * _blockWidth + x - 1 ].IsVisited )
                {
                    _blocks[ index ].HasLeftWall = false;
                    _blocks[ (y) * _blockWidth + x - 1 ].HasRightWall = false;
                    _VisitCell( (y) * _blockWidth + x - 1 );
                }
                
                if( i == randomArray[ 1 ] && y > 0 && ! _blocks[ (y - 1) * _blockWidth + x ].IsVisited )
                {
                    _blocks[ index ].HasTopWall = false;
                    _blocks[ (y - 1) * _blockWidth + x ].HasBottomWall = false;
                    _VisitCell( (y - 1) * _blockWidth + x );
                }
                
                if( i == randomArray[ 2 ] && x < _blockWidth - 1 && ! _blocks[ (y) * _blockWidth + x + 1 ].IsVisited )
                {
                    _blocks[ index ].HasRightWall = false;
                    _blocks[ (y) * _blockWidth + x + 1 ].HasLeftWall = false;
                    _VisitCell( (y) * _blockWidth + x + 1 );
                }
                
                if( i == randomArray[ 3 ] && y < _blockWidth - 1 && ! _blocks[ (y + 1) * _blockWidth + x ].IsVisited )
                {
                    _blocks[ index ].HasBottomWall = false;
                    _blocks[ (y + 1) * _blockWidth + x ].HasTopWall = false;
                    _VisitCell( (y + 1) * _blockWidth + x );
                }
            }
        }
        
        class Block
        {
            public bool HasTopWall = true;
            public bool HasBottomWall = true;
            public bool HasLeftWall = true;
            public bool HasRightWall = true;
            public bool IsVisited = false;
        }
    }
}
