using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData  {

    public static bool showAds = true;
    public static bool playSound = true;
    public static int level = 1;
    public static bool watchedAd = false;
    public static int asteroidsLeftInCurrentLevel;
    public static int CanShowAds()
    {
        if (PlayerPrefs.HasKey("ShowAds"))
            return PlayerPrefs.GetInt("ShowAds");
        else
        {
            PlayerPrefs.SetInt("ShowAds", 1);
            return 1;
        }
    }
    public static void ToggleAds(bool on)
    {
        if (on)
        {
            showAds = true;
            PlayerPrefs.SetInt("ShowAds", 1);
        }
        else
        {
            showAds = false;
            PlayerPrefs.SetInt("ShowAds", 0);
        }
    }
    public static int CanPlaySound()
    {
        if (PlayerPrefs.HasKey("PlaySound"))
            return PlayerPrefs.GetInt("PlaySound");
        else
        {
            PlayerPrefs.SetInt("PlaySound", 1);
            return 1;
        }
    }
    public static void ToggleSound(bool on)
    {
        if (on)
        {
            playSound = true;
            PlayerPrefs.SetInt("PlaySound", 1);
        }
        else
        {
            playSound = false;
            PlayerPrefs.SetInt("PlaySound", 0);
        }
    }
    public static int GetCurrentLevel()
    {
        if (PlayerPrefs.HasKey("Level"))
            return PlayerPrefs.GetInt("Level");
        else
            PlayerPrefs.SetInt("Level", 1);
        return 1;
    }
    public static void SetCurrentLevel(int currentLevel)
    {
        PlayerPrefs.SetInt("Level", currentLevel);
    }

}
