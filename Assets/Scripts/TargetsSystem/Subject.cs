using System;
using Map.Signs;
using UnityEngine;

namespace TargetsSystem
{
    public class Subject : MonoBehaviour
    {
        [SerializeField] private SignCreator _signCreator;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _signCreator.Create();
        }
    }
}
