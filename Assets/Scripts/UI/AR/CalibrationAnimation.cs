using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AR
{
    public class CalibrationAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _phoneImage;
        [SerializeField] private List<Transform> _phonePathPoints = new List<Transform>();
        [SerializeField] private List<Image> _circles = new List<Image>();

        private Tween _tween;
        

        private void OnEnable()
        {
            PlayAnimation();
        }


        private void InitializeTween()
        {
            var duration = 5;
            var pathType = PathType.CatmullRom;
            var path = _phonePathPoints.Select(point => point.position).ToArray();
            _tween = _phoneImage.DOPath(path, duration, pathType);
            
            _tween.SetLoops(-1);
            _tween.OnWaypointChange(ShowCircle);
        }
        
        private void PlayAnimation()
        {
            if (_tween == null)
                InitializeTween();
            else
                _tween.Restart();
        }

        private void ShowCircle(int index)
        {
            int indexOffset = 1;
            index -= indexOffset;

            if (index >= 0 && index < _circles.Count)
                _circles[index].enabled = true;
            else
                _circles.ForEach(circle => circle.enabled = false);
        }
    }
}
