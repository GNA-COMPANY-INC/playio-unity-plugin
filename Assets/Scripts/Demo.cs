using UnityEngine;
using PlayioSDK;

public class Demo : MonoBehaviour
{
    void Start()
    {
        Playio.Instance.SetUserId("lx5475");
    }
    public void OnClickSendRandomEvent()
    {
        string eventName = "random_event_" + UnityEngine.Random.Range(1, 1000);
        Playio.Instance.SendEvent(eventName, null);
        Debug.Log("Sent event: " + eventName);
    }
}