using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SocialBeeAR
{

    public class UIManager : UIFacade
    {

        public enum UIMode
        {
            Init,
            SelectMap,
            ActivitySetting,
            Mapping,
            ContinuousMapping,
            Localization,
            Undefined
        }
        private UIMode currentUIMode = UIMode.Undefined;

        //--------------------------physical UI elemetns-----------------------------

        //InitPanel
        [SerializeField] GameObject initPanel;

        //SelectMapPanel
        [SerializeField] GameObject selectMapPanel;

        //ActivitySettingPanel
        [SerializeField] GameObject activitySettingPanel;

        //MappingPanel
        [SerializeField] GameObject mappingPanel;

        //ContinuousMappingPanel
        [SerializeField] GameObject continousMappingPanel;

        //LocalizationPanel
        [SerializeField] GameObject localizationPanel;

        [SerializeField] GameObject bottomBanner;
        [SerializeField] Text experienceName;
        [SerializeField] Text activityGroupName;


        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                return _instance;
            }
        }


        //------------------------Monobehaviour methods-------------------------


        private void Awake()
        {
            _instance = this;

            Categorize();
        }

        /// <summary>
        /// Categorize the UI element here
        /// </summary>
        private void Categorize()
        {
            //UI model panels
            allElements.Add(initPanel);
            allElements.Add(selectMapPanel);
            allElements.Add(activitySettingPanel);
            allElements.Add(mappingPanel);
            allElements.Add(continousMappingPanel);
            allElements.Add(localizationPanel);
        }


        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }


        //-----------------------------Other methods----------------------------
        

        public void SetUIMode(UIMode uiMode)
        {
            if(uiMode != currentUIMode)
            {
                DeactiveAll();

                switch (uiMode)
                {
                    case UIMode.Init:
                        initPanel.SetActive(true);
                        break;

                    case UIMode.SelectMap:
                        selectMapPanel.SetActive(true);
                        break;

                    case UIMode.ActivitySetting:
                        activitySettingPanel.SetActive(true);
                        break;

                    case UIMode.Mapping:
                        mappingPanel.SetActive(true);
                        break;

                    case UIMode.ContinuousMapping:
                        continousMappingPanel.SetActive(true);
                        break;

                    case UIMode.Localization:
                        localizationPanel.SetActive(true);
                        break;

                    default:
                        initPanel.SetActive(true);
                        break;
                }
            }
        }


        public void EnableBottomBanner(bool enabled)
        {
            this.bottomBanner.SetActive(enabled);
        }


        public void SetSBContext(string experience, string activityGroupName)
        {
            this.experienceName.text = experience;
            this.activityGroupName.text = activityGroupName;
        }

    }

}


