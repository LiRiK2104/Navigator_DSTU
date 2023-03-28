
using Navigation;
using TargetsSystem.Points;
using TargetsSystem.Rooms;
using TMPro;
using UnityEngine;

namespace Map.Signs.States
{
    public class DefaultState : MonoBehaviour
    {
        [SerializeField] private RoomNumber _roomNumberState;
        [SerializeField] private Icon _iconState;


        public void Initialize(SignPreset signPreset, PointInfo pointInfo)
        {
            _iconState.gameObject.SetActive(false);
            _roomNumberState.gameObject.SetActive(false);
            
            if (signPreset.HasIcon)
            {
                _iconState.gameObject.SetActive(true);
                _iconState.Initialize(signPreset);
            }
            else
            {
                _roomNumberState.gameObject.SetActive(true);
                _roomNumberState.Initialize(pointInfo);
            }
        }
    }
}
