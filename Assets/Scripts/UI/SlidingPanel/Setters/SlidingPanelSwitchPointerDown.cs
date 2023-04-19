using UnityEngine.EventSystems;

namespace UI.SlidingPanel.Setters
{
    public class SlidingPanelSwitchPointerDown : SlidingPanelSwitchObject
    {
        private EventTrigger _eventTrigger;
        
        private void Awake()
        {
            AddPointerDown();
        }

        private void AddPointerDown()
        {
            var pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener(e => SetPanelPosition());
            
            _eventTrigger = gameObject.AddComponent<EventTrigger>();
            _eventTrigger.triggers.Add(pointerDown);
        }
    }
}
