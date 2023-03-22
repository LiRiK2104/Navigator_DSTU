using System;
using Map.Signs;
using UnityEngine;

namespace TargetsSystem.Rooms
{
    public class AccessibleRoom : Room
    {
        [SerializeField] private TargetPoint _targetPoint;
        [SerializeField] private Sign _sign;
        [SerializeField] private SignPreset _signPreset;

        public Vector3 TargetPointPosition => _targetPoint.transform.position;


        protected override void Initialize()
        {
            base.Initialize();
            
            _sign.gameObject.SetActive(true);
            _sign.Initialize(_signPreset, this);
        }
    }
}
