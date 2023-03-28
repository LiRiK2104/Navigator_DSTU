using System;
using System.Collections.Generic;
using System.Linq;
using Navigation;
using TargetsSystem.Points;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Search.Options
{
    public class OptionsList : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Option _optionPrefab;

        private Transform _content;
        private List<Option> _initializedOptions = new List<Option>();
    
        public delegate void OptionCallback(IOptionInfo optionInfo);


        private void Awake()
        {
            _scrollRect = GetComponentInChildren<ScrollRect>();
            _content = _scrollRect.content;
        }


        public void Initialize(List<IOptionInfo> optionInfos, OptionCallback callback)
        {
            _scrollRect = GetComponentInChildren<ScrollRect>();
            _content = _scrollRect.content;
        
            optionInfos = Sort(optionInfos);
            Add(optionInfos, callback);
        }

        public void Add(List<IOptionInfo> optionInfos, OptionCallback callback)
        {
            foreach (var optionInfo in optionInfos)
            {
                var optionObject = Instantiate(_optionPrefab, _content);
                optionObject.Initialize(optionInfo, callback);
                _initializedOptions.Add(optionObject);
            }
            
            HideScroll();
        }

        public void Filter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                foreach (var option in _initializedOptions)
                    option.gameObject.SetActive(true);
            
                _scrollRect.gameObject.SetActive(false);
                return;
            }

            var count = 0;
        
            foreach (var option in _initializedOptions)
            {
                if (option.HasInKeyWords(input))
                {
                    option.gameObject.SetActive(true);
                    count++;
                }
                else
                {
                    option.gameObject.SetActive(false);
                }
            }

            SetScrollActive(count > 0);
        }
        
        public bool Contains(string input)
        {
            return _initializedOptions.Any(option => option.HasKeyWord(input));
        }

        public void ActivateAllOptions()
        {
            foreach (var button in _initializedOptions)
                button.gameObject.SetActive(true);
        }
    
        public float GetActiveButtonsYLength()
        {
            var count = _content.transform.Cast<Transform>().Count(child => child.gameObject.activeSelf);
            var length = _optionPrefab.GetComponent<RectTransform>().sizeDelta.y * count;
            return length;
        }

        private void Clear()
        {
            foreach (var button in _initializedOptions)
                Destroy(button.gameObject);
        
            _initializedOptions.Clear();
        }

        private List<IOptionInfo> Sort(List<IOptionInfo> optionsInfo)
        {
            return optionsInfo.OrderByDescending(optionInfo =>
            {
                switch (optionInfo)
                {
                    case PointInfo pointInfo:
                        return 0;

                    case PointsGroup pointsGroup:
                        return 1;

                    default:
                        return 0;
                }
            }).ToList();
        }
    
        private void SetScrollActive(bool status)
        {
            if (status)
                ShowScroll();
            else
                HideScroll();
        }
    
        public void HideScroll()
        {
            _scrollRect.gameObject.SetActive(false);    
        }
    
        private void ShowScroll()
        {
            _scrollRect.gameObject.SetActive(true);    
        }
    
    }
}
