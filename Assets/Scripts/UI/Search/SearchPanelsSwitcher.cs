using System;
using System.Collections.Generic;
using UI.Search.Options;
using UI.Search.States;
using UnityEngine;

namespace UI.Search
{
	[RequireComponent(typeof(SearchableDropDown))]
	public class SearchPanelsSwitcher : MonoBehaviour
	{
		[SerializeField] private GameObject _defaultPanel;
		[SerializeField] private SearchHistoryState _historyPanel;
		[SerializeField] private SearchState _searchPanel;

		public event Action<SearchPanelState> StateSet; 

		public OptionsList SearchOptionsList => _searchPanel.OptionsList;
		public OptionsList HistoryOptionsList => _historyPanel.OptionsList;
		private SearchHistoryWriter SearchHistoryWriter => Global.Instance.SearchHistoryWriter;


		public void Initialize(List<IOptionInfo> searchOptionsInfos, bool displayPointsGroups, OptionsList.OnOptionSelectedDel onOptionSelectCallback)
		{
			_searchPanel.Initialize(searchOptionsInfos, displayPointsGroups, onOptionSelectCallback);
			_historyPanel.Initialize(displayPointsGroups, onOptionSelectCallback);
		}

		public void SetPanel(string searchArg)
		{
			if (searchArg == String.Empty)
				SetHistoryPanel();
			else
				SetSearchPanel();
		}
		
		public void SetDefaultPanel()
		{
			SetPanel(SearchPanelState.Default);
		}

		public void SetHistoryPanel()
		{
			if (SearchHistoryWriter.HasHistory)
				SetPanel(SearchPanelState.History);
			else
				SetDefaultPanel();
		}

		private void SetSearchPanel()
		{
			SetPanel(SearchPanelState.Search);
		}

		private void SetPanel(SearchPanelState state)
		{
			_defaultPanel.SetActive(false);
			_searchPanel.gameObject.SetActive(false);
			_historyPanel.gameObject.SetActive(false);

			switch (state)
			{
				case SearchPanelState.Default:
				_defaultPanel.SetActive(true);
				break;

				case SearchPanelState.Search:
				_searchPanel.gameObject.SetActive(true);
				break;

				case SearchPanelState.History:
				_historyPanel.gameObject.SetActive(true);
				break;
			}
			
			StateSet?.Invoke(state);
		}
	}
	
	public enum SearchPanelState
	{
		Default,
		Search,
		History
	}
}
