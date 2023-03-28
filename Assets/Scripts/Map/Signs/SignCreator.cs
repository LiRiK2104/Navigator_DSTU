using System;
using Navigation;
using TargetsSystem.Points;
using TargetsSystem.Rooms;
using UnityEngine;

namespace Map.Signs
{
    public class SignCreator : MonoBehaviour
    {
        [SerializeField] private Sign _signPrefab;
        [SerializeField] private SignPreset _signPreset;

        private Sign _sign;

        public Sign Sign => _sign;
        public SignPreset SignPreset => _signPreset;
        private Transform SignsContainer => Global.Instance.ArEnvironment.SignsContainer;
        
        
        public void Create(PointInfo pointInfo, AccessibleRoom room = null)
        {
            _sign = Instantiate(_signPrefab, transform.position, Quaternion.identity, SignsContainer);
            _sign.Initialize(pointInfo, _signPreset, room);
        }
    }
}
