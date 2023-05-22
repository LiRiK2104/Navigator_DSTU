using System;
using System.Collections.Generic;
using TMPro;
using UI.Search.Options;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace UI.Search
{
	public class SearchableDropDown : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _inputField;
		[SerializeField] private Button _cleanButton;
		[SerializeField] private SearchPanelsSwitcher _searchPanelsSwitcher;
		[SerializeField] private bool _displayPointsGroups;

		public delegate void OnValueChangedDel(string input);

		public event OptionsList.OnOptionSelectedDel OptionSelected;
		public event OnValueChangedDel ValueChanged;

		
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
				{
					_inputField.ActivateInputField();
					_searchPanelsSwitcher.SetHistoryPanel();
				}
				else
				{
					_inputField.DeactivateInputField();
					_searchPanelsSwitcher.SetDefaultPanel();
				}
			}
		}


		private void OnEnable()
		{
			_searchPanelsSwitcher.SearchOptionsList.OptionSelected += OnOptionSelected;
			_searchPanelsSwitcher.HistoryOptionsList.OptionSelected += OnOptionSelected;
		}

		private void OnDisable()
		{
			_searchPanelsSwitcher.SearchOptionsList.OptionSelected -= OnOptionSelected;
			_searchPanelsSwitcher.HistoryOptionsList.OptionSelected -= OnOptionSelected;
		}


		public void Initialize(List<IOptionInfo> optionInfos)
		{
			_searchPanelsSwitcher.Initialize(optionInfos, _displayPointsGroups, SetTextToInputField);
			_cleanButton.onClick.AddListener(Reset);
			_inputField.onValueChanged.AddListener(OnInputValueChange);
			
			_searchPanelsSwitcher.SetDefaultPanel();
		}

		public void SetTextToInputField(IOptionInfo optionInfo)
		{
			_inputField.text = optionInfo.Name;
		}

		public void Reset()
		{
			ResetDropDown();
			_searchPanelsSwitcher.SearchOptionsList.HideScroll();
			_cleanButton.gameObject.SetActive(false);
		}

		private void OnOptionSelected(IOptionInfo optionInfo)
		{
			_searchPanelsSwitcher.SetHistoryPanel();
			OptionSelected?.Invoke(optionInfo);
		}

		private void ResetDropDown()
		{
			_inputField.text = string.Empty;
		}

		private void OnInputValueChange(string arg0)
		{
			var optionsList = _searchPanelsSwitcher.SearchOptionsList;

			_cleanButton.gameObject.SetActive(arg0 != String.Empty);

			if (optionsList.Contains(arg0) == false)
				optionsList.Filter(arg0);

			_searchPanelsSwitcher.SetPanel(arg0);
			ValueChanged?.Invoke(arg0);
		}
	}
}
