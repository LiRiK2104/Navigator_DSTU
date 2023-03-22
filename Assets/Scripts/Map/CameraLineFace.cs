using UnityEngine;

namespace Map
{
    [RequireComponent(typeof(CameraFaceSwitcher))]
    public class CameraLineFace : CameraFace
    {
        private const float ScaleMultiplier = 15;
        private const float DefaultTilingX = 0.2f;
        private const float TilingCoefficient = 6.5f;
    
        [SerializeField] private LineRenderer _lineRenderer;
    
        private void Update()
        {
            UpdateScale();
            UpdateTiling();
        }

        protected override void UpdateScale()
        {
            var scale = CalculateScale();
            _lineRenderer.widthMultiplier = scale * ScaleMultiplier;
        }

        protected override float GetPerspectiveCameraHeight()
        {
            float distanceToCamera = 25;
            return distanceToCamera * Mathf.Tan(Mathf.Deg2Rad * (Preset.Camera.fieldOfView * 0.5f));
        }

        private void UpdateTiling()
        {
            var tilingX = Preset.Camera.orthographic
                ? TilingCoefficient / Preset.Camera.orthographicSize
                : DefaultTilingX;

            int tilingY = 1;
            _lineRenderer.material.mainTextureScale = new Vector2(tilingX, tilingY);
        }
    }
}
