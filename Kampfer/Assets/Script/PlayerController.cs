using UnityEngine;
using UnityEngine.AI;

namespace Scenes
{

    public class PlayerController : MonoBehaviour
    {
        public Collider playerCollider => GetComponent<Collider>();
        public NavMeshAgent playerAgent => GetComponent<NavMeshAgent>();
        public NavMeshAgent targetAgent;

        public MazeGenerator MazeGenerator;

        void Start()
        {
            playerAgent.updateRotation = false;
        }
        
        
        public NavMeshSurface Surface;
        public void FixedUpdate()
        {
            
            if (playerAgent != null && targetAgent != null)
            {
                var position = targetAgent.transform.position;
                position = new Vector3(Mathf.RoundToInt(position.x), 0f,Mathf.RoundToInt(position.z));
                
                targetAgent.transform.position = position;
                playerAgent.destination = position;

//                Debug.Log(playerAgent.path.status );
                if (playerAgent.path.status == NavMeshPathStatus.PathInvalid || playerAgent.path.status == NavMeshPathStatus.PathPartial)
                {
                    Surface.BuildNavMesh();
                }
            }
            
        }
        
        void Update()
        {
            
            
            var distance = Vector3.Distance(playerAgent.transform.position, targetAgent.transform.position);
            if (distance < 1)
            {
                MazeGenerator.GenerateMaze();
            }
            else
            {
                InstantlyTurn(playerAgent.destination);
            }
        }

        public float rotation = 20f;
        private void InstantlyTurn(Vector3 destination) {
            //When on target -> dont rotate!
            if ((destination - transform.position).magnitude < 0.1f) return; 
     
            Vector3 direction = (destination - transform.position).normalized;
            Quaternion  qDir= Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, qDir, Time.deltaTime * rotation);
        }

    }
}