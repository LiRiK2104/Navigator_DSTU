using System.Collections.Generic;
using System.Linq;
using TargetsSystem.Points;
using UI.Search.Options;
using UnityEngine;

namespace UI.Search.States
{
	public class SearchState : MonoBehaviour
	{
		[SerializeField] private OptionsList _optionsList;

		public OptionsList OptionsList => _optionsList;


		public void Initialize(List<IOptionInfo> searchOptionsInfos, bool displayPointsGroups, OptionsList.OnOptionSelectedDel callback)
		{
			if (displayPointsGroups == false)
				searchOptionsInfos = searchOptionsInfos.Where(optionInfo => optionInfo is not PointsGroup).ToList();

			_optionsList.Initialize(searchOptionsInfos, callback);
		}
	}
}
