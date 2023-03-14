using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class SearchableDropDown : MonoBehaviour
{
    [SerializeField] private Button _cleanButton;
    [SerializeField] private OptionsList _optionsList;
    [SerializeField] private TMP_InputField _inputField;

    public delegate void OnValueChangedDel(string val);
    public event OnValueChangedDel ValueChanged;

    
    public void Initialize(List<string> optionsNames)
    {
        _cleanButton.onClick.AddListener(OnCleanButtonClick);
        _inputField.onValueChanged.AddListener(OnInputValueChange);
        _inputField.onEndEdit.AddListener(OnEndEditing);
        
        _optionsList.Add(optionsNames, OnOptionSelected);
    }
    
    public string GetValue()
    {
        return _inputField.text;
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
        
        if (_optionsList.Names.Contains(arg) == false)
            _inputField.text = String.Empty;
        
        ValueChanged?.Invoke(_inputField.text);
    }
    
    private void OnInputValueChange(string arg0)
    {
        _cleanButton.gameObject.SetActive(arg0 != String.Empty);
        
        if (_optionsList.Names.Contains(arg0) == false)
            _optionsList.Filter(arg0);
    }
    
    private void OnOptionSelected(Button option)
    {
        _inputField.text = option.name;
        
        _optionsList.ActivateAllOptions();
        _optionsList.HideScroll();

        StopAllCoroutines();
        StartCoroutine(CheckIfValidInput(option.name));
    }

    private void OnCleanButtonClick()
    {
        ResetDropDown();
        _optionsList.HideScroll();
        _cleanButton.gameObject.SetActive(false);
    }
}
