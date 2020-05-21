using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SocialBeeAR
{

    /// <summary>
    /// ActivityType represents the type of the activity.
    /// </summary>
    public enum ActivityType
    {
        Undefined, //the default state of selection mode
        PoI,
        Photo,
        Post,
        Trivia,
        CV,
    }


    /// <summary>
    /// AnchorUIMode represents the role of the user and at which stage the user is.
    /// </summary>
    public enum AnchorUIMode
    {
        Creator_PreSetting,
        Creator_Setting,
        Creator_PostSetting,
        Consumer
    }


    /// <summary>
    /// This is for making the anchor object more like human behaviour...
    /// </summary>
    public enum AnchorBehaviourMode
    {
        Setting, //during configuration
        SettingDoneButNotSaved, //setting completed but not saved, it should not allow users to drag, also rotation should be disabled.
        SavedAndStandBy, //by default rotating, unless player is close to it.
    }
    
    
    public class AnchorController : MonoBehaviour
    {
        // This is set to -1 when instantiated, and assigned when changing the anchor, index is the sequence adding to the anchor list of AnchorManager.
        public int index = -1;
        public bool isEditDeleteButtonsActive = false;

        private AnchorInfo anchorInfo;
        
        private AnchorUIMode currUIMode;
        private AnchorBehaviourMode currBehaviourMode;

        private bool enableRotation = false;
        private bool watchingPlayer = false;
        private Vector3 lookAtTargetPos = Vector3.zero;
        private bool savedAndStandBy = false;

        private ObjectDragger objectDragger;

        //----------------------- UI elements -------------------------
        public Button editButton;
        public Button deleteButton;

        [SerializeField] private GameObject title;

        [SerializeField] private GameObject preSettingElements;
        [SerializeField] private GameObject postSettingElements;

        [SerializeField] private GameObject activityTypeButtons;
        [SerializeField] private GameObject buttonPoI;
        [SerializeField] private GameObject buttonPhoto;
        [SerializeField] private GameObject buttonPost;
        [SerializeField] private GameObject buttonTrivia;
        [SerializeField] private GameObject buttonCV;

        [SerializeField] private GameObject coverDefault;
        [SerializeField] private GameObject coverPoI;
        [SerializeField] private GameObject coverPhoto;
        [SerializeField] private GameObject coverPost;
        [SerializeField] private GameObject coverTrivia;
        [SerializeField] private GameObject coverCV;
        
        [SerializeField] public Text positionText;
        
        [SerializeField] private InputField activityNameInputField;

        //The activity specific content
        [SerializeField] private GameObject contentPoI;
        [SerializeField] private GameObject contentTrivia;
        

        private void Awake()
        {
            objectDragger = GetComponent<ObjectDragger>();
        }

        private void Start()
        {
        }

        
        private void Update()
        {
            positionText.text = transform.position.ToString();

            //checking its mode
            if (savedAndStandBy)
            {
                if (Vector3.Distance(transform.position, Camera.main.transform.position) <= Const.DISTANCE_TO_ACTIVE_ANCHOR)
                {
                    enableRotation = false;
                    watchingPlayer = true;
                }
                else
                {
                    enableRotation = true;
                    watchingPlayer = false;
                }
            }

            if (enableRotation)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * Const.ANCHOR_ROTATION_SPEED);
            }

            if(watchingPlayer)
            {
                lookAtTargetPos.Set(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
                transform.LookAt(lookAtTargetPos);
            }

            //checking interaction
            HandleAnchorInteraction();
        }

        
        private void HandleAnchorInteraction()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Ended)
                {
                    if (ReferenceEquals(EventSystem.current.currentSelectedGameObject, null))
                    {
                        RaycastHit hit = new RaycastHit();
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);

                        if (Physics.Raycast(ray, out hit)) //if it hits a game object
                        {
                            GameObject hitObj = hit.transform.gameObject;
                            if(hitObj.name == "CompletionObj") //if 'check-in' object is clicked
                            {
                                switch (this.anchorInfo.type)
                                {
                                    case ActivityType.PoI:
                                        contentPoI.GetComponent<PoIContentFacade>().CheckIn();
                                        break;
                                    case ActivityType.Trivia:
                                        contentTrivia.GetComponent<TriviaContentFacade>().OnCompleteAnimation();
                                        break;
                                    default:
                                        break;
                                }
                                
                            }
                            else if (hitObj.name.StartsWith("Cover")) //if a cover is clicked
                            {
                                ToggleButtons();
                                AnchorManager.Instance.RegisterCurrentAnchor(index);
                            }
                        }
                        else //if it doesn't hit an game object
                        {
                            //do nothing
                        }
                    }
                }
            }
        }
        
        //-------------------- init ----------------------------------------
        
        /// <summary>
        /// Init anchor with a few context information
        /// </summary>
        /// <param name="anchorInfo"></param>
        public void Born(int index, AnchorInfo anchorInfo)
        {
            this.index = index;
            
            //set the initial anchor info
            this.anchorInfo = anchorInfo;
            
            //init event handler
            //this.editButton.onClick.AddListener(OnEditButtonClick);
            this.deleteButton.onClick.AddListener(OnDeleteButtonClick);
            
            //set it ready
            TurnOnButtons();
        }


        /// <summary>
        /// An anchor object is 'reborn' when it's localized!
        /// </summary>
        /// <param name="anchorInfo"></param>
        public void Reborn(AnchorInfo info)
        {
            this.anchorInfo = new AnchorInfo
            {
                //clone values
                pose = info.pose,
                
                experienceId = info.experienceId,
                activityCollectionId = info.activityCollectionId,

                activityId = info.activityId,
                activityName = info.activityName,

                type = info.type,

                activitySpecific = Utilities.CopyActivitySpecificProperties(info.activitySpecific)
            };
                
            //set mode
            this.SetUIMode(AnchorUIMode.Consumer);
            this.SetActivityType(info.type);
            this.SetBehaviourMode(AnchorBehaviourMode.SavedAndStandBy);

            //init edit/delete buttons
            this.editButton.onClick.AddListener(OnEditButtonClick);
            this.deleteButton.onClick.AddListener(OnDeleteButtonClick);
            this.editButton.gameObject.SetActive(false);
            this.deleteButton.gameObject.SetActive(false);
            this.isEditDeleteButtonsActive = false;

            //apply value on UI
            this.ApplyActivityName(info.activityName);

            //activity specific properties
            if (this.anchorInfo.activitySpecific != null)
            {
                switch (info.type)
                {
                    case ActivityType.PoI:
                        string poIDesc = this.anchorInfo.activitySpecific["PoIDescription"];
                        this.ApplyPoIDesc(poIDesc);
                        break;
                    case ActivityType.Trivia:
                        break;
                    case ActivityType.Post:
                        break;
                    case ActivityType.Photo:
                        break;
                    case ActivityType.CV:
                        break;
                    default:
                        break;
                }
            }
        }
        
        //------------------- method related to activity settings ---------------------

        public void EnableRotation(bool enabled)
        {
            this.enableRotation = enabled;
        }
        public void EnableDragging(bool enabled)
        {
            this.objectDragger.enabled = enabled;
        }
        public void EnableWatchingPlayer(bool enabled)
        {
            this.watchingPlayer = enabled;
        }

        
        public void SetActivityType(ActivityType type)
        {
            anchorInfo.type = type;
            ApplyActivityType();
        }
        public ActivityType GetActivityType()
        {
            return anchorInfo.type;
        }


        public void SetUIMode(AnchorUIMode mode)
        {
            currUIMode = mode;
            ApplyUIMode();
        }
        public AnchorUIMode GetUIMode()
        {
            return currUIMode;
        }


        public void SetBehaviourMode(AnchorBehaviourMode mode)
        {
            currBehaviourMode = mode;
            ApplyBehaviourMode();
        }
        public AnchorBehaviourMode GetBehaviourMode()
        {
            return currBehaviourMode;
        }


        private void ApplyUIMode()
        {
            switch(currUIMode)
            {
                case AnchorUIMode.Creator_PreSetting:
                    title.SetActive(false);
                    preSettingElements.SetActive(true);//enable preselection elements
                    activityTypeButtons.SetActive(false);//hide all selection buttons
                    postSettingElements.SetActive(false);//hide postselection elements
                    break;

                case AnchorUIMode.Creator_Setting:
                    title.SetActive(false);
                    preSettingElements.SetActive(false); //hide preselection elements

                    activityTypeButtons.SetActive(true); //enable activity buttons
                    SetActivityType(ActivityType.Undefined); //this is the default selection

                    postSettingElements.SetActive(false);//hide postselection elements
                    break;

                case AnchorUIMode.Creator_PostSetting:
                    title.SetActive(true);
                    preSettingElements.SetActive(false); //hide preselection elements
                    activityTypeButtons.SetActive(false); //hide all selection buttons
                    postSettingElements.SetActive(true);//enable preselection elements
                    break;

                case AnchorUIMode.Consumer:
                    title.SetActive(true);
                    preSettingElements.SetActive(false); //hide preselection elements
                    activityTypeButtons.SetActive(false); //hide all selection buttons
                    postSettingElements.SetActive(true);//enable preselection elements
                    break;

                default:
                    break;
            }
        }


        private void ApplyBehaviourMode()
        {
            MessageManager.Instance.DebugMessage(string.Format("Setting anchor to '{0}' mode", currBehaviourMode.ToString()));

            switch (currBehaviourMode)
            {
                case AnchorBehaviourMode.Setting:
                    this.EnableDragging(true);
                    savedAndStandBy = false;
                    this.EnableRotation(false);
                    this.EnableWatchingPlayer(true);
                    break;

                case AnchorBehaviourMode.SettingDoneButNotSaved:
                    this.EnableDragging(false);
                    savedAndStandBy = false;
                    this.EnableRotation(false);
                    this.EnableWatchingPlayer(true);
                    break;

                case AnchorBehaviourMode.SavedAndStandBy:
                    this.EnableDragging(false);
                    savedAndStandBy = true; //for rotation and watchingPlayer, it will be controller in Update()
                    break;

                default:
                    this.EnableDragging(false);
                    savedAndStandBy = true;
                    break;
            }
        }


        private void ApplyActivityType()
        {
            switch(anchorInfo.type)
            {
                case ActivityType.Undefined:
                    DeSelectAll();
                    coverDefault.SetActive(true);
                    break;

                case ActivityType.PoI:
                    DeSelectAll();
                    SetButtonSelected(buttonPoI, true);
                    coverPoI.SetActive(true);
                    contentPoI.SetActive(true);
                    contentPoI.GetComponent<PoIContentFacade>().ApplyContentMode(currUIMode);
                    break;

                case ActivityType.Photo:
                    DeSelectAll();
                    SetButtonSelected(buttonPhoto, true);
                    coverPhoto.SetActive(true);
                    break;

                case ActivityType.Post:
                    DeSelectAll();
                    SetButtonSelected(buttonPost, true);
                    coverPost.SetActive(true);
                    break;

                case ActivityType.Trivia:
                    DeSelectAll();
                    SetButtonSelected(buttonTrivia, true);
                    coverTrivia.SetActive(true);
                    contentTrivia.SetActive(true);
                    contentTrivia.GetComponent<TriviaContentFacade>().ApplyContentMode(currUIMode);
                    break;

                case ActivityType.CV:
                    DeSelectAll();
                    SetButtonSelected(buttonCV, true);
                    coverCV.SetActive(true);
                    break;

                default:
                    DeSelectAll();
                    coverDefault.SetActive(true);
                    break;
            }
        }


        private void DeSelectAll()
        {
            SetButtonSelected(buttonPoI, false);
            SetButtonSelected(buttonPhoto, false);
            SetButtonSelected(buttonPost, false);
            SetButtonSelected(buttonTrivia, false);
            SetButtonSelected(buttonCV, false);

            coverDefault.SetActive(false);
            coverPoI.SetActive(false);
            coverPhoto.SetActive(false);
            coverPost.SetActive(false);
            coverTrivia.SetActive(false);
            coverCV.SetActive(false);

            contentPoI.SetActive(false);
            contentTrivia.SetActive(false);
            //add other content...
        }


        private void SetButtonSelected(GameObject buttonObj, bool selected)
        {
            buttonObj.transform.GetChild(1).gameObject.SetActive(selected);
        }
        

        public void ApplyActivityName(string name)
        {
            title.GetComponentInChildren<InputField>().text = name;
        }

        public void ApplyPoIDesc(string poiDesc)
        {
            contentPoI.GetComponentInChildren<InputField>().text = poiDesc;
        }

        public void OnEditPoIDescDone(InputField input)
        {
            if (!string.IsNullOrEmpty(input.text))
            {
                this.anchorInfo.activitySpecific["PoIDescription"] = input.text;
                MessageManager.Instance.DebugMessage(string.Format("PoI description of anchor '{0}' updated to '{1}'.", 
                    this.anchorInfo.activityName, this.anchorInfo.activitySpecific["PoIDescription"]));
            }

            //register
            AnchorManager.Instance.RegisterCurrentAnchor(this.index, this.gameObject);

            //disable interaction
            input.DeactivateInputField();
            input.interactable = false;
        }

        //------------------- methods called by UI buttons during activity settings ---------------------

        public void StartSetting()
        {
            SetUIMode(AnchorUIMode.Creator_Setting);
            SetBehaviourMode(AnchorBehaviourMode.Setting);
        }

        public void SelectPoI()
        {
            SetActivityType(ActivityType.PoI);
            SocialBeeARMain.Instance.OnActivitySelected();
        }

        public void SelectPhoto()
        {
            SetActivityType(ActivityType.Photo);
            SocialBeeARMain.Instance.OnActivitySelected();
        }

        public void SelectPost()
        {
            SetActivityType(ActivityType.Post);
            SocialBeeARMain.Instance.OnActivitySelected();
        }

        public void SelectTrivia()
        {
            SetActivityType(ActivityType.Trivia);
            SocialBeeARMain.Instance.OnActivitySelected();
        }

        public void SelectCV()
        {
            SetActivityType(ActivityType.CV);
            SocialBeeARMain.Instance.OnActivitySelected();
        }

        //---------------------- Handle edit/delete buttons-------------------
        private void ToggleButtons()
        {
            if (isEditDeleteButtonsActive)
                TurnOffButtons();
            else
                TurnOnButtons();
        }
        
        
        public void TurnOnButtons()
        {
            this.editButton.gameObject.SetActive(true);
            this.deleteButton.gameObject.SetActive(true);
            this.isEditDeleteButtonsActive = true;
            this.EnableRotation(false);
        }

        
        public void TurnOffButtons()
        {
            this.editButton.gameObject.SetActive(false);
            this.deleteButton.gameObject.SetActive(false);
            this.isEditDeleteButtonsActive = false;
            this.EnableRotation(true);
        }
        
        
        public void OnEditButtonClick()
        {
            MessageManager.Instance.DebugMessage(string.Format("Edit button of anchor '{0}' clicked!", anchorInfo.activityName));
            this.EnableRotation(false);

            //light-weight register
            AnchorManager.Instance.RegisterCurrentAnchor(index);
            
            EditActivityNameForCurrentAnchorObj();
        }

        
        public void EditActivityNameForCurrentAnchorObj()
        {
            MessageManager.Instance.DebugMessage("Editing selected anchor.");

            // Activate input field
            activityNameInputField.interactable = true;
            activityNameInputField.ActivateInputField();

            activityNameInputField.onEndEdit.AddListener(delegate { OnEditActivityNameCompleted(activityNameInputField); });
        }


        private void OnEditActivityNameCompleted(InputField input)
        {
            MessageManager.Instance.DebugMessage("Edit activity name completed.");

            this.anchorInfo.activityName = input.text;
            
            input.DeactivateInputField();
            input.interactable = false;
            TurnOffButtons();

            //heavy-weight registration
            AnchorManager.Instance.RegisterCurrentAnchor(this.index, this.gameObject);

            SocialBeeARMain.Instance.OnEditAcivityNameDone();

            //Start mapping
            //SocialBeeARMain.Instance.OnNewMapClick();
        }
        
        
        public void OnDeleteButtonClick()
        {
            MessageManager.Instance.DebugMessage(string.Format("Delete button of anchor: '{0}' is clicked!", this.anchorInfo.activityName));
 
            if (index >= 0)
            {
                AnchorManager.Instance.DeleteAnchorObj(index);
            }
        }

        //------------------- Generate Info ---------------------
        
        public AnchorInfo GetAnchorInfo()
        {
            //basically update the pose information in the info object
            this.anchorInfo.pose = new Pose(transform.position, transform.rotation);
            
            // //add activity specific properties, for testing only
            // switch (this.anchorInfo.type)
            // {
            //     case ActivityType.PoI:
            //         break;
            //     case ActivityType.Trivia:
            //         this.anchorInfo.activitySpecific.Add("TriviaQuestion", "trivia question 1");
            //         this.anchorInfo.activitySpecific.Add("TriviaAnswer", "trivia answer 1");
            //         break;
            //     case ActivityType.Post:
            //         break;
            //     case ActivityType.Photo:
            //         break;
            //     case ActivityType.CV:
            //         break;
            //     default:
            //         break;
            // }
            
            return this.anchorInfo;
        }
        
        
        //------------------- Simple getters ---------------------

    }
}
