
using AC.Attribute;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AC.GameTool.Ads
{
    public class AdmobDataSetting : ScriptableObject
    {
        private const string AdmobDataSettingsResDir = "Assets/Resources";
        private const string fileName = "Admob Data Setting";

        public AdmonData AdmobData;
        public static AdmobDataSetting LoadInstance()
        {
            //Read from resources.
            var instance = Resources.Load<AdmobDataSetting>(fileName);

#if UNITY_EDITOR
            //Create instance if null.
            if (instance == null)
            {
                Directory.CreateDirectory(AdmobDataSettingsResDir);
                instance = ScriptableObject.CreateInstance<AdmobDataSetting>();
                string assetPath = Path.Combine(
                    AdmobDataSettingsResDir,
                    fileName + ".asset");
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            }
#endif
            return instance;
        }
    }


    [System.Serializable]
    public class AdmonData
    {
        [Header("Admob Setting")]       
        public bool IsTagForChild;
        public int TimeBetweenInterstitial = 30;
        public bool AutoLoadBannerAdOnStartup = true;
        public AdmonDataPlatform Android;
        public AdmonDataPlatform IOS;
    }
    [System.Serializable]
    public class AdmonDataPlatform
    {
        public bool IsUseOpenApp;
        [ConditionField("IsUseOpenApp")]
        public string OpenAppUnitID;
        public string BanerUnitID;
        public string RewardUnitID;
        public string InterstitialUnitID;
        public string RewardInterUnitID;
        public string[] TestDevices;
    }
}

