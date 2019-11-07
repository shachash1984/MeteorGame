using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    #region Public Fields
    static public Player S;
    static public int level = 1;
    #endregion

    #region Private Fields
    [SerializeField] private GameObject _playerBallPrefab;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
#if !UNITY_EDITOR
        level = PlayerData.GetCurrentLevel();
#else
        level = 1;
#endif
    }

    private void Start()
    {
        
        GameManager.S.OnGameModeChanged += SpawnPlayerBall;
        GameManager.S.OnGameModeChanged += SaveCurrentLevel;
        PlayerBall.OnAsteroidHit += SpawnPlayerBall;
        PlayerBall.OnHitBGElement += SpawnPlayerBall;
    }

    private void OnDisable()
    {
        GameManager.S.OnGameModeChanged -= SpawnPlayerBall;
        GameManager.S.OnGameModeChanged -= SaveCurrentLevel;
        PlayerBall.OnAsteroidHit -= SpawnPlayerBall;
        PlayerBall.OnHitBGElement -= SpawnPlayerBall;
    }

    
#endregion

#region Public Methods
    public void SpawnPlayerBall()
    {
        StartCoroutine(SpawnPlayerBallCoroutine());
    }

    public void SpawnPlayerBall(GameMode gm)
    {
        if (gm == GameMode.Play)
            StartCoroutine(SpawnPlayerBallCoroutine());
    }
#endregion

#region Private Methods
    private IEnumerator SpawnPlayerBallCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject go = Instantiate(_playerBallPrefab, Vector3.zero, Quaternion.identity);
        go.GetComponent<Rigidbody>().velocity = Vector3.zero;
        go.GetComponent<PlayerBall>().hasHitEnemy = false;
    }

    private void SaveCurrentLevel(GameMode gm)
    {
        PlayerData.SetCurrentLevel(level);
    }
#endregion




}
