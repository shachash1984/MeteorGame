using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGElement : MonoBehaviour {

    private GameObject explosionEffect;

    private void Awake()
    {
        explosionEffect = transform.GetChild(1).gameObject;
        explosionEffect.SetActive(false);
    }

    public void Explode(bool explodeEffect)
    {
        BGHandler.S.RemoveBGElement(this);
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        PlaySound();
        if (explodeEffect)
            explosionEffect.SetActive(true);
        Destroy(gameObject, 2f);
    }

    private void PlaySound()
    {
        if(PlayerData.playSound)
            GetComponent<AudioSource>().Play();
    }
}
