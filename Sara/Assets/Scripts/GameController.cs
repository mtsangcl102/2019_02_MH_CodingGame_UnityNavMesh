using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour
{
    public NavMeshAgent Agent;
    public NavMeshSurface Surface;
    public GameObject DestinationIndicator;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!Agent.pathPending)
        {
            if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                {
                    MazeGenerator.Instance.GenerateMaze((int) Agent.transform.position.x,
                        (int) Agent.transform.position.z);
                    SetRandomDestination();
                    Surface.BuildNavMesh();
                }
            }
        }
    }

    private void SetRandomDestination()
    {
        var target2D = MazeGenerator.Instance.GetRandomMovablePosition();
        var target3D = new Vector3(target2D.x, Agent.transform.position.y, target2D.y);
        Agent.SetDestination(target3D);

        DestinationIndicator.transform.position = new Vector3(target2D.x, 0, target2D.y);
        Debug.Log($"New Target: {target3D}");
    }
}