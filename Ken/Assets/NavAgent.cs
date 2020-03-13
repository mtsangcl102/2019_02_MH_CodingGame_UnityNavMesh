using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
 
public class NavAgent : MonoBehaviour {
    public GameObject destination;
    public NavMeshAgent navMeshAgent;
    public bool isReached = false;
    
    // Use this for initialization
    void Start () {
//        destination = GameObject.Find ("Destination");
//        navMeshAgent = GetComponent<NavMeshAgent> ();
//        navMeshAgent.SetDestination(destination.transform.position);
        isReached = false;
    }
	
    // Update is called once per frame
    void Update () {
//        destination = GameObject.Find ("Destination(Clone)");
//        navMeshAgent = GetComponent<NavMeshAgent> ();
//        navMeshAgent.SetDestination(destination.transform.position);
        
//        if (navMeshAgent.remainingDistance <= 0.1f)
//        {
//            isReached = true;
//        }

    }
}