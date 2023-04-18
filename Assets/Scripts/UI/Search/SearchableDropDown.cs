using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Navigation;
using TMPro;
using UI.Search.Options;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace UI.Search
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

        private DataBase DataBase => Global.Instance.DataBase;

        public string InputFieldValue
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }
        
        public bool InputFieldIsActive
        {
            set
            {
                if (value)
                    _inputField.ActivateInputField();
                else 
                    _inputField.DeactivateInputField();
            }
        }
        
        
        public void Initialize(List<IOptionInfo> optionInfos)
        {
            _cleanButton.onClick.AddListener(Reset);
            _inputField.onValueChanged.AddListener(OnInputValueChange);

            _optionsList.Initialize(optionInfos, OnOptionClick);
        }
        
        public void Reset()
        {
            ResetDropDown();
            _optionsList.HideScroll();
            _cleanButton.gameObject.SetActive(false);
        }

        private List<IOptionInfo> GetOptionInfos()
        {
            var optionInfos = DataBase.GetAllPoints().Select(point =>
            {
                DataBase.TryGetPointInfo(point, out PointInfo pointInfo);
                return pointInfo as IOptionInfo;
            }).ToList();

            optionInfos.AddRange(DataBase.PointsGroups.Select(group => group as IOptionInfo).ToArray());

            return optionInfos;
        }

        private void ResetDropDown()
        {
            _inputField.text = string.Empty;
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
        }
    }
}
