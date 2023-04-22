using UnityEngine;

namespace UI.AR
{
    public class ARPanelView : MonoBehaviour, IARContentUI
    {
        [SerializeField] private GameObject _offPanel;
        [SerializeField] private GameObject _onPanel;
        
        public ARMain ARMain => Global.Instance.ArMain;
        

        private void OnEnable()
        {
            ValidateAREnabled();
            
            //TODO: Calibrated += Activate;
            //TODO: ARExit += Deactivate;
            //TODO: if (AR.active) Activate(); else Deactivate();
        }

        private void OnDisable()
        {
            //TODO: Calibrated -= Activate;
            //TODO: ARExit -= Deactivate;
        }


        private void Activate()
        {
            _offPanel.SetActive(false);
            _onPanel.SetActive(true);
        }
        
        private void Deactivate()
        {
            _offPanel.SetActive(true);
            _onPanel.SetActive(false);
        }

        public void ValidateAREnabled()
        {
            gameObject.SetActive(ARMain.Enabled);
        }
    }
}
