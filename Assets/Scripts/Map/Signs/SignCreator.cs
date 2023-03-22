using System;
using TargetsSystem.Rooms;
using UnityEngine;

namespace Map.Signs
{
    public class SignCreator : MonoBehaviour
    {
        [SerializeField] private Sign _signPrefab;
        [SerializeField] private SignPreset _signPreset;

        private Sign _sign;

        private Transform SignsContainer => Global.Instance.ArEnvironment.SignsContainer;
        
        
        public void Create(AccessibleRoom room = null)
        {
            _sign = Instantiate(_signPrefab, transform.position, Quaternion.identity, SignsContainer);
            _sign.Initialize(_signPreset, room);
        }
    }
}