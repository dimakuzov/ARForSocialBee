// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;
//
// using Newtonsoft.Json.Linq;
// using UnityEngine.XR.ARFoundation;
//
//
// namespace SocialBeeAR
// {
//
//     /// <summary>
//     /// This class manages the status of all the anchor('Activity') objects
//     /// </summary>
//     public class AnchorManager_OLD : MonoBehaviour
//     {
//
//         [SerializeField] ARRaycastManager raycastManager;
//
//         //Anchor prefab
//         public GameObject anchorObjPrefab;
//
//         //Anchor object list and info list
//         public List<AnchorInfo> anchorInfoList = new List<AnchorInfo>();
//         public List<GameObject> anchorObjList = new List<GameObject>();
//
//         //current anchor
//         private GameObject currentAnchorObj;
//         private AnchorInfo currentAnchorInfo;
//
//         private static AnchorManager_OLD _instance;
//         public static AnchorManager_OLD Instance
//         {
//             get
//             {
//                 return _instance;
//             }
//         }
//
//         void Awake()
//         {
//             _instance = this;
//         }
//
//         void Update()
//         {
//             //if (this.enableCreatingContent)
//             //{
//             //    HandleAnchorInteraction();
//             //}
//         }
//
//
//         public GameObject GetCurrentAnchorObj()
//         {
//             return currentAnchorObj;
//         }
//
//
//         public AnchorInfo GetCurrentAnchorInfo()
//         {
//             return currentAnchorInfo;
//         }
//
//
//         private void HandleAnchorInteraction()
//         {
//             if (Input.touchCount > 0)
//             {
//                 var touch = Input.GetTouch(0);
//
//                 if (touch.phase == TouchPhase.Ended)
//                 {
//                     if (EventSystem.current.currentSelectedGameObject == null)
//                     {
//                         Debug.Log("Not touching a UI button, moving on.");
//
//                         // Test if you are hitting an existing marker
//                         RaycastHit hit = new RaycastHit();
//                         Ray ray = Camera.main.ScreenPointToRay(touch.position);
//
//                         if (Physics.Raycast(ray, out hit)) //if it hits a game object
//                         {
//                             Debug.Log("Selected an existing anchor.");
//
//                             GameObject anchorObj = hit.transform.gameObject;
//
//                             //If the previous anchor was deleted, switch
//                             if (!currentAnchorObj)
//                             {
//                                 currentAnchorObj = anchorObj;
//                                 TurnOnButtons();
//                             }
//                             else if (anchorObj.GetComponentInChildren<AnchorController>().index != currentAnchorObj.GetComponentInChildren<AnchorController>().index)
//                             {
//                                 //New anchor selected is not the current anchor. Disable the buttons of the current anchor.
//                                 TurnOffButtons();
//
//                                 currentAnchorObj = anchorObj;
//
//                                 //Turn on buttons for the new selected anchor.
//                                 TurnOnButtons();
//                             }
//                             else
//                             {
//                                 //if selected anchor is already the current anchor, just toggle buttons.
//                                 ToggleButtons();
//                             }
//                         }
//                         else //if it doesn't hit an game object
//                         {
//                             //do nothing
//                         }
//                     }
//                 }
//             }
//         }
//
//
//         private void ToggleButtons()
//         {
//             int index = currentAnchorObj.GetComponentInChildren<AnchorController>().index;
//             currentAnchorInfo = anchorInfoList[index];
//
//             // Toggle the edit and delete buttons
//             if (!currentAnchorObj.GetComponentInChildren<AnchorController>().isEditDeleteButtonsActive)
//             {
//                 TurnOnButtons();
//             }
//             else
//             {
//                 TurnOffButtons();
//             }
//         }
//
//
//         private void TurnOnButtons()
//         {
//             currentAnchorObj.GetComponentInChildren<AnchorController>().editButton.gameObject.SetActive(true);
//             currentAnchorObj.GetComponentInChildren<AnchorController>().deleteButton.gameObject.SetActive(true);
//             currentAnchorObj.GetComponentInChildren<AnchorController>().isEditDeleteButtonsActive = true;
//
//             currentAnchorObj.GetComponentInChildren<AnchorController>().EnableRotation(false);
//         }
//
//
//         private void TurnOffButtons()
//         {
//             currentAnchorObj.GetComponentInChildren<AnchorController>().editButton.gameObject.SetActive(false);
//             currentAnchorObj.GetComponentInChildren<AnchorController>().deleteButton.gameObject.SetActive(false);
//             currentAnchorObj.GetComponentInChildren<AnchorController>().isEditDeleteButtonsActive = false;
//
//             currentAnchorObj.GetComponentInChildren<AnchorController>().EnableRotation(true);
//         }
//
//
//         /// <summary>
//         /// This is used only when user is creating anchor
//         /// </summary>
//         /// <param name="anchorPosition"></param>
//         public void SpawnAnchorObj(Vector3 anchorPosition)
//         {
//             //Instantiate new anchor prefab and set transform.
//             GameObject anchorObj = Instantiate(anchorObjPrefab);
//             anchorObj.transform.position = anchorPosition + Const.ANCHOR_Y_OFFSET;
//             anchorObj.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
//
//             if (currentAnchorObj)
//             {
//                 TurnOffButtons();
//             }
//
//             // Set new anchor as the current one.
//             currentAnchorObj = anchorObj;
//             string expId = SBContextManager.Instance.GetSBContent().experienceId;
//             string actGroupId = SBContextManager.Instance.GetSBContent().activityGroupId;
//             string activityId = Utilities.GenerateAnchorUniqueId(expId, actGroupId);
//             currentAnchorInfo = new AnchorInfo
//             {
//                 //initial values
//                 px = anchorObj.transform.position.x,
//                 py = anchorObj.transform.position.y,
//                 pz = anchorObj.transform.position.z,
//                 qx = anchorObj.transform.rotation.x,
//                 qy = anchorObj.transform.rotation.y,
//                 qz = anchorObj.transform.rotation.z,
//                 qw = anchorObj.transform.rotation.w,
//
//                 experienceId = expId,
//                 activityGroupId = actGroupId,
//
//                 activityId = activityId,
//                 activityName = "", //to be set later by creator
//
//                 type = ActivityType.Undefined,
//                 poiDesc = ""
//             };
//
//             // Set up the buttons on each anchor
//             //anchorObj.GetComponentInChildren<AnchorController>().editButton.onClick.AddListener(OnEditButtonClick);
//             anchorObj.GetComponentInChildren<AnchorController>().deleteButton.onClick.AddListener(OnDeleteButtonClick);
//             TurnOnButtons();
//
//             //EditCurrentAnchorObj();
//         }
//
//
//         public GameObject RebornAnchorFromInfo(AnchorInfo info)
//         {
//             GameObject anchorObj = Instantiate(anchorObjPrefab);
//             anchorObj.transform.position = new Vector3(info.px, info.py, info.pz);
//             anchorObj.transform.rotation = new Quaternion(info.qx, info.qy, info.qz, info.qw);
//             anchorObj.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
//             anchorObj.SetActive(true);
//
//             AnchorController anchorController = anchorObj.GetComponentInChildren<AnchorController>();
//
//             //set mode
//             anchorController.SetUIMode(AnchorUIMode.Consumer);
//             anchorController.SetActivityType(info.type);
//             anchorController.SetBehaviourMode(AnchorBehaviourMode.SavedAndStandBy);
//
//             //init edit/delete buttons
//             anchorController.editButton.onClick.AddListener(OnEditButtonClick);
//             anchorController.deleteButton.onClick.AddListener(OnDeleteButtonClick);
//             anchorController.editButton.gameObject.SetActive(false);
//             anchorController.deleteButton.gameObject.SetActive(false);
//             anchorController.isEditDeleteButtonsActive = false;
//
//             anchorController.ApplyActivityName(info.activityName);
//             //anchorObj.GetComponentInChildren<InputField>().text = info.activityName;
//             anchorController.ApplyPoIDesc(info.poiDesc);
//
//             return anchorObj;
//         }
//
//
//         public void OnEditButtonClick()
//         {
//             Debug.Log("Edit button clicked!");
//             currentAnchorObj.GetComponentInChildren<AnchorController>().EnableRotation(false);
//
//             // Set current anchor to the right edit button.
//             currentAnchorInfo = anchorInfoList[currentAnchorObj.GetComponentInChildren<AnchorController>().index];
//             EditActivityNameForCurrentAnchorObj();
//         }
//
//
//         public void EditActivityNameForCurrentAnchorObj()
//         {
//             MessageManager.Instance.DebugMessage("Editing selected anchor.");
//
//             // Activate input field
//             InputField input = currentAnchorObj.GetComponentInChildren<InputField>();
//             input.interactable = true;
//             input.ActivateInputField();
//
//             input.onEndEdit.AddListener(delegate { OnEditActivityNameDone(input); });
//         }
//
//
//         private void OnEditActivityNameDone(InputField input)
//         {
//             Debug.Log("Editor closed.");
//
//             //save input text, and set input field as non intractable
//             currentAnchorInfo.activityName = input.text;
//
//             //also save the last state of the anchor, including it's type, it's pose.
//             currentAnchorInfo.type = currentAnchorObj.GetComponentInChildren<AnchorController>().GetActivityType();
//
//             input.DeactivateInputField();
//             input.interactable = false;
//             TurnOffButtons();
//
//             UpdateAnchorInfoList();
//
//             SocialBeeARMain.Instance.OnEditAcivityNameDone();
//
//             //Start mapping
//             //SocialBeeARMain.Instance.OnNewMapClick();
//         }
//
//         public void OnEditPoIDescDone(InputField input)
//         {
//             currentAnchorInfo.poiDesc = input.text;
//             MessageManager.Instance.DebugMessage("so, desc=" + currentAnchorInfo.poiDesc);
//
//             input.DeactivateInputField();
//             input.interactable = false;
//
//             UpdateAnchorInfoList();
//         }
//
//         private void UpdateAnchorInfoList()
//         {
//             int index = currentAnchorObj.GetComponentInChildren<AnchorController>().index;
//             if (index < 0) //in case user is creating a new anchor, 
//             {
//                 currentAnchorObj.GetComponentInChildren<AnchorController>().index = anchorObjList.Count;
//                 Debug.Log("Saving anchor with ID " + anchorObjList.Count);
//
//                 anchorInfoList.Add(currentAnchorInfo);
//                 anchorObjList.Add(currentAnchorObj);
//             }
//             else //in case if user is editing an existing anchor
//             {
//                 // Need to re-save the object.
//                 anchorObjList[index] = currentAnchorObj;
//                 anchorInfoList[index] = currentAnchorInfo;
//             }
//         }
//
//         public void OnDeleteButtonClick()
//         {
//             Debug.Log("Delete button clicked!");
//             DeleteCurrentAnchor();
//         }
//
//         private void DeleteCurrentAnchor()
//         {
//             int index = currentAnchorObj.GetComponentInChildren<AnchorController>().index;
//             MessageManager.Instance.DebugMessage(string.Format("Deletinng anchor with index of '{0}'", index));
//
//             if (index >= 0)
//             {
//                 anchorObjList.RemoveAt(index);
//                 anchorInfoList.RemoveAt(index);
//
//                 // Refresh anchor index
//                 for (int i = 0; i < anchorObjList.Count; ++i)
//                 {
//                     anchorObjList[i].GetComponentInChildren<AnchorController>().index = i;
//                 }
//             }
//
//             Destroy(currentAnchorObj);
//         }
//
//
//         public void ClearAnchors()
//         {
//             foreach (var obj in anchorObjList)
//             {
//                 Destroy(obj);
//             }
//
//             anchorObjList.Clear();
//             anchorInfoList.Clear();
//         }
//
//
//         public JObject AnchorInfoListToJSON()
//         {
//             AnchorInfoList tempAnchorList = new AnchorInfoList
//             {
//                 anchorInfoArr = new AnchorInfo[anchorInfoList.Count]
//             };
//
//             for (int i = 0; i < anchorInfoList.Count; ++i)
//             {
//                 tempAnchorList.anchorInfoArr[i] = anchorInfoList[i];
//             }
//
//             return JObject.FromObject(tempAnchorList);
//         }
//
//
//         public void AnchorInfoListFromJSON(JToken mapMetadata)
//         {
//             ClearAnchors();
//
//             if (mapMetadata is JObject && mapMetadata[Const.ANCHOR_DATA_JSON_ROOT] is JObject)
//             {
//                 AnchorInfoList anchorList = mapMetadata[Const.ANCHOR_DATA_JSON_ROOT].ToObject<AnchorInfoList>();
//
//                 if (anchorList.anchorInfoArr == null)
//                 {
//                     Debug.Log("No anchors created!");
//                     return;
//                 }
//
//                 foreach (var anchorInfo in anchorList.anchorInfoArr)
//                 {
//                     GameObject anchorObj = RebornAnchorFromInfo(anchorInfo);
//                     anchorObj.GetComponentInChildren<AnchorController>().index = anchorObjList.Count;
//
//                     anchorObjList.Add(anchorObj);
//                     anchorInfoList.Add(anchorInfo);
//                 }
//             }
//         }
//
//     }
//
// }