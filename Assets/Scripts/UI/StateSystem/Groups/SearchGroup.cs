using UI.Search;
using UnityEngine;

namespace UI.StateSystem.Groups
{
    public class SearchGroup : StatesGroup
    {
        [SerializeField] private SearchableDropDown _searchableDropDown;
        private DataBase DataBase => Global.Instance.DataBase;
        
        
        public override void Initialize() { }

        public override void OnClose()
        {
            _searchableDropDown.Reset();
            DeselectAllPoints();
        }
        
        private void DeselectAllPoints()
        {
            foreach (var point in DataBase.GetAllPoints())
            {
                if (point == null || point.SignCreator == null || point.SignCreator.Sign == null)
                {
                    return;
                }
                
                point.SignCreator.Sign.Deselect();
            }
        }
    }
}
