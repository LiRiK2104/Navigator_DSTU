using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UI.StateSystem;
using UI.StateSystem.Setters;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.SlidingPanel
{
    [RequireComponent(typeof(SlidingPanelStatesStorage))]
    public class SlidingPanelHandler : MonoBehaviour
    {
        private const int DefaultIndex = 1;
        private const int ShortDragMillisecondsLimit = 200;
        
        [SerializeField] private GameObject _content;
        [SerializeField] private Transform _panelTopPoint;
        [SerializeField] private List<Transform> _targetPoints = new List<Transform>();

        private Vector3 _offset;
        private TimeSpan _beginDragTime;
        private float _beginDragY;
        private int _index = DefaultIndex;
        private Transform _currentTargetPoint;
        private SlidingPanelStatesStorage _statesStorage;
        
        public SlidingPanelStatesStorage StatesStorage 
        {
            get
            {
                if (_statesStorage == null)
                    _statesStorage = GetComponent<SlidingPanelStatesStorage>();

                return _statesStorage;
            }
        }
        
        public List<Transform> TargetPoints => _targetPoints;
        private StateSetter StateSetter => Global.Instance.UISetterV2.MapView.StateSetter;


        public void Initialize(Action<Transform> callback)
        {
            _offset = _content.transform.position - _panelTopPoint.position;
            SwitchPosition(_targetPoints[DefaultIndex], callback, true);
        }
        
        public void SwitchPosition(int index, Action<Transform> callback, bool instantly = false)
        {
            if (index < 0 || index >= _targetPoints.Count) 
                return;

            SwitchPosition(_targetPoints[index], callback, instantly);
        }
        
        public void SwitchPosition(PointerEventData eventData, Action<Transform> callback, bool instantly = false)
        {
            if (TryGetShortSwipeTargetPoint(eventData, out var nearestTargetPoint) == false)
                nearestTargetPoint = GetNearestTargetPoint();

            SwitchPosition(nearestTargetPoint, callback, instantly);
        }

        public void SetBeginDrag(PointerEventData eventData)
        {
            _beginDragY = eventData.position.y;
            _beginDragTime = DateTime.Now.TimeOfDay;
        }

        private void SwitchPosition(Transform targetPoint, Action<Transform> callback = null, bool instantly = false)
        {
            callback ??= targetPoint =>
            {
                if (StatesStorage.TryGetState(targetPoint, out StateType stateType))
                    StateSetter.SetState(stateType);
            };
            
            _currentTargetPoint = targetPoint;
            StartCoroutine(MoveTo(targetPoint, callback, instantly));
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

        private IEnumerator MoveTo(Transform targetPoint, Action<Transform> callback, bool instantly = false)
        {
            if (instantly) 
                yield return InstantlyMoveTo(targetPoint);
            else
                yield return AnimatedMoveTo(targetPoint);
            
            callback?.Invoke(targetPoint);
        }

        private IEnumerator AnimatedMoveTo(Transform targetPoint)
        {
            float duration = 0.15f;
            bool isComplete = false;
            
            _content.transform.DOMove(targetPoint.position + _offset, duration).OnComplete(() => isComplete = true);

            yield return new WaitUntil(() => isComplete);
        }

        private IEnumerator InstantlyMoveTo(Transform targetPoint)
        {
            _content.transform.position = targetPoint.position + _offset;
            yield return null;
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
