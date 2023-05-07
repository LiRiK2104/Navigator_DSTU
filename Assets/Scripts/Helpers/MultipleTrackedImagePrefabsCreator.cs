using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Helpers
{
    /// <summary>
    /// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
    /// and overlays some prefabs on top of the detected image.
    /// </summary>
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class MultipleTrackedImagePrefabsCreator : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] [HideInInspector] private List<NamedPrefab> _prefabsList = new List<NamedPrefab>();
        [SerializeField] private DynamicLibrary _dynamicLibrary;

        private Dictionary<Guid, GameObject> _mPrefabsDictionary = new Dictionary<Guid, GameObject>();
        private Dictionary<Guid, GameObject> _mInstantiated = new Dictionary<Guid, GameObject>();
        private ARTrackedImageManager _mTrackedImageManager;

        private void Awake()
        {
            _mTrackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
            _mTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        private void OnDisable()
        {
            _mTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        private void Start()
        {
            StartInitialization();
        }


        public void StartInitialization()
        {
            StartCoroutine(Initialize());
        }
        

        public GameObject GetPrefabForReferenceImage(XRReferenceImage referenceImage)
        {
            return _mPrefabsDictionary.TryGetValue(referenceImage.guid, out var prefab) ? prefab : null;
        }

        public void SetPrefabForReferenceImage(XRReferenceImage referenceImage, GameObject alternativePrefab)
        {
            _mPrefabsDictionary[referenceImage.guid] = alternativePrefab;
            if (_mInstantiated.TryGetValue(referenceImage.guid, out var instantiatedPrefab))
            {
                _mInstantiated[referenceImage.guid] = Instantiate(alternativePrefab, instantiatedPrefab.transform.parent);
                Destroy(instantiatedPrefab);
            }
        }
        
        public void OnBeforeSerialize()
        {
            _prefabsList.Clear();
            foreach (var kvp in _mPrefabsDictionary)
            {
                _prefabsList.Add(new NamedPrefab(kvp.Key, kvp.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            _mPrefabsDictionary = new Dictionary<Guid, GameObject>();
            foreach (var entry in _prefabsList)
            {
                _mPrefabsDictionary.Add(Guid.Parse(entry.imageGuid), entry.imagePrefab);
            }
        }
        
        private IEnumerator Initialize()
        {
            yield return _dynamicLibrary.Initialize();

            if (_mTrackedImageManager.referenceLibrary is not MutableRuntimeReferenceImageLibrary mutableLibrary)
                yield break;
            
            foreach (var image in mutableLibrary)
            {
                foreach (var triadMarkerData in _dynamicLibrary.TriadMarkersTriadMarkersLibrary.TriadMarkers)
                {
                    GameObject prefab = null;
                    
                    if (image.name == triadMarkerData.NameMarkerA)
                        prefab = _dynamicLibrary.TriadMarkersTriadMarkersLibrary.RedSpherePrefab;
                    else if (image.name == triadMarkerData.NameMarkerB)
                        prefab = _dynamicLibrary.TriadMarkersTriadMarkersLibrary.GreenSpherePrefab;
                    else if (image.name == triadMarkerData.NameMarkerC)
                        prefab = _dynamicLibrary.TriadMarkersTriadMarkersLibrary.BlueSpherePrefab;

                    if (prefab != null)
                    {
                        _mPrefabsDictionary.Add(image.guid, prefab);
                        break;
                    }
                }
            }
        }
        
        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                // Give the initial image a reasonable default scale
                var minLocalScalar = Mathf.Min(trackedImage.size.x, trackedImage.size.y) / 2;
                trackedImage.transform.localScale = new Vector3(minLocalScalar, minLocalScalar, minLocalScalar);
                AssignPrefab(trackedImage);
            }
        }
        
        private void AssignPrefab(ARTrackedImage trackedImage)
        {
            if (_mPrefabsDictionary.TryGetValue(trackedImage.referenceImage.guid, out var prefab))
                _mInstantiated[trackedImage.referenceImage.guid] = Instantiate(prefab, trackedImage.transform);
        }
    }
    
    /// <summary>
    /// Used to associate an `XRReferenceImage` with a Prefab by using the `XRReferenceImage`'s guid as a unique identifier for a particular reference image.
    /// </summary>
    [Serializable]
    public struct NamedPrefab
    {
        // System.Guid isn't serializable, so we store the Guid as a string. At runtime, this is converted back to a System.Guid
        public string imageGuid;
        public GameObject imagePrefab;

        public NamedPrefab(Guid guid, GameObject prefab)
        {
            imageGuid = guid.ToString();
            imagePrefab = prefab;
        }
    }
}
