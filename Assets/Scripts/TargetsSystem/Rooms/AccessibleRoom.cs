using UnityEngine;

namespace TargetsSystem.Rooms
{
    public class AccessibleRoom : Room
    {
        [SerializeField] private TargetPoint _targetPoint;

        public Vector3 TargetPointPosition => _targetPoint.transform.position;
    }
}
