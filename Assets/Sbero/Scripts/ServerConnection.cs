using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkResult<T>
{
    public bool Success { get; private set; }
    public string Error { get; private set; }
    public T Result { get; private set; }

    public NetworkResult(T result)
    {
        Success = true;
        Result = result;
    }

    public NetworkResult(string error)
    {
        Error = error;
    }
}


public class ServerConnection : MonoBehaviour
{
    private EventSendQueue eventQueue;

    public void Start()
    {
        eventQueue = new EventSendQueue();
    }

    public void Update()
    {
        eventQueue.CheckQueue(this);
    }

    public void SubmitEvent(Messages.TrackEvent data)
    {
        eventQueue.SubmitEvent(new JRaw(JsonConvert.SerializeObject(data)));
    }

    public IEnumerator Send<T, R>(string endpoint, T data, Action<NetworkResult<R>> next)
    {
        string toSend = JsonConvert.SerializeObject(data);

        UnityWebRequest www = new UnityWebRequest
        {
            url = ExternalServices.SERVER_URL + endpoint,
            method = "POST",
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(toSend)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        www.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Sending req to " + www.url);
        Debug.Log("Sending data: " + toSend);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("Result: Net Error - " + www.error);
            next.Invoke(new NetworkResult<R>(www.error));
        }
        else if (www.isHttpError)
        {
            Debug.Log("Result: HTTP Error - " + www.error + " | " + www.downloadHandler.text);
            if (www.downloadHandler.text.Length > 0)
                next.Invoke(new NetworkResult<R>(www.downloadHandler.text));
            else
                next.Invoke(new NetworkResult<R>(www.error));
        }
        else
        {
            Debug.Log("Result: OK");
            next.Invoke(new NetworkResult<R>(JsonConvert.DeserializeObject<R>(www.downloadHandler.text)));
        }
    }

    public void SendStartup(long settingsVersion, Action<NetworkResult<Messages.StartupResponse>> next)
    {
        Messages.StartupRequest req = new Messages.StartupRequest
        {
            id = SystemInfo.deviceUniqueIdentifier,
            platform = Application.platform.ToString(),
            model = SystemInfo.deviceModel,
            appVersion = Application.version,
            settingsVersion = settingsVersion
        };

        StartCoroutine(Send("init", req, next));
    }

    public void SendEvents(List<JRaw> events, Action<NetworkResult<Messages.ConfirmResponse>> next)
    {
        Messages.TrackEvents req = new Messages.TrackEvents
        {
            id = SystemInfo.deviceUniqueIdentifier,
            platform = Application.platform.ToString(),
            events = events
        };

        StartCoroutine(Send("events", req, next));
    }
}
