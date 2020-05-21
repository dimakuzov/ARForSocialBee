using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialBeeAR
{

    public class TriviaQuestion
    {
        public string question;
        public List<string> optionList;
        public int selectedIndex;
        public string hints;
    }

    public class TriviaContentFacade : MonoBehaviour
    {
        [SerializeField] private GameObject triviaBoard;
        [SerializeField] private GameObject triviaDisableBoard;
        
        [SerializeField] private Text questionText;
        [SerializeField] private Text option1Text;
        [SerializeField] private Text option2Text;
        [SerializeField] private Text option3Text;
        [SerializeField] private Text option4Text;
        [SerializeField] private Text hintsText;

        [SerializeField] private GameObject completionEffectPrefab;
        [SerializeField] private GameObject completionObj;
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
            triviaBoard.SetActive(true);

            if (uiMode != AnchorUIMode.Consumer) //for creator
            {
                // triviaDisableBoard.SetActive(true);
            }
            else //for consumer
            {
                // triviaDisableBoard.SetActive(false);
            }
        }


        public void OnCompleteAnimation()
        {
            isCheckin = true;
            PlayCheckinEffect();
            completionObj.SetActive(false);
        }


        public void PlayCheckinEffect()
        {
            StartCoroutine(EffectLoop());
        }


        IEnumerator EffectLoop()
        {
            GameObject effectPlayer = (GameObject)Instantiate(completionEffectPrefab, completionObj.transform.position, completionObj.transform.rotation);
            yield return new WaitForSeconds(loopTimeLimit);

            Destroy(effectPlayer);

            //loopping
            //PlayCheckinEffect();  
        }


        public void EditTrivia()
        {
            MessageManager.Instance.DebugMessage("Start editing trivia question.");

            // call native UI here...
            NativeCall nativeCall = FindObjectOfType<NativeCall>();
            nativeCall.triviaContentFacade = this;
            nativeCall.GiveActivityData("ActivityID", "ActivityTitle");
            nativeCall.ShowNative("Trivia");
            
        }

        //void OnEditTriviaDone(TriviaQuestion triviaQuestion)
        public void OnEditTriviaDone(TriviaQuestion triviaQuestion)
        {
            // Convert info from native to TriviaQuestion
            /*
            char[] charSeparators = new char[] {','};
            string[] result = triviaInfo.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

            TriviaQuestion triviaQuestion = new TriviaQuestion();
            triviaQuestion.question = result[0];
            triviaQuestion.optionList.Add(result[1]);
            triviaQuestion.optionList.Add(result[2]);
            triviaQuestion.optionList.Add(result[3]);
            triviaQuestion.optionList.Add(result[4]);
            triviaQuestion.selectedIndex = Int32.Parse(result[5]);
            triviaQuestion.hints = result[6];
            */
            Debug.Log("**** Trivia Question in AR: " + triviaQuestion.question);
            
            MessageManager.Instance.DebugMessage("Editing trivia done");
            
            //code to update content...
            
        }

    }

}
