using System;
using Navigation;
using TMPro;
using UnityEngine;

namespace UI.Views
{
    public class PathInfoPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _defaultState;
        [SerializeField] private GameObject _infoState;
        [SerializeField] private TextMeshProUGUI _timeLabel;
        [SerializeField] private TextMeshProUGUI _distanceLabel;

        private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;


        public void Initialize()
        {
            if (PathFinder.TryGetCurrentPathDistance(out float distance))
            {
                _defaultState.SetActive(false);
                _infoState.SetActive(true);
                _timeLabel.text = GetTimeString(distance);
                _distanceLabel.text = GetDistanceString(distance);
            }
            else
            {
                _defaultState.SetActive(true);
                _infoState.SetActive(false);   
            }
        }

        private string GetDistanceString(float distance)
        {
            return $"{Mathf.Round(distance)} м";
        }
        
        private string GetTimeString(float distance)
        {
            // speed m/s
            const float speed = 1.62f;
            const int secondsInMinute = 60;

            if (distance <= 0)
                return String.Empty;
            
            float time = distance / speed;

            if (time < secondsInMinute)
                return $"{Mathf.Round(time)} сек";
            
            return $"{Mathf.Round(time / secondsInMinute)} мин";
        }
    }
}
