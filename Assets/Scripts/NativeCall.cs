using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SocialBeeAR;
using UnityEngine.UI;
using UnityEngine;

#if UNITY_IOS
public class NativeAPI {
    [DllImport("__Internal")]
    public static extern void showNativePage(string page);
    
    [DllImport("__Internal")]
    public static extern void giveActivityData(string mapID, string activity);
    
    
}
#endif

public class NativeCall : MonoBehaviour {
    
    private SBContextManager sbContextManager;
    [HideInInspector]public TriviaContentFacade triviaContentFacade;

    public void Start() {
        sbContextManager = gameObject.GetComponent<SBContextManager>();
    }

    public void ShowNative(string page) {
        NativeAPI.showNativePage(page);
    }

    public void GiveActivityData(string activityID, string activityTitle) {
        string mapID = "mapID123";
        double lat = 1.23;
        double lon = 4.56;
        double alt = 7.89;
        double x = 0.12;
        double y = 3.45;
        double z = 6.78;
        
        string activity = activityID + "," + activityTitle + "," 
                          + lat + "," + lon + "," + alt + "," + x + "," + y + "," + z;
        NativeAPI.giveActivityData(mapID, activity);
    }
    
    /*
    public void GiveDataButton() {
        string mapID = "mapID123";
        double lat = 1.23;
        double lon = 4.56;
        double alt = 7.89;
        double x = 0.12;
        double y = 3.45;
        double z = 1/3;
        
        string activity = "activityID123" + "," + "activityTitleTrivia" + "," 
                          + lat + "," + lon + "," + alt + "," + x + "," + y + "," + z;
        NativeAPI.giveActivityData(mapID, activity);
    }
    */

    /*
    void GetInitInfo(string info) {
        char[] charSeparators = new char[] {','};
        string[] result = info.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
        // Convert info from native to sbContextManager.InitSBContext()
        
        Debug.Log("**** Info From Native Data:" + "\n**** " + result[0] + 
                  "\n**** " + result[1]);
    }
    */
    
    void GetTriviaInfo(string triviaInfo) {
        // Convert info from native to TriviaQuestion
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
        
        triviaContentFacade.OnEditTriviaDone(triviaQuestion);
    }
    
}


