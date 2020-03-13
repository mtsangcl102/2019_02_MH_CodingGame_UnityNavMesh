using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Treasure : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<NavMeshAgent>())
		{
			other.transform.localPosition = new Vector3(GameManager.endX*2, other.transform.localPosition.y, GameManager.endY*2);
			GameManager.instance.GenerateMaze(GameManager.endX, GameManager.endY);
		}
	}

	public void SetPosition(float x, float z)
	{
		transform.localPosition = new Vector3(x, 0.5f, z);
	}
}
