using System.Collections;
using UnityEngine;

namespace PlayioSDK
{
    internal class PlayTimeTracker
    {
        private float accumulatedPlayTime = 0f;
        private float sessionStartTime = 0f;
        private bool isTracking = false;
        private Coroutine trackingRoutine;
        private MonoBehaviour coroutineRunner;

        public delegate void PlayTimeEventHandler(float playTime);
        public event PlayTimeEventHandler OnPlayTimeRecorded;

        internal PlayTimeTracker(MonoBehaviour runner)
        {
            coroutineRunner = runner;
        }

        public void StartTracking()
        {
            if (!isTracking)
            {
                isTracking = true;
                sessionStartTime = Time.unscaledTime;

                if (trackingRoutine != null)
                {
                    coroutineRunner.StopCoroutine(trackingRoutine);
                }
                trackingRoutine = coroutineRunner.StartCoroutine(TrackingRoutine());
                Debug.Log("Playtime tracking started.");
            }
        }

        public void StopTracking()
        {
            if (isTracking)
            {
                isTracking = false;

                float currentSessionTime = Time.unscaledTime - sessionStartTime;
                accumulatedPlayTime += currentSessionTime;

                if (trackingRoutine != null)
                {
                    coroutineRunner.StopCoroutine(trackingRoutine);
                    trackingRoutine = null;
                }
            }
        }

        private IEnumerator TrackingRoutine()
        {
            while (isTracking)
            {
                yield return new WaitForSecondsRealtime(5f);

                if (isTracking)
                {
                    float currentSessionTime = Time.unscaledTime - sessionStartTime;
                    accumulatedPlayTime += currentSessionTime;

                    sessionStartTime = Time.unscaledTime;

                    InvokeHandlerAndReset();
                }
            }
        }

        public void InvokeHandlerAndReset()
        {
            if (accumulatedPlayTime > 0)
            {
                // Send the accumulated playtime to the server or analytics
                Debug.Log($"Sending playtime event: {accumulatedPlayTime}");
                OnPlayTimeRecorded?.Invoke(accumulatedPlayTime);
                accumulatedPlayTime = 0f;
            }
        }
    }
}