using System;
using System.Collections;
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
        private List<Option> _createdOptions = new List<Option>();
        
        public delegate void OnOptionSelectedDel(IOptionInfo optionInfo);
        public event OnOptionSelectedDel OptionSelected;


        private void Awake()
        {
            _scrollRect = GetComponentInChildren<ScrollRect>();
            _content = _scrollRect.content;
        }


        public void Initialize(List<IOptionInfo> optionInfos, OnOptionSelectedDel callback, bool isStoryList = false)
        {
            _scrollRect = GetComponentInChildren<ScrollRect>();
            _content = _scrollRect.content;
        
            optionInfos = Sort(optionInfos);
            Add(optionInfos, callback, isStoryList);
        }

        public void Filter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                foreach (var option in _createdOptions)
                    option.gameObject.SetActive(true);
            
                _scrollRect.gameObject.SetActive(false);
                return;
            }

            var count = 0;
        
            foreach (var option in _createdOptions)
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
            return _createdOptions.Any(option => option.HasKeyWord(input));
        }

        public void ActivateAllOptions()
        {
            foreach (var button in _createdOptions)
                button.gameObject.SetActive(true);
        }
    
        public float GetActiveButtonsYLength()
        {
            var count = _content.transform.Cast<Transform>().Count(child => child.gameObject.activeSelf);
            var length = _optionPrefab.GetComponent<RectTransform>().sizeDelta.y * count;
            return length;
        }
        
        private IEnumerator SelectOption(IOptionInfo optionInfo)
        {
            float delay = 1;
            yield return new WaitForSeconds(delay);
            OptionSelected?.Invoke(optionInfo);
        }
        
        private void OnOptionClick(IOptionInfo optionInfo)
        {
            ActivateAllOptions();
            HideScroll();

            StopAllCoroutines();
            StartCoroutine(SelectOption(optionInfo));
        }
        
        private void Add(List<IOptionInfo> optionInfos, OnOptionSelectedDel callback, bool isStoryList)
        {
            Queue<Option> createdNotInitializedOptions = new Queue<Option>(_createdOptions);
            _createdOptions.Clear();

            foreach (var optionInfo in optionInfos)
            {
                var optionObject = createdNotInitializedOptions.Count > 0 ? 
                    createdNotInitializedOptions.Dequeue() : 
                    Instantiate(_optionPrefab, _content);

                callback += OnOptionClick;
                optionObject.Initialize(optionInfo, callback, isStoryList);
                _createdOptions.Add(optionObject);
            }
            
            HideScroll();
        }

        private void Clear()
        {
            foreach (var button in _createdOptions)
                Destroy(button.gameObject);
        
            _createdOptions.Clear();
        }

        private List<IOptionInfo> Sort(List<IOptionInfo> optionsInfo)
        {
            return optionsInfo.OrderByDescending(optionInfo =>
            {
                switch (optionInfo)
                {
                    case PointInfo:
                        return 0;

                    case PointsGroup:
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
