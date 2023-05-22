using UI.Search;
using UnityEngine;

namespace UI.CarouselData
{
    [RequireComponent(typeof(SearchPanelsSwitcher))]
    public class CarouselDisplayer : MonoBehaviour
    {
        [SerializeField] private Carousel _carousel;
        
        private SearchPanelsSwitcher _searchPanelsSwitcher;
        
        
        private void Awake()
        {
            _searchPanelsSwitcher = GetComponent<SearchPanelsSwitcher>();
        }

        private void OnEnable()
        {
            _searchPanelsSwitcher.StateSet += UpdateCarouselVisible;
        }

        private void OnDisable()
        {
            _searchPanelsSwitcher.StateSet -= UpdateCarouselVisible;
        }

        
        private void UpdateCarouselVisible(SearchPanelState state)
        {
            bool visible = state != SearchPanelState.Search;
            _carousel.gameObject.SetActive(visible);
        }
    }
}
