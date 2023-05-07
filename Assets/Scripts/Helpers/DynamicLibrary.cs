using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Calibration;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Helpers
{
    /// <summary>
    /// Adds images to the reference library at runtime.
    /// </summary>
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class DynamicLibrary : MonoBehaviour
    {
        [SerializeField] private TriadMarkersLibrary _triadMarkersLibrary;

        /// <summary>
        /// The set of images to add to the image library at runtime
        /// </summary>
        public List<ImageData> Images { get; set; }

        public TriadMarkersLibrary TriadMarkersTriadMarkersLibrary => _triadMarkersLibrary;

        public bool IsInitialized { get; private set; }

        enum State
        {
            NoImagesAdded,
            AddImagesRequested,
            AddingImages,
            Done,
            Error
        }

        private State _mState;
        private ARTrackedImageManager _arTrackedImageManager;


        public IEnumerator Initialize()
        {
            if (IsInitialized || Application.isEditor)
                yield break;

            _arTrackedImageManager = GetComponent<ARTrackedImageManager>();

            const float imageManagerInitTime = 1;
            yield return new WaitForSeconds(imageManagerInitTime);

            Images = new List<ImageData>();

            foreach (var triadMarker in _triadMarkersLibrary.TriadMarkers)
            {
                Images.Add(new ImageData(triadMarker.MarkerA, triadMarker.NameMarkerA, _triadMarkersLibrary.Width));
                Images.Add(new ImageData(triadMarker.MarkerB, triadMarker.NameMarkerB, _triadMarkersLibrary.Width));
                Images.Add(new ImageData(triadMarker.MarkerC, triadMarker.NameMarkerC, _triadMarkersLibrary.Width));
            }

            yield return AddImagesRoutine();
            IsInitialized = true;
        }

        private IEnumerator AddImagesRoutine()
        {
            _mState = State.AddImagesRequested;

            while (_mState != State.Done)
            {
                switch (_mState)
                {
                    case State.AddImagesRequested:
                        AddImages();
                        break;
                
                    case State.AddingImages:
                        CheckAllImagesAdded();
                        break;
                }

                yield return null;
            }
        }

        private void AddImages()
        {
            if (Images == null || Images.Count == 0)
            {
                SetError("No images to add.");
                return;
            }

            if (_arTrackedImageManager == null)
            {
                SetError($"No {nameof(ARTrackedImageManager)} available.");
                return;
            }

            // You can either add raw image bytes or use the extension method (used below) which accepts
            // a texture. To use a texture, however, its import settings must have enabled read/write
            // access to the texture.
            foreach (var image in Images)
            {
                if (image.Texture.isReadable == false)
                {
                    SetError($"Image {image.Name} must be readable to be added to the image library.");
                    break;
                }
            }

            if (_arTrackedImageManager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                try
                {
                    foreach (var image in Images)
                    {
                        // Note: You do not need to do anything with the returned JobHandle, but it can be
                        // useful if you want to know when the image has been added to the library since it may
                        // take several frames.
                        image.JobState = mutableLibrary.ScheduleAddImageWithValidationJob(image.Texture, image.Name, image.Width);
                    }

                    _mState = State.AddingImages;
                }
                catch (InvalidOperationException e)
                {
                    SetError($"ScheduleAddImageJob threw exception: {e.Message}");
                }
            }
            else
            {
                SetError($"The reference image library is not mutable.");
            }
        }
        
        private void CheckAllImagesAdded()
        {
            var done = true;
                    
            foreach (var image in Images)
            {
                if (image.JobState.jobHandle.IsCompleted == false)
                {
                    done = false;
                    break;
                }
            }

            if (done) 
                _mState = State.Done;
        }
        
        private void SetError(string errorMessage)
        {
            _mState = State.Error;
            Debug.LogError(errorMessage);
        }
    }
    
    [Serializable]
    public class ImageData
    {
        public readonly Texture2D Texture;
        public readonly string Name;
        public readonly float Width;

        public AddReferenceImageJobState JobState;

        
        public ImageData(Texture2D texture, string name, float width)
        {
            Texture = texture;
            Name = name;
            Width = width;
        }
    }
}
