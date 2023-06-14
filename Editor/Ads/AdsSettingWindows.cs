
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using UnityEngine.Networking;
using AC.Base;

namespace AC.GameTool.Ads
{
    [InitializeOnLoad]
    public class AdsSettingWindows : EditorWindow
    {
        private const string Version = "1.2.4";
        private const string MobileAdsFolder = "GoogleMobileAds";
        private const string MobileAdsFileCore = "GoogleMobileAds";
        private const string UrlAdmobVersionLatest = "https://api.github.com/repos/googleads/googleads-mobile-unity/releases/latest";

        [SerializeField]
        VisualTreeAsset _visualTreeAsset;
        [SerializeField]
        StyleSheet _lbStatusStyle;

        static string _admobVer = "N/A";
        static bool _isAdmobInstall;
        static Label _lbStatus, _lbVersion;
        [SerializeField]
        AdmonData _admobData;
        AdmobDataSetting _admobSetting;
        PropertyField _admobDataFeild;
        bool _isStartLoadData;
        static bool _isSupportVersion;
        AdmobPackageData _admobPackageData;
        Button _btnGetNewAdmobVer, _btnUpdateNewAdmobVer;
        Label _lbLatestAdmonver;
        Label _progessDownload;
        static AdmobVersion _currentAdmobVer, _latestAdmonVer;

        [MenuItem("Game Tool/Ads Setting")]
        public static void ShowAdsSettingWindows()
        {
            AdsSettingWindows wnd = GetWindow<AdsSettingWindows>();
            wnd.maxSize = new Vector2(500, 500);
            wnd.minSize = wnd.maxSize;
            wnd.titleContent = new GUIContent("Ads Setting Windows");
        }


        static AdsSettingWindows()
        {
            EditorApplication.projectChanged += ApplicationUpdate;
        }
        public void CreateGUI()
        {
            _isStartLoadData = false;
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            SerializedObject so = new SerializedObject(this);

            // Import UXML
#if UNITY_2021_3_OR_NEWER
            VisualElement labelFromUXML = _visualTreeAsset.Instantiate();
#else
            VisualElement labelFromUXML = _visualTreeAsset.CloneTree();
#endif
            root.Add(labelFromUXML);
            root.styleSheets.Add(_lbStatusStyle);
            QueryAllElementInView();
            bool checkAdmobExist = CheckGoogleAdsExist();
            SetAdmonStatus(checkAdmobExist);
            LoadMobileAdsID();
            CreateOrRemoveDefineScript(checkAdmobExist);
            root.Bind(so);
            _isStartLoadData = true;
            BtnGetNewAdmonVer_Click();
        }
       
        
        static void SetAdmonStatus(bool isExits)
        {
            if (isExits)
            {
                if(_lbStatus != null)
                {
                    _lbStatus.text = "INSTALLED";
                    _lbStatus.AddToClassList("StatusOn");
                    _lbStatus.RemoveFromClassList("StatusOff");
                }
                if(_lbVersion != null)
                {                   
                    string versionText = _admobVer;
                    if(_currentAdmobVer.Num1 != 0)
                    {
                        if (!_isSupportVersion)
                        {
                            _lbVersion.AddToClassList("StatusOff");
                            _lbVersion.RemoveFromClassList("StatusOn");
                            versionText += "\n(Not Support)";
                        }
                        else
                        {
                            _lbVersion.AddToClassList("StatusOn");
                            _lbVersion.RemoveFromClassList("StatusOff");
                        }
                    }
                    else
                    {
                        versionText = "N/A";
                        _lbVersion.AddToClassList("StatusOff");
                        _lbVersion.RemoveFromClassList("StatusOn");
                    }
                    
                    _lbVersion.text = versionText;
                }               
            }
            else
            {
                if (_lbStatus != null)
                {
                    _lbStatus.text = "NOT FOUND";
                    _lbStatus.AddToClassList("StatusOff");
                    _lbStatus.RemoveFromClassList("StatusOn");
                }
                if(_lbVersion != null)
                {
                    _lbVersion.AddToClassList("StatusOff");
                    _lbVersion.RemoveFromClassList("StatusOn");
                    _lbVersion.text = _admobVer;
                }               
            }    
            
            CreateOrRemoveDefineScript(isExits);
        }

