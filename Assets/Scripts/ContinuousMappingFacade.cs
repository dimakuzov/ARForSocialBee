using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialBeeAR
{
    public class ContinuousMappingFacade : UIFacade
    {

        public enum UIMode
        {
            UnDefined,
            Init,
            SavingMap
        }
        private UIMode currentUIMode = UIMode.UnDefined;


        //SelectMapPanel elements
        [SerializeField] GameObject continuosNewActivityButton; //+
        [SerializeField] GameObject saveMapButton; //+


        private static ContinuousMappingFacade _instance;
        public static ContinuousMappingFacade Instance
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
            allElements.Add(continuosNewActivityButton);
            allElements.Add(saveMapButton);

            bottomElements.Add(continuosNewActivityButton);
            bottomElements.Add(saveMapButton);
        }


        public UIMode GetUIMode()
        {
            return this.currentUIMode;
        }


        public void SetUIMode(UIMode uiMode)
        {
            if(this.currentUIMode != uiMode)
            {
                this.currentUIMode = uiMode;
                switch (uiMode)
                {
                    case UIMode.Init:
                        DeactiveAll();
                        continuosNewActivityButton.SetActive(true);
                        saveMapButton.SetActive(true);
                        break;

                    case UIMode.SavingMap: //do not show any element when saving map
                        DeactiveAll();
                        break;

                    default:
                        DeactiveAll();
                        continuosNewActivityButton.SetActive(true);
                        saveMapButton.SetActive(true);
                        break;
                }
            }

            UpdateBottomBanner();
        }

    }

}