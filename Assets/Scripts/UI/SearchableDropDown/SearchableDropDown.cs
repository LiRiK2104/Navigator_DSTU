using System;
using System.Collections;
using System.Collections.Generic;
using UI.SearchableDropDown.Options;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace UI.SearchableDropDown
{
    public class SearchableDropDown : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private OptionsList _optionsList;
        [SerializeField] private Button _cleanButton;

        public delegate void OnValueChangedDel(string input);
        public delegate void OnOptionSelectedDel(IOptionInfo optionInfo);
    
        public event OnOptionSelectedDel OptionSelected;
        public event OnValueChangedDel ValueChanged;
        public event OnValueChangedDel EndEditing;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _inputField.ActivateInputField();
            }
        }
    
    
        public void Initialize(List<IOptionInfo> optionInfos)
        {
            _cleanButton.onClick.AddListener(OnCleanButtonClick);
            _inputField.onValueChanged.AddListener(OnInputValueChange);
            _inputField.onEndEdit.AddListener(OnEndEditing);
        
            _optionsList.Initialize(optionInfos, OnOptionClick);
        }
    
        public string GetValue()
        {
            return _inputField.text;
        }

        public void ActivateInputField()
        {
            _inputField.ActivateInputField();
        }

        private void ResetDropDown()
        {
            _inputField.text = string.Empty;
        }

        private void OnEndEditing(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                Debug.Log("no value entered ");
                return;
            }
        
            StartCoroutine(CheckIfValidInput(arg));
        }
    
        private IEnumerator CheckIfValidInput(string arg)
        {
            const int waitTime = 1;
            yield return new WaitForSeconds(waitTime);

            if (_optionsList.Contains(arg) == false)
                _inputField.text = String.Empty;
        
            EndEditing?.Invoke(_inputField.text);
        }
        
        
        private IEnumerator SelectOption(IOptionInfo optionInfo)
        {
            float delay = 1;
            yield return new WaitForSeconds(delay);
            OptionSelected?.Invoke(optionInfo);
        }
    
        private void OnInputValueChange(string arg0)
        {
            _cleanButton.gameObject.SetActive(arg0 != String.Empty);
        
            if (_optionsList.Contains(arg0) == false)
                _optionsList.Filter(arg0);
        
            ValueChanged?.Invoke(arg0);
        }
    
        private void OnOptionClick(IOptionInfo optionInfo)
        {
            _inputField.text = optionInfo.Name;
        
            _optionsList.ActivateAllOptions();
            _optionsList.HideScroll();

            StopAllCoroutines();
            StartCoroutine(SelectOption(optionInfo));
            StartCoroutine(CheckIfValidInput(optionInfo.Name));
        }

        private void OnCleanButtonClick()
        {
            ResetDropDown();
            _optionsList.HideScroll();
            _cleanButton.gameObject.SetActive(false);
        }
    }
}
