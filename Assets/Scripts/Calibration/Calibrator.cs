using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Menus;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Calibration
{
    public class Calibrator : MonoBehaviour
    {
        private bool _shouldCalibrate;

        public event Action CalibrationReset;
        public event Action Calibrated;
    
        public bool IsCalibrated { get; private set; } = true;
    
        private Button CalibrationButton => Global.Instance.UiSetter.CalibrationMenu.CalibrationButton;
        private List<Button> RecalibrationButtons => Global.Instance.UiSetter.TrackingMenu.RecalibrationButtons;
        private CalibrationMenu CalibrationMenu => Global.Instance.UiSetter.CalibrationMenu;
        private DataBase DataBase => Global.Instance.DataBase;
        private ARSession ArSession => Global.Instance.ArMain.Session;
        private ARSessionOrigin ArSessionOrigin => Global.Instance.ArMain.SessionOrigin;
        private ARCameraManager ArCameraManager => Global.Instance.ArMain.CameraManager;
        private ARTrackedImageManager ArTrackedImageManager => Global.Instance.ArMain.TrackedImageManager;
        private AREnvironment ArEnvironment => Global.Instance.ArEnvironment;

    

        private void OnEnable()
        {
            ArTrackedImageManager.trackedImagesChanged += StartCalibration;
            RecalibrationButtons.ForEach(button => button.onClick.AddListener(ResetCalibration));
            CalibrationButton.onClick.AddListener(SetShouldCalibrate);
        }

        private void OnDisable()
        {
            ArTrackedImageManager.trackedImagesChanged -= StartCalibration;
            RecalibrationButtons.ForEach(button => button.onClick.RemoveListener(ResetCalibration));
            CalibrationButton.onClick.RemoveListener(SetShouldCalibrate);
        }


        private void Calibrate(Anchor anchor)
        {
            if (IsCalibrated/* || _shouldCalibrate == false*/)
                return;
        
            UpdateOrigin(anchor);
            //UpdateMarkerLocation(anchor);
            UpdateEnvironmentLocation(anchor);
            ArEnvironment.gameObject.SetActive(true);
            
            IsCalibrated = true;
            _shouldCalibrate = false;
            Calibrated?.Invoke();
        }

        private void ResetCalibration()
        {
            ArSession.Reset();
            ArEnvironment.gameObject.SetActive(false);
            IsCalibrated = false;
            CalibrationReset?.Invoke();
        }

        private void SetShouldCalibrate()
        {
            if (IsCalibrated)
                return;

            StopCoroutine(WaitCalibration());
            StartCoroutine(WaitCalibration());
        }

        private IEnumerator WaitCalibration()
        {
            float time = 5f;
        
            CalibrationMenu.SetCalibrationState();
            _shouldCalibrate = true;
            yield return new WaitForSeconds(time);
            _shouldCalibrate = false;
            CalibrationMenu.SetPointingState();
        }

        private void StartCalibration(ARTrackedImagesChangedEventArgs args)
        {
            if (IsCalibrated/* || _shouldCalibrate == false*/)
                return;
        
            //Debug.Log("Calibration started!");

            if (DataBase.TryGetVirtualMarker(GetActiveMarkers(args), 
                    out TriadMarker triadMarker,
                    out ARTrackedImage image1st, 
                    out ARTrackedImage image2nd, 
                    out ARTrackedImage image3rd))
            {
                triadMarker.ApplyTransformation(image1st.transform, image2nd.transform, image3rd.transform);
                Calibrate(triadMarker.Triad.Anchor);
                Debug.Log("Calibration successfully!");
            }
            else
            {
                //Debug.Log("Calibration failed!");
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
}
