using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class SlidingPanelHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        private const int DefaultIndex = 1;
        private const int ShortDragMillisecondsLimit = 100;
        
        [SerializeField] private GameObject _content;
        [SerializeField] private Transform _panelTopPoint;
        [SerializeField] private List<Transform> _targetPoints = new List<Transform>();

        private Vector3 _offset;
        private TimeSpan _beginDragTime;
        private float _beginDragY;
        private int _index = DefaultIndex;
        private Transform _currentTargetPoint;
        
        public List<Transform> TargetPoints => _targetPoints;

        public event Action<Transform> PositionChanged;

        
        private void Start()
        {
            _offset = _content.transform.position - _panelTopPoint.position;
            InstantlyMoveTo(_targetPoints[DefaultIndex]);
        }
        
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _beginDragY = eventData.position.y;
            _beginDragTime = DateTime.Now.TimeOfDay;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            SwitchPosition(eventData);
        }


        public void SwitchPosition(int index)
        {
            if (index >= 0 && index < _targetPoints.Count)
                MoveTo(_targetPoints[index]);
        }
        
        private void SwitchPosition(PointerEventData eventData)
        {
            if (TryGetShortSwipeTargetPoint(eventData, out var nearestTargetPoint) == false)
                nearestTargetPoint = GetNearestTargetPoint();

            _currentTargetPoint = nearestTargetPoint;
            MoveTo(nearestTargetPoint);
        }
        
        private bool TryGetShortSwipeTargetPoint(PointerEventData eventData, out Transform foundTargetPoint)
        {
            foundTargetPoint = null;
            
            TimeSpan dragTime = DateTime.Now.TimeOfDay - _beginDragTime;
            int direction = Math.Sign(eventData.position.y - _beginDragY);

            if (dragTime.Milliseconds < ShortDragMillisecondsLimit)
            {
                var nextIndex = Mathf.Clamp(GetIndex(_currentTargetPoint) + direction, 0, _targetPoints.Count - 1);
                foundTargetPoint = _targetPoints[nextIndex];
                return true;
            }

            return false;
        }

        private void MoveTo(Transform targetPoint)
        {
            float duration = 0.15f;
            _content.transform.DOMove(targetPoint.position + _offset, duration).OnComplete(() => InvokePositionChanged(targetPoint));
        }

        private void InstantlyMoveTo(Transform targetPoint)
        {
            _content.transform.position = targetPoint.position + _offset;
            InvokePositionChanged(targetPoint);
        }

        private void InvokePositionChanged(Transform targetPoint)
        {
            PositionChanged?.Invoke(targetPoint);
        }

        private Transform GetNearestTargetPoint()
        {
            return _targetPoints.OrderBy(targetPoint => Vector3.Distance(_panelTopPoint.position, targetPoint.position)).FirstOrDefault();
        }
        
        private int GetIndex(Transform targetPoint)
        {
            for (int i = 0; i < _targetPoints.Count; i++)
            {
                if (_targetPoints[i] == targetPoint)
                    return i;
            }

            return DefaultIndex;
        }
    }
}
