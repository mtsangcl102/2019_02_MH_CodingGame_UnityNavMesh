using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public NavMeshAgent mAgent;

    // Start is called before the first frame update
    void Start()
    {
    }


    public void setDestination(Vector3 dest)
    {
        mAgent.SetDestination(dest);
    }

}
