using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsHolderBehaviour : MonoBehaviour {

    [SerializeField] private GameObject[] clouds;
    [SerializeField] private float cloudSpeed;

   // private Vector3[] cloudsNewPos = {new Vector3(), new Vector3() };

	private void Start ()
    {
       /* cloudsNewPos[0] = clouds[0].transform.position;
        cloudsNewPos[1] = clouds[1].transform.position;*/

    }
	

	private void Update ()
    {
        CloudMover(clouds[0]);
        CloudMover(clouds[1]);
    }

    private void CloudMover(GameObject cloud)
    {
        float smoothSpeed = 0.125f;

        if (cloud.transform.position.x < -20)
        {
            float offset = 25.0f;

            if (cloud == clouds[0])
                clouds[0].transform.localPosition = new Vector3(clouds[1].transform.localPosition.x + offset, clouds[1].transform.localPosition.y, clouds[1].transform.localPosition.z);

            else
                clouds[1].transform.localPosition = new Vector3(clouds[0].transform.localPosition.x + offset, clouds[0].transform.localPosition.y, clouds[0].transform.localPosition.z);
        }

        Vector3 newPos = new Vector3(cloud.transform.localPosition.x - cloudSpeed, cloud.transform.localPosition.y, cloud.transform.localPosition.z);
        Vector3 smoothMove = Vector3.Lerp(cloud.transform.localPosition, newPos, smoothSpeed);
        cloud.transform.localPosition = smoothMove;
    }
}
