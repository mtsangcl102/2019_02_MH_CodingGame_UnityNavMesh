using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Main : MonoBehaviour
{
    public GameObject UnitCube;
    public NavMeshSurface Floor;
    public int Size = 15 ;
    public int MazeCount = 3; 

    // AI
    public NavMeshAgent NavMeshAgent;
    
    // Treasure Model
    public GameObject Treasure;
    
    private GameObject[] _mazes;
    private int _currentMazeIndex = -1;
    private bool _inited = false;
    private GameObject _ai;
    private GameObject _treasureObject; 
    
    void Start()
    {
        _mazes = new GameObject[ MazeCount ];
        
        for ( var i = 0 ; i < MazeCount ; ++i )
        {
            var maze = MazeGenerator.GenerateMaze( Size );
            _mazes[i] = MazeGenerator.CreateMazeGameObject( maze , Size , UnitCube , Floor );
            
            _mazes[i].SetActive( false );
        }
     
    }

    // Update is called once per frame
    void Update()
    {
        if ( !_inited )
        {
            _inited = true;
            _currentMazeIndex = (_currentMazeIndex + 1) % MazeCount;
            _mazes[_currentMazeIndex].SetActive( true );

            // create the treasure
            _treasureObject = Instantiate( Treasure );

            // create the player
            _ai = Instantiate( NavMeshAgent.gameObject );
            _ai.transform.position = new Vector3(10,0,10);
            
            // set position of the treasure
            var targetPosition = _GetRandomPosition();
            _treasureObject.transform.position = targetPosition;

            _ai.transform.position = _GetRandomPosition();
            var agent = _ai.GetComponent<NavMeshAgent>();
            agent.SetDestination( targetPosition );

        }

        var dx = _ai.transform.position.x - _treasureObject.transform.position.x;
        var dy = _ai.transform.position.z - _treasureObject.transform.position.z;
        
        // Check if the agent react the target
        if ( dx * dx + dy * dy < 0.1f )
        {
            // update the maze
            _mazes[_currentMazeIndex].SetActive( false );
            
            _currentMazeIndex = (_currentMazeIndex + 1) % MazeCount;
            _mazes[_currentMazeIndex].SetActive( true );

            // set position of the treasure
            var targetPosition = _GetRandomPosition();
            _treasureObject.transform.position = targetPosition;

            // set position of the player
            _ai.transform.position = _GetRandomPosition();
            var agent = _ai.GetComponent<NavMeshAgent>();
            
            // set the target position
            agent.SetDestination( targetPosition );
            
        }
    }

    private Vector3 _GetRandomPosition()
    {
        return new Vector3( Random.Range( 0 , Size ) * 2 , 0 , Random.Range( 0 , Size ) * 2 );
    }
}
