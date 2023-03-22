using System;
using Map.Signs;
using UnityEngine;

namespace TargetsSystem.Rooms
{
    public class AccessibleRoom : Room
    {
        [SerializeField] private TargetPoint _targetPoint;
        [SerializeField] private SignCreator _signCreator;

        public Vector3 TargetPointPosition => _targetPoint.transform.position;


        protected override void Initialize()
        {
            base.Initialize();
            _signCreator.Create(this);
        }
    }
}
