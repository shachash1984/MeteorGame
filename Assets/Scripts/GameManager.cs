using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public enum GameMode { Standby = 0, Play, EndLevel}

public class GameManager : MonoBehaviour {

    #region Public Fields
    static public GameManager S;
    #endregion

    #region Private Fields
    [SerializeField] private GameMode _gameMode;
    #endregion

    #region Properties
    public GameMode gameMode
    {
        get { return _gameMode; }
        private set { _gameMode = value; }
    }
    #endregion

    #region Events
    public Action<GameMode> OnGameModeChanged;
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
        PlayerBall.OnLastAsteroidHit += PlayerDestroyedAllEnemies;
        AsteroidManager.OnEarthExploded += EarthExploded;
        UIManager.OnConfirmButtonTapped += ChangeGameMode;
        UIManager.OnWatchedAd += RestartLevel;
    }

    private void OnDisable()
    {
        PlayerBall.OnLastAsteroidHit -= PlayerDestroyedAllEnemies;
        AsteroidManager.OnEarthExploded -= EarthExploded;
        UIManager.OnConfirmButtonTapped -= ChangeGameMode;
        UIManager.OnWatchedAd -= RestartLevel;
    }

    private void Start()
    {
        ChangeGameMode(GameMode.Play);
    }
    #endregion

    #region Public Methods
    public void ChangeGameMode(GameMode newGameMode)
    {
        gameMode = newGameMode;
        if (OnGameModeChanged != null)
            OnGameModeChanged(newGameMode);
    }
    #endregion

    #region Private Methods
    private void PlayerDestroyedAllEnemies()
    {
        Debug.Log("PlayerDestroyedAllEnemies Level: " + Player.level);
        SetNextLevel();
        ChangeGameMode(GameMode.EndLevel);
        
    }

    private void EarthExploded()
    {
        ChangeGameMode(GameMode.Standby);
        RestartLevel(PlayerData.watchedAd);
        
    }

    private void SetNextLevel()
    {
        AsteroidManager.S.SpawnEnemyBalls(++Player.level);
    }

    private void RestartLevel(bool watchedAd)
    {
        AsteroidManager.S.ClearAsteroids();
        AsteroidManager.S.SpawnEarth();
        if (watchedAd)
        {
            AsteroidManager.S.SpawnEnemyBalls(PlayerData.asteroidsLeftInCurrentLevel);
            PlayerData.watchedAd = false;
        }
        else
            AsteroidManager.S.SpawnEnemyBalls(Player.level);


    }
    #endregion

}
