using System.Collections.Generic;
using System.Linq;
using UI.Search.Options;
using UnityEngine;

namespace UI.Search.States
{
	public class SearchHistoryState : MonoBehaviour
	{
		[SerializeField] private OptionsList _optionsList;

		private OptionsList.OnOptionSelectedDel _callback;
		private bool _displayPointsGroups;

		public OptionsList OptionsList => _optionsList;
		private SearchHistoryWriter SearchHistoryWriter => Global.Instance.SearchHistoryWriter;


		private void OnEnable()
		{
			UpdateOptions();
		}


		public void Initialize(bool displayPointsGroups, OptionsList.OnOptionSelectedDel callback)
		{
			_displayPointsGroups = displayPointsGroups;
			_callback = callback;
			UpdateOptions();
		}

		private void UpdateOptions()
		{
			var optionInfos = new List<IOptionInfo>();

			optionInfos.AddRange(SearchHistoryWriter.PointsInfos.Select(pointInfo => pointInfo as IOptionInfo));

			if (_displayPointsGroups)
				optionInfos.AddRange(SearchHistoryWriter.Groups.Select(group => group as IOptionInfo));

			optionInfos.Reverse();
			_optionsList.Initialize(optionInfos, _callback, true);
		}
	}
}
