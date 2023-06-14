using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AC.GameTool.Ads
{
    [System.Serializable]
    public class AdmobPackageData
    {
        public int id;
        public string name;
        public string url;
        public string assets_url;
        public string tag_name;
        public List<AdmobPackageDataAsset> assets;
    }


    [System.Serializable]
    public class AdmobPackageDataAsset
    {
        public int id;
        public string name;
        public string url;
        public string browser_download_url;
    }

    [System.Serializable]
    public class AdmobVersion
    {
        public int Num1;
        public int Num2;
        public int Num3;
        public AdmobVersion(string verText)
        {
            verText = Regex.Replace(verText, "[^\\d\\.]", "");
            string[] verStr = verText.Split('.');
            for (int i = 0; i < 3 && i < verStr.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        int.TryParse(verStr[0], out Num1);
                        break;
                    case 1:
                        int.TryParse(verStr[1], out Num2);
                        break;
                    case 2:
                        int.TryParse(verStr[2], out Num3);
                        break;
                }
            }
        }

        public string ToStringVer()
        {
            return string.Format("{0}.{1}.{2}", Num1, Num2, Num3);
        }
    }
}
