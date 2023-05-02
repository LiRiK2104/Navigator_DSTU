using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UI.SlidingPanel;
using UI.StateSystem;
using UI.StateSystem.Setters;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map
{
    public class MapControl : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public const int ZoomMin = 10;
        public const int ZoomMax = 220;
        private const float ZoomSensitivity = 0.03f;

        private IEnumerator _animatedMoveCoroutine;
        
        private Vector3 _leftBordersCorner;
        private Vector3 _rightBordersCorner;
        private Vector3 _bottomBordersCorner;
        private Vector3 _topBordersCorner;
        
        private Vector3 _leftScreenCorner;
        private Vector3 _rightScreenCorner;
        private Vector3 _bottomScreenCorner;
        private Vector3 _topScreenCorner;

        public event Action StartedDrag;
        
        private Camera Camera => Global.Instance?.CameraContainer.MapCamera;
        private BordersSetter BordersSetter => Global.Instance?.BordersSetter;
        private StateSetter StateSetter => Global.Instance.UISetterV2.MapView.StateSetter;
        private SlidingPanelHandler SlidingPanelHandler => Global.Instance.UISetterV2.MapView.SlidingPanelHandler;
    
    
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

            /*foreach (var point in cameraAnglesPoints)
                Gizmos.DrawSphere(point, radius);*/
            
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_leftBordersCorner, radius);
            Gizmos.DrawSphere(_leftScreenCorner, radius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_rightBordersCorner, radius);
            Gizmos.DrawSphere(_rightScreenCorner, radius);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_topBordersCorner, radius);
            Gizmos.DrawSphere(_topScreenCorner, radius);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_bottomBordersCorner, radius);
            Gizmos.DrawSphere(_bottomScreenCorner, radius);
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
        
        private void ClampCameraPositionV2(Vector2 delta)
        {
            var cameraCornersPoints = new []
            {
                Camera.ScreenToWorldPoint(Vector3.zero),
                Camera.ScreenToWorldPoint(new Vector3(0, Camera.pixelHeight, 0)),
                Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth, 0, 0)),
                Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth, Camera.pixelHeight, 0))
            };

            GetRectCornersPointsInWorldspace(cameraCornersPoints, 
                out Vector3 leftScreenCorner, 
                out Vector3 rightScreenCorner, 
                out Vector3 bottomScreenCorner, 
                out Vector3 topScreenCorner);
            
            _leftScreenCorner = leftScreenCorner;
            _rightScreenCorner = rightScreenCorner;
            _topScreenCorner = topScreenCorner;
            _bottomScreenCorner = bottomScreenCorner;
            
            float screenHalfWidth = Camera.pixelWidth / 2;
            float screenHalfHeight = Camera.pixelHeight / 2;
            Vector3 screenCenter = Camera.ScreenToWorldPoint(new Vector3(screenHalfWidth, screenHalfHeight, 0));
            Vector3 leftCornerCenterDifferent = screenCenter - leftScreenCorner;
            Vector3 rightCornerCenterDifferent = screenCenter - rightScreenCorner;
            Vector3 topCornerCenterDifferent = screenCenter - topScreenCorner;
            Vector3 bottomCornerCenterDifferent = screenCenter - bottomScreenCorner;

            GetRectCornersPointsInWorldspace(BordersSetter.Corners, 
                out Vector3 leftBordersCorner, 
                out Vector3 rightBordersCorner, 
                out Vector3 bottomBordersCorner, 
                out Vector3 topBordersCorner);

            _leftBordersCorner = leftBordersCorner;
            _rightBordersCorner = rightBordersCorner;
            _topBordersCorner = topBordersCorner;
            _bottomBordersCorner = bottomBordersCorner;

            float leftXBorder;
            float rightXBorder;
            float leftZBorder;
            float rightZBorder;
            
            float topZBorder;
            float bottomZBorder;
            
            //todo ClampCameraPosition v1
            if (IsRightAngleRect(leftBordersCorner, topBordersCorner, rightBordersCorner, bottomBordersCorner) ||
                IsRightAngleRect(leftScreenCorner, topScreenCorner, rightScreenCorner, bottomScreenCorner))
            {
                
            }

            
            /*leftScreenCorner.x = Mathf.Clamp(leftScreenCorner.x, leftBordersCorner.x, rightBordersCorner.x);
            leftScreenCorner.z = Mathf.Clamp(leftScreenCorner.z, bottomBordersCorner.z, topBordersCorner.z);
            rightScreenCorner.x = Mathf.Clamp(rightScreenCorner.x, leftBordersCorner.x, rightBordersCorner.x);
            rightScreenCorner.z = Mathf.Clamp(rightScreenCorner.x, bottomBordersCorner.z, topBordersCorner.z);
            bottomScreenCorner.x = Mathf.Clamp(bottomScreenCorner.x, leftBordersCorner.x, rightBordersCorner.x);
            bottomScreenCorner.z = Mathf.Clamp(bottomScreenCorner.z, bottomBordersCorner.z, topBordersCorner.z);
            topScreenCorner.x = Mathf.Clamp(topScreenCorner.x, leftBordersCorner.x, rightBordersCorner.x);
            topScreenCorner.z = Mathf.Clamp(topScreenCorner.z, bottomBordersCorner.z, topBordersCorner.z);*/

            // TODO:
            // Если угол камеры находится за границей, то нужно определить вектор, по которому двигалась камера.
            // Берем противоположный вектор и находим точку пересечения с границей.
            // По этому же вектору определяем противоположную точку.

            Vector3 newCameraPosition = Camera.transform.position;

            var cameraYRotation = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
            var deltaXZ = new Vector3(delta.x, 0, delta.y);
            var rotatedDelta = cameraYRotation * deltaXZ;
            Debug.DrawLine(leftScreenCorner, leftScreenCorner + rotatedDelta, Color.blue);

            var leftTopBorder = new StraightLineXZ(leftBordersCorner, topBordersCorner);
            var leftBottomBorder = new StraightLineXZ(leftBordersCorner, bottomBordersCorner);
            var rightTopBorder = new StraightLineXZ(rightBordersCorner, topBordersCorner);
            var rightBottomBorder = new StraightLineXZ(rightBordersCorner, bottomBordersCorner);
            
            newCameraPosition = ClampScreenCorner(newCameraPosition, leftScreenCorner, leftCornerCenterDifferent, rotatedDelta,
                leftTopBorder, leftBottomBorder, Dimension.X, ClampType.Max);
            
            newCameraPosition = ClampScreenCorner(newCameraPosition, rightScreenCorner, rightCornerCenterDifferent, rotatedDelta,
                rightTopBorder, rightBottomBorder, Dimension.X, ClampType.Min);
            
            newCameraPosition = ClampScreenCorner(newCameraPosition, bottomScreenCorner, bottomCornerCenterDifferent, rotatedDelta,
                leftBottomBorder, rightBottomBorder, Dimension.Z, ClampType.Max);
            
            newCameraPosition = ClampScreenCorner(newCameraPosition, topScreenCorner, topCornerCenterDifferent, rotatedDelta,
                leftTopBorder, rightTopBorder, Dimension.Z, ClampType.Min);

            
            //Debug.DrawLine(leftScreenCorner, new Vector3(leftXBorder, leftScreenCorner.y, leftScreenCorner.z), Color.blue);
            
            /*var rightTopXBorder = GetClampedXInStraightLine(rightBordersCorner, topBordersCorner, rightScreenCorner.z, Side.Below);
            var rightBottomXBorder = GetClampedXInStraightLine(rightBordersCorner, bottomBordersCorner, rightScreenCorner.z, Side.Higher);
            rightXBorder = Mathf.Min(rightTopXBorder, rightBottomXBorder);
            Debug.DrawLine(rightScreenCorner, new Vector3(rightXBorder, rightScreenCorner.y, rightScreenCorner.z), Color.blue);*/
            
            /*var leftTopZBorder = GetClampedZInStraightLine(leftBordersCorner, topBordersCorner, leftScreenCorner.x, Side.Below);
            var leftBottomZBorder = GetClampedZInStraightLine(leftBordersCorner, bottomBordersCorner, leftScreenCorner.x, Side.Higher);
            leftZBorder = Mathf.Max(leftTopZBorder, leftBottomZBorder);

            var rightTopZBorder = GetClampedZInStraightLine(rightBordersCorner, topBordersCorner, rightScreenCorner.z, Side.Below);
            var rightBottomZBorder = GetClampedZInStraightLine(rightBordersCorner, bottomBordersCorner, rightScreenCorner.z, Side.Higher);
            rightZBorder = Mathf.Min(rightTopZBorder, rightBottomZBorder);*/

            /*newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, 
                leftXBorder + leftCornerCenterDifferent.x, 
                rightXBorder + rightCornerCenterDifferent.x);*/
            
            // newCameraPosition.z = Mathf.Clamp(newCameraPosition.z, 
            //     leftZBorder + leftCornerCenterDifferent.z, 
            //     rightZBorder + rightCornerCenterDifferent.z);
            
            
            /*var leftTopZBorder = GetClampedZInStraightLine(leftBordersCorner, topBordersCorner, topScreenCorner.x, Side.Right);
            var rightTopZBorder = GetClampedZInStraightLine(rightBordersCorner, topBordersCorner, topScreenCorner.x, Side.Left);
            topZBorder = Mathf.Min(leftTopZBorder, rightTopZBorder);
            Debug.DrawLine(topScreenCorner, new Vector3(topScreenCorner.x, topScreenCorner.y, topZBorder), Color.blue);
            
            var leftBottomZBorder = GetClampedZInStraightLine(leftBordersCorner, bottomBordersCorner, bottomScreenCorner.x, Side.Right);
            var rightBottomZBorder = GetClampedZInStraightLine(rightBordersCorner, bottomBordersCorner, bottomScreenCorner.x, Side.Left);
            bottomZBorder = Mathf.Max(leftBottomZBorder, rightBottomZBorder);
            Debug.DrawLine(bottomScreenCorner, new Vector3(bottomScreenCorner.x, bottomScreenCorner.y, bottomZBorder), Color.blue);
            
            newCameraPosition.z = Mathf.Clamp(newCameraPosition.z, 
                bottomZBorder + bottomCornerCenterDifferent.z, 
                topZBorder + topCornerCenterDifferent.z);*/

            Camera.transform.position = newCameraPosition;
        }
        
        private Vector3 ClampScreenCorner(
            Vector3 cameraPosition, 
            Vector3 screenCorner, 
            Vector3 cornerCenterDifferent,
            Vector3 delta,
            StraightLineXZ borderlineA,
            StraightLineXZ borderlineB, 
            Dimension dimension,
            ClampType clampType)
        {
            float dimensionValue = 0;

            switch (dimension)
            {
                case Dimension.Z:
                    dimensionValue = screenCorner.x;
                    break;
                
                case Dimension.X:
                    dimensionValue = screenCorner.z;
                    break;
            }
            
            GetDimensionBorder(borderlineA, borderlineB, dimension, dimensionValue, clampType, 
                out float dimensionBorder, 
                out StraightLineXZ nearestLine);

            if ((dimension == Dimension.X && 
                ((clampType == ClampType.Min && screenCorner.x > dimensionBorder) || 
                (clampType == ClampType.Max && screenCorner.x < dimensionBorder))) ||
                (dimension == Dimension.Z && 
                 ((clampType == ClampType.Min && screenCorner.z > dimensionBorder) || 
                 (clampType == ClampType.Max && screenCorner.z < dimensionBorder))))
            {
                cameraPosition = ClampScreenCorner(cameraPosition, screenCorner, cornerCenterDifferent, nearestLine, delta);
            }
            
            return cameraPosition;
        }
        
        private Vector3 ClampScreenCorner(
            Vector3 cameraPosition, 
            Vector3 screenCorner, 
            Vector3 cornerCenterDifferent, 
            StraightLineXZ nearestBorder, 
            Vector3 delta)
        {
            var deltaLine = new StraightLineXZ(screenCorner, screenCorner + delta);
            StraightLineXZ.IntersectLineSegments2D(nearestBorder, deltaLine, out Vector3 intersection);
            cameraPosition.x = intersection.x + cornerCenterDifferent.x;
            cameraPosition.z = intersection.z + cornerCenterDifferent.z;

            return cameraPosition;
        }

        private void GetDimensionBorder(
            StraightLineXZ borderlineA,
            StraightLineXZ borderlineB, 
            Dimension dimension,
            float dimensionValue, 
            ClampType clampType, 
            out float dimensionBorder, 
            out StraightLineXZ nearestLine)
        {
            float borderA = 0;
            float borderB = 0;
            
            switch (dimension)
            {
                case Dimension.X:
                    borderA = borderlineA.GetX(dimensionValue);
                    borderB = borderlineB.GetX(dimensionValue);
                    break;
                
                case Dimension.Z:
                    borderA = borderlineA.GetZ(dimensionValue);
                    borderB = borderlineB.GetZ(dimensionValue);
                    break;
            }
            
            GetNearestBorderAndLine(borderlineA, borderlineB, borderA, borderB, clampType, out dimensionBorder, out nearestLine);
        }

        private void GetNearestBorderAndLine(
            StraightLineXZ linaA, 
            StraightLineXZ lineB, 
            float borderA, 
            float borderB, 
            ClampType clampType, 
            out float border, 
            out StraightLineXZ nearestLine)
        {
            border = 0;
            
            switch (clampType)
            {
                case ClampType.Min:
                    border = Mathf.Min(borderA, borderB);
                    break;
                
                case ClampType.Max:
                    border = Mathf.Max(borderA, borderB);
                    break;
                
                case ClampType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(clampType), clampType, null);
            }
            
            nearestLine = border == borderA ? linaA : lineB;
        }

        private bool IsRightAngleRect(Vector3 leftCorner, Vector3 topCorner, Vector3 rightCorner, Vector3 bottomCorner)
        {
            return leftCorner.z == topCorner.z ||
                   topCorner.x == rightCorner.x ||
                   rightCorner.z == bottomCorner.z ||
                   bottomCorner.x == leftCorner.x;
        }

        private void GetRectCornersPointsInWorldspace(
            Vector3[] corners, 
            out Vector3 leftCorner, 
            out Vector3 rightCorner, 
            out Vector3 bottomCorner, 
            out Vector3 topCorner)
        {
            const int rectCornersCount = 4;

            leftCorner = Vector3.zero; 
            rightCorner = Vector3.zero;
            bottomCorner = Vector3.zero;
            topCorner = Vector3.zero;
            
            if (corners.Length != rectCornersCount)
                return;
            
            var sortedByX = corners.OrderBy(point => point.x);
            var sortedByZ = corners.OrderBy(point => point.z);

            leftCorner = sortedByX.First();
            rightCorner = sortedByX.Last();
            bottomCorner = sortedByZ.First();
            topCorner = sortedByZ.Last();
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
