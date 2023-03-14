using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class OptionsList : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Button _buttonPrefab;

    private Transform _content;
    private List<Button> _initializedButtons = new List<Button>();
    
    public delegate void OptionCallback(Button button);
    
    [HideInInspector] public List<string> Names = new List<string>();


    private void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _content = _scrollRect.content;
    }
    

    public void Add(List<string> options, OptionCallback callback)
    {
        foreach (var option in options)
        {
            var button = Instantiate(_buttonPrefab, _content);
            button.GetComponentInChildren<TMP_Text>().text = option;
            button.name = option;
            button.gameObject.SetActive(true);
            button.onClick.AddListener(delegate { callback.Invoke(button); });
            _initializedButtons.Add(button);
        }
        
        Names.AddRange(options);
        HideScroll();
    }

    public void Filter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            foreach (var button in _initializedButtons)
                button.gameObject.SetActive(true);
            
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
    }

    public void SetScrollActive(bool status)
    {
        if (status)
            ShowScroll();
        else
            HideScroll();
    }

    public void ActivateAllOptions()
    {
        foreach (var button in _initializedButtons)
            button.gameObject.SetActive(true);
    }
    
    public float GetActiveButtonsYLength()
    {
        var count = _content.transform.Cast<Transform>().Count(child => child.gameObject.activeSelf);
        var length = _buttonPrefab.GetComponent<RectTransform>().sizeDelta.y * count;
        return length;
    }

    private void Clear()
    {
        foreach (var button in _initializedButtons)
            Destroy(button.gameObject);
        
        _initializedButtons.Clear();
        Names.Clear();
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
