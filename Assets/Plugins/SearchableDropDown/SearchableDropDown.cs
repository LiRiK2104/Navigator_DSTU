using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class SearchableDropDown : MonoBehaviour
{
    [SerializeField] private Button _blockerButton;
    [SerializeField] private Button _buttonPrefab;
    [SerializeField] private int _maxScrollRectSize = 180;


    private Button _ddButton;
    private TMP_InputField _inputField;
    private ScrollRect _scrollRect;
    private Transform _content;
    private RectTransform _scrollRectTrans;
    private bool _isContentHidden = true;
    private List<Button> _initializedButtons = new List<Button>();
    private List<string> _optionsNames = new List<string>();

    public delegate void OnValueChangedDel(string val);
    public event OnValueChangedDel ValueChanged;
    

    /// <summary>
    /// Initilize all the Fields
    /// </summary>
    public void Initialize(List<string> optionsNames)
    {
        _ddButton = GetComponentInChildren<Button>();
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _inputField = GetComponentInChildren<TMP_InputField>();
        _scrollRectTrans = _scrollRect.GetComponent<RectTransform>();
        _content = _scrollRect.content;

        //blocker is a button added and scaled it to screen size so that we can close the dd on clicking outside
        _blockerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        _blockerButton.gameObject.SetActive(false);
        _blockerButton.transform.SetParent(GetComponentInParent<Canvas>().transform);

        _blockerButton.onClick.AddListener(OnBlockerButtClick);
        _ddButton.onClick.AddListener(OnDDButtonClick);
        _inputField.onValueChanged.AddListener(OnInputvalueChange);
        _inputField.onEndEdit.AddListener(OnEndEditing);

        _optionsNames = new List<string>(optionsNames);
        AddOptions(optionsNames);
    }

    /// <summary>
    /// public method to get the selected value
    /// </summary>
    /// <returns></returns>
    public string GetValue()
    {
        return _inputField.text;
    }

    public void ResetDropDown()
    {
        _inputField.text = string.Empty;
    }
    
    /// <summary>
    /// remove the elements from the dropdown based on Filters
    /// </summary>
    /// <param name="input"></param>
    public void FilterDropdown(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            foreach (var button in _initializedButtons)
                button.gameObject.SetActive(true);
            
            ResizeScrollRect();
            _scrollRect.gameObject.SetActive(false);
            return;
        }

        var count = 0;
        
        foreach (var button in _initializedButtons)
        {
            if (!button.name.ToLower().Contains(input.ToLower()))
            {
                button.gameObject.SetActive(false);
            }
            else
            {
                button.gameObject.SetActive(true);
                count++;
            }
        }

        SetScrollActive(count > 0);
        ResizeScrollRect();
    }
    
    private void AddOptions(List<string> options)
    {
        foreach (var option in options)
        {
            var button = Instantiate(_buttonPrefab, _content);
            button.GetComponentInChildren<TMP_Text>().text = option;
            button.name = option;
            button.gameObject.SetActive(true);
            button.onClick.AddListener(delegate { OnOptionSelected(button); });
            _initializedButtons.Add(button);
        }
        
        ResizeScrollRect();
        _scrollRect.gameObject.SetActive(false);
    }


    /// <summary>
    /// listner To Input Field End Editing
    /// </summary>
    /// <param name="arg"></param>
    private void OnEndEditing(string arg)
    {
        if (string.IsNullOrEmpty(arg))
        {
            Debug.Log("no value entered ");
            return;
        }
        
        StartCoroutine(CheckIfValidInput(arg));
    }

    /// <summary>
    /// Need to wait as end inputField and On option button  Contradicted and message was poped after selection of button
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private IEnumerator CheckIfValidInput(string arg)
    {
        const int waitTime = 1;
        yield return new WaitForSeconds(waitTime);
        
        if (!_optionsNames.Contains(arg))
            _inputField.text = String.Empty;
        
        ValueChanged?.Invoke(_inputField.text);
    }
    
    /// <summary>
    /// Called ever time on Drop down value is changed to resize it
    /// </summary>
    private void ResizeScrollRect()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_content.transform);
        var length = _content.GetComponent<RectTransform>().sizeDelta.y;

        _scrollRectTrans.sizeDelta = length > _maxScrollRectSize ? new Vector2(_scrollRectTrans.sizeDelta.x,
            _maxScrollRectSize) : new Vector2(_scrollRectTrans.sizeDelta.x, length + 5);
    }

    /// <summary>
    /// listner to the InputField
    /// </summary>
    /// <param name="arg0"></param>
    private void OnInputvalueChange(string arg0)
    {
        if (!_optionsNames.Contains(arg0))
        {
            FilterDropdown(arg0);
        }
    }

    /// <summary>
    /// Listner to option Buttons
    /// </summary>
    /// <param name="obj"></param>
    private void OnOptionSelected(Button option)
    {
        _inputField.text = option.name;
        
        foreach (var button in _initializedButtons)
            button.gameObject.SetActive(true);
        
        _isContentHidden = false;
        OnDDButtonClick();
        StopAllCoroutines();
        StartCoroutine(CheckIfValidInput(option.name));
    }

    /// <summary>
    /// listner to arrow button on input field
    /// </summary>
    private void OnDDButtonClick()
    {
        if (GetActiveButtons() <= 0)
            return;
        
        ResizeScrollRect();
        SetScrollActive(_isContentHidden);
    }
    
    private void OnBlockerButtClick()
    {
        SetScrollActive(false);
    }

    /// <summary>
    /// respondisble to enable and disable scroll rect component 
    /// </summary>
    /// <param name="status"></param>
    private void SetScrollActive(bool status)
    {
        _scrollRect.gameObject.SetActive(status);
        _blockerButton.gameObject.SetActive(status);
        _isContentHidden = !status;
        _ddButton.transform.localScale = status ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Return numbers of active buttons in the dropdown
    /// </summary>
    /// <returns></returns>
    private float GetActiveButtons()
    {
        var count = _content.transform.Cast<Transform>().Count(child => child.gameObject.activeSelf);
        var length = _buttonPrefab.GetComponent<RectTransform>().sizeDelta.y * count;
        return length;
    }
}
