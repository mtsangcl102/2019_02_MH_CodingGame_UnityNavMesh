using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    public void RefreshSetup(Transform treasure, int x, int y)
    {
        transform.position = new Vector3(x * 2, 1.3f, y * 2);
        agent.destination = treasure.transform.position;
    }

}
