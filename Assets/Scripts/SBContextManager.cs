using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialBeeAR
{

    /// <summary>
    /// This represents the context from SocialBee native app
    /// </summary>
    public class SBContext
    {
        public string experienceId;
        public string experienceName;

        public string activityGroupId;
        public string activityGroupName;
    }


    /// <summary>
    /// This class is for storing the state info synchronized from the native socialbee app.
    /// All other class in AR get/set state info through this class
    /// </summary>
    public class SBContextManager : MonoBehaviour
    {

        private SBContext currContext;

        private static SBContextManager _instance;
        public static SBContextManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            //TODO: , it should be removed later!
            
            //InitSBContext("EXP_001,Seattle City Tour,ACTG_001,Seattle Art Museum");
            //UIManager.Instance.SetSBContext(currContext.experienceName, currContext.activityGroupName);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void InitSBContext(string info) {
            char[] charSeparators = new char[] {','};
            string[] result = info.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

            //TODO: to be replaced by retrieving from the native app, this hard-coding is only for testing!
            SetSBContext(result[0], result[1], result[2], result[3]);
            Debug.Log("**** In AR. experienceId: " + result[0] + "experienceName: " + result[1] + 
                      "activityGroupId: " + result[2] + "activityGroupName: " + result[3]);
            UIManager.Instance.SetSBContext(currContext.experienceName, currContext.activityGroupName);
        }

        private void SetSBContext(string expId, string expName, string actGroupId, string actGroupName)
        {
            this.currContext = new SBContext
            {
                experienceId = expId,
                experienceName = expName,

                activityGroupId = actGroupId,
                activityGroupName = actGroupName
            };
        }

        public SBContext GetSBContent()
        {
            return currContext;
        }

    }

}

