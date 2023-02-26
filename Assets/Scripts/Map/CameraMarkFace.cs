using System;
using Map.Hiders;
using UnityEditor;
using UnityEngine;

namespace Map
{
    [RequireComponent(typeof(CameraFaceSwitcher))]
    public partial class CameraMarkFace : CameraFace
    {
        [SerializeField] private bool _isRotateToCamera = true;
        [SerializeField] private bool _isClampScaling;
        [SerializeField] private Hider _hider;
        [SerializeField] private int _stopScalingHeight;
        [SerializeField] private int _stopDisplayHeight;


        private void Update()
        {
            if (Preset.Camera == null)
                return;
        
            if (_isRotateToCamera)
                RotateToCamera();
        
            UpdateScale();
        }
        
    
        protected override void UpdateScale()
        {
            var scale = CalculateScale();
            transform.localScale = new Vector3(scale, scale, scale);
        }
        
        protected override float GetPerspectiveCameraHeight()
        {
            float distanceToCamera = Vector3.Distance(Preset.Camera.transform.position, transform.position);
            return distanceToCamera * Mathf.Tan(Mathf.Deg2Rad * (Preset.Camera.fieldOfView * 0.5f));
        }

        protected override float CalculateCameraHeight()
        {
            float height = base.CalculateCameraHeight();
            return HandleCameraHeight(height);
        }
        
        private float HandleCameraHeight(float height)
        {
            if (_isClampScaling)
            {
                if (height > _stopDisplayHeight)
                    _hider.Hide();
                else
                    _hider.Show();
                
                height = ClampCameraHeight(height);
            }

            return height;
        }

        private float ClampCameraHeight(float height)
        {
            return Mathf.Min(height, _stopScalingHeight);
        }

        private void RotateToCamera()
        {
            var cameraRotation = Preset.Camera.transform.rotation;
            transform.LookAt(transform.position + cameraRotation * Vector3.forward, cameraRotation * Vector3.up);
        }
    }

    public partial class CameraMarkFace
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(CameraMarkFace))]
        public class CameraFaceEditor : Editor
        {
            private CameraMarkFace _origin;

            private void OnEnable()
            {
                _origin = target as CameraMarkFace;
            }

            public override void OnInspectorGUI()
            {
                _origin._isRotateToCamera = GUILayout.Toggle(_origin._isRotateToCamera, "Should rotate to camera");
                _origin._isClampScaling = GUILayout.Toggle(_origin._isClampScaling, "Should clamp scaling");

                if (_origin._isClampScaling)
                {
                    GUILayout.Space(10);
                    _origin._hider = EditorGUILayout.ObjectField("Hider:", _origin._hider, typeof(Hider)) as Hider;
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Stop scaling height:");
                    GUILayout.Space(50);
                    _origin._stopScalingHeight = EditorGUILayout.IntSlider(_origin._stopScalingHeight,
                        MapCameraHandler.ZoomMin, MapCameraHandler.ZoomMax);
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Stop display height:");
                    GUILayout.Space(50);
                    _origin._stopDisplayHeight = EditorGUILayout.IntSlider(_origin._stopDisplayHeight,
                        MapCameraHandler.ZoomMin, MapCameraHandler.ZoomMax);
                    EditorGUILayout.EndHorizontal();
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}