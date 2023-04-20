using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.SlidingPanel
{
    public class SlidingPanelHandleView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private SlidingPanelHandler _slidingPanelHandler;
        
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _slidingPanelHandler.SetBeginDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _slidingPanelHandler.SwitchPosition(eventData, null);
        }
    }
}
