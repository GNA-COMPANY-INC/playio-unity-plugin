using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace PlayioSDK
{
    public class Playio : MonoBehaviour
    {
        [SerializeField]
        private LogLevel logLevel = LogLevel.NONE;

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
            PlayioConfig config = new PlayioConfig.Builder()
                .SetLogLevel(logLevel)
                .Build();
            Init(config);
        }

        void Start()
        {
            playTimeTracker = new PlayTimeTracker(this);
            playTimeTracker.OnPlayTimeRecorded += OnPlayTimeRecorded;
            PlayioLogger.Log("Playio SDK initialized, IsSDKStopped: " + IsSdkStopped());
            if (!IsSdkStopped())
            {
                playTimeTracker.StartTracking();
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (playTimeTracker == null) return;

            if (pauseStatus)
            {
                // App is going to background - stop tracking and send data
                playTimeTracker.StopTracking();
                playTimeTracker.FlushPlayTime();
            }
            else
            {
                // App is resuming - restart tracking if SDK is active
                if (!IsSdkStopped())
                {
                    playTimeTracker.StartTracking();
                }
            }
        }

        void OnApplicationQuit()
        {
            if (playTimeTracker == null) return;

            // Stop tracking and send final data
            playTimeTracker.StopTracking();
            playTimeTracker.FlushPlayTime();
            playTimeTracker.Dispose();
        }

        void OnDestroy()
        {
            if (playTimeTracker != null)
            {
                playTimeTracker.OnPlayTimeRecorded -= OnPlayTimeRecorded;
                playTimeTracker.Dispose();
                playTimeTracker = null;
            }
        }

        public static Playio Instance
        {
            get
            {
                if (instance == null)
                {
                    PlayioLogger.LogError("Playio instance is not found in the scene. Please add the Playio prefab to your scene.");
                }
                return instance;
            }
        }

        public void Init(PlayioConfig config)
        {
            PlayioLogger.SetLogLevel(config.logLevel);
            PlayioAPI.Init(config);
        }

        public void SetUserId(string userId)
        {
            PlayioAPI.SetUserId(userId);
        }

        public void SetUserAttributes(Dictionary<string, object> attributes)
        {
            foreach (var attribute in attributes)
            {
                PlayioLogger.Log($"Playio user attribute set: {attribute.Key} = {attribute.Value}");
            }
        }

        public void StartSdk()
        {
            PlayioAPI.Start();
            if (playTimeTracker != null && !IsSdkStopped())
            {
                playTimeTracker.StartTracking();
            }
        }

        public void StopSdk()
        {
            PlayioAPI.Stop();
            if (playTimeTracker != null)
            {
                playTimeTracker.StopTracking();
                playTimeTracker.FlushPlayTime(); // Send any remaining playtime
            }
        }


        public bool IsSdkStopped()
        {
            return PlayioAPI.IsStopped();
        }

        public string GetSdkVersion()
        {
            return PlayioAPI.GetSdkVersion();
        }

        public void SendEvent(string eventName, Dictionary<string, object> eventParams)
        {
            PlayioAPI.SendEvent(eventName, eventParams);
        }

        public void DisableCollectAdvertisingIdentifier(bool disable)
        {
            PlayioAPI.DisableCollectAdvertisingIdentifier(disable);
        }

        private void OnPlayTimeRecorded(long startMillis, long endMillis)
        {
            PlayioLogger.Log($"Time event: {startMillis} ~ {endMillis}");
            PlayioAPI.OnTimeEvent(startMillis, endMillis);
        }
    }
}