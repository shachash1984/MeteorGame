using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class PlayerBall : MonoBehaviour {

    #region Public Fields
    public bool hasHitEnemy = false;
    public bool hasHitBGElement = false;
    public Vector3 initialPosition = Vector3.zero;
    #endregion

    #region Events
    public static event Action OnAsteroidHit;
    public static event Action OnLastAsteroidHit;
    public static event Action OnMiss;
    public static event Action OnHitBGElement;
    #endregion

    #region Monobehaviour Callbacks
    private void Start()
    {
        hasHitEnemy = false;
        hasHitBGElement = false;
        PlayerInputHandler.S.SetCurrentBall(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitEnemy)
            return;
        if (other.gameObject.layer == 9)
        {
            hasHitEnemy = true;
            GetComponent<Collider>().enabled = false;
            GetComponent<Renderer>().enabled = false;
            other.GetComponent<Asteroid>().Explode(true);
        }
        else if (other.gameObject.layer == 13)
        {
            hasHitBGElement = true;
            GetComponent<Collider>().enabled = false;
            GetComponent<Renderer>().enabled = false;
            other.GetComponent<BGElement>().Explode(true);
            if (OnHitBGElement != null)
                OnHitBGElement();
        }
        
    }

    private void OnDestroy()
    {
        if (!hasHitEnemy && !hasHitBGElement && transform.position != initialPosition)
        {
            if (OnMiss != null)
            {
                SaveAsteroidsLeftAmount();
                OnMiss();
                //EventFireIndicator(3);
            }
                
        }
    }
    #endregion

    #region Public Methods
    public void Shoot(params Vector3[] wayPoints)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOPath(wayPoints, 0.5f, PathType.Linear, PathMode.Full3D).SetEase(Ease.Linear));
        seq.AppendInterval(Time.deltaTime);
        seq.OnComplete(() =>
        {
            
            if (hasHitEnemy && AsteroidManager.S.GetBallsLeftAmount() == 0)
            {
                GetComponent<Renderer>().enabled = false;
                if (OnLastAsteroidHit != null)
                {
                    OnLastAsteroidHit();
                }
                    
                Destroy(gameObject, 0.05f);
            }
            else if(hasHitEnemy)
            {
                GetComponent<Renderer>().enabled = false;
                if (OnAsteroidHit != null)
                {
                    OnAsteroidHit();
                }
                    
                Destroy(gameObject, 0.05f);
            }
            else if (hasHitBGElement)
            {
                GetComponent<Renderer>().enabled = false;
                Destroy(gameObject, 0.05f);
            }
            else if(!hasHitEnemy && !hasHitBGElement)
            {
                GetComponent<Renderer>().enabled = false;
                Destroy(gameObject, 0.05f);
            }
        });
    }

    public void SaveAsteroidsLeftAmount()
    {
        PlayerData.asteroidsLeftInCurrentLevel = AsteroidManager.S.GetBallsLeftAmount();
    }
    #endregion

    ///////////TEST
    public void EventFireIndicator(int eventIndex)
    {
        switch (eventIndex)
        {
            case 1:
                Debug.Log("OnAsteroidHit Time: " + Time.time );
                break;
            case 2:
                Debug.Log("OnLastAsteroidHit Time: " + Time.time);
                Debug.Log("Asteroids left: " + AsteroidManager.S.GetBallsLeftAmount());
                break;
            case 3:
                Debug.Log("OnMiss Time: " + Time.time);
                break;
            default:
                break;
        }
    }
    //////////////

}
