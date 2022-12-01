using System;
using TMPro;
using UnityEngine;

public class UISetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private GameObject _calibrateButton;
    [SerializeField] private GameObject _recalibrateButton;
    [SerializeField] private GameObject _searchableDropdown;
    [SerializeField] private GameObject _markerFrame;
    [SerializeField] private GameObject _loadingImage;
    [SerializeField] private GameObject _failImage;
    [SerializeField] private GameObject _minimap;

    
    public void SetState(SessionStates state, string text = "")
    {
        DeactivateAll();
        SetText(text);

        switch (state)
        {
            case SessionStates.Loading:
                SetLoadingState();
                break;
            
            case SessionStates.Calibration:
                SetCalibrationState();
                break;
            
            case SessionStates.Calibrated:
                SetCalibratedState();
                break;
            
            case SessionStates.Failed:
                SetFailedState();
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    private void SetLoadingState()
    {
        _loadingImage.SetActive(true);
    }
    
    private void SetCalibrationState()
    {
        _markerFrame.SetActive(true);
        _calibrateButton.SetActive(true);
    }
    
    private void SetCalibratedState()
    {
        _recalibrateButton.SetActive(true);
        _searchableDropdown.SetActive(true);
        _minimap.SetActive(true);
    }
    
    private void SetFailedState()
    {
        _failImage.SetActive(true);
    }

    private void SetText(string text)
    {
        _infoText.text = text;
    }

    private void DeactivateAll()
    {
        _calibrateButton.SetActive(false);
        _recalibrateButton.SetActive(false);
        _searchableDropdown.SetActive(false);
        _markerFrame.SetActive(false);
        _loadingImage.SetActive(false);
        _failImage.SetActive(false);
        _minimap.SetActive(false);
    }
}

public enum SessionStates
{
    Loading,
    Calibration,
    Calibrated,
    Failed
}
