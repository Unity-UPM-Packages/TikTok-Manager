using System.IO;
using UnityEditor;
using UnityEngine;

namespace TheLegends.Base.TikTok
{
    [CustomEditor(typeof(TikTokSettings))]
    public class TikTokSettingsEditor : Editor
    {
        private static TikTokSettings instance = null;

        public static TikTokSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<TikTokSettings>(TikTokSettings.FileName);
                }

                if (instance != null)
                {
                    Selection.activeObject = instance;
                }
                else
                {
                    Directory.CreateDirectory(TikTokSettings.ResDir);

                    instance = CreateInstance<TikTokSettings>();

                    string assetPath = Path.Combine(TikTokSettings.ResDir, TikTokSettings.FileName);
                    string assetPathWithExtension = Path.ChangeExtension(assetPath, TikTokSettings.FileExtension);
                    AssetDatabase.CreateAsset(instance, assetPathWithExtension);
                    AssetDatabase.SaveAssets();
                }

                return instance;
            }
        }

        [MenuItem("TripSoft/TikTok Setting")]
        public static void OpenInspector()
        {
            if (Instance == null)
            {
                Debug.Log("Creat new TikTok Setting");
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("iOS", EditorStyles.boldLabel);
            Instance.iosAppSecret = EditorGUILayout.TextField("App Secret", Instance.iosAppSecret);
            Instance.iosAppID = EditorGUILayout.TextField("App ID (Apple ID)", Instance.iosAppID);
            Instance.iosTikTokAppID = EditorGUILayout.TextField("TikTok App ID", Instance.iosTikTokAppID);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Android", EditorStyles.boldLabel);
            Instance.androidAppSecret = EditorGUILayout.TextField("App Secret", Instance.androidAppSecret);
            Instance.androidAppID = EditorGUILayout.TextField("App ID (Package Name)", Instance.androidAppID);
            Instance.androidTikTokAppID = EditorGUILayout.TextField("TikTok App ID", Instance.androidTikTokAppID);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            Instance.disableAutoTrack = EditorGUILayout.Toggle("Disable Auto Track", Instance.disableAutoTrack);
            Instance.isDebug = EditorGUILayout.Toggle("Is Debug", Instance.isDebug);

            if (Instance.isDebug)
            {
                EditorGUILayout.HelpBox("Is Debug sends all events to Test Events only. Test events are discarded and NOT used for campaign reporting. Turn this OFF before release!", MessageType.Warning);
            }

            EditorGUILayout.HelpBox("Multiple TikTok App IDs: comma-separated, no spaces, no trailing comma.", MessageType.Info);

            if (EditorGUI.EndChangeCheck())
            {
                Save((TikTokSettings)target);
            }
        }

        private void Save(Object target)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
