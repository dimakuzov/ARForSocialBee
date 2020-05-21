using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.XR.ARFoundation;


namespace SocialBeeAR
{

    /// <summary>
    /// The manager class for handling the UI element interaction
    /// </summary>
    public class SocialBeeARMain : MonoBehaviour, PlacenoteListener
    {

        private float currentAnchorScanningProgress = 0f;
        private int lastMapSize = 0;
        private bool mapQualityThresholdCrossed = false;
        private bool isNewExperience = true;
        public bool IsNewExperience()
        {
            return isNewExperience;
        }

        private LibPlacenote.MapMetadataSettable currMapDetails;
        private LibPlacenote.MapInfo selectedMapInfo;
        private string SelectedMapId
        {
            get
            {
                return selectedMapInfo != null ? selectedMapInfo.placeId : null;
            }
        }

        private static SocialBeeARMain _instance;
        public static SocialBeeARMain Instance
        {
            get
            {
                return _instance;
            }
        }


        //------------------------Monobehaviour methods-------------------------


        void Awake()
        {
            _instance = this;
        }


        void Start()
        {
            //set UI mode
            UIManager.Instance.SetUIMode(UIManager.UIMode.Init);
            InitFacade.Instance.SetUIMode(InitFacade.UIMode.Initializing);

            // wait for ar session to start tracking and for placenote to initialize
            StartCoroutine(WaitForARSessionThenStart());

            //by default, hide the plane detected
            VirtualContentManager.Instance.SetARPlanesVisible(false);
        }


        IEnumerator WaitForARSessionThenStart()
        {
            MessageManager.Instance.ShowMessage("Initializing Placenote AR Session...");
            while (ARSession.state != ARSessionState.SessionTracking || !LibPlacenote.Instance.Initialized())
            {
                yield return null;
            }

            LibPlacenote.Instance.RegisterListener(this); // Register listener for onStatusChange and OnPose
            FeaturesVisualizer.EnablePointcloud(Const.FEATURE_POINT_WEAK, Const.FEATURE_POINT_STRONG);

            // AR Session has started tracking here. Now start the session
            Input.location.Start();

            //selectMapPanel.SetActive(false);

            // Localization thumbnail handler.
            //mapThumbnail.gameObject.SetActive(false);

            // Set up the localization thumbnail texture event.
            LocalizationThumbnailSelector.Instance.TextureEvent += (thumbnailTexture) =>
            {
                if (LocalizationFacade.Instance.mapThumbnail == null)
                {
                    return;
                }

                RectTransform rectTransform = LocalizationFacade.Instance.mapThumbnail.rectTransform;
                if (thumbnailTexture.width != (int)rectTransform.rect.width)
                {
                    rectTransform.SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Horizontal, thumbnailTexture.width * 2);
                    rectTransform.SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Vertical, thumbnailTexture.height * 2);
                    rectTransform.ForceUpdateRectTransforms();
                }
                LocalizationFacade.Instance.mapThumbnail.texture = thumbnailTexture;
            };

