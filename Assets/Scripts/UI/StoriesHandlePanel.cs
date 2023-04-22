using System;
using UI.AR;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class StoriesHandlePanel : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        public event Action<PointerEventData> PointerDown;
        public event Action<float, StoriesDirection> PointerClick;

        private float _pointerDownTime;
        
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownTime = Time.realtimeSinceStartup;
            PointerDown?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            float clickDeltaTime = Time.realtimeSinceStartup - _pointerDownTime;
            float halfScreenWidth = Screen.width / 2f;
            StoriesDirection direction = eventData.position.x < halfScreenWidth
                ? StoriesDirection.Previous
                : StoriesDirection.Next;

            PointerClick?.Invoke(clickDeltaTime, direction);
        }
    }
}
