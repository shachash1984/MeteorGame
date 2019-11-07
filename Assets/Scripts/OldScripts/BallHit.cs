using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHit : MonoBehaviour {

public GameObject Player;
void OnCollisionEnter(Collision other)
{
	if(other.collider.tag == "Player")
	{
		other.gameObject.SetActive(false);		
		Destroy(this.gameObject ,0.1f);
	}
}
}
