using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AR.Views
{
    public class ErrorView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageLabel;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _closeButton;

        private ARMain ARMain => Global.Instance.ArMain;

        
        private void Awake()
        {
            _closeButton.onClick.AddListener(ARMain.Exit);
        }
        

        public void SetError(ErrorType errorType = ErrorType.Default)
        {
            _retryButton.gameObject.SetActive(false);
            
            switch (errorType)
            {
                case ErrorType.TimeUp:
                    _messageLabel.text = "Время ожидания истекло. Откройте камеру устройства и повторите попытку.";
                    _retryButton.gameObject.SetActive(true);
                    break;
                
                case ErrorType.NotSupported:
                    _messageLabel.text = "AR не поддерживается на вашем устройстве.";
                    break;

                case ErrorType.Default:
                default:
                    _messageLabel.text = "Что-то пошло не так. Откройте камеру устройства и повторите попытку.";
                    _retryButton.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public enum ErrorType
    {
        Default,
        TimeUp,
        NotSupported
    }
}
