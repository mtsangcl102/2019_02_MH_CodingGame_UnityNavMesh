using System;
using Boo.Lang;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour {

	public int width = 50;
	public int height = 50;

	public GameObject wall;
	public GameObject player;
	public GameObject destination;
	public NavMeshSurface navMeshSurObject;
	public NavAgent playerAgent;
	public GameObject mainPlayer;
	public GameObject mainDestination;
	public int destinationX = -1; 
	public int destinationY = -1;
	public bool shouldStart = false;
	public List<GameObject> wallList = new List<GameObject>();

	private bool playerSpawned = false;
	private bool _destinationSpawned = false;
	

	// Use this for initialization
	void Start () {
		GenerateLevel();
	}

	public void Update()
	{
		
		Debug.Log("Mainplayer position is " + mainPlayer.GetComponent<NavMeshAgent>().remainingDistance.ToString());

		if (shouldStart && mainPlayer.GetComponent<NavMeshAgent>().remainingDistance == 0f)
		{
			shouldStart = false;
			ResetLevel();
			GenerateLevel();
			mainPlayer.GetComponent<NavAgent>().isReached = false;

		}
	}

	void ResetLevel()
	{
		foreach (GameObject tmpObject in wallList)
		{
			Destroy(tmpObject);
		}

		Destroy(mainDestination);
		_destinationSpawned = false; 
		wallList.Clear();
	}

	// Create a grid based level
	void GenerateLevel()
	{
		Debug.Log("Should restart world ");
		// Loop over the grid
		
		destinationX = Random.Range((width * -1), width) / 2;
		destinationY = Random.Range((height * -1), height) / 2;
		
		for (int x = 0; x <= width; x+=2)
		{
			for (int y = 0; y <= height; y+=2)
			{
				if (x == destinationX && y == destinationY)
				{
					if (!_destinationSpawned)
					{
						Vector3 destPos = new Vector3(destinationX, 1.25f, destinationY);
						mainDestination = Instantiate(destination, destPos, Quaternion.identity);
						_destinationSpawned = true;
			
					}
				}
				else
				{
					// Should we place a wall?
					if (Random.value > .7f)
					{
						float targetX = x - width / 2f;
						float targetY = y - height / 2f;
					
						if(targetX != destinationX && targetY != destinationY)
						{
							// Spawn a wall
							Vector3 pos = new Vector3(targetX, 1f, targetY);
							wallList.Add(Instantiate(wall, pos, Quaternion.identity, transform));
						}
					}
					else if (!playerSpawned) // Should we spawn a player?
					{
						Vector3 pos = new Vector3(x - width / 2f, 1.25f, y - height / 2f);
						mainPlayer = Instantiate(player, pos, Quaternion.identity);
						playerSpawned = true;
					}
					else
					{
						if (!_destinationSpawned)
						{
							Vector3 destPos = new Vector3(destinationX, 1.25f, destinationY);
							mainDestination = Instantiate(destination, destPos, Quaternion.identity);
							_destinationSpawned = true;
			
						}
					}
				}
				
			}
		}
		if (navMeshSurObject != null )
		{
			navMeshSurObject.BuildNavMesh();
		}
		
		mainPlayer.GetComponent<NavMeshAgent>().SetDestination(mainDestination.transform.position);
		shouldStart = true;
	}

}
