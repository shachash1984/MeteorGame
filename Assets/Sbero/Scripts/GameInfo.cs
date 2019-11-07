using System;

public class GameInfo
{
    public static long SettingsVersion = -1;
    public static GameSettings Settings = new GameSettings
    {
        ads = new AdsSettings
        {
            showAdAfterCompletes = 2,
            showAdAfterFails = 1
        }
    };
}

[Serializable]
public class GameSettings
{
    public AdsSettings ads;
}

[Serializable]
public class AdsSettings
{
    public int showAdAfterCompletes;
    public int showAdAfterFails;
}
