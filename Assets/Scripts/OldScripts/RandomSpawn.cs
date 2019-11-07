using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour {

    public GameObject character;
    public GameObject planet;
    
    public float ranged;
	public GameObject[] balls;
	public int ballIndex;

    private void Start()
    {
        ballIndex = balls.Length -1;
 
            for(int i = 0; i<balls.Length; i++)
    {
        Vector3 spawnPosition = Random.onUnitSphere * ((planet.transform.localScale.x + ranged) + character.transform.localScale.y * 0.5f) + planet.transform.position;
        Quaternion spawnRotation = Quaternion.identity;
        
        balls[i] = Instantiate(character, spawnPosition, spawnRotation) as GameObject;
        balls[i].transform.SetParent(planet.transform);
        balls[i].transform.LookAt(planet.transform);
        balls[i].transform.Rotate(-90, 0, 0);
    }
    
    }
}

