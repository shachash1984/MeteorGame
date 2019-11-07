using System;
using UnityEngine;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public enum AdResult
{
    SHOWED, SKIPPED, NOT_AVAILABLE
}

public class AdsManager
{
    private static bool initialized;

    public static void Initialize()
    {

        if (!initialized)
        {
            initialized = true;
#if UNITY_ADS
            Advertisement.Initialize(ExternalServices.UNITY_GAME_ID);
            Debug.LogFormat("Initialized ads for ID {0} isInitialized={1} isSupported={2}", ExternalServices.UNITY_GAME_ID, Advertisement.isInitialized, Advertisement.isSupported);
#endif
        }
    }

    public static bool IsAvailable(string placement)
    {
#if UNITY_ADS
        return Advertisement.IsReady(placement);
#else
        return false;
#endif
    }

    public static void Show(string placement, Action<AdResult, String> after)
    {
#if UNITY_ADS
        Debug.LogFormat("Showing ad {0} state is {1}", placement, Advertisement.GetPlacementState(placement));
        if (Advertisement.GetPlacementState(placement) == PlacementState.Ready)
        {
            Advertisement.Show(placement, new ShowOptions
            {
                resultCallback = (res) =>
                {
                    Debug.Log("Ad complete: " + res.ToString());
                    switch (res)
                    {
                        case ShowResult.Failed: after.Invoke(AdResult.NOT_AVAILABLE, "UNITY"); break;
                        case ShowResult.Finished: after.Invoke(AdResult.SHOWED, "UNITY"); break;
                        case ShowResult.Skipped: after.Invoke(AdResult.SKIPPED, "UNITY"); break;
                    }
                }
            });
        }
        else
        {
            after.Invoke(AdResult.NOT_AVAILABLE, "UNITY");
        }
#else
        after.Invoke(AdResult.NOT_AVAILABLE, "N/A");
#endif
    }
}
