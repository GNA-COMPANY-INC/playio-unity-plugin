using System;
using System.Collections;
using UnityEngine;

namespace PlayioSDK
{
    internal class PlayTimeTracker : IDisposable
    {
        private float accumulatedPlayTime = 0f;
        private float sessionStartTime = 0f;
        private bool isTracking = false;
        private bool isDisposed = false;
        private Coroutine trackingRoutine;
        private MonoBehaviour coroutineRunner;
        private readonly float reportIntervalSeconds;

        public delegate void PlayTimeEventHandler(float playTime);
        public event PlayTimeEventHandler OnPlayTimeRecorded;

        /// <summary>
        /// Creates a new PlayTimeTracker instance.
        /// </summary>
        /// <param name="runner">MonoBehaviour to run coroutines on</param>
        /// <param name="reportIntervalSeconds">Interval in seconds to report playtime (default: 300 seconds = 5 minutes)</param>
        internal PlayTimeTracker(MonoBehaviour runner, float reportIntervalSeconds = 300f)
        {
            if (runner == null)
            {
                throw new ArgumentNullException(nameof(runner));
            }

            coroutineRunner = runner;
            this.reportIntervalSeconds = Mathf.Max(1f, reportIntervalSeconds); // Minimum 1 second
        }

        public void StartTracking()
        {
            if (isDisposed)
            {
                PlayioLogger.LogWarning("Cannot start tracking on disposed PlayTimeTracker");
                return;
            }

            if (coroutineRunner == null || !coroutineRunner.gameObject.activeInHierarchy)
            {
                PlayioLogger.LogWarning("Cannot start tracking: coroutineRunner is null or inactive");
                return;
            }

            if (!isTracking)
            {
                isTracking = true;
                sessionStartTime = Time.realtimeSinceStartup;

                if (trackingRoutine != null)
                {
                    coroutineRunner.StopCoroutine(trackingRoutine);
                }
                trackingRoutine = coroutineRunner.StartCoroutine(TrackingRoutine());
                
                PlayioLogger.Log("Playtime tracking started.");
            }
        }

        public void StopTracking()
        {
            if (!isTracking)
            {
                return;
            }

            // Accumulate the remaining session time before stopping
            AccumulateCurrentSession();
            isTracking = false;

            if (trackingRoutine != null && coroutineRunner != null)
            {
                coroutineRunner.StopCoroutine(trackingRoutine);
                trackingRoutine = null;
            }

            PlayioLogger.Log($"Playtime tracking stopped. Accumulated time: {accumulatedPlayTime}s");
        }

        private IEnumerator TrackingRoutine()
        {
            while (isTracking)
            {
                yield return new WaitForSecondsRealtime(reportIntervalSeconds);

                if (!isTracking)
                {
                    yield break;
                }

                // Check if coroutineRunner is still valid
                if (coroutineRunner == null || !coroutineRunner.gameObject.activeInHierarchy)
                {
                    PlayioLogger.LogWarning("CoroutineRunner became invalid. Stopping tracking.");
                    isTracking = false;
                    yield break;
                }

                // Accumulate time and reset session start
                AccumulateCurrentSession();

                // Send accumulated playtime
                SendPlayTimeEvent();
            }
        }

        private void SendPlayTimeEvent()
        {
            if (accumulatedPlayTime > 0)
            {
                float playTimeToSend = accumulatedPlayTime;
                accumulatedPlayTime = 0f;

                try
                {
                    PlayioLogger.Log($"Sending playtime event: {playTimeToSend}s");
                    OnPlayTimeRecorded?.Invoke(playTimeToSend);
                }
                catch (Exception ex)
                {
                    PlayioLogger.LogError($"Error invoking OnPlayTimeRecorded: {ex.Message}");
                    // Restore accumulated time on failure
                    accumulatedPlayTime += playTimeToSend;
                }
            }
        }

        /// <summary>
        /// Accumulates the current session time if tracking is active.
        /// Resets the session start time to now.
        /// </summary>
        private void AccumulateCurrentSession()
        {
            if (isTracking)
            {
                float currentSessionTime = Time.realtimeSinceStartup - sessionStartTime;
                accumulatedPlayTime += currentSessionTime;
                sessionStartTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// Manually triggers sending of accumulated playtime.
        /// Useful for force-flushing data.
        /// </summary>
        public void FlushPlayTime()
        {
            // If currently tracking, update the accumulated time first
            AccumulateCurrentSession();
            SendPlayTimeEvent();
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            StopTracking();
            OnPlayTimeRecorded = null;
            isDisposed = true;
        }

    }
}