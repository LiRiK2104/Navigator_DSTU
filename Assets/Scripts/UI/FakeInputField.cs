using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class FakeInputField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _placeholder;
        [SerializeField] private TextMeshProUGUI _textField;

        public string Text => _textField.text;

        
        public void SetText(string text)
        {
            _textField.text = text;
            SwitchField();
        }

        public void Clear()
        {
            SetText(String.Empty);
        }

        private void SwitchField()
        {
            if (_textField.text == String.Empty)
            {
                _placeholder.gameObject.SetActive(true);
                _textField.gameObject.SetActive(false);
            }
            else
            {
                _placeholder.gameObject.SetActive(false);
                _textField.gameObject.SetActive(true);
            }
        }
    }
}
