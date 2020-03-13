using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour {

	public NavMeshSurface surface;
	public int width = 10;
	public int height = 10;

	public GameObject wall;
	public GameObject player;
	public GameObject treasure ;

	private PlayerController playerController ;
	private TreasureController treasureController ;
	private List<GameObject> wallList = new List<GameObject>() ;

	// Use this for initialization
	void Start ()
	{
		GeneratePlayer() ;
		StartLevel() ;
	}
	
	void Update()
	{
		if( playerController.position == treasureController.position )
		{
			StartLevel() ;
		}
	}

	private void StartLevel()
	{
		GenerateTreasure() ;
		GenerateLevel();
		surface.BuildNavMesh();
		playerController.SetDestination( treasureController.position );
	}

	private void GeneratePlayer()
	{
		int x = Random.Range( 0, ( width / 2 ) ) * 2 ;
		int y = Random.Range( 0, ( height / 2 ) ) * 2 ;
		// Spawn the player
		Vector3 pos = new Vector3(x - width / 2f, 1.25f, y - height / 2f);
		playerController = Instantiate(player, pos, Quaternion.identity).GetComponent<PlayerController>();
	}

	private void GenerateTreasure()
	{
		int x = Random.Range( 0, ( width / 2 ) ) * 2 ;
		int y = Random.Range( 0, ( height / 2 ) ) * 2 ;
		Vector3 pos = new Vector3( x - width / 2f, 1.25f, y - height / 2f ) ;
		if( treasureController == null )
		{
			treasureController = Instantiate( treasure, pos, Quaternion.identity ).GetComponent<TreasureController>() ;
		}
		else
		{
			treasureController.transform.localPosition = pos ;
		}
		treasureController.position = pos ;
	}

	// Create a grid based level
	void GenerateLevel()
	{
		for( int i = 0; i < wallList.Count; i++ )
		{
			DestroyImmediate( wallList[ i ] );
		}
		wallList.Clear() ;

		// Loop over the grid
		for (int x = 0; x <= width; x+=4)
		{
			for (int y = 0; y <= height; y+=4)
			{
				GenerateWallGroup( x - width / 2, y - height / 2 ) ;
			}
		}
	}

	void GenerateWallGroup( int x, int y )
	{
		if( Random.value > .1f )
		{
			GenerateWall( x, y ) ;
			
			
			int a = Random.value < .5 ? 0 : (Random.value < .5 ? -2 : 2);
			int b = a != 0 ? 0 : (Random.value < .5 ? -2 : 2);
			GenerateWall( x + a, y + b ) ;
		}
	}
	
	void GenerateWall( int x, int y )
	{
		if( ( x != playerController.position.x || y != playerController.position.z ) &&
			( x != treasureController.position.x || y != treasureController.position.z ) )
		{
			Vector3 pos = new Vector3( x, 1f, y ) ;
			wallList.Add( Instantiate( wall, pos, Quaternion.identity, transform ) ) ;
		}
	}

}
