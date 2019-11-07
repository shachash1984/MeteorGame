using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class AsteroidManager : MonoBehaviour {

    #region Public Fields
    static public AsteroidManager S;
    public bool earthHit = false;
    public Vector3 hitPoint = Vector3.zero;
    public int explosionCounter = 0;
    public int MAX_EXPLOSIONS = 6;
    public float MAX_HITPOINT_DISTANCE = 2f;
    #endregion

    #region Private Fields
    [SerializeField] private GameObject[] _enemyBallPrefabs;
    [SerializeField] private Vector2 _spawnPosRange;
    private List<Asteroid> _asteroids = new List<Asteroid>();
    [SerializeField] private GameObject _earthPrefab;
    private GameObject _earth;
    private GameObject _earthExplosionEffect;
    #endregion

    #region Events
    public static event Action OnEarthExploded;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
    }

    private void OnEnable()
    {
        PlayerBall.OnMiss += ExplodeEarth;
    }

    private void OnDisable()
    {
        PlayerBall.OnMiss -= ExplodeEarth;
    }

    private void Start()
    {
        SpawnEarth();
        SpawnEnemyBalls(Player.level);
    }

    #endregion

    #region Public Methods
    public void SpawnEnemyBalls(int level)
    {
        for (int i = 0; i < level; i++)
        {
            int rand = UnityEngine.Random.Range(0, _enemyBallPrefabs.Length);
            GameObject go = Instantiate(_enemyBallPrefabs[rand], _earth.transform);
            Vector3 wantedPos = UnityEngine.Random.insideUnitSphere;
            for (int j = 0; j < 3; j++)
            {
                if (Mathf.Abs(wantedPos[j]) < 0.35f)
                {
                    if (wantedPos[j] < 0)
                        wantedPos[j] = -0.35f;
                    else
                        wantedPos[j] = 0.35f;
                }

            }
            go.transform.localPosition = wantedPos;
            go.transform.localRotation = UnityEngine.Random.rotation;
            _asteroids.Add(go.GetComponent<Asteroid>());
        }
    }

    public void SpawnEarth()
    {
        if(_earth == null)
        {
            _earth = Instantiate(_earthPrefab);
            _earthExplosionEffect = _earth.transform.GetChild(0).gameObject;
            earthHit = false;
        }
        
    }

    public void ClearAsteroids()
    {
        foreach (Asteroid a in _asteroids)
        {
            if (a)
                Destroy(a.gameObject);
        }
        _asteroids.Clear();
        explosionCounter = 0;
    }

    public void RemoveEnemyBall(Asteroid eb)
    {
        _asteroids.Remove(eb);
    }

    public int GetBallsLeftAmount()
    {
        return _asteroids.Count;
    }

    public void ExplodeEarth()
    {
        StopAllCoroutines();
        StartCoroutine(ExplodeEarthCoroutine());
    }
    #endregion

    #region Private Methods
    private IEnumerator ExplodeEarthCoroutine()
    {
        float timer = Time.time;
        while (!earthHit)
        {
            if (Time.time - timer >= 0.75f)
                earthHit = true;
            yield return new WaitForEndOfFrame();
        }
        
        Vector3 lookPosition = transform.position;
        lookPosition.z = 20f;
        _earthExplosionEffect.transform.LookAt(lookPosition);
        if (Mathf.Abs(hitPoint.y - _earth.transform.position.y) > MAX_HITPOINT_DISTANCE)
            hitPoint = _earth.transform.position;
        hitPoint.z = -5f;
        
        _earthExplosionEffect.transform.position = hitPoint;
        _earthExplosionEffect.SetActive(true);
        Camera.main.GetComponent<CameraShake>().ShakeCamera();
        PlaySound();
        _earth.GetComponent<Renderer>().enabled = false;
        _earth.GetComponent<MeshExploder>().Explode();
        Destroy(_earth.gameObject, 2f);
        yield return new WaitForSeconds(2f);
        if (OnEarthExploded != null)
            OnEarthExploded();
    }

    private void PlaySound()
    {
        if (PlayerData.playSound)
            _earth.GetComponent<AudioSource>().Play();
    }
    #endregion






}
