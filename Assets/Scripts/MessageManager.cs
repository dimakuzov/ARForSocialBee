using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SocialBeeAR
{

    public class MessageManager : MonoBehaviour
    {
        //The debug panel
        [SerializeField] private GameObject debugPanel;
        private DebugPanelController debugPanelController;

        //The notification in top for end users
        [SerializeField] Text messageText;
        
        //other debug info elements
        [SerializeField] private Text infoMapSizeText;
        [SerializeField] private GameObject infoMappingQualityTextParent;
        private Text infoMappingQualityText;

        [SerializeField] private GameObject infoStatusParent;
        private Text infoMode;
        private Text infoStatus;

        [SerializeField] private Text infoAnchorSizeText;

        private static MessageManager _instance;
        public static MessageManager Instance
        {
            get
            {
                return _instance;
            }
        }


        private void Awake()
        {
            _instance = this;

            if (debugPanel != null)
                debugPanelController = debugPanel.GetComponent<DebugPanelController>();

            if (infoMappingQualityTextParent != null)
                infoMappingQualityText = infoMappingQualityTextParent.transform.GetChild(1).GetComponent<Text>();

            if (infoStatusParent != null)
            {
                infoMode = infoStatusParent.transform.GetChild(1).GetComponent<Text>();
                infoStatus = infoStatusParent.transform.GetChild(3).GetComponent<Text>();
            }
        }


        private void Start()
        {

        }


        //------------------------Notification for users------------------------

        public void ShowMessage(string message)
        {
            if (messageText != null)
            {
                MainThreadTaskQueue.InvokeOnMainThread(() =>
                {
                    messageText.text = message;
                });
            }
        }

        //------------------------Debug panel-----------------------------------

        public void DebugMessage(string msgLine)
        {
            if (!debugPanelController.Equals(null))
            {
                string[] msgLineArr = msgLine.Split(Environment.NewLine.ToCharArray());
                if (msgLineArr != null && msgLineArr.Length > 0)
                {
                    foreach (string line in msgLineArr)
                    {
                        MainThreadTaskQueue.InvokeOnMainThread(() =>
                        {
                            debugPanelController.PushMessage(line);
                        });   
                    }
                }
                else
                {
                    MainThreadTaskQueue.InvokeOnMainThread(() =>
                    {
                        debugPanelController.PushMessage(msgLine);
                    });    
                }
            }
        }
        
        // public void DebugMessage(string msgLine)
        // {
        //     if (!debugPanelController.Equals(null))
        //     {
        //         MainThreadTaskQueue.InvokeOnMainThread(() =>
        //         {
        //             debugPanelController.PushMessage(msgLine);
        //         });
        //     }
        // }
        

        //------------------------Other debug info------------------------------

        public void UpdateMapSize(int mapLength)
        {
            if (this.infoMapSizeText != null)
            {
                this.infoMapSizeText.text = mapLength.ToString();
            }
        }

        public void UpdateCurrentAnchorSize(int currentAnchorSize)
        {
            if(infoAnchorSizeText != null)
            {
                infoAnchorSizeText.text = currentAnchorSize.ToString();
            }   
        }


        public void UpdateMappingQuality(string mappingQuality)
        {
            if (this.infoMappingQualityText != null)
            {
                this.infoMappingQualityText.text = mappingQuality;
            }
        }


        public void EnableMappingQualityInfo(bool enabled)
        {
            if(infoMappingQualityTextParent != null)
            {
                infoMappingQualityTextParent.SetActive(enabled);
            }
        }


        public void UpdateStatus(string mode, string status)
        {
            if (!infoMode.Equals(null))
                this.infoMode.text = mode;

            if (!infoStatus.Equals(null))
                this.infoStatus.text = status;
        }


        

    }

}


