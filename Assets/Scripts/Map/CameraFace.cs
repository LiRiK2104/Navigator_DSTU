using UnityEngine;

namespace Map
{
    public abstract class CameraFace : MonoBehaviour
    {
        protected CameraFacePreset Preset;
        
        protected abstract void UpdateScale();

        protected abstract float GetPerspectiveCameraHeight();
        
        public void SetPreset(CameraFacePreset preset)
        {
            Preset = preset;
        }
        
        protected virtual float CalculateCameraHeight()
        {
            return Preset.Camera.orthographic ? Preset.Camera.orthographicSize : GetPerspectiveCameraHeight();
        }
        
        protected float CalculateScale()
        {
            if (Preset.Camera == null)
                return 1;

            float dividend = 10040;
            float scale = (CalculateCameraHeight() / dividend) * Preset.ScaleFactor;
            return scale;
        }
    }
}
