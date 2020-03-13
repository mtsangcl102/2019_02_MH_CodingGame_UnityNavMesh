using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(MazeConstructor))]
public class GameController : MonoBehaviour
{

    public GameObject _agentGO;
    public GameObject _destinationG0;
    public GameObject lightGO;
    public int completedTimes;
    [SerializeField] private int width = 0;
    [SerializeField] private int height = 0;
    [SerializeField] private bool _hasReachedDestination;
    [SerializeField] private Vector3 _destination;

    private MazeConstructor _generator;
    private bool _isFirstTime = true;
    private NavMeshAgent _navAgent;
    private float time;

    void Start() {
        _agentGO = GameObject.Instantiate(_agentGO, this.transform);
        _destinationG0 = GameObject.Instantiate(_destinationG0, this.transform);
        _generator = GetComponent<MazeConstructor>();
        _navAgent = _agentGO.GetComponent<NavMeshAgent>();
        StartNewMaze();
        _isFirstTime = false;
    }

    private void StartNewMaze()
    {
        _generator.GenerateNewMaze(width, height, _isFirstTime);
        _agentGO.transform.position = new Vector3(_generator.startRow, 0f, _generator.startCol);
        _destination = new Vector3(_generator.goalRow, 0f, _generator.goalCol);
        _destinationG0.transform.position = _destination;
        _navAgent.SetDestination(_destination);
    }

    void Update()
    {
        time = Time.deltaTime;
        if(Vector3.Distance(_navAgent.transform.position, _destinationG0.transform.position) < 0.1f) {
            completedTimes++;
            StartNewMaze();
        }
        _destinationG0.transform.localEulerAngles = new Vector3(_destinationG0.transform.localEulerAngles.x, _destinationG0.transform.localEulerAngles.y + time * 30f, _destinationG0.transform.localEulerAngles.z);

    }
}