        void QueryAllElementInView()
        {
            _lbStatus = rootVisualElement.Q<Label>("lbStatus");
            _lbVersion = rootVisualElement.Q<Label>("lbVersion");
            _admobDataFeild = rootVisualElement.Q<PropertyField>("admobDataField");
            _admobDataFeild.RegisterCallback<ChangeEvent<bool>>(OnTestToggleChanged);
            _admobDataFeild.RegisterCallback<InputEvent>(OnInputEvent);
            _btnGetNewAdmobVer = rootVisualElement.Q<Button>("btnGetNewAdmobVer");
            _btnGetNewAdmobVer.clickable.clicked += BtnGetNewAdmonVer_Click;
            _btnUpdateNewAdmobVer = rootVisualElement.Q<Button>("btnUpdateNewVer");
            _btnUpdateNewAdmobVer.clickable.clicked += BtnUpdateNewAdmobVer_Click;
            _progessDownload = rootVisualElement.Q<Label>("lbProcessBar");
            _lbLatestAdmonver = rootVisualElement.Q<Label>("lbLatestVer");
        }
        void OnTestToggleChanged(ChangeEvent<bool> evt)
        {

            if (_admobSetting != null && _isStartLoadData)
            {

                SaveSettingData();
            }
        }
        void OnInputEvent(InputEvent evt)
        {
            if (_admobSetting != null && _isStartLoadData)
            {
                SaveSettingData();
            }
        }
        static bool CheckGoogleAdsExist()
        {
            string pathFolderAdmob = Path.Combine(Application.dataPath, MobileAdsFolder);
            if (!Directory.Exists(pathFolderAdmob))
                return false;
            string[] files = Directory.GetFiles(pathFolderAdmob, MobileAdsFileCore + "*.dll", SearchOption.TopDirectoryOnly);
            if(files.Length > 0)
            {
                string fileCore = files[0].Replace("\\","/").Replace(Application.dataPath,"Assets");
                var asset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(fileCore);
                var labelList = AssetDatabase.GetLabels(asset);
                if(labelList.Length > 0)
                {
                    for (int i = 0; i < labelList.Length; i++)
                    {
                        if (labelList[i].Contains("version"))
                        {
                            _currentAdmobVer = new AdmobVersion(labelList[i]);
                            _isSupportVersion = (_currentAdmobVer.Num1 == 7 && _currentAdmobVer.Num2 > 0);
                            _admobVer = _currentAdmobVer.ToStringVer();
                            break;
                        }
                    }
                }
                else
                {
                    _admobVer = "N/A";
                }               
            }           
            return files.Length > 0;
        }

        void LoadMobileAdsID()
        {
            _admobSetting = AdmobDataSetting.LoadInstance();
            _admobData = _admobSetting.AdmobData;
            //_isStartLoadData = true;                      
        }

        void SaveSettingData()
        {
            _admobSetting.AdmobData = _admobData;
            EditorUtility.SetDirty(_admobSetting);
        }

        [InitializeOnLoadMethod]
        static void ApplicationReLoad()
        {
            bool check = CheckGoogleAdsExist();
            if (_isAdmobInstall != check)
            {
                _isAdmobInstall = check;
                SetAdmonStatus(check);
                
            }
        }

