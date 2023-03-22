using System;
using Map.Signs;
using UnityEngine;

namespace TargetsSystem
{
    public class Subject : MonoBehaviour
    {
        [SerializeField] private Sign _sign;
        [SerializeField] private SignPreset _signPreset;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _sign.gameObject.SetActive(true);
            _sign.Initialize(_signPreset);
        }
    }
}
