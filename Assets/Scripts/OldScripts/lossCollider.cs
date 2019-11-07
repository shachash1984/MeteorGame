using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class lossCollider : MonoBehaviour {


void OnCollisionEnter(Collision other)
{
SceneManager.LoadScene("MainMenu");	
}
}
