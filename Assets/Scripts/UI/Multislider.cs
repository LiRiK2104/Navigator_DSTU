using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

namespace UI
{
    public class Multislider : MonoBehaviour
    {
        private int MinSlides = 1;
        private int MaxSlides = 10;
        
        [SerializeField] private Slider _sliderTemplate;

        private List<Slider> _sliders = new List<Slider>();
        private int _currentSliderIndex = 0;
        private IEnumerator _currentSliderRoutine;
        private IEnumerator _allSlidersRoutine;

        public event Action AllCompleted;
        public event Action<int> SliderStarted;

        public bool Active { get; private set; }
        public bool IsPaused { get; private set; }
        public int CurrentSliderIndex => _currentSliderIndex;


        public void Initialize(int slidesCount)
        {
            slidesCount = Mathf.Clamp(slidesCount, MinSlides, MaxSlides);

            for (int i = 0; i < slidesCount; i++)
            {
                var slider = Instantiate(_sliderTemplate, transform);
                _sliders.Add(slider);
            }
        }

        public void Play(int startIndex = 0)
        {
            if (IsPaused)
            {
                IsPaused = false;
            }
            else
            {
                Stop();
                Active = true;
                _allSlidersRoutine = WaitAllSliders(startIndex);
                StartCoroutine(_allSlidersRoutine);   
            }
        }

        public void ForceSetPrevious()
        {
            if (Active == false)
                return;
            
            int lastSliderIndex = _currentSliderIndex;
            Stop();
            _currentSliderIndex = lastSliderIndex - 1;
            Play(_currentSliderIndex);
        }
        
        public void ForceSetNext()
        {
            if (Active == false)
                return;
            
            int lastSliderIndex = _currentSliderIndex;
            Stop();
            int newSliderIndex = lastSliderIndex + 1;

            if (newSliderIndex >= _sliders.Count)
            {
                AllCompleted?.Invoke();
                return;
            }

            _currentSliderIndex = newSliderIndex;
            Play(_currentSliderIndex);
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Stop()
        {
            StopAllSlidersRoutine();
            StopSliderRoutine();
            Reset();
        }
        
        private void Reset()
        {
            Active = false;
            IsPaused = false;
            
            foreach (var slider in _sliders)
                slider.value = 0;

            _currentSliderIndex = 0;
            _currentSliderRoutine = null;
            _allSlidersRoutine = null;
        }

        private void StopAllSlidersRoutine()
        {
            if (_allSlidersRoutine != null) 
                StopCoroutine(_allSlidersRoutine);
        }
        
        private void StopSliderRoutine()
        {
            if (_currentSliderRoutine != null)
                StopCoroutine(_currentSliderRoutine);
        }

        private IEnumerator WaitAllSliders(int startIndex = 0)
        {
            if (SlidersIsNullOrEmpty())
                yield break;

            startIndex = Mathf.Clamp(startIndex, 0, _sliders.Count - 1);

            for (int i = 0; i < _sliders.Count; i++)
            {
                var slider = _sliders[i];
                
                if (i >= startIndex)
                {
                    _currentSliderIndex = i;
                    _currentSliderRoutine = WaitSlider(slider);
                    yield return _currentSliderRoutine;   
                }

                slider.value = 1;
            }
            
            Stop();
            AllCompleted?.Invoke();
        }

        private IEnumerator WaitSlider(Slider slider)
        {
            if (slider == null)
                yield break;

            SliderStarted?.Invoke(_currentSliderIndex);
            
            const float minTime = 0;
            const float maxTime = 5;
            float time = minTime;

            while (time < maxTime)
            {
                if (IsPaused == false)
                {
                    time += Time.deltaTime;
                    slider.value = Mathf.InverseLerp(minTime, maxTime, time);    
                }
                
                yield return null;
            }
        }
        
        private bool SlidersIsNullOrEmpty()
        {
            return _sliders == null || _sliders.Count == 0;
        }
    }
}
