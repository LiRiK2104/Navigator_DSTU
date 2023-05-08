using System.Collections.Generic;
using AR;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.AR.Views
{
    public class ARTutorialView : MonoBehaviour, IARContentUI
    {
        [SerializeField] private StoriesHandlePanel _handlePanel;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Multislider _multislider;
        [SerializeField] private List<GameObject> _cards = new List<GameObject>();

        private bool _isInitialized;

        public ARMain ARMain => Global.Instance.ArMain;


        private void OnEnable()
        {
            if (ValidateArAvailable() == false)
                return;
            
            _multislider.AllCompleted += Close;
            _multislider.SliderStarted += SetCard;
            _handlePanel.PointerDown += Pause;
            _handlePanel.PointerClick += ProcessClick;

            Play();
        }

        private void OnDisable()
        {
            _multislider.AllCompleted -= Close;
            _multislider.SliderStarted -= SetCard;
            _handlePanel.PointerDown -= Pause;
            _handlePanel.PointerClick -= ProcessClick;
        }

        
        public bool ValidateArAvailable()
        {
            gameObject.SetActive(ARMain.Available);
            return ARMain.Available;
        }

        private void Initialize()
        {
            _multislider.Initialize(_cards.Count);
            
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(Close);

            _isInitialized = true;
        }

        private void ProcessClick(float clickDeltaTime, StoriesDirection storiesDirection)
        {
            const float maxClickTime = 0.6f;
            
            if (clickDeltaTime > maxClickTime)
                _multislider.Play();
            else
            {
                switch (storiesDirection)
                {
                    case StoriesDirection.Previous:
                        SetPrevious();
                        break;
                    
                    case StoriesDirection.Next:
                        SetNext();
                        break;
                }
            }
        }

        private void Play()
        {
            if (_isInitialized == false)
                Initialize();
            
            _multislider.Play();
            SetCard(0);
        }

        private void Pause(PointerEventData eventData)
        {
            _multislider.Pause();
        }
        
        private void SetPrevious()
        {
            _multislider.ForceSetPrevious();
            SetCard(_multislider.CurrentSliderIndex);
        }
        
        private void SetNext()
        {
            _multislider.ForceSetNext();
            SetCard(_multislider.CurrentSliderIndex);
        }
        
        private void Close()
        {
            if (_multislider.Active)
                _multislider.Stop();
            
            gameObject.SetActive(false);
        }

        private void SetCard(int index)
        {
            if (index < 0 || index >= _cards.Count)
                return;
            
            foreach (var card in _cards)
                card.SetActive(false);
            
            _cards[index].SetActive(true);
        }
    }

    public enum StoriesDirection
    {
        Previous, 
        Next
    }
}
