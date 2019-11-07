using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Asteroid : MonoBehaviour {

    #region Private Fields
    private GameObject explosionEffect;
    private bool _allowedToExplode = false;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        explosionEffect = transform.GetChild(0).gameObject;
        explosionEffect.SetActive(false);
        _allowedToExplode = false;
    }

    private void OnEnable()
    {
        PlayerBall.OnMiss += HitEarth;
    }

    private void OnDisable()
    {
        PlayerBall.OnMiss -= HitEarth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_allowedToExplode && other.gameObject.layer == 11 && GameManager.S.gameMode == GameMode.Play)
        {
            AsteroidManager.S.hitPoint = transform.position;
            AsteroidManager.S.explosionCounter++;

            Explode(AsteroidManager.S.explosionCounter < AsteroidManager.S.MAX_EXPLOSIONS);
            AsteroidManager.S.earthHit = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_allowedToExplode && other.gameObject.layer == 11 && GameManager.S.gameMode == GameMode.Play)
        {
            AsteroidManager.S.hitPoint = transform.position;
            AsteroidManager.S.explosionCounter++;

            Explode(AsteroidManager.S.explosionCounter < AsteroidManager.S.MAX_EXPLOSIONS);
            AsteroidManager.S.earthHit = true;
        }
    }


    #endregion

    #region Public Methods
    public void Explode(bool explodeEffect)
    {
        AsteroidManager.S.RemoveEnemyBall(this);
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        PlaySound();
        if (explodeEffect)
            explosionEffect.SetActive(true);
        Destroy(gameObject, 2f);
    }

    public void HitEarth()
    {
        transform.DOLocalMove(Vector3.zero, 0.5f).OnStart(() => _allowedToExplode = true);
    }

    private void PlaySound()
    {
        if (PlayerData.playSound)
            GetComponent<AudioSource>().Play();
    }
    #endregion

}
