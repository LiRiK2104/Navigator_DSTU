using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Helpers
{
    public class Test : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        private Vector2 _pointerDownPosition;
        
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownPosition = eventData.position;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            float maxClickDelta = 50;
            Vector2 pointerUpPosition = eventData.position;
            var delta = (pointerUpPosition - _pointerDownPosition).magnitude;
            
            if (delta < maxClickDelta)
            {
                var clickEventData = new PointerEventData(EventSystem.current) { position = pointerUpPosition };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(clickEventData, results);
                var sign = results.Select(result => result.gameObject.GetComponent<TestA>()).FirstOrDefault(result => result != null);

                if (sign != null)
                    sign.MakeRed();
            }
            
            Debug.Log($"Delta: {delta} --------------------");
        }
    }
}





