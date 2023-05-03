using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Calibration;
using Helpers.Tests;
using UI.SlidingPanel;
using UI.StateSystem;
using UI.StateSystem.Setters;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map
{
    public class MapControl : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        public const int ZoomMin = 10;
        public const int ZoomMax = 140;
        private const float ZoomSensitivity = 0.03f;

        private IEnumerator _animatedMoveCoroutine;
        private Vector3 _relativePosition;
        private Quaternion _relativeRotation;

        public event Action StartedDrag;
        
        private Camera Camera => Global.Instance?.CameraContainer.MapCamera;
        private BordersSetter BordersSetter => Global.Instance?.BordersSetter;
        private StateSetter StateSetter => Global.Instance.UISetterV2.MapView.StateSetter;
        private Calibrator Calibrator => Global.Instance.ArMain.Calibrator;
        private SlidingPanelHandler SlidingPanelHandler => Global.Instance.UISetterV2.MapView.SlidingPanelHandler;

        private void Awake()
        {
            SetRelativePositionRotation();
        }

        private void OnEnable()
        {
            SynchronizeTransform();
            Calibrator.Completed += SynchronizeTransform;
        }

        private void OnDisable()
        {
            Calibrator.Completed -= SynchronizeTransform;
        }

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

            int radius = 5;

            foreach (var point in cameraAnglesPoints)
                Gizmos.DrawSphere(point, radius);
        }
    
        public void OnPointerDown(PointerEventData eventData)
        {
            StartedDrag?.Invoke();
            StopAnimatedMove();
            SetMapViewPosition();
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
        
        public void OnEndDrag(PointerEventData eventData)
        {
            SetRelativePositionRotation();
        }


        public void GoToTarget(Transform target, bool needSetRotation, bool instantly, params Action[] callbacks)
        {
            StartCoroutine(GoToTargetRoutine(target, needSetRotation, instantly, callbacks));
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

        private void SetRelativePositionRotation()
        {
            _relativePosition = Camera.transform.position - BordersSetter.transform.position;
            _relativeRotation = Quaternion.Inverse(BordersSetter.transform.rotation) * Camera.transform.rotation;
        }

        private void SynchronizeTransform()
        {
            Camera.transform.position = _relativePosition + BordersSetter.transform.position;
            Camera.transform.rotation = BordersSetter.transform.rotation * _relativeRotation;
            ClampCameraPosition();
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

        private IEnumerator GoToTargetRoutine(Transform target, bool needSetRotation, bool instantly, params Action[] callbacks)
        {
            if (instantly)
            {
                yield return GoToTargetInstantly(target, needSetRotation);
            }
            else
            {
                _animatedMoveCoroutine = GoToTargetAnimated(target, needSetRotation);
                yield return _animatedMoveCoroutine;
            }

            foreach (var callback in callbacks)
                callback?.Invoke();
        }
        
        private IEnumerator GoToTargetInstantly(Transform target, bool needSetRotation)
        {
            Camera.transform.position = GetTargetPosition(target);
            
            if (needSetRotation)
                Camera.transform.rotation = GetTargetRotation(target);
            
            yield return null;
        }

        private IEnumerator GoToTargetAnimated(Transform target, bool needSetRotation)
        {
            float speed = 2;
            float minDistance = 0.1f;
            float distance;

            do
            {
                Vector3 targetPosition = GetTargetPosition(target);
                Camera.transform.position = Vector3.Lerp(Camera.transform.position, targetPosition, Time.deltaTime * speed);

                if (needSetRotation)
                {
                    Quaternion transformRotation = GetTargetRotation(target);
                    Camera.transform.rotation = Quaternion.Lerp(Camera.transform.rotation, transformRotation, Time.deltaTime * speed);   
                }

                distance = Vector3.Distance(Camera.transform.position, targetPosition);
                yield return null;
            } 
            while (distance > minDistance);
        }

        private void StopAnimatedMove()
        {
            if (_animatedMoveCoroutine == null)
                return;
            
            StopCoroutine(_animatedMoveCoroutine);
            _animatedMoveCoroutine = null;
        }

        private Vector3 GetTargetPosition(Transform target)
        {
            return new Vector3(target.position.x, Camera.transform.position.y, target.position.z);
        }
        
        private Quaternion GetTargetRotation(Transform target)
        {
            return Quaternion.Euler(0, target.rotation.eulerAngles.y, 0) * Quaternion.Euler(90, 0, 0);
        }

        private void SetMapViewPosition()
        {
            if (StateSetter.CurrentState != StateType.Default) 
                return;
            
            int bottomIndex = 0;
            Action<Transform> callback = point =>
            {
                if (SlidingPanelHandler.StatesStorage.TryGetState(point, out StateType stateType) &&
                    stateType == StateType.MapView)
                {
                    StateSetter.SetState(StateType.MapView);
                }
            };

            SlidingPanelHandler.SwitchPosition(bottomIndex, callback);
        }
    }

    public struct StraightLineXZ
    {
        public readonly Vector3 PointA;
        public readonly Vector3 PointB;

        public StraightLineXZ(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }

        public static bool IntersectLineSegments2D(StraightLineXZ firstLine, StraightLineXZ secondLine, out Vector3 intersection)
        {
            // Consider:
            //   p1start = p
            //   p1end = p + r
            //   p2start = q
            //   p2end = q + s
            // We want to find the intersection point where :
            //  p + t*r == q + u*s
            // So we need to solve for t and u
            var p = firstLine.PointA;
            var r = firstLine.PointB - firstLine.PointA;
            var q = secondLine.PointA;
            var s = secondLine.PointB - secondLine.PointA;
            var qminusp = q - p;

            float cross_rs = CrossProduct2D(r, s);

            if (Approximately(cross_rs, 0f))
            {
                // Parallel lines
                if (Approximately(CrossProduct2D(qminusp, r), 0f))
                {
                    // Co-linear lines, could overlap
                    float rdotr = Vector3.Dot(r, r);
                    float sdotr = Vector3.Dot(s, r);
                    // this means lines are co-linear
                    // they may or may not be overlapping
                    float t0 = Vector3.Dot(qminusp, r / rdotr);
                    float t1 = t0 + sdotr / rdotr;

                    if (sdotr < 0)
                    {
                        // lines were facing in different directions so t1 > t0, swap to simplify check
                        Swap(ref t0, ref t1);
                    }

                    if (t0 <= 1 && t1 >= 0)
                    {
                        // Nice half-way point intersection
                        float t = Mathf.Lerp(Mathf.Max(0, t0), Mathf.Min(1, t1), 0.5f);
                        intersection = p + t * r;
                        return true;
                    }
                    else
                    {
                        // Co-linear but disjoint
                        intersection = Vector3.zero;
                        return false;
                    }
                }
                else
                {
                    // Just parallel in different places, cannot intersect
                    intersection = Vector3.zero;
                    return false;
                }
            }
            else
            {
                // Not parallel, calculate t and u
                float t = CrossProduct2D(qminusp, s) / cross_rs;
                float u = CrossProduct2D(qminusp, r) / cross_rs;
                
                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    intersection = p + t * r;
                    return true;
                }
                else
                {
                    // Lines only cross outside segment range
                    intersection = Vector2.zero;
                    return false;
                }
            }
        }

        public float GetX(float z)
        {
            float x = (z - PointA.z) / (PointB.z - PointA.z) * (PointB.x - PointA.x) + PointA.x;
            return x;
        }

        public float GetZ(float x)
        {
            float z = (x - PointA.x) / (PointB.x - PointA.x) * (PointB.z - PointA.z) + PointA.z;
            return z;
        }
        
        private static bool Approximately(float a, float b, float tolerance = 1e-5f) 
        {
            return Mathf.Abs(a - b) <= tolerance;
        }

        private static float CrossProduct2D(Vector3 a, Vector3 b) 
        {
            return a.x * b.z - b.x * a.z;
        }
        
        private static void Swap<T>(ref T lhs, ref T rhs) 
        {
            (lhs, rhs) = (rhs, lhs);
        }
    }

    public enum ClampType
    {
        None,
        Min,
        Max
    }
    
    public enum Dimension
    {
        X,
        Z
    }
}