        static void ApplicationUpdate()
        {
            bool check = CheckGoogleAdsExist();
            if (_isAdmobInstall != check)
            {
                _isAdmobInstall = check;
                SetAdmonStatus(check);
            }
        }
        static void CreateOrRemoveDefineScript(bool isAdmobExist)
        {
            if(isAdmobExist && _isSupportVersion)
            {
                BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                ConditionalCompilationUtils.AddDefineIfNecessary("ADMOB_AD", buildTargetGroup);
            }
            else
            {
                BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                ConditionalCompilationUtils.RemoveDefineIfNecessary("ADMOB_AD", buildTargetGroup);
            }
        }


        void BtnGetNewAdmonVer_Click()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetGoogleDataVerSionLatest());
        }
        IEnumerator GetGoogleDataVerSionLatest()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(UrlAdmobVersionLatest))
            {
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        _admobPackageData = JsonUtility.FromJson<AdmobPackageData>(webRequest.downloadHandler.text);
                        _latestAdmonVer = new AdmobVersion(_admobPackageData.tag_name);
                        _lbLatestAdmonver.text = _latestAdmonVer.ToStringVer();
                        _lbLatestAdmonver.RemoveFromClassList("StatusOff");
                        _lbLatestAdmonver.AddToClassList("StatusOn");
                        if (_currentAdmobVer == null ||(_currentAdmobVer != null && (_currentAdmobVer.Num1< _latestAdmonVer.Num1 || (_currentAdmobVer.Num1 == _latestAdmonVer.Num1 && _currentAdmobVer.Num2 < _latestAdmonVer.Num2) || ((_currentAdmobVer.Num1 == _latestAdmonVer.Num1 && _currentAdmobVer.Num2 == _latestAdmonVer.Num2 && _currentAdmobVer.Num3 < _latestAdmonVer.Num3)))))
                        {
                            //hien buttom Update
                            if(_btnUpdateNewAdmobVer != null)
                                _btnUpdateNewAdmobVer.SetEnabled(true);
                        }
                        else
                        {
                            //an buttom update
                            if (_btnUpdateNewAdmobVer != null)
                                _btnUpdateNewAdmobVer.SetEnabled(false);
                        }
                        break;
                    default:
                        _lbLatestAdmonver.text = "N/A";
                        _lbLatestAdmonver.AddToClassList("StatusOff");
                        _lbLatestAdmonver.RemoveFromClassList("StatusOn");
                        break;
                }
                webRequest.Dispose();
            }
        }


        IEnumerator DownLoadNewAdmobVersionPackage(string fileUrl, string fileName)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(fileUrl))
            {
                webRequest.downloadHandler = new DownloadHandlerFile(fileName);
                UnityWebRequestAsyncOperation downloadAsync =  webRequest.SendWebRequest();
                while(!downloadAsync.isDone)
                {
                    //_progessDownload.value = Mathf.FloorToInt(downloadAsync.progress * 100);
                    _progessDownload.style.width = downloadAsync.progress * 450;
                    yield return null;
                }
                if(webRequest.result == UnityWebRequest.Result.Success)
                {
                    //Done
                    _progessDownload.style.width = 450;
                    AssetDatabase.ImportPackage(fileName, true);
                }
                webRequest.Dispose();
            }

        }
        void BtnUpdateNewAdmobVer_Click()
        {
            if(_admobPackageData != null && _admobPackageData.assets != null && _admobPackageData.assets.Count > 0 && !string.IsNullOrEmpty(_admobPackageData.assets[0].browser_download_url))
            {
                string filename = _latestAdmonVer.ToStringVer() + ".unitypackage";
                string pathPackage = Path.Combine(Application.temporaryCachePath, filename);
                _progessDownload.style.width = 0;
                LogManager.Log("Inport Package: " + pathPackage);
                if (!File.Exists(pathPackage))
                {
                    EditorCoroutineUtility.StartCoroutineOwnerless(DownLoadNewAdmobVersionPackage(_admobPackageData.assets[0].browser_download_url, pathPackage));
                }
                else
                {
                    _progessDownload.style.width = 450;
                    AssetDatabase.ImportPackage(pathPackage, true);
                }
            }
        }
    }
}
