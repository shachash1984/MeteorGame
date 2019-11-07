using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoSingleton<MobileInput>{
public bool release,hold;

private bool isBreakingStuff;
public Vector3 swipeDelta;
private Vector3 initialPosition;

void Start(){
	release  = false;
	isBreakingStuff = false;
}
private void Update()
{
	if(Input.GetMouseButtonDown(0))
	{
		initialPosition = Input.mousePosition;
		hold  = true;
	}
	else if(Input.GetMouseButtonUp(0))
	{
		release = true;
		hold = false;

	}

	if(hold)
	{
		swipeDelta = (Vector3)Input.mousePosition - initialPosition;
	}
}
public bool IsBallHasBeenPressed()
{
	return hold;
}
public bool IsBallHasBeenReleased()
{
	return release;
}
public void SetHoldState(bool state){
	hold = state;
}
public void SetRealseState(bool state){
	release = state;
}
public bool IsBreakingStuff()
{
	return isBreakingStuff;
}
public void SetBreakingStuff(bool state){
	isBreakingStuff = state;
}
}
