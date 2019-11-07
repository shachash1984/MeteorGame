using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rounded : MonoBehaviour {

    public float rotationSpeedY;
    public float rotationSpeedX;
    public float rotationSpeedZ;
    public float timer;

    private void Start()
    {
        rotationSpeedY = Random.Range(0, 30);
        rotationSpeedX = Random.Range(0, 30);
        rotationSpeedZ = Random.Range(0, 30);
        timer = 0;
    }


void Update () 
      {
        timer += Time.deltaTime;
        if (timer > 5)
        {
            CheckNewPos();
        }
        else
        {
            transform.Rotate(rotationSpeedX * Time.deltaTime, rotationSpeedY * Time.deltaTime, rotationSpeedZ * Time.deltaTime);
        }

    }
    void CheckNewPos()
    {
        rotationSpeedY = Random.Range(0, 30);
        rotationSpeedX = Random.Range(0, 30);
        rotationSpeedZ = Random.Range(0, 30);
        timer = 0;

    }
}
