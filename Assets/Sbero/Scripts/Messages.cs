using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public static class Messages
{
    [Serializable]
    public class StartupRequest
    {
        public string id;
        public string platform;
        public string model;
        public string appVersion;
        public long settingsVersion;
    }

    [Serializable]
    public class StartupResponse
    {
        public GameSettings settings;
        public long settingsVersion;
    }

    public abstract class TrackEvent { }

    [Serializable]
    public class TrackAd : TrackEvent
    {
        public string kind = "ad";
        public string network;
        public string place;
    }

    [Serializable]
    public class TrackLevel : TrackEvent
    {
        public string kind = "level";
        public int number;
        public bool completed;
    }

    [Serializable]
    public class TrackPurchace: TrackEvent
    {
        public string kind = "purchace";
        public string transaction;
        public string product;
        public decimal price;
        public string currency;
    }

    [Serializable]
    public class TrackEvents
    {
        public string id;
        public string platform;
        public List<JRaw> events;
    }


    [Serializable]
    public class ConfirmResponse
    {
        public bool ok;
    }
}
