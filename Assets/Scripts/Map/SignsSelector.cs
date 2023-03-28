using System.Collections.Generic;
using System.Linq;
using Map.Signs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map
{
    public class SignsSelector : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        private Vector2 _pointerDownPosition;
        
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownPosition = eventData.position;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            SelectSign(eventData);
        }
        

        private void SelectSign(PointerEventData eventData)
        {
            const float maxClickDelta = 50;
            Vector2 pointerUpPosition = eventData.position;
            var delta = (pointerUpPosition - _pointerDownPosition).magnitude;

            if (delta >= maxClickDelta) 
                return;
            
            var clickEventData = new PointerEventData(EventSystem.current) { position = pointerUpPosition };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(clickEventData, results);
            
            var sign = results.Select(result => result.gameObject.GetComponent<SignCollider>()).FirstOrDefault(result => result != null);

            if (sign != null)
                sign.SetPointInfoState();
        }
    }
}
