using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SocialBeeAR
{

    public class UIFacade : MonoBehaviour
    {

        //logical elements
        protected List<GameObject> allElements = new List<GameObject>();
        protected List<GameObject> bottomElements = new List<GameObject>();


        protected void DeactiveAll()
        {
            foreach (GameObject element in allElements)
            {
                if(element != null)
                {
                    element.SetActive(false);
                }
            }
        }


        public bool IsAnyBottomButtonActive()
        {
            foreach (GameObject button in this.bottomElements)
            {
                if (button != null && button.activeSelf)
                    return true;
            }
            return false;
        }


        protected void UpdateBottomBanner()
        {

            bool isAnyElementActive = false;
            foreach (GameObject element in bottomElements)
            {
                if (element != null && element.activeSelf)
                {
                    isAnyElementActive = true;
                    break;
                }
            }

            UIManager.Instance.EnableBottomBanner(isAnyElementActive);
        }

    }
    
}

