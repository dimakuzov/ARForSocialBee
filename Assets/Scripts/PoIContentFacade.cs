using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialBeeAR
{

    public class PoIContentFacade : MonoBehaviour
    {
        [SerializeField] private GameObject checkinObj;
        [SerializeField] private GameObject poiBoard;
        [SerializeField] private GameObject editButton;

        [SerializeField] private GameObject checkinEffectPrefab;
        private float loopTimeLimit = 2.0f;


        private bool isCheckin = false;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        public void ApplyContentMode(AnchorUIMode uiMode)
        {
            poiBoard.SetActive(true);

            if (uiMode != AnchorUIMode.Consumer) //for creator
            {
                checkinObj.SetActive(false);
                editButton.SetActive(true);
            }
            else //for consumer
            {
                if (!isCheckin)
                    checkinObj.SetActive(true);
                else
                    checkinObj.SetActive(false);

                editButton.SetActive(false);
            }
        }


        public void CheckIn()
        {
            isCheckin = true;
            PlayCheckinEffect();
            checkinObj.SetActive(false);
        }


        public void PlayCheckinEffect()
        {
            StartCoroutine(EffectLoop());
        }


        IEnumerator EffectLoop()
        {
            GameObject effectPlayer = (GameObject)Instantiate(checkinEffectPrefab, checkinObj.transform.position, checkinObj.transform.rotation);
            yield return new WaitForSeconds(loopTimeLimit);

            Destroy(effectPlayer);

            //loopping
            //PlayCheckinEffect();  
        }


        public void EditPoIDesc()
        {
            MessageManager.Instance.DebugMessage("Start editing PoI description");

            // Activate input field
            InputField input = GetComponentInChildren<InputField>();
            input.interactable = true;
            input.ActivateInputField();

            input.onEndEdit.AddListener(delegate { OnEditPoIDescDone(input); });
        }

        private void OnEditPoIDescDone(InputField input)
        {
            MessageManager.Instance.DebugMessage("End editing PoI description");

            AnchorController myAnchorController = GetComponentInParent<AnchorController>();
            myAnchorController.OnEditPoIDescDone(input);

        }


    }

}
