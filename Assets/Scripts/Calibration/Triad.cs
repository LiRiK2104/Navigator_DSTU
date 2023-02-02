using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Calibration
{
    public class Triad : MonoBehaviour
    {
        [SerializeField] private VirtualMarker _refMarker1st;
        [SerializeField] private VirtualMarker _refMarker2nd;
        [SerializeField] private VirtualMarker _refMarker3rd;
        [Space] 
        [SerializeField] private Anchor _anchor;

        public VirtualMarker RefMarker1St => _refMarker1st;
        public VirtualMarker RefMarker2Nd => _refMarker2nd;
        public VirtualMarker RefMarker3Rd => _refMarker3rd;
        public Anchor Anchor => _anchor;
        
        
        public bool HasTrackedImage(List<ARTrackedImage> trackedImages, 
            out ARTrackedImage image1st, 
            out ARTrackedImage image2nd, 
            out ARTrackedImage image3rd)
        {
            image1st = null;
            image2nd = null;
            image3rd = null;
            
            return HasTrackedImage(trackedImages, _refMarker1st, out image1st) &&
                   HasTrackedImage(trackedImages, _refMarker2nd, out image2nd) &&
                   HasTrackedImage(trackedImages, _refMarker3rd, out image3rd);
        }
        
        private bool HasTrackedImage(List<ARTrackedImage> trackedImages, VirtualMarker targetMarker, out ARTrackedImage trackedImage)
        {
            trackedImage = trackedImages.FirstOrDefault(image => image.referenceImage.name == targetMarker.Id);
            return trackedImage != default(ARTrackedImage) && trackedImage.trackingState != TrackingState.None;
        }
    }
}
