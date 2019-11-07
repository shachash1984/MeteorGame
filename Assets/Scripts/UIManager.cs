using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

[System.Serializable]
public struct Message
{
    public string key;
    [TextArea(3,3)] public string content;
}
public class UIManager : MonoBehaviour {

    #region Public Fields
    static public UIManager S;
    #endregion

    #region Private Fields
    private CanvasGroup _endGamePanel;
    private CanvasGroup _settingsPanel;
    private Button _confirmButton;
    private Text _messageSuccessText;
    private Text _messageMissText;
    private Text _levelText;
    [SerializeField] private Message[] _messages;
    private Button _noAdsButton;
    private Image _timerCircle;
    private Button _watchToPlayButton;
    private Button _settingsButton;
    private Button _soundButton;
    private Dictionary<string, string> _messageDictionary = new Dictionary<string, string>();
    private float _watchToPlayTime = 5f;
    [SerializeField] private Sprite _soundActiveSprite;
    [SerializeField] private Sprite _soundDisabledSprite;
    private ServerConnection _serverConnection;
    private StoreManager _storeConnection;
    private bool _lastLevelSuccess;
    private int _levelPassCount, _levelFailCount;
    #endregion

    #region Events
    static public Action<GameMode> OnConfirmButtonTapped;
    static public Action<bool> OnWatchedAd;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        Init();
    }

    private void OnEnable()
    {
        GameManager.S.OnGameModeChanged += HandleGameMode;
    }

    private void OnDisable()
    {
        GameManager.S.OnGameModeChanged -= HandleGameMode;
    }
    #endregion

    #region Private Methods
    private void Init()
    {
        AdsManager.Initialize();

        //Initialize messages
        foreach (Message m in _messages)
        {
            if (!_messageDictionary.ContainsKey(m.key))
                _messageDictionary.Add(m.key, m.content);
        }

        _serverConnection = GetComponent<ServerConnection>();

        //Assign UI Elements
        _endGamePanel = transform.GetChild(0).GetChild(0).GetComponent<CanvasGroup>();
        _settingsPanel = transform.GetChild(0).GetChild(2).GetChild(2).GetComponent<CanvasGroup>();
        _messageSuccessText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        _messageMissText = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        _levelText = transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>();
        _timerCircle = transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Image>();

        //Making sure only relevant items are displayed
        ToggleUIItem(_endGamePanel, false, true);
        ToggleUIItem(_settingsPanel, false, true);

        //Assign Buttons
        _confirmButton = transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>();
        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(() =>
        {
            CheckAdAfterLevel(() =>
            {
                if (OnConfirmButtonTapped != null)
                    OnConfirmButtonTapped(GameMode.Play);
                ToggleEndGamePanel(false);
                StopAllCoroutines();
                _timerCircle.fillAmount = 1f;
            });
        });

        _watchToPlayButton = transform.GetChild(0).GetChild(0).GetChild(4).GetComponent<Button>();
        _watchToPlayButton.onClick.RemoveAllListeners();
        _watchToPlayButton.onClick.AddListener(() =>
        {
            PlayAd();
        });

        _noAdsButton = transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>();
        ToggleUIItem(_noAdsButton.GetComponent<CanvasGroup>(), true, true);
        _noAdsButton.onClick.RemoveAllListeners();
        _noAdsButton.onClick.AddListener(() =>
        {
            StopAds();
        });

        _settingsButton = transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>();
        _settingsButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.AddListener(() =>
        {
            ToggleSettingsPanel(true);
        });

        _soundButton = _settingsPanel.transform.GetChild(0).GetComponent<Button>();
        _soundButton.onClick.RemoveAllListeners();
        _soundButton.onClick.AddListener(() =>
        {
            
            PlayerData.ToggleSound(!PlayerData.playSound);
            if (PlayerData.playSound)
                _soundButton.GetComponent<Image>().sprite = _soundActiveSprite;
            else
                _soundButton.GetComponent<Image>().sprite = _soundDisabledSprite;
        });
        PlayerData.playSound = PlayerData.CanPlaySound() == 1 ? true : false;
        if (PlayerData.playSound)
            _soundButton.GetComponent<Image>().sprite = _soundActiveSprite;
        else
            _soundButton.GetComponent<Image>().sprite = _soundDisabledSprite;

        UpdateLevelText();

    }

    private void CheckAdAfterLevel(Action next)
    {
        if (PlayerData.CanShowAds() == 1 && AdsManager.IsAvailable("video"))
        {
            if (_lastLevelSuccess && ++_levelPassCount >= GameInfo.Settings.ads.showAdAfterCompletes)
            {
                AdsManager.Show("video", (result, network) =>
                {
                    if (result == AdResult.SHOWED)
                        _serverConnection.SubmitEvent(new Messages.TrackAd { network = network, place = "VIDEO" });
                    if (result != AdResult.NOT_AVAILABLE)
                        _levelPassCount = 0;
                    next.Invoke();
                });
            }
            else if (++_levelFailCount >= GameInfo.Settings.ads.showAdAfterFails)
            {
                AdsManager.Show("video", (result, network) =>
                {
                    if (result == AdResult.SHOWED)
                        _serverConnection.SubmitEvent(new Messages.TrackAd { network = network, place = "VIDEO" });
                    if (result != AdResult.NOT_AVAILABLE)
                        _levelFailCount = 0;
                    next.Invoke();
                });
            }
            else
            {
                next.Invoke();
            }
        }
        else
        {
            next.Invoke();
        }
    }

    private void ToggleSettingsPanel(bool on)
    {
        ToggleUIItem(_noAdsButton.GetComponent<CanvasGroup>(), !on);
        ToggleUIItem(_settingsPanel, on);
        _settingsButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.AddListener(() =>
        {
            ToggleSettingsPanel(!on);
        });
    }

    private void PlayAd()
    {
        Debug.Log("PlayAd()");
        ToggleUIItem(_timerCircle.GetComponent<CanvasGroup>(), false, true);
        ToggleUIItem(_watchToPlayButton.GetComponent<CanvasGroup>(), false, true);
        _timerCircle.fillAmount = 1f;
        StopAllCoroutines();
        AdsManager.Show("rewardedVideo", (result, network) =>
        {
            if (result == AdResult.SHOWED)
            {
                _serverConnection.SubmitEvent(new Messages.TrackAd { network = network, place = "REWARDEDVIDEO" });
                PlayerData.watchedAd = true;
                if (OnWatchedAd != null)
                    OnWatchedAd(PlayerData.watchedAd);
                if (OnConfirmButtonTapped != null)
                    OnConfirmButtonTapped(GameMode.Play);
                ToggleEndGamePanel(false);
            }
        });
    }

    IEnumerator StartTimer()
    {
        while (true)
        {
            _timerCircle.fillAmount -= Time.deltaTime / _watchToPlayTime;
            yield return new WaitForEndOfFrame();
            if(_timerCircle.fillAmount <= 0)
            {
                ToggleUIItem(_timerCircle.GetComponent<CanvasGroup>(), false);
                ToggleUIItem(_watchToPlayButton.GetComponent<CanvasGroup>(), false);
            }
        }
    }

    private void ToggleEndGamePanel(bool on)
    {
        ToggleUIItem(_endGamePanel, on);
    }

    private void ToggleUIItem(CanvasGroup cv, bool show, bool immediate = false)
    {
        if (immediate)
        {
            if (show)
            {
                cv.gameObject.SetActive(true);
                cv.alpha = 1f;
                cv.blocksRaycasts = true;
            }
            else
            {
                cv.alpha = 0f;
                cv.blocksRaycasts = false;
                cv.gameObject.SetActive(false);
            }
        }
        else
        {
            if (show)
            {
                cv.gameObject.SetActive(true);
                cv.DOFade(1, 0.75f).SetEase(Ease.OutQuad);
                cv.blocksRaycasts = true;
            }
            else
            {
                cv.DOFade(0, 0.75f).SetEase(Ease.OutQuad);
                cv.blocksRaycasts = false;
                cv.gameObject.SetActive(false);
            }
        }

    }

    private void HandleGameMode(GameMode gm)
    {
        switch (gm)
        {
            case GameMode.Standby:
                bool haveVideo = AdsManager.IsAvailable("rewardedVideo");
                ToggleUIItem(_timerCircle.GetComponent<CanvasGroup>(), haveVideo, true);
                ToggleUIItem(_watchToPlayButton.GetComponent<CanvasGroup>(), haveVideo, true);
                ToggleEndGamePanel(true);
                _timerCircle.fillAmount = 1f;
                if (haveVideo)
                {
                    StartCoroutine(StartTimer());
                }
                SetMessageText("Miss");
                _lastLevelSuccess = false;
                break;
            case GameMode.Play:
                break;
            case GameMode.EndLevel:
                ToggleUIItem(_timerCircle.GetComponent<CanvasGroup>(), false, true);
                ToggleUIItem(_watchToPlayButton.GetComponent<CanvasGroup>(), false, true);
                ToggleEndGamePanel(true);
                SetMessageText("Success");
                UpdateLevelText();
                _lastLevelSuccess = true;
                break;
            default:
                break;
        }
    }

    private void SetMessageText(string messageKey)
    {
        if(messageKey == "Success")
        {
            _messageSuccessText.gameObject.SetActive(true);
            _messageMissText.gameObject.SetActive(false);
            
        }
        else
        {
            _messageSuccessText.gameObject.SetActive(false);
            _messageMissText.gameObject.SetActive(true);
        }
        
        /*if (_messageDictionary.ContainsKey(messageKey))
            _messageText.text = _messageDictionary[messageKey];
        else
            Debug.LogError("Can't find message in dictionary");*/
    }

    private void UpdateLevelText()
    {
        _levelText.text = Player.level.ToString();
    }

    private void StopAds()
    {
        //Ask for payment in order to stop ads
        if (PlayerData.CanShowAds() == 1)
            PlayerData.ToggleAds(false);
        else
            PlayerData.ToggleAds(true);
    }

    
    #endregion

}
