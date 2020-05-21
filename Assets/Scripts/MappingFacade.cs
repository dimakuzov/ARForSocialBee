using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialBeeAR
{
    public class MappingFacade : UIFacade
    {

        public enum UIMode
        {
            UnDefined,
            Mapping_PreScanning,
            Mapping_Scanning
        }
        private UIMode currentUIMode = UIMode.UnDefined;


        //SelectMapPanel elements
        [SerializeField] GameObject preMappingInfo; //+
        [SerializeField] GameObject startMappingButton; //+
        [SerializeField] GameObject mappingProgressBar; //+
        [SerializeField] GameObject mappingInfo; //+
        [SerializeField] Text mappingProgressNum;
        [SerializeField] Image mappingProgressFillImage;


        private static MappingFacade _instance;
        public static MappingFacade Instance
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
            allElements.Add(preMappingInfo);
            allElements.Add(startMappingButton);
            allElements.Add(mappingProgressBar);
            allElements.Add(mappingInfo);

            //any button or button parent must be added into the list.
            bottomElements.Add(startMappingButton);
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
                    case UIMode.Mapping_PreScanning:
                        DeactiveAll();
                        preMappingInfo.SetActive(true);
                        startMappingButton.SetActive(true);
                        mappingProgressBar.SetActive(false);
                        mappingInfo.SetActive(false);
                        break;

                    case UIMode.Mapping_Scanning:
                        DeactiveAll();
                        preMappingInfo.SetActive(false);
                        startMappingButton.SetActive(false);
                        mappingProgressBar.SetActive(true);
                        mappingInfo.SetActive(true);
                        break;

                    default:
                        DeactiveAll();
                        preMappingInfo.SetActive(true);
                        startMappingButton.SetActive(true);
                        mappingProgressBar.SetActive(false);
                        mappingInfo.SetActive(false);
                        break;
                }
            }
            UpdateBottomBanner();
        }


        public void ResetMappingProgress()
        {
            mappingProgressFillImage.gameObject.GetComponent<Image>().fillAmount = 0;
            mappingProgressNum.text = "";
        }


        public void SetMappingProgress(float percentageUnderOne)
        {
            float percentage = Mathf.Max(percentageUnderOne, 0f);
            mappingProgressFillImage.gameObject.GetComponent<Image>().fillAmount = percentage;
            mappingProgressNum.text = (percentage * 100).ToString();
        }


    }

}