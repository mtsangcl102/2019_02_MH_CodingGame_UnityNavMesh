using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

    public NavMeshAgent agent;

    public Vector3 position
    {
        get
        {
            return new Vector3( Mathf.RoundToInt( transform.position.x ), 1.25f, Mathf.RoundToInt( transform.position.z ) );
        }
    }

    public void SetDestination( Vector3 position )
    {
        agent.SetDestination(position);
    }
}
