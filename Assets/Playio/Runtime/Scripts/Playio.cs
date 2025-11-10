using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace PlayioSDK
{
    public class Playio : MonoBehaviour
    {
        [SerializeField]
        private string clientId;

        private static Playio instance;
        private PlayTimeTracker playTimeTracker;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;

            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            if (string.IsNullOrEmpty(clientId))
            {
                Debug.LogError("Playio Client ID is not set!");
                return;
            }
            PlayioConfig config = new PlayioConfig.Builder()
                .SetClientId(clientId)
                .SetLogLevel(LogLevel.DEBUG)
                .Build();
            Init(config);
        }

        void Start()
        {
            playTimeTracker = new PlayTimeTracker(this);
            playTimeTracker.OnPlayTimeRecorded += OnPlayTimeRecorded;
            Debug.Log("Playio SDK initialized, IsSDKStopped: " + IsSdkStopped());
            if (!IsSdkStopped())
            {
                playTimeTracker.StartTracking();
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (IsSdkStopped()) return;
            if (playTimeTracker == null) return;
            if (pauseStatus)
            {
                playTimeTracker.StopTracking();
                playTimeTracker.InvokeHandlerAndReset();
            }
            else
            {
                playTimeTracker.StartTracking();
            }
        }

        void OnApplicationQuit()
        {
            if (IsSdkStopped()) return;
            if (playTimeTracker == null) return;
            playTimeTracker.StopTracking();
            playTimeTracker.InvokeHandlerAndReset();
        }

        void OnDestroy()
        {
            if (playTimeTracker != null)
            {
                playTimeTracker.OnPlayTimeRecorded -= OnPlayTimeRecorded;
            }
        }

        public static Playio Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogError("Playio instance is not found in the scene. Please add the Playio prefab to your scene.");
                }
                return instance;
            }
        }

        public void Init(PlayioConfig config)
        {
            Debug.Log("Initializing Playio SDK with Client ID: " + config.clientId);
            PlayioAPI.Init(config);
        }

        public void SetUserId(string userId)
        {
            PlayioAPI.SetUserId(userId);
        }

        public void SetUserAttributes(Dictionary<string, string> attributes)
        {
            foreach (var attribute in attributes)
            {
                Debug.Log($"Playio user attribute set: {attribute.Key} = {attribute.Value}");
            }
        }

        public void StartSdk()
        {
            PlayioAPI.Start();
        }

        public void StopSdk()
        {
            PlayioAPI.Stop();
            playTimeTracker.StopTracking();
        }


        public bool IsSdkStopped()
        {
            return PlayioAPI.IsStopped();
        }

        public string GetSdkVersion()
        {
            return PlayioAPI.GetSdkVersion();
        }

        public void SendEvent(string eventName, Dictionary<string, string> eventParams)
        {
            PlayioAPI.SendEvent(eventName, eventParams);
        }

        public void DisableCollectAdvertisingIdentifier(bool disable)
        {
            PlayioAPI.DisableCollectAdvertisingIdentifier(disable);
        }

        private void OnPlayTimeRecorded(float playTime)
        {
            Debug.Log("Playtime recorded: " + playTime + " seconds.");
            var eventParams = new Dictionary<string, string>
            {
                { "play_time_seconds", playTime.ToString() }
            };
            SendEvent("play_time_recorded", eventParams);
        }
    }
}