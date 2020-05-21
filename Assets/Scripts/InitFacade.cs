using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SocialBeeAR
{
    public class InitFacade : UIFacade
    {

        public enum UIMode
        {
            UnDefined,
            Initializing,
            Inititialized
        }
        private UIMode currentUIMode = UIMode.UnDefined;


        //InitPanel elements
        [SerializeField] GameObject newActivityButton;
        [SerializeField] GameObject searchActivityButton; //+


        private static InitFacade _instance;
        public static InitFacade Instance
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
            allElements.Add(newActivityButton);
            allElements.Add(searchActivityButton);

            bottomElements.Add(newActivityButton);
            bottomElements.Add(searchActivityButton);
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
                    case UIMode.Initializing:
                        DeactiveAll();
                        break;

                    case UIMode.Inititialized:
                        DeactiveAll();
                        newActivityButton.SetActive(true);
                        searchActivityButton.SetActive(true);
                        break;

                    default:
                        DeactiveAll();
                        newActivityButton.SetActive(true);
                        searchActivityButton.SetActive(true);
                        break;
                }
            }

            UpdateBottomBanner();
        }


    }

}
