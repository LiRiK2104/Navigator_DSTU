using UnityEngine;

namespace UI.AR
{
    public class ARPanel : MonoBehaviour, IARContentUI
    {
        [SerializeField] private GameObject _offPanel;
        [SerializeField] private GameObject _onPanel;
        
        public ARMain ARMain => Global.Instance.ArMain;
        

        private void OnEnable()
        {
            if (ValidateArAvailable() == false)
                return;

            if (ARMain.Active)
                Activate();
            else
                Deactivate();
            
            ARMain.Entered += Activate;
            ARMain.Exited += Deactivate;
        }

        private void OnDisable()
        {
            ARMain.Entered -= Activate;
            ARMain.Exited -= Deactivate;
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

        public bool ValidateArAvailable()
        {
            gameObject.SetActive(ARMain.Available);
            return ARMain.Available;
        }
    }
}
