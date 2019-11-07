using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCheck : MonoBehaviour {

    public GameObject Player;
    //private bool createPlayer;
    public GameObject InitialPos;
    void Start () 
    {
        //createPlayer = false;
    }

    // Update is called once per frame
    void Update () 
    {
        if(!Player.activeSelf)
        {
            Player.SetActive(true);
            Rigidbody rigid = Player.GetComponent<Rigidbody>();
		    rigid.useGravity = false;
            //Vector3 playerRigid = Player.GetComponent<Rigidbody>().velocity = new Vector3 (0,0,0);
            Player.transform.position = InitialPos.transform.position;
            MobileInput.Instance.SetBreakingStuff(false);
        }
        MobileInput.Instance.SetRealseState(false); 
        
    }
}
