using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


namespace SocialBeeAR
{

    /// <summary>
    /// This is just modified from 'ReticleController' in 'HellowWorld' sample.
    /// </summary>
    public class ReticleController : MonoBehaviour
    {
        [SerializeField] GameObject reticle;

        static List<ARRaycastHit> hits = new List<ARRaycastHit>();
        public ARRaycastManager raycastManager;

        private IEnumerator continuousHitTest;

        void Start()
        {
            reticle.SetActive(false);
            continuousHitTest = ContinuousHittest();
        }

        // starts the cursor
        public void StartReticle()
        {
            reticle.SetActive(false);
            StartCoroutine(continuousHitTest);
        }


        public void StopReticle()
        {
            StopCoroutine(continuousHitTest);
            reticle.SetActive(false);
        }


        private IEnumerator ContinuousHittest()
        {
            while (true)
            {
                // getting screen point
                var screenPosition = new Vector2(Screen.width / 2, Screen.height / 2);

                // World Hit Test
                if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinBounds))
                {
                    // Raycast hits are sorted by distance, so get the closest hit.
                    var targetPose = hits[0].pose;
                    reticle.transform.position = targetPose.position;

                    reticle.SetActive(true);

                    Vector3 screenCenter = Camera.main.ScreenToWorldPoint(screenPosition);
                    float distanceToReticle = Vector3.Magnitude(targetPose.position - screenCenter);
                }
                else
                {
                    reticle.SetActive(false);
                }

                // go to next frame
                yield return null;
            }
        }


    }
}