            MessageManager.Instance.ShowMessage("Initialization compelted, GO!");
            InitFacade.Instance.SetUIMode(InitFacade.UIMode.Inititialized);
        }


        // Update is called once per frame
        void Update()
        {
            if (LibPlacenote.Instance.Initialized())
            {
                //Print some info for debugging, to be removed later
                LibPlacenote.PNFeaturePointUnity[] map = LibPlacenote.Instance.GetMap();
                if (map != null && map.Length > 0)
                {
                    //map size can be shown not only for mapping mode but also for localizing mode.
                    MessageManager.Instance.UpdateMapSize(map.Length);

                    if (LibPlacenote.Instance.GetMode() == LibPlacenote.MappingMode.MAPPING)
                    {
                        MessageManager.Instance.UpdateMappingQuality(LibPlacenote.Instance.GetMappingQuality().ToString());
                        MessageManager.Instance.UpdateCurrentAnchorSize(GetCurrentAnchorSize());
                    }
                }
            }
        }

        
        //-----------------------------InitPanel-------------------------------


        public void OnSearchMapsClick()
        {

            MessageManager.Instance.ShowMessage("Searching for saved maps");

            //TODO: later we can use this location info for querying the related maps.
            LocationInfo locationInfo = Input.location.lastData;

            //LibPlacenote.Instance.SearchMaps(locationInfo.latitude, locationInfo.longitude, radiusSearch, (mapList) =>
            LibPlacenote.Instance.SearchMaps(Const.MAP_PREFIX, (mapList) =>
            {
                foreach (Transform t in SelectMapFacade.Instance.mapListContentParent.transform)
                {
                    Destroy(t.gameObject);
                }

                MessageManager.Instance.DebugMessage(string.Format("Found {0} maps.", mapList.Length));
                if (mapList.Length == 0)
                {
                    MessageManager.Instance.ShowMessage("No maps found. Create a map first!");
                    return;
                }

                // Render the map list!
                foreach (LibPlacenote.MapInfo mapId in mapList)
                {
                    if (mapId.metadata.userdata != null)
                    {
                        Debug.Log(mapId.metadata.userdata.ToString(Formatting.None));
                    }

                    AddMapToList(mapId);
                }

                MessageManager.Instance.ShowMessage("Please select a map to load");

                //update UI mode
                UIManager.Instance.SetUIMode(UIManager.UIMode.SelectMap);
                SelectMapFacade.Instance.SetUIMode(SelectMapFacade.UIMode.SelectMap_NoMapSelected);

            });
        }


        private void AddMapToList(LibPlacenote.MapInfo mapInfo)
        {
            GameObject newElement = Instantiate(SelectMapFacade.Instance.mapInfoElementPrefab) as GameObject;
            if (newElement != null)
            {
                SBMapInfoElement listElement = newElement.GetComponent<SBMapInfoElement>();
                listElement.Initialize(mapInfo, SelectMapFacade.Instance.mapListContentToggleGroup, SelectMapFacade.Instance.mapListContentParent, (value) =>
                {
                    OnMapSelected(mapInfo);
                });
            }
        }


        private void OnMapSelected(LibPlacenote.MapInfo mapInfo)
        {
            selectedMapInfo = mapInfo;

            //UIManager.Instance.SetUIMode(UIManager.UIMode.SelectMap); //toggle button state will be ruined if enabled...
            SelectMapFacade.Instance.SetUIMode(SelectMapFacade.UIMode.SelectMap_MapSelected);
            //MessageManager.Instance.DebugMessage("Map selected: " + mapInfo.placeId);
        }


        public void OnNewActivityClick()
        {
            if (!SanityCheck()) return;

            // UI navigation and label updates to signal entry into mapping mdoe
            MessageManager.Instance.ShowMessage("Point at any flat surface, like a table, then hit the + button to place the model");

            //update UI mode
            UIManager.Instance.SetUIMode(UIManager.UIMode.ActivitySetting);

            //start placing reticle
            GetComponent<ReticleController>().StartReticle();

            //set detected planes visible
            VirtualContentManager.Instance.SetARPlanesVisible(true);
        }


        //-----------------------------MapListPanel-----------------------------


        public void OnCancelSelectingMapClick()
        {
            //update UI mode
            UIManager.Instance.SetUIMode(UIManager.UIMode.Init);
            InitFacade.Instance.SetUIMode(InitFacade.UIMode.Inititialized);
        }


        public void OnLoadMapClick()
        {
            if (!SanityCheck()) return;

            string message = string.Format("Loading map ID: {0}", SelectedMapId);
            MessageManager.Instance.ShowMessage(message);
            MessageManager.Instance.DebugMessage(message);

            LibPlacenote.Instance.LoadMap(SelectedMapId, (completed, faulted, percentage) =>
            {
                if (completed) //this DOES NOT mean map 'locating' completed, it means map 'loaded' from the memory!
                {
                    //update UI mode
                    UIManager.Instance.SetUIMode(UIManager.UIMode.Localization);
                    LocalizationFacade.Instance.SetUIMode(LocalizationFacade.UIMode.Localization_Unlocalized);

                    //Start localization session
                    LibPlacenote.Instance.StartSession();
                    MessageManager.Instance.DebugMessage("Session started in Locating mode");
                }
                else if (faulted)
                {
                    string failedMsg = string.Format("Failed to load map '{0}'", SelectedMapId);
                    MessageManager.Instance.ShowMessage(failedMsg);
                    MessageManager.Instance.DebugMessage(failedMsg);
                }
                else //success
                {
                    MessageManager.Instance.ShowMessage(string.Format("Map downloaded: {0}", percentage.ToString("F2") + "/1.0"));
                }
            });
        }


        public void OnDeleteMapClick()
        {
            if (!SanityCheck()) return;

            MessageManager.Instance.ShowMessage(string.Format("Deleting Map ID: '{0}'", SelectedMapId));
            LibPlacenote.Instance.DeleteMap(SelectedMapId, (deleted, errMsg) =>
            {
                if (deleted)
                {
                    //update UI mode
                    UIManager.Instance.SetUIMode(UIManager.UIMode.SelectMap);
                    SelectMapFacade.Instance.SetUIMode(SelectMapFacade.UIMode.SelectMap_MapDeleted);

                    MessageManager.Instance.ShowMessage(string.Format("Deleted map '{0}'", SelectedMapId));
                    OnSearchMapsClick();
                }
                else
                {
                    MessageManager.Instance.ShowMessage(string.Format("Failed to delete map '{0}'", SelectedMapId));
                }
            });
        }


        //-------------------------ActivitySettingPanel-------------------------


        public void OnConfirmActivitySettingClick()
        {
            AnchorController anchorController = AnchorManager.Instance.GetCurrentAnchorObj().GetComponent<AnchorController>();

            anchorController.SetUIMode(AnchorUIMode.Creator_PostSetting);
            anchorController.SetBehaviourMode(AnchorBehaviourMode.SettingDoneButNotSaved);

            //start editing activity name
            AnchorManager.Instance.StartEditingCurrentAnchor();
        }


        //----------------------------ScanningPanel-----------------------------


        public void OnStartScanningClick()
        {
            //start progress info panel
            UIManager.Instance.SetUIMode(UIManager.UIMode.Mapping);
            MappingFacade.Instance.SetUIMode(MappingFacade.UIMode.Mapping_Scanning);

            StartMapping();
        }


        //-----------------------ContinuousMappingPanel-------------------------


        public void OnSaveMapClick()
        {
            if (!SanityCheck()) return;

            if (!mapQualityThresholdCrossed)
            {
                MessageManager.Instance.ShowMessage(string.Format("Map quality is not good enough to save. Scan a small area with many features and try again."));
                return;
            }

            bool isLocationServiceAvailable = Input.location.status == LocationServiceStatus.Running;
            LocationInfo locationInfo = Input.location.lastData;

            //update UI mode
            UIManager.Instance.SetUIMode(UIManager.UIMode.ContinuousMapping);
            ContinuousMappingFacade.Instance.SetUIMode(ContinuousMappingFacade.UIMode.SavingMap);

            MessageManager.Instance.ShowMessage("Saving...");

            LibPlacenote.Instance.SaveMap((mapId) =>
            {
                LibPlacenote.Instance.StopSession();
                FeaturesVisualizer.ClearPointcloud();

                //continousMappingPanel.SetActive(false); //why again???

                LibPlacenote.MapMetadataSettable metadata = new LibPlacenote.MapMetadataSettable();

                metadata.name = Utilities.GenerateMapId();
                MessageManager.Instance.ShowMessage("Saving map: " + metadata.name);

                JObject userdata = new JObject();
                metadata.userdata = userdata;

                JObject anchorList = GetComponent<AnchorManager>().AnchorInfoListToJSON();

                userdata[Const.ANCHOR_DATA_JSON_ROOT] = anchorList;
                GetComponent<AnchorManager>().ClearAnchors();

                if (isLocationServiceAvailable)
                {
                    metadata.location = new LibPlacenote.MapLocation();
                    metadata.location.latitude = locationInfo.latitude;
                    metadata.location.longitude = locationInfo.longitude;
                    metadata.location.altitude = locationInfo.altitude;
                }

                LibPlacenote.Instance.SetMetadata(mapId, metadata, (success) =>
                {
                    if (success)
                    {
                        Debug.Log("Meta data successfully saved!");
                    }
                    else
                    {
                        Debug.Log("Meta data failed to save");
                    }
                });
                currMapDetails = metadata;
            }, (completed, faulted, percentage) =>
            {
                if (completed)
                {
                    MessageManager.Instance.ShowMessage("Upload Complete! You can now click My Maps and choose a map to load.");

                    //update UI mode
                    UIManager.Instance.SetUIMode(UIManager.UIMode.Init);
                    InitFacade.Instance.SetUIMode(InitFacade.UIMode.Inititialized);
                    
                    this.lastMapSize = 0;
                    isNewExperience = true;  //reset the new experience mode
                }
                else if (faulted)
                {
                    MessageManager.Instance.ShowMessage(string.Format("Upload of map '{0}' failed", currMapDetails.name));
                }
                else
                {
                    MessageManager.Instance.ShowMessage(string.Format("Uploading map ( {0} %)", (percentage * 100.0f).ToString("F2")));
                }
            });
        }


        //------------------------------LoadMapPanel--------------------------------


        private void StartMapping()
        {
            MessageManager.Instance.ShowMessage("Slowly move around the object.");

            //disable detected planes
            VirtualContentManager.Instance.SetARPlanesVisible(false);

            //reset progress status
            MappingFacade.Instance.ResetMappingProgress();
            mapQualityThresholdCrossed = false;
            currentAnchorScanningProgress = 0f;

            // Enable pointcloud
            FeaturesVisualizer.EnablePointcloud(Const.FEATURE_POINT_WEAK, Const.FEATURE_POINT_STRONG);

            if (isNewExperience)
            {
                LibPlacenote.Instance.StartSession();
                MessageManager.Instance.DebugMessage("New mapping started");
            }
            else
            {
                LibPlacenote.Instance.RestartSendingFrames();
                MessageManager.Instance.DebugMessage("Mapping resumed");
            }
        }


        public void OnExitMapClick()
        {
            MessageManager.Instance.ShowMessage("Session was reset. You can start new map or load your map again.");

            //update UI mode
            UIManager.Instance.SetUIMode(UIManager.UIMode.Init);
            InitFacade.Instance.SetUIMode(InitFacade.UIMode.Inititialized);

            LibPlacenote.Instance.StopSession();
            FeaturesVisualizer.ClearPointcloud();

            GetComponent<AnchorManager>().ClearAnchors();
        }


        
        public void OnExtendMapClick()
        {
            /*TODO: This is not correct! Before switching to ContinuousMappingMode, we should enter 'ExtendMap' mode,
            which guide user to go back to one of the previous anchor and locate there, after then switch
            to 'ContinuosMappingMode' */
            //UIManager.Instance.SetUIMode(UIManager.UIMode.ContinuousMapping);

            ////LibPlacenote.Instance.StopSession(); //NO NEED!
            ////DebugMessageManager.Instance.PrintDebugMessage("Session stopped");

            //FeaturesVisualizer.EnablePointcloud(Const.FEATURE_POINT_WEAK, Const.FEATURE_POINT_STRONG);

            //LibPlacenote.Instance.StartSession(true);
            //MessageManager.Instance.DebugMessage("Session started in extending mode");
        }


        public void OnUpdateActivityClick()
        {
            if (!SanityCheck()) return;

            MessageManager.Instance.ShowMessage("Updating anchor info...");

            LibPlacenote.MapMetadataSettable metadataUpdated = new LibPlacenote.MapMetadataSettable();

            metadataUpdated.name = selectedMapInfo.metadata.name;

            JObject userdata = new JObject();
            metadataUpdated.userdata = userdata;

            JObject notesList = GetComponent<AnchorManager>().AnchorInfoListToJSON();
            userdata[Const.ANCHOR_DATA_JSON_ROOT] = notesList;
            metadataUpdated.location = selectedMapInfo.metadata.location;

            LibPlacenote.Instance.SetMetadata(SelectedMapId, metadataUpdated, (success) =>
            {
                if (success)
                {
                    MessageManager.Instance.ShowMessage("Anchor updated! To end the session, click Exit.");
                    MessageManager.Instance.DebugMessage("Anchor info successfully updated.");
                }
                else
                {
                    MessageManager.Instance.DebugMessage("Anchor info failed to save");
                }
            });
        }


        //---------------------------------Others--------------------------------


        public void OnEditAcivityNameDone()
        {
            //update UI mode
            UIManager.Instance.SetUIMode(UIManager.UIMode.Mapping);
            MappingFacade.Instance.SetUIMode(MappingFacade.UIMode.Mapping_PreScanning);

            MessageManager.Instance.ShowMessage("Make sure to stay closed to the activity object, tap 'Yes' when ready");
        }


        public void OnActivitySelected()
        {
            //enable confirm button
            UIManager.Instance.SetUIMode(UIManager.UIMode.ActivitySetting);
            ActivitySettingFacade.Instance.SetUIMode(ActivitySettingFacade.UIMode.ActivitySetting_ActivitySelected);
        }


        public void OnReticlePlaced(GameObject reticle)
        {
            //place the anchor object
            AnchorManager.Instance.SpawnAnchorObj(reticle.transform.position);
            AnchorManager.Instance.GetCurrentAnchorObj().GetComponent<AnchorController>().SetUIMode(AnchorUIMode.Creator_PreSetting);
            AnchorManager.Instance.GetCurrentAnchorObj().GetComponent<AnchorController>().SetBehaviourMode(AnchorBehaviourMode.Setting);

            GetComponent<ReticleController>().StopReticle();

            //update UI mode
            UIManager.Instance.SetUIMode(UIManager.UIMode.ActivitySetting);
            ActivitySettingFacade.Instance.SetUIMode(ActivitySettingFacade.UIMode.ActivitySetting_NoActivitySeleted);

            MessageManager.Instance.ShowMessage("Tap the object to choose an activity.");
        }


        //-------------------------- PlaceNote callback---------------------------

        /// <summary>
        /// PlaceNote callback: When 'Pose'
        /// </summary>
        /// <param name="outputPose"></param>
        /// <param name="arkitPose"></param>
        public void OnPose(Matrix4x4 outputPose, Matrix4x4 arkitPose)
        {
            if (LibPlacenote.Instance.IsPerformingMapping())
            {
                //CheckIfEnoughPointcloudCollectedForMap();
                CheckIfEnoughPointcloudCollectedForAnchor();
            }
        }


        private void CheckIfEnoughPointcloudCollectedForAnchor()
        {
            // get the full point built so far
            int currentAnchorSize = GetCurrentAnchorSize();

            currentAnchorScanningProgress = (float)currentAnchorSize / (float)Const.MIN_MAP_SIZE;
            MappingFacade.Instance.SetMappingProgress(currentAnchorScanningProgress); 

            if (currentAnchorSize >= Const.MIN_MAP_SIZE)
            {
                if (LibPlacenote.Instance.GetMode() == LibPlacenote.MappingMode.MAPPING)
                    MessageManager.Instance.ShowMessage("Enough information collected for this activity.");

                // Check the map quality to confirm whether you can save
                if (LibPlacenote.Instance.GetMappingQuality() == LibPlacenote.MappingQuality.GOOD)
                {
                    OnScanningComplete();
                }
            }
        }


        private void OnScanningComplete()
        {
            //when enough feature points are collected!
            mapQualityThresholdCrossed = true;

            //set the last map size!!!
            this.lastMapSize = LibPlacenote.Instance.GetMap().Length;

            //update UI mode
            UIManager.Instance.SetUIMode(UIManager.UIMode.ContinuousMapping);
            ContinuousMappingFacade.Instance.SetUIMode(ContinuousMappingFacade.UIMode.Init);

            MessageManager.Instance.ShowMessage("Execellent! \nYou may continue to creat more activities or save the map.");

            //pause mapping
            PauseMapping();
        }

        private void PauseMapping()
        {
            this.isNewExperience = false;

            //pause scanning
            LibPlacenote.Instance.StopSendingFrames();
        }


        private int GetCurrentAnchorSize()
        {
            //another method to get feature point count:
            //List<Vector3> fullPointCloudMap = FeaturesVisualizer.GetPointCloud();
            //fullPointCloudMap.Count

            LibPlacenote.PNFeaturePointUnity[] map = LibPlacenote.Instance.GetMap();
            if (map != null && map.Length > 0)
                return map.Length - this.lastMapSize;
            else
                return 0;
        }


        /// <summary>
        /// PlaceNote event: When PlaceNote mapping status is changed.
        /// </summary>
        /// <param name="prevStatus"></param>
        /// <param name="currStatus"></param>
        public void OnStatusChange(LibPlacenote.MappingStatus prevStatus, LibPlacenote.MappingStatus currStatus)
        {
            string currentMode = LibPlacenote.Instance.GetMode().ToString();
            string status = currStatus.ToString();

            MessageManager.Instance.UpdateStatus(currentMode, status);
            Debug.Log(string.Format("Mode: '{0}', Status changed: '{1}'->'{2}'", currentMode, prevStatus.ToString(), status));

            if (currStatus == LibPlacenote.MappingStatus.LOST && prevStatus == LibPlacenote.MappingStatus.WAITING)
            {
                MessageManager.Instance.ShowMessage("Point your phone at the area shown in the thumbnail");
            }

            MessageManager.Instance.EnableMappingQualityInfo(LibPlacenote.Instance.IsPerformingMapping());
        }


        /// <summary>
        /// PlaceNote callback: when re-localisation happened
        /// </summary>
        public void OnLocalized()
        {
            this.lastMapSize = 0;
                
            MessageManager.Instance.ShowMessage("Localized. Add or edit notes and click Update. Or click Exit to end the session.");
            MessageManager.Instance.DebugMessage("Localized, loading virtual objects...");
            GetComponent<AnchorManager>().AnchorInfoListFromJSON(selectedMapInfo.metadata.userdata);
            MessageManager.Instance.DebugMessage("Content loaded.");

            //update UI mode
            UIManager.Instance.SetUIMode(UIManager.UIMode.Localization);
            LocalizationFacade.Instance.SetUIMode(LocalizationFacade.UIMode.Localization_Localized);

            FeaturesVisualizer.DisablePointcloud();
        }


        //--------------------------------- Utils ------------------------------
        private bool SanityCheck()
        {
            if (!LibPlacenote.Instance.Initialized())
            {
                Debug.Log("SDK not yet initialized");            
                return false;
            }
            else
                return true;
        }


        //---------------------------backup methods-----------------------------
        //private void CheckEnableContentPlacing()
        //{
        //    ContentManager.Instance.EnableCreatingContent(LibPlacenote.Instance.IsPerformingMapping());
        //}

        //private void CheckIfEnoughPointcloudCollectedForMap()
        //{
        //    // get the full point built so far
        //    List<Vector3> fullPointCloudMap = FeaturesVisualizer.GetPointCloud();

        //    // Check if either are null
        //    if (fullPointCloudMap == null)
        //    {
        //        Debug.Log("Point cloud is null");
        //        return;
        //    }
        //    Debug.Log("Point cloud size = " + fullPointCloudMap.Count);

        //    currentAnchorScanningProgress = (float)fullPointCloudMap.Count / (float)Const.MIN_MAP_SIZE;
        //    saveButtonProgressBar.gameObject.GetComponent<Image>().fillAmount = currentAnchorScanningProgress;

        //    if (fullPointCloudMap.Count >= Const.MIN_MAP_SIZE)
        //    {
        //        if (LibPlacenote.Instance.GetMode() == LibPlacenote.MappingMode.MAPPING)
        //            mLabelText.text = "Reeady to save the map.";

        //        // Check the map quality to confirm whether you can save
        //        if (LibPlacenote.Instance.GetMappingQuality() == LibPlacenote.MappingQuality.GOOD)
        //        {
        //            mapQualityThresholdCrossed = true;
        //        }
        //    }
        //}

    }

}