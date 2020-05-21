using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


namespace SocialBeeAR
{

    /// <summary>
    /// This class is the manager for all virtual objects (except anchors). This can be extended with different sub-managers for different type of objects.
    /// </summary>
    public class VirtualContentManager : MonoBehaviour
    {

        [SerializeField] ARPlaneManager arPlaneManager;

        private static VirtualContentManager _instance;
        public static VirtualContentManager Instance
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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetARPlanesVisible(bool visible)
        {
            if (arPlaneManager != null)
            {
                //control the visibility of all the created planes
                foreach (var plane in arPlaneManager.trackables)
                {
                    plane.gameObject.SetActive(visible);
                }

                //enable/disable plane manager
                arPlaneManager.enabled = visible;
            }
        }
    }

}
