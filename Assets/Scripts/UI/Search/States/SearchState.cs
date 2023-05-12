using System.Collections.Generic;
using UI.Search.Options;
using UnityEngine;

namespace UI.Search.States
{
    public class SearchState : MonoBehaviour
    {
        [SerializeField] private OptionsList _optionsList;

        public void Initialize(List<IOptionInfo> optionInfos, OptionsList.OnOptionSelectedDel callback)
        {
            _optionsList.Initialize(optionInfos, callback);
        }
    }
}
