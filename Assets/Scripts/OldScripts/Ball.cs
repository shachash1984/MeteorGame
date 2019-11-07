using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	private Rigidbody rigid;
    public float speed;

    void Start()
	{
		rigid = GetComponent<Rigidbody>();
		rigid.useGravity =false;
	}
	private void Update () 
	{
		if(!MobileInput.Instance.IsBreakingStuff()){
			if(MobileInput.Instance.IsBallHasBeenReleased()){
				ShootBall(MobileInput.Instance.swipeDelta.normalized);
				rigid.useGravity =true;
			}
		}
	}

	 private void ShootBall(Vector3 dir)
	 {
		 MobileInput.Instance.SetBreakingStuff(true);
		 rigid.velocity = dir * speed;
	 }

}
