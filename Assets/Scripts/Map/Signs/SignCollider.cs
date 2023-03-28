using Navigation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map.Signs
{
    public class SignCollider : MonoBehaviour
    {
        private PointInfo _pointInfo;
        private SearchResultsSelector SearchResultsSelector => Global.Instance.UISetterV2.SearchResultsSelector;
        

        public void Initialize(PointInfo pointInfo)
        {
            _pointInfo = pointInfo;
        }

        public void SetPointInfoState()
        {
            SearchResultsSelector.SetPointInfoState(_pointInfo);
        }
        
    }
}
