using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AR
{
    public class ARValidator : MonoBehaviour
    {
        public event Action Loading;
        public event Action Completed;
        public event Action Failed;

        public ARValidationState State { get; private set; } = ARValidationState.None;


        private void OnEnable()
        {
            if (Application.isEditor)
            {
                Debug.Log("Unity editor: AR not supported, Device Not Capable");
                return;
            }
        
            ARSession.stateChanged += OnARSessionStateChanged;
        }
    
        private void OnDisable()
        {
            ARSession.stateChanged -= OnARSessionStateChanged;
        }

    
        public IEnumerator CheckAvailability()
        {
            var availabilityCheckingRoutine = CheckAvailabilityRoutine();
            StartCoroutine(availabilityCheckingRoutine);

            return availabilityCheckingRoutine;
        }
    
        private IEnumerator CheckAvailabilityRoutine()
        {
            yield return ARSession.CheckAvailability();
            ProcessState();
        }
    
        private void OnARSessionStateChanged(ARSessionStateChangedEventArgs obj)
        {
            ProcessState();
        }

        private void ProcessState()
        {
            switch (ARSession.state)
            {
                case ARSessionState.CheckingAvailability:
                    SetLoadingState();
                    Debug.Log("Still Checking Availability...");
                    break;
            
                case ARSessionState.NeedsInstall:
                    SetLoadingState();
                    Debug.Log("Supported, not installed, requesting installation");
                    //TODO: Request ARCore services apk installation and install only if user allows
                    StartCoroutine(InstallARCoreApp());
                    break;
            
                case ARSessionState.Installing:
                    SetLoadingState();
                    Debug.Log("Supported, apk installing");
                    StartCoroutine(InstallARCoreApp());
                    break;
            
                case ARSessionState.Ready:
                    SetCompleteState();
                    Debug.Log("Supported and installed");
                    break;
            
                case ARSessionState.SessionInitializing:
                    SetCompleteState();
                    Debug.Log("Supported, apk installed. SessionInitializing...");
                    break;
            
                case ARSessionState.SessionTracking:
                    SetCompleteState();
                    Debug.Log("Supported, apk installed. SessionTracking...");
                    break;
            
                default:
                    SetFailedState();
                    Debug.Log("Unsupported, Device Not Capable");
                    break;
            }
        }

        private void SetLoadingState()
        {
            State = ARValidationState.Loading;
            Loading?.Invoke();
        }
    
        private void SetCompleteState()
        {
            State = ARValidationState.Completed;
            Completed?.Invoke();
        }
    
        private void SetFailedState()
        {
            State = ARValidationState.Failed;
            Failed?.Invoke();
        }

        private IEnumerator InstallARCoreApp()
        {
            yield return ARSession.Install();
        }
    }

    public enum ARValidationState
    {
        None,
        Loading,
        Completed,
        Failed
    }
}