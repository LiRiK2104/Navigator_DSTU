using System.Collections;
using AR;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AR.Buttons.UserPositionButtons
{
    [RequireComponent(typeof(Button))]
    public abstract class UserPositionButton : MonoBehaviour, IARContentUI
    {
        private Button _button;

        public ARMain ARMain => Global.Instance.ArMain;
        protected UserPositionFinder UserPositionFinder => Global.Instance.ArMain.UserPositionFinder;

        
        private void Awake()
        { 
            Initialize();   
        }
        
        private void OnEnable()
        {
            ValidateArAvailable();
        }

        
        public bool ValidateArAvailable()
        {
            gameObject.SetActive(ARMain.Available);
            return ARMain.Available;
        }

        protected virtual void Initialize()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(FindPosition);
        }
        
        protected abstract void Action();

        private void FindPosition()
        {
            StartCoroutine(FindPositionRoutine());
        }

        private IEnumerator FindPositionRoutine()
        {
            if (UserPositionFinder.State == ARConnectingState.Processing)
                yield break;

            yield return UserPositionFinder.FindUserPosition();

            if (UserPositionFinder.State != ARConnectingState.Completed)
                yield break;


            yield return null;
            Action();
        }
    }
}
