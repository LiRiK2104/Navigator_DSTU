using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using Helpers;
using UnityEngine;

namespace UI.AR
{
    public class ContentAnimation : MonoBehaviour
    {
        [SerializeField] private MovableMask _phoneMask;
        [SerializeField] private List<Transform> _phonePathPoints = new List<Transform>();

        private Tween _tween;
        
        
        private void OnEnable()
        {
            PlayAnimation();
        }


        private void InitializeTween()
        {
            var duration = 8;
            var pathType = PathType.CatmullRom;
            var path = _phonePathPoints.Select(point => point.position).ToArray();
            
            _tween = _phoneMask.DOPath(path, duration, pathType);
            _tween.SetEase(Ease.Linear);
            _tween.SetLoops(-1);
        }

        private void PlayAnimation()
        {
            if (_tween == null)
                InitializeTween();
            else
                _tween.Restart();
        }
    }
}
