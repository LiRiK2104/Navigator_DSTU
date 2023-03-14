using System.Collections.Generic;
using System.Linq;
using Followers;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCameraHandler : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public const int ZoomMin = 10;
    public const int ZoomMax = 120;
    private const float ZoomSensitivity = 0.03f;
    
    [SerializeField] private FlyingFollower flyingFollower;

    private Camera Camera => Global.Instance?.CameraContainer.MapCamera;
    private BordersSetter BordersSetter => Global.Instance?.BordersSetter;
    
    
    private void OnDrawGizmos()
    {
        if (Camera == null)
            return;
        
        var cameraAnglesPoints = new List<Vector3>
        {
            Camera.ScreenToWorldPoint(Vector3.zero),
            Camera.ScreenToWorldPoint(new Vector3(0, Camera.pixelHeight, 0)),
            Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth, 0, 0)),
            Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth, Camera.pixelHeight, 0))
        };

        foreach (var point in cameraAnglesPoints)
            Gizmos.DrawSphere(point, 2);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        flyingFollower.StopFollow();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1)
            {
                float sensitivity = 2f;
                var touchA = Input.GetTouch(0);
                MoveCamera(touchA, sensitivity);
            }

            if (Input.touchCount == 2)
            {
                float sensitivity = 0.5f;
                var touchA = Input.GetTouch(0);
                var touchB = Input.GetTouch(1);
            
                MoveCamera(touchA, sensitivity);
                MoveCamera(touchB, sensitivity);
                Zoom(touchA, touchB);
                RotateCamera(touchA, touchB);
            }
        }

        ClampCameraPosition();
    }
    
    private void Zoom(Touch touchA, Touch touchB)
    {
        var distanceBetweenTouches = Vector2.Distance(touchA.position, touchB.position);
        var distanceBetweenPreviousTouches = Vector2.Distance(touchA.GetPreviousTouchPosition(), touchB.GetPreviousTouchPosition());
        var touchesDistanceDelta = distanceBetweenTouches - distanceBetweenPreviousTouches;
        var currentZoom = Camera.orthographicSize - touchesDistanceDelta * ZoomSensitivity;
        
        Camera.orthographicSize = Mathf.Clamp(currentZoom, ZoomMin, ZoomMax);
    }

    private void RotateCamera(Touch touchA, Touch touchB)
    {
        var touchBPreviousPosition = touchB.GetPreviousTouchPosition();
        
        if (touchBPreviousPosition != touchB.position)
        {
            var touchAPreviousPosition = touchA.GetPreviousTouchPosition();
            var angle = Vector2.SignedAngle(touchBPreviousPosition - touchAPreviousPosition, touchB.position - touchA.position);
            Camera.transform.RotateAround(Camera.transform.position, -Camera.transform.forward, angle);
        }
    }
    
    private void MoveCamera(Touch touch, float sensitivity)
    {
        var vector = ConVertVectorYToZ(ConvertToUnits(touch.deltaPosition));
        var rotatedVector = Quaternion.AngleAxis(Camera.transform.rotation.eulerAngles.y, -Camera.transform.forward) * vector;
        Camera.transform.position -= rotatedVector * sensitivity;
    }
    
    private float ConvertToUnits(float pixels)
    {
        return (pixels * Camera.orthographicSize) / Camera.pixelHeight;
    }
    
    private Vector2 ConvertToUnits(Vector2 vectorInPixels)
    {
        return new Vector2(ConvertToUnits(vectorInPixels.x), ConvertToUnits(vectorInPixels.y));
    }

    private Vector3 ConVertVectorYToZ(Vector3 vector3)
    {
        return new Vector3(vector3.x, 0, vector3.y);
    }
    
    private void ClampCameraPosition()
    {
        Vector3 cameraPosition = Camera.transform.position;
        Vector3 screenCenter = Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth / 2, Camera.pixelHeight / 2, 0));

        var cameraAnglesPoints = new List<Vector3>
        {
            Camera.ScreenToWorldPoint(Vector3.zero),
            Camera.ScreenToWorldPoint(new Vector3(0, Camera.pixelHeight, 0)),
            Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth, 0, 0)),
            Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth, Camera.pixelHeight, 0))
        };
        
        var sortedByX = cameraAnglesPoints.OrderBy(point => point.x);
        var sortedByZ = cameraAnglesPoints.OrderBy(point => point.z);

        Vector3 leftScreenPoint = sortedByX.First();
        Vector3 rightScreenPoint = sortedByX.Last();
        Vector3 bottomScreenPoint = sortedByZ.First();
        Vector3 topScreenPoint = sortedByZ.Last();

        var x = Mathf.Clamp(cameraPosition.x, 
            (new Vector3(BordersSetter.LeftBorder, cameraPosition.y, 0) + screenCenter - leftScreenPoint).x,
            (new Vector3(BordersSetter.RightBorder, cameraPosition.y, 0) + screenCenter - rightScreenPoint).x);

        var z = Mathf.Clamp(cameraPosition.z, 
            (new Vector3(0, cameraPosition.y, BordersSetter.BottomBorder) + screenCenter - bottomScreenPoint).z,
            (new Vector3(0, cameraPosition.y, BordersSetter.TopBorder) + screenCenter - topScreenPoint).z);
        
        Camera.transform.position = new Vector3(x, cameraPosition.y, z);
    }
}