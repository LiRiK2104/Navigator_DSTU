using System;
using System.Collections.Generic;
using Map.Hiders;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Map
{
    [RequireComponent(typeof(CameraFaceSwitcher))]
    public partial class CameraSignFace : CameraFace
    {
        [SerializeField] private bool _isRotateToCamera = true;
        [SerializeField] private bool _isClampScaling;
        [SerializeField] private List<Hider> _hiders = new List<Hider>();
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
                foreach (var hider in _hiders)
                {
                    if (height > _stopDisplayHeight)
                        hider.Hide();
                    else
                        hider.Show();   
                }

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

    public partial class CameraSignFace
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(CameraSignFace))]
        public class CameraFaceEditor : Editor
        {
            private CameraSignFace _origin;

            private void OnEnable()
            {
                _origin = target as CameraSignFace;
            }

            public override void OnInspectorGUI()
            {
                _origin._isRotateToCamera = GUILayout.Toggle(_origin._isRotateToCamera, "Should rotate to camera");
                _origin._isClampScaling = GUILayout.Toggle(_origin._isClampScaling, "Should clamp scaling");

                if (_origin._isClampScaling)
                {
                    GUILayout.Space(10);
                    
                    var hidersProperty = serializedObject.FindProperty(nameof(_origin._hiders));
                    EditorGUILayout.PropertyField(hidersProperty, true);

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