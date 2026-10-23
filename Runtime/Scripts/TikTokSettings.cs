using UnityEngine;

namespace TheLegends.Base.TikTok
{
    [CreateAssetMenu(fileName = "TikTokSettings", menuName = "DataAsset/TikTokSettings")]
    public class TikTokSettings : ScriptableObject
    {
        public const string ResDir = "Assets/TripSoft/TikTok/Resources";
        public const string FileName = "TikTokSettingsAsset";
        public const string FileExtension = ".asset";

        private static TikTokSettings _instance;
        public static TikTokSettings Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = Resources.Load<TikTokSettings>(FileName);
                return _instance;
            }
        }

        [Header("iOS")]
        public string iosAppSecret;
        public string iosAppID;
        public string iosTikTokAppID;

        [Header("Android")]
        public string androidAppSecret;
        public string androidAppID;
        public string androidTikTokAppID;

        [Header("Options")]
        public bool disableAutoTrack;
        public bool isDebug;
    }
}
