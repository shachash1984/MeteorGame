using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public struct Background
{
    public float parallaxScaleFactor;
    public float parallaxSpeed;
    public float parallaxSmoothFactor;
    public float parallaxOffset;
    public float parallaxSwitchXPos;
    public Material parallaxMaterial;
    public Sprite backgroundImage;
    public GameObject[] randomObjectPrefabs;
    public BackgroundMode backgroundMode;

}

public enum BackgroundMode { Static, Parallax, RandomObjects, Both}

public class BGHandler : MonoBehaviour {

    static public BGHandler S;

    [Header("Backgrounds")]
    [SerializeField] private Background[] _backgrounds;

    [Space][Header("BGHandler objects")]
    [SerializeField] private List<GameObject> _parallaxImages;
    [SerializeField] private GameObject _staticBGImage;
    [SerializeField] private List<GameObject> _randomObjects;

    [Space]
    [Header("Behaviour parameters")]
    [SerializeField] private float _minMovementTime = 4f;
    [SerializeField] private float _maxMovementTime = 6f;

    private Background _currentBG;

    private const float MIN_X_BOUND = -2.5f;
    private const float MAX_X_BOUND = 2.5f;
    private const float MIN_Y_BOUND = 1.5f;
    private const float MAX_Y_BOUND = 9.5f;
    private const float Z_POS = 2.5f;
    private const int MAX_AMOUNT_OF_RANDOM_OBJECTS = 4;
    

    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
    }

    private void Start()
    {
        Init(GameMode.Play);
        GameManager.S.OnGameModeChanged += Init;
    }

    private void OnDisable()
    {
        GameManager.S.OnGameModeChanged -= Init;
    }

    private void Update()
    {
        Play(_currentBG.backgroundMode);
    }

    private void Init(GameMode gm)
    {
        //Clear existing BG elements
        /*BGElement[] bgElements = FindObjectsOfType<BGElement>();
        foreach (BGElement bge in bgElements)
        {
            Destroy(bge.gameObject);
        }*/

        if(gm == GameMode.Play)
        {
            
            if (Player.level - 1 < _backgrounds.Length)
                _currentBG = _backgrounds[Player.level - 1];
            else
                _currentBG = _backgrounds[Random.Range(0, _backgrounds.Length)];
            _staticBGImage.GetComponent<SpriteRenderer>().sprite = _currentBG.backgroundImage;
            foreach (GameObject bgo in _parallaxImages)
            {
                bgo.GetComponent<Renderer>().material = _currentBG.parallaxMaterial;
                bgo.transform.localScale = Vector3.one * _currentBG.parallaxScaleFactor;
            }
            if(_currentBG.randomObjectPrefabs.Length > 0)
            {
                int spawnAmount = Mathf.Min((MAX_AMOUNT_OF_RANDOM_OBJECTS - _randomObjects.Count), _currentBG.randomObjectPrefabs.Length);
                for (int i = 0; i < spawnAmount; i++)
                {
                    Vector3 spawnPos = new Vector3(Random.Range(MIN_X_BOUND, MAX_X_BOUND), Random.Range(MIN_Y_BOUND, MAX_Y_BOUND), Z_POS);
                    _randomObjects.Add(Instantiate(_currentBG.randomObjectPrefabs[i], spawnPos, Quaternion.identity));
                }
            }
        }
    }

    public void Play(BackgroundMode bgm)
    {
        switch (bgm)
        {
            case BackgroundMode.Static:
                break;
            case BackgroundMode.Parallax:
                foreach (GameObject go in _parallaxImages)
                {
                    PlayParallax(go);
                }
                break;
            case BackgroundMode.RandomObjects:
                foreach (GameObject go in _randomObjects)
                {
                    //Debug.Log("<color=yellow>Moving: " + go.name + "</color>");
                    if (!DOTween.IsTweening(go.transform))
                        PlayRandomObjects(go);
                }
                break;
            case BackgroundMode.Both:
                break;
            default:
                break;
        }
    }

    private void PlayParallax(GameObject bgObj)
    {
        if (bgObj.transform.position.x < -_currentBG.parallaxSwitchXPos)
        {
            if (bgObj == _parallaxImages[0])
                _parallaxImages[0].transform.localPosition = new Vector3(_parallaxImages[1].transform.localPosition.x + _currentBG.parallaxOffset, _parallaxImages[1].transform.localPosition.y, _parallaxImages[1].transform.localPosition.z);

            else
                _parallaxImages[1].transform.localPosition = new Vector3(_parallaxImages[0].transform.localPosition.x + _currentBG.parallaxOffset, _parallaxImages[0].transform.localPosition.y, _parallaxImages[0].transform.localPosition.z);
        }

        Vector3 newPos = new Vector3(bgObj.transform.localPosition.x - _currentBG.parallaxSpeed, bgObj.transform.localPosition.y, bgObj.transform.localPosition.z);
        Vector3 smoothMove = Vector3.Lerp(bgObj.transform.localPosition, newPos, _currentBG.parallaxSmoothFactor);
        bgObj.transform.localPosition = smoothMove;
    }

    private void PlayRandomObjects(GameObject go)
    {
        if (!DOTween.IsTweening(go.transform))
        {
            float tweenTime = Random.Range(_minMovementTime, _maxMovementTime);
            Vector3 newPos = new Vector3(Random.Range(MIN_X_BOUND, MAX_X_BOUND), Random.Range(MIN_Y_BOUND, MAX_Y_BOUND), Z_POS);
            go.transform.DOMove(newPos, tweenTime);
        }
    }

    public void RemoveBGElement(BGElement bge)
    {
        _randomObjects.Remove(bge.gameObject);
    }
}
