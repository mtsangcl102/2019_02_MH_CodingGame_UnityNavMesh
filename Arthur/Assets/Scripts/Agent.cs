using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

public class Agent : MonoBehaviour
{
    private NavMeshAgent _navAgent;
    private Transform _target;

    private bool _stoped;
    
    public void SetPos(Vector3 vec)
    {
//        transform.position = vec;
        _navAgent.Move(vec - transform.position);
        _navAgent.isStopped = true;
    }

    public void SetTarget(Transform t)
    {
        _target = t;
    }

    public void StartAgent()
    {
        _navAgent.isStopped = false;
        _stoped = false;
    }
    
    public void StopAgent()
    {
        _navAgent.isStopped = true;
        _stoped = true;
    }
    
    
    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
    }

    private int step;
    private void Update()
    {
        if(_stoped)
            return;

        if (_navAgent.remainingDistance <= _navAgent.stoppingDistance)
        {
            _navAgent.SetDestination(_target.position);
        }
    }
    
}
