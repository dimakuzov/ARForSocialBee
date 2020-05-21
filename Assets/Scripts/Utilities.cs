using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SocialBeeAR
{

    public class Utilities
    {
        public static string GenerateAnchorUniqueId(string experienceId, string activityGroupId)
        {
            return string.Format("ACT_{0}-{1}-{2}",
                experienceId,
                activityGroupId,
                new System.DateTimeOffset(System.DateTime.UtcNow).ToUnixTimeSeconds());
        }

        public static string GenerateMapId()
        {
            return Const.MAP_PREFIX + "-" + System.DateTime.Now.ToString("yyyyMMdd-HHmmss");
        }

        public static Dictionary<string, string> CopyActivitySpecificProperties(Dictionary<string, string> sourceDic)
        {
            Dictionary<string, string> targetDic = new Dictionary<string, string>();
            
            if (sourceDic != null)
            {
                foreach(string key in sourceDic.Keys)
                {
                    string value = sourceDic[key];
                    targetDic.Add(key, value);
                }
            }
            
            return targetDic;   
        }
    }

}

