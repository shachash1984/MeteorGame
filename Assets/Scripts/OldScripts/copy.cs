using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copy : MonoBehaviour {

	public float ballDistance;
		public float ballDistanceValue;

	public int ballIndex;
	public GameObject originBall;
	public GameObject[] balls;
	public int ballNum;
	public int ballToShow;
	// Use this for initialization

	void Start ()
	{
		ballIndex = balls.Length -1;
		CreateBalls();
		ShowBalls();
	
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			Vector3 vector = new Vector3(Random.Range(1,4),Random.Range(1,4),Random.Range(1,4));
            GetBall(vector);
		}
	}

	void CreateBalls()
	{
		for(int i = 0; i<balls.Length; i++)
		{
			
			balls[i] = Instantiate(originBall,transform.position,Quaternion.identity);
			balls[i].SetActive(false);
		}
	}
	void ShowBalls()
	{
		
		for(int j = 0; j<ballToShow; j++)
		{
			if (ballIndex -j < 0){
				return;
			}
			balls[ballIndex - j].SetActive(true);
			balls[ballIndex - j].transform.position = originBall.transform.position;
			balls[ballIndex  - j].transform.position = new Vector3 (originBall.transform.position.x, 
			originBall.transform.position.y + ballDistance , originBall.transform.position.z);
			ballDistance += ballDistanceValue;

		}
		ballDistance = ballDistanceValue;
	}
	void GetBall(Vector3 pos)
	{		
		
		balls[ballIndex].transform.position = pos;
		Debug.Log(pos);
		ballIndex -= 1;
		ShowBalls();
	}
}
