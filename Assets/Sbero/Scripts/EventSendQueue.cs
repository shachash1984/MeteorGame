using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSendQueue
{

    // Sbumitted events go here first, then are moved to sendQueue when sending or saving/loading
    private List<JRaw> submitQueue = new List<JRaw>();

    // Events that are ready to send
    private List<JRaw> sendQueue = new List<JRaw>();

    // Send in progress
    private bool sendingEvents;

    // Have fresh events to send
    private bool haveEvents;

    // Queue is dirty and needs saving
    private bool queueDirty;

    // Wait timer after errors
    private float sendErrorTimer = float.MinValue;

    private object queueLock = new object();

    public EventSendQueue()
    {
        if (PlayerPrefs.HasKey("eventQueue"))
        {
            List<JRaw> loaded = JsonConvert.DeserializeObject<List<JRaw>>(PlayerPrefs.GetString("eventQueue"));
            sendQueue.AddRange(loaded);
            Debug.LogFormat("Loaded {0} unsent events", sendQueue.Count);
        }
    }

    // Adds event to the queue
    public void SubmitEvent(JRaw data)
    {
        lock (queueLock)
        {
            submitQueue.Add(data);
            haveEvents = true;
            queueDirty = true;
        }
    }

    // Checks the queue, saves if needed then starts a send if there's anything to send 
    public void CheckQueue(ServerConnection connection)
    {
        lock (queueLock)
        {
            if (queueDirty)
            {
                List<JRaw> toSave = new List<JRaw>();
                toSave.AddRange(submitQueue);
                toSave.AddRange(sendQueue);
                PlayerPrefs.SetString("eventQueue", JsonConvert.SerializeObject(toSave));
                PlayerPrefs.Save();
                Debug.LogFormat("Saved {0} unsent events", toSave.Count);
                queueDirty = false;
            }

            if (haveEvents && !sendingEvents && Time.time > sendErrorTimer)
            {
                // Try sending a batch of events
                sendingEvents = true;
                haveEvents = false;
                sendQueue.AddRange(submitQueue);
                submitQueue.Clear();
                Debug.LogFormat("Sending {0} events to server", sendQueue.Count);
                connection.SendEvents(sendQueue, AfterSend);
            }
        }
    }


    // Finishes a send
    private void AfterSend(NetworkResult<Messages.ConfirmResponse> result)
    {
        lock (queueLock)
        {
            if (result.Success)
            {
                Debug.LogFormat("Sent {0} events to server", sendQueue.Count);
                sendQueue.Clear();
                queueDirty = true;
            }
            else
            {
                Debug.LogFormat("Failed to send {0} events to server: {1}", sendQueue.Count, result.Error);
                sendErrorTimer = Time.time + 60;
                haveEvents = true;
            }
            sendingEvents = false;
        }
    }
}
