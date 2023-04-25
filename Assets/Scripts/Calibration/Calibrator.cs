using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Calibration
{
    public class Calibrator : MonoBehaviour
    {
        private TriadMarker _triadMarker;
        private IEnumerator _calibrationRoutine;
        
        public event Action Started;
        public event Action Completed;
        public event Action Failed;
        
    
        public CalibrationState State { get; private set; } = CalibrationState.None;
        private DataBase DataBase => Global.Instance.DataBase;
        private ARMain ARMain => Global.Instance.ArMain;
        private ARSession ArSession => ARMain.Session;
        private ARSessionOrigin ArSessionOrigin => ARMain.SessionOrigin;
        private ARCameraManager ArCameraManager => ARMain.CameraManager;
        private ARTrackedImageManager ArTrackedImageManager => ARMain.TrackedImageManager;
        private AREnvironment ArEnvironment => Global.Instance.ArEnvironment;
        

        private void OnEnable()
        {
            ArTrackedImageManager.trackedImagesChanged += FindTriadMarker;
            ARMain.Exited += StopCalibration;
        }

        private void OnDisable()
        {
            ArTrackedImageManager.trackedImagesChanged -= FindTriadMarker;
            ARMain.Exited -= StopCalibration;
        }

        public void StartCalibration()
        {
            StartCoroutine(GetPreparedCalibrationRoutine());
        }

        public void StopCalibration()
        {
            if (_calibrationRoutine == null)
                return;
            
            StopCoroutine(_calibrationRoutine);
            
            State = CalibrationState.Failed;
            Debug.Log("Calibration: Failed.");
            Failed?.Invoke();
        }
        
        public IEnumerator GetPreparedCalibrationRoutine()
        {
            ResetCalibration();
            _calibrationRoutine = CalibrateRoutine();

            return _calibrationRoutine;
        }

        private void ResetCalibration()
        {
            ArSession.Reset();
            ArEnvironment.gameObject.SetActive(false);
            State = CalibrationState.None;
            _calibrationRoutine = null;
        }

        private IEnumerator CalibrateRoutine()
        {
            if (State == CalibrationState.Completed)
                yield break;

            Started?.Invoke();
            State = CalibrationState.MarkersSearch;
            Debug.Log("Calibration: Started.");

            while (State != CalibrationState.Completed &&
                   State != CalibrationState.Failed)
            {
                if (State == CalibrationState.MarkerFound)
                    Calibrate(_triadMarker.Triad.Anchor);

                yield return null;
            }
        }

        private void Calibrate(Anchor anchor)
        {
            UpdateOrigin(anchor);
            UpdateEnvironmentLocation(anchor);
            ArEnvironment.gameObject.SetActive(true);
            
            State = CalibrationState.Completed;
            Debug.Log("Calibration: Completed.");
            Completed?.Invoke();
        }
        
        private void FindTriadMarker(ARTrackedImagesChangedEventArgs args)
        {
            if (State != CalibrationState.MarkersSearch)
                return;
            
            if (DataBase.TryGetVirtualMarker(GetActiveMarkers(args), 
                    out TriadMarker triadMarker,
                    out ARTrackedImage image1st, 
                    out ARTrackedImage image2nd, 
                    out ARTrackedImage image3rd))
            {
                triadMarker.ApplyTransformation(image1st.transform, image2nd.transform, image3rd.transform);
                _triadMarker = triadMarker;
                State = CalibrationState.MarkerFound;

                Debug.Log("Calibration: TriadMarker found.");
            }
        }

        private void UpdateOrigin(Anchor anchor)
        {
            var cameraTransform = ArCameraManager.transform;
            ArSessionOrigin.transform.position = anchor.transform.position;
        
            ArCameraManager.transform.position = cameraTransform.position;
            ArCameraManager.transform.rotation = cameraTransform.rotation;
        }

        private void UpdateEnvironmentLocation(Anchor anchor)
        {
            var targetRotation = anchor.transform.rotation.eulerAngles;
            anchor.transform.rotation = Quaternion.Euler(new Vector3(0, targetRotation.y, 0));
            ArEnvironment.transform.rotation = anchor.transform.rotation * anchor.RelativeRotation;
            ArEnvironment.transform.position = anchor.transform.TransformPoint(anchor.RelativePosition);
        }

        private List<ARTrackedImage> GetActiveMarkers(ARTrackedImagesChangedEventArgs args)
        {
            return args.added.Concat(args.updated).ToList();
        }
    }

    public enum CalibrationState
    {
        None,
        MarkersSearch,
        MarkerFound,
        Completed,
        Failed
    }
}
