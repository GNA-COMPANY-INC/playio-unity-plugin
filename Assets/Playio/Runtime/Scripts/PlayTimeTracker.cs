using System;
using System.Collections;
using UnityEngine;

namespace PlayioSDK
{
    internal class PlayTimeTracker : IDisposable
    {
        private long sessionStartMillis = 0;
        private bool isTracking = false;
        private bool isDisposed = false;
        private Coroutine trackingRoutine;
        private MonoBehaviour coroutineRunner;
        private readonly float reportIntervalSeconds;

        public delegate void PlayTimeEventHandler(long startMillis, long endMillis);
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
                sessionStartMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                if (trackingRoutine != null)
                {
                    coroutineRunner.StopCoroutine(trackingRoutine);
                }
                trackingRoutine = coroutineRunner.StartCoroutine(TrackingRoutine());

                PlayioLogger.Log($"Playtime tracking started at {sessionStartMillis}");
            }
        }

        public void StopTracking()
        {
            if (!isTracking)
            {
                return;
            }

            isTracking = false;

            if (trackingRoutine != null && coroutineRunner != null)
            {
                coroutineRunner.StopCoroutine(trackingRoutine);
                trackingRoutine = null;
            }

            PlayioLogger.Log("Playtime tracking stopped.");
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

                // Send time event and start new session
                SendPlayTimeEvent();
            }
        }

        private void SendPlayTimeEvent()
        {
            if (sessionStartMillis <= 0)
            {
                return;
            }

            long endMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long startMillis = sessionStartMillis;

            // Start new session immediately
            sessionStartMillis = endMillis;

            try
            {
                PlayioLogger.Log($"Sending time event: {startMillis} ~ {endMillis}");
                OnPlayTimeRecorded?.Invoke(startMillis, endMillis);
            }
            catch (Exception ex)
            {
                PlayioLogger.LogError($"Error invoking OnPlayTimeRecorded: {ex.Message}");
            }
        }

        /// <summary>
        /// Manually triggers sending of time event.
        /// Useful for force-flushing data before pause/quit.
        /// </summary>
        public void FlushPlayTime()
        {
            if (sessionStartMillis > 0)
            {
                SendPlayTimeEvent();
            }
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