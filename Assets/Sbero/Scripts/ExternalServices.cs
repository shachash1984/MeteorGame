public class ExternalServices
{
    public const string SERVER_URL = "http://mob.tbm2.com/meteor/";

#if UNITY_IOS
    public const string UNITY_GAME_ID = "2579460";
#elif UNITY_ANDROID
    public const string UNITY_GAME_ID = "2579461";
#endif

    public const string NO_ADS_PRODUCT_ID = "com.tbm2.meteor.noads";
}
