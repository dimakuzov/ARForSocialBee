using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


namespace SocialBeeAR
{

    [System.Serializable]
    public class AnchorInfo
    {
        public Pose pose;
        
        public string experienceId;
        public string activityCollectionId;

        public string activityId;
        public string activityName;
        public ActivityType type;
        
        // public string poiDesc;

        public Dictionary<string, string> activitySpecific;
    }
    
    [System.Serializable]
    public class AnchorInfoList
    {
        // List of all anchors stored in the current Place.
        public AnchorInfo[] sbActivityCollection;
    }
    
    
    /// <summary>
    /// This class manages the status of all the anchor('Activity') objects
    /// </summary>
    public class AnchorManager : MonoBehaviour
    {

        [SerializeField] ARRaycastManager raycastManager;

        //Anchor prefab
        public GameObject anchorObjPrefab;

        //Anchor object list
        public List<GameObject> anchorObjList = new List<GameObject>();

        //current anchor
        private GameObject currAnchorObj;

        private static AnchorManager _instance;
        public static AnchorManager Instance
        {
            get
            {
                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;
        }

        void Update()
        {
            
        }
        
        
        //-------------------------------- Spawn & Reborn AnchorObject-------------------------
        
        /// <summary>
        /// This is used only when user is creating anchor
        /// </summary>
        /// <param name="anchorPosition"></param>
        public void SpawnAnchorObj(Vector3 anchorPosition)
        {
            //Instantiate new anchor prefab and set transform.
            GameObject anchorObj = Instantiate(anchorObjPrefab);
            anchorObj.transform.position = anchorPosition + Const.ANCHOR_Y_OFFSET;
            anchorObj.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            AnchorController anchorController = anchorObj.GetComponent<AnchorController>();
            if (currAnchorObj != null)
                anchorController.TurnOffButtons();
            currAnchorObj = anchorObj;
            
            //set some initial information
            string expId = SBContextManager.Instance.GetSBContent().experienceId;
            string actGroupId = SBContextManager.Instance.GetSBContent().activityGroupId;
            string activityId = Utilities.GenerateAnchorUniqueId(expId, actGroupId);
            Pose anchorPose = new Pose(anchorObj.transform.position, anchorObj.transform.rotation);
            Dictionary<string, string> actSpecific = new Dictionary<string, string>();
            AnchorInfo initialAnchorInfo = new AnchorInfo
            {
                //initial values
                pose = anchorPose,

                experienceId = expId,
                activityCollectionId = actGroupId,

                activityId = activityId,
                activityName = "", //to be set later by creator

                type = ActivityType.Undefined,
                // poiDesc = "",
                
                activitySpecific = actSpecific
            };
            anchorController.Born(anchorObjList.Count, initialAnchorInfo); //init!

            //add to anchor list
            anchorObjList.Add(currAnchorObj);
            MessageManager.Instance.DebugMessage(string.Format("New anchor spawned, ID: '{0}'", activityId));

        }
        
        public GameObject RebornAnchorObj(AnchorInfo info)
        {
            GameObject anchorObj = Instantiate(anchorObjPrefab);
            anchorObj.transform.position = info.pose.position;
            anchorObj.transform.rotation = info.pose.rotation;
            anchorObj.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            anchorObj.SetActive(true);

            AnchorController anchorController = anchorObj.GetComponentInChildren<AnchorController>();
            anchorController.Reborn(info);
            
            return anchorObj;
        }
        
        //--------------- Register current selected anchor object -----------------

        
        /// <summary>
        /// Light-weight registration of current anchor, in this case only report the index number
        /// </summary>
        /// <param name="anchorIndex"></param>
        public void RegisterCurrentAnchor(int anchorIndex)
        {
            //check if the current focus object is another anchor, then 'turn it off'.
            AnchorController currentAnchorObjController = currAnchorObj.GetComponent<AnchorController>();
            if (currentAnchorObjController.index != anchorIndex)
            {
                currentAnchorObjController.TurnOffButtons();
            }
            
            //then switch the focus
            this.currAnchorObj = anchorObjList[anchorIndex];
        }
        
        /// <summary>
        /// Heavy-weight registration of current anchor, in case anchor is modified/updated.
        /// </summary>
        /// <param name="anchorIndex"></param>
        /// <param name="anchorObject"></param>
        public void RegisterCurrentAnchor(int anchorIndex, GameObject anchorObject)
        {
            anchorObjList[anchorIndex] = anchorObject;
            this.currAnchorObj = anchorObjList[anchorIndex];
        }


        public void DeleteAnchorObj(int index)
        {
            if (index >= 0)
            {
                GameObject toBeDeletedAnchorObject = this.anchorObjList[index];    
                anchorObjList.RemoveAt(index);

                //refresh anchor index
                for (int i = 0; i < anchorObjList.Count; ++i)
                {
                    anchorObjList[i].GetComponentInChildren<AnchorController>().index = i;
                }
                
                Destroy(toBeDeletedAnchorObject);
            }
        }

        
        //------------------------  Converting to JSON --------------------------
        
        /// <summary>
        /// Converting AnchorInfo list to JSON data
        /// </summary>
        /// <returns></returns>
        public JObject AnchorInfoListToJSON()
        {
            //prepare anchor info list
            List<AnchorInfo> anchorInfoList = new List<AnchorInfo>();
            foreach (GameObject anchorObj in anchorObjList)
            {
                AnchorInfo info = anchorObj.GetComponent<AnchorController>().GetAnchorInfo();
                anchorInfoList.Add(info);
            }
            
            //prepare an arr for converting to JSON
            AnchorInfoList tempAnchorList = new AnchorInfoList
            {
                sbActivityCollection = new AnchorInfo[anchorInfoList.Count]
            };
            
            for (int i = 0; i < anchorInfoList.Count; ++i)
            {
                tempAnchorList.sbActivityCollection[i] = anchorInfoList[i];
            }

            //convert to JSON
            JObject jObject = JObject.FromObject(tempAnchorList);
            MessageManager.Instance.DebugMessage(jObject.ToString());
            return jObject;
        }


        /// <summary>
        /// Parsing JSON data to have a AnchorInfo list
        /// </summary>
        /// <param name="mapMetadata"></param>
        public void AnchorInfoListFromJSON(JToken mapMetadata)
        {
            ClearAnchors();

            if (mapMetadata is JObject && mapMetadata[Const.ANCHOR_DATA_JSON_ROOT] is JObject)
            {
                AnchorInfoList anchorList = mapMetadata[Const.ANCHOR_DATA_JSON_ROOT].ToObject<AnchorInfoList>();
            
                if (anchorList.sbActivityCollection == null)
                {
                    Debug.Log("No anchors created!");
                    return;
                }
            
                foreach (var anchorInfo in anchorList.sbActivityCollection)
                {
                    GameObject anchorObj = RebornAnchorObj(anchorInfo);
                    anchorObj.GetComponentInChildren<AnchorController>().index = anchorObjList.Count;
            
                    anchorObjList.Add(anchorObj);
                }
            }
        }
        
        //-------------------------------- Others -------------------------------

        public void StartEditingCurrentAnchor()
        {
            this.currAnchorObj.GetComponent<AnchorController>().EditActivityNameForCurrentAnchorObj();
        }
        
        public GameObject GetCurrentAnchorObj()
        {
            return currAnchorObj;
        }

        
        public void ClearAnchors()
        {
            foreach (var obj in anchorObjList)
            {
                Destroy(obj);
            }

            anchorObjList.Clear();
        }

    }
    
}

