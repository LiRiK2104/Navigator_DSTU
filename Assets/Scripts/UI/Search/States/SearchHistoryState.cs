using System.Collections.Generic;
using System.Linq;
using UI.Search.Options;
using UnityEngine;

namespace UI.Search.States
{
    public class SearchHistoryState : MonoBehaviour
    {
        [SerializeField] private bool _displayPointsGroups;
        [SerializeField] private OptionsList _optionsList;

        private OptionsList.OnOptionSelectedDel _callback;

        private SearchHistoryWriter SearchHistoryWriter = Global.Instance.SearchHistoryWriter;
        
        
        private void OnEnable()
        {
            UpdateOptions();
        }


        public void Initialize(OptionsList.OnOptionSelectedDel callback)
        {
            _callback = callback;
            UpdateOptions();
        }

        private void UpdateOptions()
        {
            List<IOptionInfo> optionInfos = new List<IOptionInfo>();

            optionInfos.AddRange(SearchHistoryWriter.PointsInfos.Select(pointInfo => pointInfo as IOptionInfo));
            
            if (_displayPointsGroups)
                optionInfos.AddRange(SearchHistoryWriter.Groups.Select(group => group as IOptionInfo));
            
            _optionsList.Initialize(optionInfos, _callback, true);
        }
    }
}
