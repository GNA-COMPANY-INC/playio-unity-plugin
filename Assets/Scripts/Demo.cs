using UnityEngine;
using PlayioSDK;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
    void Start()
    {
        Playio.Instance.SetUserId("lx5475");
    }

    public void OnClickSendRandomEvent()
    {
        string eventName = "random_event_" + UnityEngine.Random.Range(1, 1000);
        Playio.Instance.SendEvent(eventName, new Dictionary<string, object>
        {
            {"level", 30},
            {"name", "Colandlxl"}
        });
        Debug.Log("Sent event: " + eventName);
    }
}