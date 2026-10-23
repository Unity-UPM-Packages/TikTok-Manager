using System;
using System.Collections;
using System.Collections.Generic;
using SDK;
using TheLegends.Base.UnitySingleton;
using UnityEngine;

namespace TheLegends.Base.TikTok
{
    public class TikTokManager : PersistentMonoSingleton<TikTokManager>
    {
        public enum TikTokStatus
        {
            None,
            Initializing,
            Initialized,
            Failed,
        }

        public TikTokStatus Status { get; private set; }

        public Action OnInitComplete = null;
        public Action<int, string> OnInitFailed = null;

        private readonly WaitForSeconds initDelay = new WaitForSeconds(0.1f);

        public void Init()
        {
            StartCoroutine(DoInit());
        }

        public IEnumerator DoInit(Action onComplete = null, Action<int, string> onFailed = null)
        {
#if USE_TIKTOK
            if (Status == TikTokStatus.Initializing || Status == TikTokStatus.Initialized)
            {
                yield break;
            }

            OnInitComplete = onComplete;
            OnInitFailed = onFailed;

            var settings = TikTokSettings.Instance;
            if (settings == null)
            {
                LogError("TikTokSettings not found. Create it via menu TripSoft/TikTok Setting");
                Status = TikTokStatus.Failed;
                OnInitFailed?.Invoke(-1, "TikTokSettings not found");
                yield break;
            }

            Status = TikTokStatus.Initializing;

            InitTikTokSDK(settings);

            while (Status == TikTokStatus.Initializing)
            {
                yield return null;
            }

            yield return initDelay;
#else
            yield break;
#endif
        }

        private void InitTikTokSDK(TikTokSettings settings)
        {
#if USE_TIKTOK
            var config = new TikTokConfig(
                settings.iosAppSecret,
                settings.iosAppID,
                settings.iosTikTokAppID,
                settings.androidAppSecret,
                settings.androidAppID,
                settings.androidTikTokAppID);

            if (settings.disableAutoTrack)
            {
                config.DisableAutoTrack();
            }

            // AppsFlyer is the MMP managing SKAN conversion values
            config.IOS_DisableSKAdNetworkSupport();

            if (settings.isDebug)
            {
                config.OpenDebugMode();
                config.SetLogLevel(TiktokLogLevel.Verbose);
            }

#if UNITY_EDITOR
            // UNITY_ANDROID/UNITY_IOS are also defined in the Editor, which makes the SDK call native code that doesn't exist here
            Log("TIKTOK SDK INIT SKIPPED IN EDITOR");
            Status = TikTokStatus.Initialized;
            OnInitComplete?.Invoke();
#else
            try
            {
                TikTokBusinessSDK.InitializeSdk(config, (success, code, message) =>
                {
                    if (Status != TikTokStatus.Initializing)
                    {
                        return;
                    }

                    if (success)
                    {
                        Log("TIKTOK SDK INIT COMPLETE");
                        Status = TikTokStatus.Initialized;
                        OnInitComplete?.Invoke();
                    }
                    else
                    {
                        LogError("TIKTOK SDK INIT FAILED: code=" + code + ", message=" + message);
                        Status = TikTokStatus.Failed;
                        OnInitFailed?.Invoke(code, message);
                    }
                });
            }
            catch (Exception e)
            {
                LogError("TIKTOK SDK INIT EXCEPTION: " + e);
                Status = TikTokStatus.Failed;
                OnInitFailed?.Invoke(-1, e.Message);
            }
#endif
#endif
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
#if USE_TIKTOK
            LogEvent(new TikTokBaseEvent(eventName, parameters, null));
#endif
        }

        public void LogAdRevenue(Dictionary<string, object> adRevenue)
        {
#if USE_TIKTOK
            LogEvent(new TikTokAdRevenueEvent(adRevenue, null));
#endif
        }

        private void LogEvent(TikTokBaseEvent tikTokEvent)
        {
#if USE_TIKTOK
            if (Status != TikTokStatus.Initialized)
            {
                LogError("TikTok not initialized");
                return;
            }

#if UNITY_EDITOR
            Log("Event Recorded (Editor): " + tikTokEvent.EventParams["eventName"] + " " + string.Join(", ", tikTokEvent.EventParams));
#else
            try
            {
                TikTokBusinessSDK.TrackTTEvent(tikTokEvent);
                Log("Event Recorded: " + tikTokEvent.EventParams["eventName"]);
            }
            catch (Exception e)
            {
                LogError("Event Record Failed: " + tikTokEvent.EventParams["eventName"] + " - " + e);
            }
#endif
#endif
        }

        public void Log(string message)
        {
            Debug.Log("TikTokManager------: " + message);
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning("TikTokManager------: " + message);
        }

        public void LogError(string message)
        {
            Debug.LogError("TikTokManager------: " + message);
        }
    }
}
