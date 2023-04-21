using UnityEngine;

namespace UI.AR
{
    public class ARPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _offPanel;
        [SerializeField] private GameObject _onPanel;


        private void OnEnable()
        {
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
    }
}
