using UI.CarouselData;
using UI.Search;
using UnityEngine;

namespace UI.Views
{
    public class SearchPanelView : MonoBehaviour
    {
        [SerializeField] private SearchableDropDown _searchableDropDown;
        [SerializeField] private Carousel _carousel;
        
        private DataBase DataBase => Global.Instance.DataBase;
        

        public void Initialize()
        {
            _searchableDropDown.Initialize(DataBase.GetAllOptionInfos(true));
            _carousel.Initialize();
        }

        public void Activate()
        {
            _searchableDropDown.InputFieldIsActive = true;
        }
        
        public void Deactivate()
        {
            _searchableDropDown.InputFieldValue = string.Empty;
            _searchableDropDown.Reset();
            _searchableDropDown.InputFieldIsActive = false;
        }
    }
}
