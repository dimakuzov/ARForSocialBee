using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SocialBeeAR
{
    public class ActivitySettingFacade : UIFacade
    {

        public enum UIMode
        {
            UnDefined,
            ActivitySetting_NoActivitySeleted,
            ActivitySetting_ActivitySelected
        }
        private UIMode currentUIMode = UIMode.UnDefined;


        //SelectMapPanel elements
        [SerializeField] GameObject confirmActivitySettingButton; //+


        private static ActivitySettingFacade _instance;
        public static ActivitySettingFacade Instance
        {
            get
            {
                return _instance;
            }
        }


        private void Awake()
        {
            _instance = this;
            Categorize();
        }


        private void Categorize()
        {
            allElements.Add(confirmActivitySettingButton);

            bottomElements.Add(confirmActivitySettingButton);
        }


        public UIMode GetUIMode()
        {
            return this.currentUIMode;
        }


        public void SetUIMode(UIMode uiMode)
        {
            if (this.currentUIMode != uiMode)
            {
                this.currentUIMode = uiMode;
                switch (uiMode)
                {
                    case UIMode.ActivitySetting_NoActivitySeleted:
                        DeactiveAll();
                        confirmActivitySettingButton.SetActive(false);
                        break;

                    case UIMode.ActivitySetting_ActivitySelected:
                        DeactiveAll();
                        confirmActivitySettingButton.SetActive(true);
                        break;

                    default:
                        DeactiveAll();
                        confirmActivitySettingButton.SetActive(false);
                        break;
                }
            }

            UpdateBottomBanner();
        }


    }

}